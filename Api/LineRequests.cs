using Newtonsoft.Json;
using QueueBotAzureFunction.Api;
using QueueBotAzureFunction.Entities;
using System.Net;

namespace TelegramBotFunction.Api
{
    public static class LineRequests
    {
        private static readonly string _adminUrl = "https://steamqueue20221003152847.azurewebsites.net";
        
        public async static Task<HttpStatusCode> AddPosititon(Dictionary<string, string> position)
        {
            using (var client = new HttpClient())
            {
                string address = $"api/Position/Add";
                string json = JsonConvert.SerializeObject(position);
                var result = await ResponseBuilder.PostRequest(client, _adminUrl, address, json);
                
                return result;
            }
        }

        public async static Task<string> GetUsersPosititonStatus(string lineId, long authorId)
        {
            var line = await GetFullLineInfo(lineId);
            var positions = line.Positions.Where(pos=> pos.TelegramRequesterId == authorId).ToList();
            string positionString = await ResponseBuilder.ConvertPosition(positions);
            return positionString;
        }

        public async static Task<List<PositionDto>> GetUserPositions(string lineId, long authorId)
        {
            var line = await GetFullLineInfo(lineId);
            var positions = line.Positions.Where(pos => pos.TelegramRequesterId == authorId).ToList();

            var resultList = positions
                .Select(x => new PositionDto() { Id = x.Id, DescriptionText = x.DescriptionText, NumberInTheQueue = x.NumberInTheQueue })
                .ToList();

            return resultList.OrderBy(pos => pos.NumberInTheQueue).ToList();
        }

        public async static Task<HttpStatusCode> DeletePosition(string positionId)
        {
            using (var client = new HttpClient())
            {
                string address = $"api/Position/Delete/{positionId}";
                var result = await ResponseBuilder.PostRequest(client, _adminUrl, address, "");

                return result;
            }
        }

      
        public async static Task<List<Line>> GetList()
        {
            using (var client = new HttpClient())
            {
                string responseBody = "";
                string address = $"api/Line/GetAll";
                try
                {
                    responseBody = await ResponseBuilder.GetRequest(_adminUrl, address, client);
                }
                catch (InvalidCastException e)
                {
                    throw new Exception();
                }

                if (responseBody == null)
                {
                    return null;
                }

                List<Line> result = JsonConvert.DeserializeObject<List<Line>>(responseBody);
                return result;
            }
        }

        public async static Task<FullInfoLineDto> GetFullLineInfo(string lineId)
        {
            using (var client = new HttpClient())
            {
                string address = $"api/Line/GetInfo/{lineId}";
                string responseBody = await ResponseBuilder.GetRequest(_adminUrl, address, client);
                if (responseBody == null)
                {
                    return null;
                }
                FullInfoLineDto result = JsonConvert.DeserializeObject<FullInfoLineDto>(responseBody);

                return result;
            }
        }
    }
}
