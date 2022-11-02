using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace QueueBotAzureFunction.DialogCases
{
    public static class DialogException
    {
        public static bool IsMessageExist(Update update)
        {
            if (update.Message == null)
            {
                throw new ArgumentNullException(nameof(update.Message));
            }
            return true;
        }
        public static string HandleErrorMessage(Exception exception)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            return errorMessage;
        }
    }

   
}
