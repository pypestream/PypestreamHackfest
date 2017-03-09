using System;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace PypestreamHackathon.Helpers
{
    public class BingHelper
    {
        public async Task<List<SearchResult>> GetSearchResults(string query)
        {
            List<SearchResult> results = new List<SearchResult>();
            var bingSubKey = ConfigurationManager.AppSettings["BingSubKey"];

            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", bingSubKey);

            // Request parameters
            //queryString["q"] = "domain:microsoft.com " + query;
            queryString["q"] = "" + query;
            queryString["count"] = "1";
            queryString["offset"] = "0";
            queryString["mkt"] = "en-us";
            queryString["safesearch"] = "strict";
            var uri = "https://api.cognitive.microsoft.com/bing/v5.0/search?" + queryString;

            var response = await client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            JObject data = JObject.Parse(json);

            string title = "";
            string url = "";

            if (data.Count > 0)
            {
                var webPages = data["webPages"]["value"];
                title = (string)webPages[0]["name"];
                url = (string)webPages[0]["url"];
            }
            results.Add(new SearchResult { Title = title, Url = url });
            return results;
        }
    }
    public class SearchResult
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Provider { get; set; }
    }
}