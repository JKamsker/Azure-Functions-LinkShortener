using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Schema;
using AutoMapper;
using LinkShortener.Extensions;
using LinkShortener.Models;
using LinkShortener.StringUtils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;

namespace LinkShortener
{
    /// <summary>
    /// Idee:
    ///     0.0: Jeder darf einen link erstellen
    ///     1: Link-Ersteller bekommt einen Token, um seinen link zu bearbeiten
    /// </summary>

    public class LinkShortenerApi
    {

        private readonly CosmosDbContext _context;
        private readonly IMapper _mapper;

        public LinkShortenerApi(CosmosDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [FunctionName("Warmup")]
        public async Task Warmup([TimerTrigger("0 1 * 1 *", RunOnStartup = true)] TimerInfo context, ILogger log)
        {
            await _context.InitializeAsync();
        }

        [FunctionName("Link")]
        public async Task<IActionResult> Relay
        (
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{id}")] HttpRequest req,
            [Bind("id")] string id,
            ILogger log
        )
        {
            var linkItem = await _context.ShortenerContainer
                .AsQueryable<LinkItem>()
                .Where(x => x.Id == id)
                .ToCosmosAsyncEnumerable()
                .FirstOrDefaultAsync();

            if (linkItem != null)
            {
                return new RedirectResult(linkItem.Url, false);
            }

            return new NotFoundObjectResult(new
            {
                Success = false,
                Message = "No item with this link found",
            });
        }

        [FunctionName("ListAll")]
        public async Task<IActionResult> GetAllLinks
        (
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "api/list")] HttpRequest req
        )
        {
            var linkItem = await _context.ShortenerContainer
                .AsQueryable<LinkItem>()
                .Select(x => new { x.Id, x.Url })
                .ToCosmosAsyncEnumerable()
                .Select(x => new { shortenedLink = $"{req.GetHostPath()}/{x.Id}", x.Url })
                .ToListAsync();

            return new OkObjectResult(linkItem);
        }

        [FunctionName("Create")]
        public async Task<IActionResult> CreateLink
        (
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "api/Shortener")] LinkItemCreationDto creationDto,
            HttpRequest req
        )
        {
            creationDto.Id = !string.IsNullOrEmpty(creationDto.Id) ? creationDto.Id : RandomEx.RandomString(10);
            creationDto.AccessKey = !string.IsNullOrEmpty(creationDto.AccessKey) ? creationDto.AccessKey : RandomEx.RandomString(10);

            var itemResponse = await _context.ShortenerContainer.CreateItemAsync(_mapper.Map<LinkItem>(creationDto));

            var result = _mapper.Map<LinkItemAdminDto>(itemResponse.Resource)
                .SetHost(req.GetHostPath());

            return new OkObjectResult(result);
        }

        [FunctionName("Update")]
        //[Route()]
        public async Task<IActionResult> UpdateLink
        (
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "api/Shortener/{id}")] LinkItemUpdateDto updateDto,
            [Bind("id")] string id,
            HttpRequest req

        )
        {
            var linkItem = await _context.ShortenerContainer
                .AsQueryable<LinkItem>()
                .Where(x => x.Id == id)
                .ToCosmosAsyncEnumerable()
                .FirstOrDefaultAsync();

            if (linkItem == null)
            {
                return new NotFoundObjectResult(new
                {
                    Success = false,
                    Message = "No item with this link found",
                });
            }

            if (string.IsNullOrEmpty(updateDto.AccessKey))
            {
                return new NotFoundObjectResult(new
                {
                    Success = false,
                    Message = "AccessKey was incorrect",
                });
            }

            if (linkItem.AccessKey != updateDto.AccessKey && !await IsAdminKeyAsync(updateDto.AccessKey))
            {
                return new NotFoundObjectResult(new
                {
                    Success = false,
                    Message = "AccessKey was incorrect",
                });
            }

            linkItem.Url = updateDto.Url;
            await _context.ShortenerContainer.UpsertItemAsync(linkItem);
            var result = _mapper.Map<LinkItemAdminDto>(linkItem).SetHost(req.GetHostPath());
            return new ObjectResult(result);
        }

        private async Task<bool> IsAdminKeyAsync(string accessKey)
        {
            return await _context.AdminContainer.AsQueryable<AdminItem>()
                .Where(x => x.AdminKey == accessKey)
                .ToCosmosAsyncEnumerable()
                .AnyAsync();
        }
    }
}