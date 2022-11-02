using QueueBotAzureFunction.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotFunction.Api;

namespace QueueBotAzureFunction.Options
{
    public static class AddLineOption
    {
        public static async Task<FullInfoLineDto> OnAddLineReceived(CallbackQuery callbackQuery)
        {

            FullInfoLineDto line = await LineRequests.GetFullLineInfo(callbackQuery.Data);

            return line;
        }
    }
}
