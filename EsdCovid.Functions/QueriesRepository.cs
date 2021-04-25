using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;

namespace EsdCovid.Functions
{
    /// <summary>
    /// Serves to keep our "business logic" separate from the database managament logic.
    /// </summary>
    public class QueriesRepository
    {
        private readonly Container _container;

        public QueriesRepository(Container container)
        {
            _container = container;
        }


        public async Task UpsertAsync(CovidDataQuery row)
        {
            try
            {
                // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen".
                ItemResponse<CovidDataQuery> insertResponse = await _container.UpsertItemAsync<CovidDataQuery>(row);
                // Note that after creating the item, we can access the body of the item with the Resource property of the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
                Console.WriteLine("Created item in database with text: {0} Operation consumed {1} RUs.\n", insertResponse.Resource.QueryText, insertResponse.RequestCharge);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                Console.WriteLine("Item in database with id: {0} already exists\n", row.QueryText);
            }
        }

        public async Task<CovidDataQuery> Fetch(string queryText)
        {
            var sqlQueryText = $"SELECT * FROM Queries q WHERE q.{nameof(CovidDataQuery.QueryText)} = @queryText";

            Console.WriteLine("Running query: {0}\n", sqlQueryText);

            // parameterize since this is exposed
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText)
                .WithParameter("@queryText", queryText);

            FeedIterator<CovidDataQuery> queryResultSetIterator = _container.GetItemQueryIterator<CovidDataQuery>(queryDefinition);

            List<CovidDataQuery> results = new List<CovidDataQuery>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<CovidDataQuery> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (CovidDataQuery result in currentResultSet)
                {
                    results.Add(result);
                    Console.WriteLine("Found result {0}\n", result);
                }
            }

            // in a "real" app, it would be better to have the application recover automatically from this error condition,
            // such as by having it reduce the duplicate rows down to a single one.
            if (results.Count > 1)
            {
                throw new Exception("found more than one result for queryText " + queryText);
            }

            return results.FirstOrDefault();
        }

        public async Task<List<CovidDataQuery>> FetchTopQueriesAsync()
        {
            var sqlQueryText = $"SELECT TOP 5 * FROM Queries q ORDER BY q.{nameof(CovidDataQuery.NumTimesHit)} DESC";

            Console.WriteLine("Running query: {0}\n", sqlQueryText);

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<CovidDataQuery> queryResultSetIterator = _container.GetItemQueryIterator<CovidDataQuery>(queryDefinition);

            List<CovidDataQuery> results = new List<CovidDataQuery>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<CovidDataQuery> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (CovidDataQuery result in currentResultSet)
                {
                    results.Add(result);
                    Console.WriteLine("Found result {0}\n", result);
                }
            }

            return results;
        }

    }
}
