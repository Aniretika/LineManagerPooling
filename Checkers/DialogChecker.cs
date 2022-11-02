using QueueBotAzureFunction.CalendarControl.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace QueueBotAzureFunction.Checkers
{
    public static class DialogChecker
    {
        public static bool IsInitialMessage(Update update)
        {
            return update.MyChatMember != null || (update!.CallbackQuery == null && update.Message.Entities != null);
        }

        public static bool IsCalendarOption(CallbackQuery callbackQuery)
        {
            if (callbackQuery.Data.StartsWith(Constants.PickYear) 
                || callbackQuery.Data.StartsWith(Constants.SetYear)
                || callbackQuery.Data.StartsWith(Constants.PickMonth)
                || callbackQuery.Data.StartsWith(Constants.SetMonth))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool IsCalendarConfirmation(CallbackQuery callbackQuery)
        {
            if (callbackQuery.Data.StartsWith(Constants.PickDate))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
