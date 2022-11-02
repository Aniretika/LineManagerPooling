using LineManagerBot.Options;
using QueueBotAzureFunction.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotFunction.Api;
using TelegramBotFunction.Replics;

namespace QueueBotAzureFunction.Options
{
    public class InitialOption
    {
        public static List<Line> CurrentLines = new();
        public static async Task GreetingSender(ITelegramBotClient botClient, Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var inlineKeyboardMarkup = LineTypeChooser();
            await botClient.SendTextMessageAsync(chatId: message!.Chat.Id, text: EngReplies.ChoseLineType, replyMarkup: inlineKeyboardMarkup);
            await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
        }

        public static async Task DateChooser(ITelegramBotClient botClient, CallbackQuery callbackQuery, DateTime datePosition)
        {
            if (callbackQuery.Message == null)
            {
                throw new ArgumentNullException(nameof(callbackQuery.Message));
            }
            bool isSteamLine = callbackQuery.Data.Equals(EngReplies.SteamLineCallback);
            CurrentLines = await LineRequests.GetList();
            CurrentLines = CurrentLines.Where(line => line.IsSteamAccount == isSteamLine).ToList();
            await LineIsNotExist(botClient, callbackQuery.Message);

            if (CurrentLines!.Count > 0)
            {
                await CalendarPicker.SendCalendar(botClient, callbackQuery.Message, datePosition);
                var inlineKeyboardOption = WithoutTimeOption();
                await botClient.SendTextMessageAsync(chatId: callbackQuery.Message!.Chat.Id, text: "Or you can:", replyMarkup: inlineKeyboardOption);
            }
        }

        public static async Task SendList(ITelegramBotClient botClient, CallbackQuery callbackQuery, DateTimeOffset datePosition)
        {
            if(callbackQuery.Message == null)
            {
                throw new ArgumentNullException(nameof(callbackQuery.Message));
            }

            if(callbackQuery.Data == EngReplies.WithoutLineOptionCallback)
            {
                CurrentLines = CurrentLines.Where(line => line.LineType == "live").ToList();
            }
            else
            {
                CurrentLines = CurrentLines.Where(line => line.LineStart.Date <= datePosition.Date && line.LineFinish.Date >= datePosition.Date).ToList();
                
            }
            await LineIsNotExist(botClient, callbackQuery.Message);

            if (CurrentLines!.Count > 0)
            {
                var inlineKeyboardMarkup = SendListOfLines(CurrentLines);
                await botClient.SendTextMessageAsync(chatId: callbackQuery.Message!.Chat.Id, text: EngReplies.ChoseLine, replyMarkup: inlineKeyboardMarkup);
                await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
            }
        }
        public static InlineKeyboardMarkup SendListOfLines(List<Line> lines)
        {
            InlineKeyboardButton[][] inlineKeyboard = lines.Select(item => new[]
            {
                new InlineKeyboardButton(item.LineName)
                {
                    CallbackData = $"{item.Id}",
                }
            }).ToArray();

            return new InlineKeyboardMarkup(inlineKeyboard);
        }

        public static InlineKeyboardMarkup LineTypeChooser()
        {
            InlineKeyboardButton[] inlineKeyboard =
           {
                new InlineKeyboardButton(EngReplies.NormalLine)
                {
                    CallbackData = EngReplies.NormalLineCallback,
                },
                new InlineKeyboardButton(EngReplies.SteamLine)
                {
                    CallbackData = EngReplies.SteamLineCallback,
                }
            };

            return new InlineKeyboardMarkup(inlineKeyboard);
        }

        public static InlineKeyboardMarkup WithoutTimeOption()
        {
            InlineKeyboardButton[] inlineKeyboard =
           {
                new InlineKeyboardButton(EngReplies.WithoutLineOption)
                {
                    CallbackData = EngReplies.WithoutLineOptionCallback,
                }
            };

            return new InlineKeyboardMarkup(inlineKeyboard);
        }

        private static async Task LineIsNotExist(ITelegramBotClient botClient, Message message)
        {
            if (CurrentLines!.Count == 0)
            {
                await botClient.SendTextMessageAsync(message!.Chat.Id, "We seek everywhere, but there is no lines with such configuration.");
                await GreetingSender(botClient, message);
            }
            else if (CurrentLines is null)
            {
                await TryAgainOption.Go(botClient, message, "There is no connection with admin site. Try again later");
            }
            
            return;
        }
    }
}
