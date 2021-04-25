using System;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace EsdCovid.Functions
{
    /// <summary>
    /// This class holds operations against our database.
    /// 
    /// In a normal app, this should be separated out into:
    /// a) the database connection itself
    /// b) a repository for handling "business" operations
    /// c) an interface (or two) to facilitate unit testing
    /// 
    /// However, architectural purity and increased code complexity is not always warranted,
    /// such as on small one-off projects. :)
    /// </summary>
    public class Data
    {

        private static Data _instance;
        /// <summary>
        /// Constructor for a singleton pattern.
        /// Even though we have dependency injection, we still want to make sure 
        /// nobody can instantiate this class directly.
        /// Additionally, we don't want initialization logic in the constructor itself
        /// since it requires external resources (aka, the database...).
        /// </summary>
        private Data() { }

        public static async Task<Data> GetInstance()
        {
            if (_instance != null) return _instance;

            _instance = new Data();

            _instance.cosmosClient = new CosmosClient(_instance.EndpointUrl);

            await _instance.CreateDatabaseIfNotExistsAsync();
            await _instance.CreateContainerIfNotExistsAsync();
            return _instance;
        }

        // real world this should come from an environment variable
        private string EndpointUrl = System.Environment.GetEnvironmentVariable("dburi");

        // The Cosmos client instance
        private CosmosClient cosmosClient;

        // The database we will create
        private Database database;

        // The container we will create.
        public Container Container { get; private set; }

        // The name of the database and container we will create
        private string databaseId = "database";
        private string containerId = "container";
        private string partitionKeyPath = "/queries";



        public async Task Clear()
        {
            await database.DeleteAsync();
        }

        private async Task CreateDatabaseIfNotExistsAsync()
        {
            // Create a new database
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            Console.WriteLine("Created Database: {0}\n", this.database.Id);
        }

        /// Create the container if it does not exist. 
        private async Task CreateContainerIfNotExistsAsync()
        {
            // Create a new container
            this.Container = await this.database.CreateContainerIfNotExistsAsync(containerId, partitionKeyPath);
            Console.WriteLine("Created Container: {0}\n", this.Container.Id);
        }
    }
}
