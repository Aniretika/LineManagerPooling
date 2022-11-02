using QueueBotAzureFunction.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotFunction.Api;
using TelegramBotFunction.Replics;

namespace TelegramBotFunction.Options
{
    public static class StatusOfLine
    {
        public static async Task<FullInfoLineDto> Get(ITelegramBotClient botClient, CallbackQuery callbackQuery, FullInfoLineDto line)
        {
            string userPositions = await LineRequests.GetUsersPosititonStatus(line.Id, callbackQuery!.From!.Id);
            
            if(userPositions == "")
            {
                await botClient.SendTextMessageAsync
                    (callbackQuery!.Message!.Chat.Id,
                    $"Нou have not added positions to the queue yet.");
            }
            else
            {

                await botClient.SendTextMessageAsync
                       (callbackQuery!.Message!.Chat.Id,
                       $"{EngReplies.UserPositionsStatus}{userPositions}");
            }
            return await LineRequests.GetFullLineInfo(line.Id);
        }

        public static async Task<FullInfoLineDto> CheckListOfUsersPositions(ITelegramBotClient botClient, CallbackQuery callbackQuery, string lineId)
        {
            string userPositions = await LineRequests.GetUsersPosititonStatus(lineId, callbackQuery!.From!.Id);

            if (userPositions == "")
            {
               
                return null;
            }

            await botClient.SendTextMessageAsync
                   (callbackQuery!.Message!.Chat.Id,
                   $"{EngReplies.UserPositionsStatus}{userPositions}");

            return await LineRequests.GetFullLineInfo(lineId);

        }
    }
}
