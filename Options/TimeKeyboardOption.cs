using QueueBotAzureFunction.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotFunction.Options;
using TelegramBotFunction.Replics;

namespace QueueBotAzureFunction.Options
{
    public static class TimeKeyboardOption
    {
        private static DateTime IncrementDate(DateTime current, TimeSpan timeSpan)
        {
            return current + timeSpan;
        }
        public static async Task SendDateTimePosition(ITelegramBotClient botClient, Message message, FullInfoLineDto line)
        {
            TimeSpan slotProlongation = TimeSpan.FromTicks(line.PositionPeriod);

            var sessionProlongation = line.LineFinish - line.LineStart;

          
            List<DateTime> dateList = new List<DateTime>();
            for (DateTime startDate = line.LineStart.UtcDateTime; startDate < line.LineFinish; startDate = IncrementDate(startDate, slotProlongation))
            {
                if (startDate.Date == DateTime.Now.Date)
                {
                    dateList.Add(startDate);
                }
            }
            if(line.Positions is not null)
            {
                List<DateTime> positions = line.Positions.Select(pos => pos.TimelineStart).ToList();
                foreach (var date in positions)
                {
                    dateList.RemoveAll(posDate => posDate == date);
                }
            }
            
            // Buttons
            InlineKeyboardButton[][] inlineKeyboard = dateList.Select(item => new[]
            {
                new InlineKeyboardButton(item.ToString())
                {
                    CallbackData = $"{item}"
                }
            }).ToArray();
            
            if(inlineKeyboard.Length > 0)
            {
                InlineKeyboardMarkup inline = new InlineKeyboardMarkup(inlineKeyboard);

                await botClient.SendTextMessageAsync(chatId: message!.Chat.Id, text: EngReplies.TimeAsking, replyMarkup: inline);
            }
            else
            {
                await botClient.SendTextMessageAsync(chatId: message!.Chat.Id, text: EngReplies.NoAvailableTime);
                await MainDialog.Menu(botClient, message, line);
            }    
        }
    }
}
