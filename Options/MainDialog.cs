using QueueBotAzureFunction.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotFunction.Replics;

namespace TelegramBotFunction.Options
{
    public static class MainDialog
    {
        private static readonly List<string> spamList = new() { "http:", "https:", "wwww." };

        public static async Task Menu(ITelegramBotClient botClient, Message message, FullInfoLineDto line)
        {
            if (message.Type != MessageType.Text)
                return;

            bool spamChecker = spamList.Any(spam => message!.Text!.ToLower().Contains(spam));

            if (spamChecker)
                await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            var action = SendInlineKeyboard(botClient, message, line);

            Message sentMessage = await action;
            await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
        }

        private static async Task<Message> SendInlineKeyboard(ITelegramBotClient botClient, Message message, FullInfoLineDto line)
        {
            await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
            InlineKeyboardButton[][] buttons = 
                new []
                {
                     new []
                     {
                         InlineKeyboardButton.WithCallbackData(EngReplies.ChangeLine, EngReplies.ChangeLine),
                     },
                     new []
                     {
                         InlineKeyboardButton.WithCallbackData(EngReplies.AddToQueue, EngReplies.AddToQueue),
                     },
                     new []
                     {
                         InlineKeyboardButton.WithCallbackData(EngReplies.AskPositions, EngReplies.AskPositions),
                     },
                     new [] 
                     {
                         InlineKeyboardButton.WithCallbackData(EngReplies.DeleteFromQueue, EngReplies.DeleteFromQueue),
                     },
                };
            var buttonsNew = buttons.ToList();

            if(line.LineType == "live")
            {
                var plainLineCase = new[]
                {
                    InlineKeyboardButton.WithCallbackData(EngReplies.EndWork, EngReplies.EndWork)
                };

                buttonsNew.Add(plainLineCase);
            }
            

            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(buttonsNew);


            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: EngReplies.Switch + $"\nYour selected line is {line.LineName}",
                                                        replyMarkup: inlineKeyboard);
        }
    }
}
