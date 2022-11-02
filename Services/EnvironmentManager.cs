using Microsoft.Extensions.Logging;
using System;

namespace QueueBotAzureFunction.Services
{
    public static class EnvironmentManager
    {
        public static string GetToken(ILogger logger)
        {
            var token = Environment.GetEnvironmentVariable("token", EnvironmentVariableTarget.Process);

            if (token is null)
            {
                var exception =  new ArgumentException("Cant find the token from enviroment");
                logger.LogInformation("HandleError: {ErrorMessage}", exception.ToString());
            }

            return token;
        }

        public static string GetAdminUrl()
        {
            var url = Environment.GetEnvironmentVariable("adminUrl", EnvironmentVariableTarget.Process);

            if (url is null)
            {
                throw new ArgumentException("Cant find the admin url from enviroment");
            }

            return url;
        }

        public static string GetTestUrl()
        {
            var url = Environment.GetEnvironmentVariable("adminUrl", EnvironmentVariableTarget.Process);

            if (url is null)
            {
                throw new ArgumentException("Cant find the admin url from enviroment");
            }

            return url + "/api/Line/GetLines";
        }

        public static string GetHostUrl()
        {
            var host = Environment.GetEnvironmentVariable("hostUrl", EnvironmentVariableTarget.Process);


            if (host is null)
            {
                var exception = new ArgumentException("Cant find the host url from enviroment");
                return exception.ToString();
            }

            return host;
        }

    }
}
