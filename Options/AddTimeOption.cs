using LineManagerBot.Options;
using QueueBotAzureFunction.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotFunction.Replics;

namespace QueueBotAzureFunction.Options
{
    public static class AddTimeOption
    {
        public static async Task SendPositionTime(ITelegramBotClient botClient, CallbackQuery callbackQuery, DateTime datePosition, FullInfoLineDto line)
        {
            var date = DateTime.Now.TimeOfDay;
            
            List<TimeSpan> availableTime = GetScheduleForDay(datePosition, line);
            if (availableTime is null || availableTime.Count() == 0)
            {
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Not available position on this time");
                await CalendarPicker.SendCalendar(botClient, callbackQuery.Message, datePosition);
            }
            await botClient.SendTextMessageAsync(chatId: callbackQuery!.Message!.Chat.Id, text: EngReplies.TimeCalendarAsking, replyMarkup: InlineKeyboardMarkupMaker(datePosition, availableTime));
        }
        private static List<TimeSpan> GetScheduleForDay(DateTime datePosition, FullInfoLineDto line)
        {
            TimeSpan endDay = new TimeSpan(1, 00, 00, 00);

            TimeSpan startPeriod = line.LineStart.TimeOfDay;
            TimeSpan endPeriod = line.LineFinish.TimeOfDay;

            long lineTick = line.PositionPeriod;
            TimeSpan sessionGapTime = TimeSpan.FromTicks(lineTick);

            List<TimeSpan> scheduleForDay = new List<TimeSpan>();

            if(line.LineFinish.Date >= DateTime.Now.Date)
            {
                if(datePosition.Date == DateTime.Now.Date)
                {
                    for (TimeSpan totalMinutes = startPeriod; totalMinutes <= endPeriod; totalMinutes = MinutesIterator(totalMinutes, sessionGapTime))
                    {
                        if (DateTime.Now.TimeOfDay < totalMinutes)
                        {
                            scheduleForDay.Add(totalMinutes);
                        }
                    }
                }
                else if(line.LineFinish.Date == datePosition)
                {
                    for (TimeSpan totalMinutes = startPeriod; totalMinutes <= endPeriod; totalMinutes = MinutesIterator(totalMinutes, sessionGapTime))
                    {
                        if (line.LineFinish.TimeOfDay >= totalMinutes)
                        {
                            scheduleForDay.Add(totalMinutes);
                        }
                    }
                }
                else
                {
                    for (TimeSpan totalMinutes = startPeriod; totalMinutes <= endPeriod; totalMinutes = MinutesIterator(totalMinutes, sessionGapTime))
                    {
                        scheduleForDay.Add(totalMinutes);
                    }
                }
                

                scheduleForDay.RemoveAll(p => p == endDay);
                foreach (TimeSpan positionDate in line.Positions.Select(p => p.TimelineStart.TimeOfDay).ToList())
                {
                    scheduleForDay.RemoveAll(p => p == positionDate);
                }
            }
          
            return scheduleForDay;
        }

        private static TimeSpan MinutesIterator(TimeSpan totalMinutes, TimeSpan sessionGapTime)
        {
            return totalMinutes + sessionGapTime;
        }
        private static InlineKeyboardMarkup InlineKeyboardMarkupMaker(DateTime datePosition, List<TimeSpan> availableTime)
        {
            datePosition = new DateTime(datePosition.Year, datePosition.Month, datePosition.Day, 0, 0, 0);
            InlineKeyboardButton[][] inlineKeyboard = availableTime.Select(item => new[]
            {

                new InlineKeyboardButton(item.ToString())
                {
                    CallbackData = $"{datePosition + item}",
                }
            }).ToArray();
            return new InlineKeyboardMarkup(inlineKeyboard);
        }
    }
}
