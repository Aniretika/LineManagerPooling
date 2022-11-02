using LineManagerBot.Options;
using QueueBotAzureFunction.CalendarControl.Common;
using QueueBotAzureFunction.Entities;
using QueueBotAzureFunction.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotFunction.Api;
using TelegramBotFunction.Options;
using TelegramBotFunction.Replics;

namespace QueueBotAzureFunction.DialogCases
{
    public static class DialogSender
    {
        public static async Task UserChoice(ITelegramBotClient botClient, Message message, string userChoice)
        {
            if (userChoice != "")
            {
                await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: userChoice );
            }
        }
        public static async Task DeletePosition(ITelegramBotClient botClient, CallbackQuery callbackQuery, FullInfoLineDto line)
        {
            HttpStatusCode deleteRequest = await LineRequests.DeletePosition(callbackQuery!.Data!.Remove(36));
            if (deleteRequest == HttpStatusCode.OK)
            {
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "The position was successfully deleted!");
              
            }
            else
            {
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Something went wrong on the server. Try again later.");
            }
        }

        public static async Task TryAction(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
            await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Please, wait");
        }

        public static async Task<FullInfoLineDto> AskPositions(ITelegramBotClient botClient, CallbackQuery callbackQuery, FullInfoLineDto line)
        {
            if (line is null)
            {
                await InitialOption.GreetingSender(botClient, callbackQuery.Message);
            }
            else
            {
                line = await StatusOfLine.Get(botClient, callbackQuery, line);
                await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
                await MainDialog.Menu(botClient, callbackQuery.Message, line);
            }
            return line;
        }

        public static async Task ChooseLine(ITelegramBotClient botClient, CallbackQuery callbackQuery, FullInfoLineDto line)
        {
            if (line is null)
            {
                await TryAgainOption.Go(botClient, callbackQuery.Message, "Something wrong occured on the server");

            }
            else
            {
                await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
                await UserChoice(botClient, callbackQuery.Message, $"You have choose {line.LineName}");
                await MainDialog.Menu(botClient, callbackQuery.Message, line);
            }
        }

        public static async Task SelectForDeleting(ITelegramBotClient botClient, CallbackQuery callbackQuery, FullInfoLineDto currentLineInfo)
        {
            if (currentLineInfo == null)
            {
                await InitialOption.GreetingSender(botClient, callbackQuery.Message);
            }
            else
            {
                string userChoice = "You have choose position with number " + callbackQuery.Data + " for deleting";
                await UserChoice(botClient, callbackQuery.Message, userChoice);
                await DeletePosition(botClient, callbackQuery, currentLineInfo);
                var status = await StatusOfLine.CheckListOfUsersPositions(botClient, callbackQuery, currentLineInfo.Id);
                if(status is null)
                {
                    await botClient.SendTextMessageAsync(callbackQuery!.Message!.Chat.Id, $"There is no positions to delete");
                }
                await MainDialog.Menu(botClient, callbackQuery.Message, currentLineInfo);
            }
        }

        public async static Task PlainMessageProcessor(ITelegramBotClient botClient, Update update, FullInfoLineDto line, Dictionary<string, string> chatHistory)
        {
            if (update.Message.Entities != null || line is null)
            {
                await InitialOption.GreetingSender(botClient, update!.Message);
            }
            else if (update.Message.Entities == null && update!.Message.Type == MessageType.Text)
            {
                var posOption = await AddOptions.OnAddMessageReceived(botClient, update!.Message, chatHistory, line);
                if (posOption.Count <= 0)
                {
                    await MainDialog.Menu(botClient, update!.Message, line);
                }
            }
        }

        public static async Task<DateTime> CalendarSender(ITelegramBotClient botClient, CallbackQuery callbackQuery, DateTimeOffset currentDate)
        {
            DateTime datePosition = currentDate.UtcDateTime;
            string calendarOption = callbackQuery.Data;

            if (calendarOption.StartsWith(Constants.PickYear))
            {
                await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
                await CalendarPicker.SendYearList(botClient, callbackQuery.Message);
            }
            if (calendarOption.StartsWith(Constants.SetYear))
            {
                string data = callbackQuery.Data.Remove(0, 6);
                int selectedYear = int.Parse(data);
                DateTime dateTimeY = new(selectedYear, datePosition.Month, datePosition.Day);
                if (dateTimeY < DateTime.Now)
                {
                    datePosition = new(selectedYear, DateTime.Now.Month, datePosition.Day);
                }
                else
                {
                    datePosition = dateTimeY;
                }
                await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Current date: {datePosition}");
                await CalendarPicker.SendCalendar(botClient, callbackQuery.Message, datePosition);
            }

            if (calendarOption.StartsWith(Constants.PickMonth))
            {
                string data = callbackQuery.Data.Remove(0, 6);
                int selectedMonth = int.Parse(data);
                DateTime dateTimePos = new(datePosition.Year, selectedMonth, datePosition.Day);
                await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
                await CalendarPicker.SendMonthList(botClient, callbackQuery.Message, dateTimePos);
            }
            if (calendarOption.StartsWith(Constants.SetMonth))
            {
                string data = callbackQuery.Data.Remove(0, 6);
                int selectedMonth = int.Parse(data);
                datePosition = new(datePosition.Year, selectedMonth, datePosition.Day);
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Current date: {datePosition}");
                await CalendarPicker.SendCalendar(botClient, callbackQuery.Message, datePosition);
            }

                           
            return datePosition;
        }

        public static async Task<DateTime> CalendarDateSelector(ITelegramBotClient botClient, CallbackQuery callbackQuery, DateTime datePosition, FullInfoLineDto line)
        {
            string data = callbackQuery.Data.Remove(0, 6);
            int selectedDay = int.Parse(data);
            datePosition = new DateTime(datePosition.Year, datePosition.Month, selectedDay);
            if (datePosition.Date < DateTime.Now.Date)
            {
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, EngReplies.WrongDate);
                await CalendarSender(botClient, callbackQuery, DateTime.Now);
            }
            else
            {
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"You selected: {datePosition}");
                //await AddTimeOption.SendPositionTime(botClient, callbackQuery, datePosition, line);
            }
            return datePosition;
        }
        public static async Task<DateTime> CalendarDateSender(ITelegramBotClient botClient, CallbackQuery callbackQuery, DateTime datePosition, FullInfoLineDto line)
        {
            string data = callbackQuery.Data.Remove(0, 6);
            TimeSpan selectedTime = TimeSpan.Parse(data);
            datePosition = new DateTime(datePosition.Year, datePosition.Month, datePosition.Day, 0, 0, 0) + selectedTime;
            if (datePosition < DateTime.Now)
            {
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, EngReplies.WrongDate);
                await CalendarSender(botClient, callbackQuery, DateTime.Now);
            }
            else
            {
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Current date: {datePosition}");
                //await AddTimeOption.SendPositionTime(botClient, callbackQuery, datePosition, line);
            }
            return datePosition;
        }

        public static async Task DeleteFirstPosition(ITelegramBotClient botClient, CallbackQuery callbackQuery, FullInfoLineDto line)
        {
            
            var position = line.Positions.SingleOrDefault(pos => pos.TelegramRequesterId == callbackQuery.From.Id && pos.NumberInTheQueue == 1);
            bool isPositionForDeletingExists = position is not null;
            
            if (isPositionForDeletingExists)
            {
                var result = await LineRequests.DeletePosition(position.Id);
                if(result == HttpStatusCode.OK)
                {
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, EngReplies.GetOutOfLineSuccess);
                }
                else
                {
                    await TryAgainOption.Go(botClient, callbackQuery.Message, "Bad connection to the server");
                }
            }
            else
            {
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, EngReplies.NotFirstPosition);
            }
            await MainDialog.Menu(botClient, callbackQuery.Message, line);
        }
    }
}
