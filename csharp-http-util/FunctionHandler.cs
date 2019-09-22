using System;
using System.Collections.Generic;
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
            //result.StatusCodes = new List<int>();

            string currentUrl = string.Empty;
            string redirectUrl;

            for (int i = 0; i < 10; i++)
            {
                if (i == 0)
                {
                    currentUrl = url;
                }

                result.Hops = i;

                var httpReq = new HttpRequestMessage(HttpMethod.Head, currentUrl);

                var httpResp = await _httpClient.SendAsync(httpReq);
                
                // Always set last status
                result.FinalStatus = httpResp.StatusCode;

                // Record first status
                if (i == 0)
                    result.FirstStatus = httpResp.StatusCode;

                if (httpResp.StatusCode == HttpStatusCode.OK)
                {
                    break;
                }

                redirectUrl = httpResp.Headers.Location.ToString();

                if ((int)httpResp.StatusCode >= 300 &&
                    (int)httpResp.StatusCode <= 399 &&
                    !string.IsNullOrEmpty(redirectUrl))
                {
                    currentUrl = redirectUrl;
                    continue;
                }
                else
                {
                    // 4xx or 5xx
                    break;
                }

            }

            //var httpResult = await _httpClient.GetAsync(url);
            //result.FirstStatus = httpResult.StatusCode;

            /*
            if (httpResult.Content != null)
            {
                result.ContentLength = httpResult.Content.ToString().Length;
            }
            else
            {
                result.ContentLength = -1;
            }
            */

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
        public HttpStatusCode FinalStatus { get; set; }
        public List<int> StatusCodes { get; set; }
        //public int ContentLength { get; set; }
        public int Hops { get; set; }
    }
}
