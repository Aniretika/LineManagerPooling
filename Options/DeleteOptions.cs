using QueueBotAzureFunction.Entities;
using QueueBotAzureFunction.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotFunction.Api;
using TelegramBotFunction.Replics;

namespace TelegramBotFunction.Options
{
    public static class DeleteOptions
    {        
        public static async Task<List<PositionDto>> SendDeletePosition(ITelegramBotClient botClient, CallbackQuery callbackQuery, FullInfoLineDto line)
        {
            List<PositionDto> userPositions = await LineRequests.GetUserPositions(lineId: line.Id, callbackQuery!.From!.Id);
            if(userPositions == null)
            {
                await TryAgainOption.Go(botClient, callbackQuery.Message, "Bad server connection");
                await MainDialog.Menu(botClient, callbackQuery.Message, line);
                return default;
            }
            else if (userPositions!.Count == 0)
            {
                await botClient.SendTextMessageAsync(callbackQuery!.Message!.Chat.Id, "No items to delete.");
                await MainDialog.Menu(botClient, callbackQuery.Message, line);
                return default;
            }
            else
            {
                var inlineKeyboardMarkup = InlineKeyboardMarkupMaker(userPositions);
                await botClient.SendTextMessageAsync(chatId: callbackQuery!.Message!.Chat.Id, text: EngReplies.SelectForDeleting, replyMarkup: inlineKeyboardMarkup);
                await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
                return userPositions;
            }
        }

        public static InlineKeyboardMarkup InlineKeyboardMarkupMaker(List<PositionDto> items)
        {
            string currentPosInfo = "";
            InlineKeyboardButton[][] inlineKeyboard = items.Select(item => new[]
            {

                new InlineKeyboardButton($"Number in queue: {item.NumberInTheQueue}\n Description: {item.DescriptionText}")
                {
                    CallbackData = $"{item.Id}{item.NumberInTheQueue}",
                }
            } ).ToArray();
            return new InlineKeyboardMarkup(inlineKeyboard);
        }
    }
}
