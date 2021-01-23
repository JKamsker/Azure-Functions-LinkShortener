using System.Diagnostics;
using LinkShortener.Models.Settings;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: FunctionsStartup(typeof(LinkShortener.Startup))]

namespace LinkShortener
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;

            var config = builder.GetContext().Configuration;

            var cosmosClient = new CosmosClient(config["CosmosDb"]);
            services.AddSingleton(cosmosClient);
            services.AddSingleton<CosmosDbContext>();
            services.AddSingleton(x => new DefaultAdminSettings(config["DefaultAdminPassword"]));
            
            
            var mapper = new AutoMapper.MapperConfiguration(config =>
            {
                config.CreateMap<LinkItem, LinkItemCreationDto>();
                config.CreateMap<LinkItemCreationDto, LinkItem>();

                config.CreateMap<LinkItem, LinkItemAdminDto>()
                    .ForMember(x => x.ShortenedLink, m => m.MapFrom(x => string.Empty));
                
                config.CreateMap<LinkItemCreationDto, LinkItem>();
                config.CreateMap<LinkItemUpdateDto, LinkItem>();
            }).CreateMapper();
            services.AddSingleton(mapper);
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            base.ConfigureAppConfiguration(builder);
        }
    }
}