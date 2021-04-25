using System;

using Newtonsoft.Json;

namespace EsdCovid.Functions
{
    public class CovidDataQuery
    {
        /// <summary>
        /// Empty constructor for when it comes from the database
        /// </summary>
        public CovidDataQuery() { }
        
        public CovidDataQuery(string text)
        {
            // Id is required by the database.
            Id = text;
            // for readability, just duplicate it.  We're dealing with tiny amounts of data.
            QueryText = text;
            NumTimesHit = 1;
            LastUpdated = DateTime.UtcNow;
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string QueryText { get; set; }
        public int NumTimesHit { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
