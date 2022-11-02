using Newtonsoft.Json;
using QueueBotAzureFunction.Entities;
using System.Net;
using System.Text;

namespace QueueBotAzureFunction.Api
{
    public static class ResponseBuilder
    {
        public static async Task<string> ConvertPosition(List<Position> positionList)
        {
            if (positionList.Count == 0)
            {
                return "";
            }
            positionList.OrderBy(p => p.NumberInTheQueue);
            string positionString = "";
            foreach (var pos in positionList)
            {
                positionString += $"Number in the queue {pos.NumberInTheQueue}\n";
                positionString += $"Author {pos.Requester}\n";
                positionString += $"Decsription {pos.DescriptionText}.\n";
                positionString += "\n";
            }
            return positionString;
        }
        public static async Task<string> GetRequest(string adminUrl, string address, HttpClient client )
        {
            if (adminUrl == "")
            {
                return null;
            }

            client.BaseAddress = new Uri(adminUrl);

            var response = await client.GetAsync(address);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            if (responseBody is null || responseBody == "")
            {
                return null;
            }
            return responseBody;
        }

        public static async Task<bool> GetBoolRequest(string adminUrl, string address, HttpClient client)
        {
            if (adminUrl == "")
            {
                throw new NotImplementedException();
            }

            client.BaseAddress = new Uri(adminUrl);

            var response = await client.GetAsync(address);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            if (responseBody is null || responseBody == "")
            {
                throw new NotImplementedException();
            }
            Boolean myBool;

            if (Boolean.TryParse(responseBody, out myBool))
            {
                return myBool;
            }
            return false;
        }

        public static async Task<HttpStatusCode> PostRequest(HttpClient client, string adminUrl, string address, string json)
        {
            if (adminUrl == "")
            {
                return HttpStatusCode.BadRequest;
            }

            client.BaseAddress = new Uri(adminUrl);
            //client.DefaultRequestHeaders.Add("IsBot", "1");

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(address, content);
            var result = response.StatusCode;
            string resultContent = await response.Content.ReadAsStringAsync();
            return result;
        }
    }
}
