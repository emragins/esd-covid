using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EsdCovid.Functions
{
    public class MyFunctions
    {
        private readonly Data _data;
        private readonly QueriesRepository _queriesRepo;

        public MyFunctions(Data data, QueriesRepository queriesRepo)
        {
            _data = data;
            _queriesRepo = queriesRepo;
        }

        [FunctionName("SaveQuery")]
        public async Task<IActionResult> SaveQuery(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            var text = (string)(data?.text?.ToString());
            
            if (string.IsNullOrWhiteSpace(text))
            {
                return new BadRequestObjectResult("expected request body to contain 'text' as a json field");
            }

            CovidDataQuery query = await _queriesRepo.Fetch(text);

            if (query == null)
            {
                query = new CovidDataQuery(text);
            }
            else
            {
                query.NumTimesHit++;
                query.LastUpdated = DateTime.UtcNow;
            }

            try
            {
                await _queriesRepo.UpsertAsync(query);
            }
            catch (Exception)
            {
                // TODO: a typical app should log this.
                // This will automatically translate to an http 500 error for the caller.  That's enough for this.
                // This try/catch block is here for demonstration -- it's other useless code.
                throw;
            }

            return new OkResult();
        }


        [FunctionName("CommonQueries")]
        public async Task<IActionResult> CommonQueries(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var queries = await _queriesRepo.FetchTopQueriesAsync();

            var body = JsonConvert.SerializeObject(queries);

            return new OkObjectResult(body);
        }

        /// <summary>
        /// This primarily exists for testing.  It's destructive, but who really cares if an end-user
        /// finds and hits it?
        /// </summary>
        [FunctionName("ClearHistory")]
        public async Task<IActionResult> ClearHistory(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // I realized as I was falling asleep that there was a bug with this implementation.
            // It seems unfair to fix it, but if you're poking through the code and decide to
            // call the function, it breaks the historical queries functionality due a combination of
            // null'ing out `_instance` and dependency injection. (DI came after I wrote this).
            // As this function was built to facilitate testing or "administrative" use early on,
            // it seems best to comment it rather than let you put the app in a broken state. :)
            //
            // As for this comment/commit happening around 3:00am (if you're looking), I've unfortunately
            // been waking at this hour pretty consistently for a while now. :(
            //
            // - EMR 2021/04/26

            //await _data.Clear();

            return new ForbidResult();
            //return new OkResult();
        }
    }
}
