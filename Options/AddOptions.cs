using LineManagerBot.Options;
using QueueBotAzureFunction.DialogCases;
using QueueBotAzureFunction.Entities;
using QueueBotAzureFunction.Options;
using System;
using System.Collections.Concurrent;
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
    public static class AddOptions
    {
        static private ConcurrentDictionary<long, string> Answers = new();
    public static async Task SendAddPosition(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {

            ReplyKeyboardMarkup replyKeyboardMarkup =
            new(
                     new[]
                     {
                            new KeyboardButton[] { $"{callbackQuery!.Message!.Chat.FirstName} {callbackQuery.Message.Chat.LastName}" },
                     })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true,
            };

            await botClient.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id,
                                                       text: EngReplies.NameAsking,
                                                       replyMarkup: replyKeyboardMarkup);

            ///await botClient.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id, null, replyMarkup: new ReplyKeyboardRemove());

            Answers.TryAdd(callbackQuery.From.Id, EngReplies.NameAsking);

            await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
        }

        public static async Task<Dictionary<string, string>> OnAddMessageReceived(ITelegramBotClient botClient, 
            Message message, 
            Dictionary<string, string> position, 
            FullInfoLineDto line, 
            DateTime datePosition = default)
        {
            long userId = message!.Chat!.Id;

            if (Answers.TryGetValue(userId, out var answer) &&  line is not null)
            {
                if (answer == EngReplies.NameAsking && position.Count() == 0)
                {
                    position.Add("requester", message.Text);
                    position.Add("telegramRequesterId", message.Chat.Id.ToString());
                    position.Add("botId", botClient.BotId.ToString());
                    position.Add("lineId", line.Id);
                    //is line is session

                    switch (line.LineType.ToLower())
                        {
                            case "timeslot":
                                {
                                await TimeKeyboardOption.SendDateTimePosition(botClient, message, line);
                                Answers.TryUpdate(userId, EngReplies.TimeAsking, answer);
                                break;
                                }
                            case "live":
                                {
                                await botClient.SendTextMessageAsync(message!.Chat!, EngReplies.DescriptionAsking);
                                Answers.TryUpdate(userId, EngReplies.DescriptionAsking, answer);
                                break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                   

                    return position;
                }
                else if((answer == EngReplies.TimeAsking || answer == EngReplies.NameAsking))
                {
                    await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
                    await DialogSender.UserChoice(botClient, message, $"Your selected time {position.Values.LastOrDefault()}");
                    await botClient.SendTextMessageAsync(message!.Chat!, EngReplies.DescriptionAsking);
                    Answers.TryUpdate(userId, EngReplies.DescriptionAsking, answer);
                    return position;
                }
                else if (answer == EngReplies.DescriptionAsking && (position.Count() == 4 || position.Count() == 5))
                {
                    position.Add("descriptionText", message.Text);
                    Answers.Clear();
                    var requestStatus = await LineRequests.AddPosititon(position);

                    if (requestStatus == System.Net.HttpStatusCode.OK)
                    {
                        int lastNumberInQueue = 0;
                        if (line.Positions!=null)
                        {
                            lastNumberInQueue = line.Positions.Select(p => p.NumberInTheQueue).Max();
                        }
                        
                        string conclusion = $"You have been successfully added to the queue using the name {position["requester"]}! " +
                            $"\nThe description: {position["descriptionText"]}.\nYour number in line: {lastNumberInQueue + 1}";
                        if(line.LineType.ToLower() == "timeslot")
                        {
                            conclusion += $"\nYour position time: {position["timelineStart"]}.";
                        }
                            await botClient.SendTextMessageAsync(message!.Chat!, conclusion);

                        var positionList = await LineRequests.GetUsersPosititonStatus(line.Id, message.Chat.Id);
                       
                        await botClient.SendTextMessageAsync(message!.Chat!, $"{EngReplies.UsersTurn}{positionList}");
                    }
                    else
                    {
                        await TryAgainOption.Go(botClient, message, "Oh, no! Something went wrong! Please, try again or connect with admins!");
                    }
                    await MainDialog.Menu(botClient, message, line);
                    return position;
                }
                position.Clear();
                Answers.Clear();
                return position;
            }
            else
            {
                position.Clear();
                Answers.Clear();
                if (line is null)
                {
                    await InitialOption.GreetingSender(botClient, message);
                }
                return position;
            }
        }
    }
}
