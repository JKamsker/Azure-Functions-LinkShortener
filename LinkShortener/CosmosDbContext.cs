using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinkShortener.Extensions;
using LinkShortener.Models;
using LinkShortener.Models.Settings;
using Microsoft.Azure.Cosmos;

namespace LinkShortener
{
    public class CosmosDbContext
    {
        private readonly CosmosClient _client;
        private readonly DefaultAdminSettings _adminSettings;

        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private volatile bool _initialized = false;

        public bool IsInitialized => _initialized;
        public Database Database => _client.GetDatabase("LinkShortener");
        public Container ShortenerContainer => Database.GetContainer("LinkShortener");

        public Container AdminContainer => Database.GetContainer("Admins");

        public CosmosDbContext(CosmosClient client, DefaultAdminSettings adminSettings)
        {
            _client = client;
            _adminSettings = adminSettings;
        }

        public async ValueTask InitializeAsync()
        {
            if (_initialized)
            {
                return;
            }

            await semaphoreSlim.WaitAsync();
            try
            {
                if (_initialized)
                {
                    return;
                }

                var databaseResponse = await _client.CreateDatabaseIfNotExistsAsync("LinkShortener");
                var database = databaseResponse.Database;

                await database.CreateContainerIfNotExistsAsync("LinkShortener", "/Id");
                
                var adminContainerResponse = await database.CreateContainerIfNotExistsAsync("Admins", "/AdminKey");
                var adminContainer = adminContainerResponse.Container;
                if (!await adminContainer.AsQueryable<AdminItem>().ToCosmosAsyncEnumerable().AnyAsync())
                {
                    await adminContainer.CreateItemAsync(new AdminItem
                    {
                         AdminKey = string.IsNullOrEmpty(_adminSettings?.AdminKey) ? "123456" : _adminSettings.AdminKey
                    });
                }
                
                
                _initialized = true;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
    }
}