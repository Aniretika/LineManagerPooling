using QueueBotAzureFunction.CalendarControl.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotFunction.Replics;

namespace LineManagerBot.Options
{
    public static class CalendarPicker
    {
        readonly static List<string> DayOfWeeks = new() { "Mon", "Tu", "We", "Thu", "Fr", "Sa", "Su" };
        readonly static Dictionary<int, string> Months = new(){
            {1, "Jan"},
            {2, "Feb"},
            {3, "March"},
            {4, "April"},
            {5, "May"},
            {6, "June"},
            {7, "July"},
            {8, "Aug"},
            {9, "Sep"},
            {10, "Oct"},
            {11, "Nov"},
            {12, "Dec"} };
        
        private static Dictionary<string, string> GetDaysInMonth(DateTime datePosition)
        {
            Dictionary<string, string> days = new();
            DateTime dateTimeFirstDayOfMonth = new(datePosition.Year, datePosition.Month, 1);
            int gappers = (int)dateTimeFirstDayOfMonth.DayOfWeek -1;
            for (int i = 0; i < gappers; i++)
            {
                days.Add($"no-date{i}.", "no-date");
            }
            int daysInMonth = DateTime.DaysInMonth(datePosition.Year, datePosition.Month)+1;
            for (int i = 1; i < daysInMonth; ++i)
            {
                days.Add(i.ToString(), $"{Constants.PickDate}{i}");
            }
            if(days.Count >= 29)
            {
                if(days.Count >= 29 || days.Count < 35)
                {
                    for (int i = days.Count; i < 35; i++)
                    {
                        days.Add($"no-date{i}.", "no-date");
                    }
                }
                else
                {
                    for (int i = daysInMonth; i < 42; i++)
                    {
                        days.Add($"no-date{i}.", "no-date");
                    }
                }
                
            }
            return days;
        }

        private static string ButtonTextValidator(string value)
        {
            if (value.StartsWith("no-date"))
            {
                 return value.Substring(value.Length - 1);
            }

            return value;
        }

        private static InlineKeyboardMarkup InitializeCalendar(DateTime datePosition)
        {
            Dictionary<string, string> daysInMonth = GetDaysInMonth(datePosition);
            InlineKeyboardMarkup inlineKeyboard = new(
            new[]
            {
                  new InlineKeyboardButton[]
                  {
                      InlineKeyboardButton.WithCallbackData(
                          Months[datePosition.Month],
                          $"{Constants.PickMonth}{datePosition.Month}"
                      ),
                      InlineKeyboardButton.WithCallbackData(
                          datePosition.Year.ToString(),
                          $"{Constants.PickYear}{datePosition.Year}"
                      )
                  },
                  
                  DayOfWeeks.Select(o =>
                     new InlineKeyboardButton(o)
                     {
                         CallbackData = $"day of week select: {o}"
                     }).ToArray(),

                  daysInMonth.Take(7).Select(d =>
                     new InlineKeyboardButton(ButtonTextValidator(d.Key))
                     {
                         CallbackData = d.Value
                     }).ToArray(),
                  //14
                  daysInMonth.Skip(7).Take(7).Select(d =>
                     new InlineKeyboardButton(d.Key)
                     {
                         CallbackData = d.Value
                     }).ToArray(),
                  //21
                   daysInMonth.Skip(14).Take(7).Select(d =>
                     new InlineKeyboardButton(d.Key)
                     {
                         CallbackData = d.Value
                     }).ToArray(),
                  //28
                   daysInMonth.Skip(21).Take(7).Select(d =>
                     new InlineKeyboardButton(d.Key)
                     {
                         CallbackData = d.Value
                     }).ToArray(),
                  //35
                   daysInMonth.Skip(28).Take(7).Select(d =>
                     new InlineKeyboardButton(ButtonTextValidator(d.Key))
                     {
                         CallbackData = d.Value
                     }).ToArray(),
                   //42
                   daysInMonth.Skip(35).Take(7).Select(d =>
                     new InlineKeyboardButton(ButtonTextValidator(d.Key))
                     {
                         CallbackData = d.Value
                     }).ToArray()

            });
            return inlineKeyboard;

        }
        public static InlineKeyboardMarkup InitializeMonthCalendar(DateTime datePosition)
        {
            DateTime currentDateTime = DateTime.Now;
            int minMonth = currentDateTime.Month;
            if (datePosition.Year > currentDateTime.Year)
            {
                minMonth = 0;
            }
            
            InlineKeyboardMarkup inlineKeyboard = new(
                new[]
                {
                    Months.Where(m => m.Key >= minMonth).Select(o =>
                       new InlineKeyboardButton(o.Value)
                       {
                           CallbackData = $"{Constants.SetMonth}{o.Key}"
                       }).ToArray(),
                });
            return inlineKeyboard;
        }

        public static InlineKeyboardMarkup InitializeYearCalendar()
        {
            DateTime dateTime = DateTime.Now;
            List<int> Years = new List<int> { dateTime.Year, dateTime.Year + 1 };

            InlineKeyboardMarkup inlineKeyboard = new(
                new[]
                {
                    Years.Select(o =>
                       new InlineKeyboardButton(o.ToString())
                       {
                           CallbackData = $"{Constants.SetYear}{o}"
                       }).ToArray(),
                });
            return inlineKeyboard;
        }

        public static async Task<Message> SendCalendar(ITelegramBotClient botClient, Message message, DateTime datePosition)
        {

            var inlineKeyboardMarkup = InitializeCalendar(datePosition);
            return await botClient.SendTextMessageAsync(chatId: message!.Chat.Id, text: EngReplies.SelectDate, replyMarkup: inlineKeyboardMarkup);
        }

        public static async Task<Message> SendMonthList(ITelegramBotClient botClient, Message message, DateTime datePosition)
        {
            var inlineKeyboardMarkup = InitializeMonthCalendar(datePosition);
            return await botClient.SendTextMessageAsync(chatId: message!.Chat.Id, text: EngReplies.AddMonth, replyMarkup: inlineKeyboardMarkup);
        }

        public static async Task<Message> SendYearList(ITelegramBotClient botClient, Message message)
        {
            var inlineKeyboardMarkup = InitializeYearCalendar();
            return await botClient.SendTextMessageAsync(chatId: message!.Chat.Id, text: EngReplies.AddYear, replyMarkup: inlineKeyboardMarkup);
        }
    }
}
