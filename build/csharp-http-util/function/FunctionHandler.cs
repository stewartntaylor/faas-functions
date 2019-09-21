using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Function
{
    public class FunctionHandler
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public Task<string> Handle(string input)
        {
            var request = JsonConvert.DeserializeObject<QueryRequest>(input);
            if (request == null)
            {
                return Task.FromResult($"Error input");
            }

            var result = GetStatusForUrl(request.Url).Result;
            return Task.FromResult(JsonConvert.SerializeObject(result));
        }

        private async Task<QueryResult> GetStatusForUrl(string url)
        {
            var result = new QueryResult();

            var httpResult = await _httpClient.GetAsync(url);

            result.FirstStatus = httpResult.StatusCode;

            if (httpResult.Content != null)
            {
                result.ContentLength = httpResult.Content.ToString().Length;
            }
            else
            {
                result.ContentLength = -1;
            }

            return result;
        }
    }

    internal class QueryRequest
    {
        public string Url { get; set; }
    }

    internal class QueryResult
    {
        public HttpStatusCode FirstStatus { get; set; }
        public int FinalStatus { get; set; }
        public int ContentLength { get; set; }
        public int Hops { get; set; }
    }
}
