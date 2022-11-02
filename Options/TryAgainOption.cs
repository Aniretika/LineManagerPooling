using QueueBotAzureFunction.Replics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotFunction.Replics;

namespace QueueBotAzureFunction.Options
{
    public static class TryAgainOption
    {
        public static InlineKeyboardMarkup TryAgainKeyboard()
        {
            InlineKeyboardButton[] inlineKeyboard =
            {
                new InlineKeyboardButton(EngReplies.TryAgain)
                {
                    CallbackData = EngReplies.TryAgain,
                }
            };

            return new InlineKeyboardMarkup(inlineKeyboard);

        }
        public static async Task Go(ITelegramBotClient botClient, Message message, string errorText)
        {
            var random = new Random();
            int index = random.Next(StickersGenerator.Collection.Count);
            await botClient.SendStickerAsync(message.Chat.Id, StickersGenerator.Collection[index]);
            await botClient.SendTextMessageAsync(chatId: message!.Chat.Id, text: errorText, replyMarkup: TryAgainKeyboard());
        }
       
    }
}
