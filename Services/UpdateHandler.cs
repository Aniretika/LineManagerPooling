using LineManagerBot.Options;
using Microsoft.VisualBasic;
using QueueBotAzureFunction.Checkers;
using QueueBotAzureFunction.DialogCases;
using QueueBotAzureFunction.Entities;
using QueueBotAzureFunction.Options;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotFunction.Options;
using TelegramBotFunction.Replics;

namespace Telegram.Bot.Services;

public class UpdateHandler : IUpdateHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<UpdateHandler> _logger;

    public UpdateHandler(ITelegramBotClient botClient, ILogger<UpdateHandler> logger)
    {
        _botClient = botClient;
        _logger = logger;
    }
    public static Dictionary<string, string> ChatHistory = new();
    //public static Dictionary<string, string> CurrentLineInfo = new();
    private static FullInfoLineDto CurrentLineInfo { get; set; }
    public static DateTime DatePosition = DateTime.Now;

    public async Task HandleUpdateAsync(ITelegramBotClient _, Update update, CancellationToken cancellationToken)
    {
        if (update is null)
            return;

        try
        {
            switch (update.Type)
            {
                case UpdateType.EditedMessage | UpdateType.Message:
                    {
                        if (DialogChecker.IsInitialMessage(update))
                        {
                            ChatHistory.Clear();
                            await BotOnInitialMessageReceived(_botClient, update!.Message);
                        }
                        else
                        {
                            await BotOnMessageReceived(_botClient, update);
                        }
                        break;
                    }
                case UpdateType.CallbackQuery:
                    {
                        await BotOnCallbackOptionalReceived(_botClient, update.CallbackQuery!);
                        break;
                    }
                case UpdateType.ChosenInlineResult:
                    {
                        await BotOnChosenInlineResultReceived(update.ChosenInlineResult!);
                        break;
                    }
                default:
                    {
                        if (DialogChecker.IsInitialMessage(update))
                        {
                            ChatHistory.Clear();
                            await StartOnMessageReceived(_botClient, update);
                        }
                        else
                        {
                            await BotOnMessageReceived(_botClient, update);
                        }

                        break;
                    }
            }
        }
        catch (Exception exception)
        {
            await HandleErrorAsync(exception);
        }
    }

    private Task BotOnChosenInlineResultReceived(ChosenInlineResult chosenInlineResult)
    {
        _logger.LogInformation($"Received inline result: {chosenInlineResult.ResultId}");

        return Task.CompletedTask;
    }

    public async Task BotOnMessageReceived(ITelegramBotClient botClient, Update update)
    {
        await DialogSender.PlainMessageProcessor(botClient, update, CurrentLineInfo, ChatHistory);
    }

    public async Task StartOnMessageReceived(ITelegramBotClient botClient, Update update)
    {
        DialogException.IsMessageExist(update);
        CurrentLineInfo = null;
        await InitialOption.GreetingSender(botClient, update!.Message);
    }
    public async Task BotOnInitialMessageReceived(ITelegramBotClient botClient, Message message)
    {
        if (message.Type == MessageType.Text)
        {
            CurrentLineInfo = null;
            await InitialOption.GreetingSender(botClient, message);
        }
    }

    public Task HandleErrorAsync(Exception exception)
    {
        string errorMessage = DialogException.HandleErrorMessage(exception);
        _logger.LogInformation("HandleError: {ErrorMessage}", errorMessage);
        return Task.CompletedTask;
    }

    private async Task BotOnCallbackOptionalReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
        switch (callbackQuery.Data)
        {
            case EngReplies.DateAsking:
                {
                    await CalendarPicker.SendCalendar(botClient, callbackQuery.Message, DatePosition);
                    break;
                }
            case EngReplies.ChangeLine:
                {
                    CurrentLineInfo = null;
                    ChatHistory.Clear();
                    await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
                    await InitialOption.GreetingSender(botClient, callbackQuery.Message);
                    break;
                }

            case EngReplies.DeleteFromQueue:
                {
                    ChatHistory.Clear();
                    await DeleteOptions.SendDeletePosition(botClient, callbackQuery, CurrentLineInfo);

                    break;
                }

            case EngReplies.AddToQueue:
                {
                    ChatHistory.Clear();
                    await AddOptions.SendAddPosition(botClient, callbackQuery);
                    break;
                }

            case EngReplies.AskPositions:
                {
                    ChatHistory.Clear();
                    await DialogSender.AskPositions(botClient, callbackQuery, CurrentLineInfo);

                    break;
                }

            case EngReplies.EndWork:
                {
                    ChatHistory.Clear();
                    await DialogSender.DeleteFirstPosition(botClient, callbackQuery, CurrentLineInfo);

                    break;
                }

            case EngReplies.TryAgain:
                {
                    await DialogSender.TryAction(botClient, callbackQuery);
                    CurrentLineInfo = null;
                    ChatHistory.Clear();
                    await InitialOption.GreetingSender(botClient, callbackQuery.Message);
                    break;
                }
            case EngReplies.SteamLineCallback:
            case EngReplies.NormalLineCallback:
                {
                    await InitialOption.DateChooser(botClient, callbackQuery, DatePosition);
                    //await DialogSender.TryAction(botClient, callbackQuery);
                    CurrentLineInfo = null;
                    ChatHistory.Clear();
                    break;
                }
            case EngReplies.WithoutLineOptionCallback:
                {
                    await InitialOption.SendList(botClient, callbackQuery, DatePosition);
                    CurrentLineInfo = null;
                    ChatHistory.Clear();
                    break;
                }
        }

        switch (callbackQuery.Message.Text)
        {

            case EngReplies.ChoseLine:
                {
                    ChatHistory.Clear();
                    CurrentLineInfo = await AddLineOption.OnAddLineReceived(callbackQuery);
                    await DialogSender.ChooseLine(botClient, callbackQuery, CurrentLineInfo);
                    break;
                }

            case EngReplies.TimeAsking:
                {
                    ChatHistory.Add("timelineStart", callbackQuery.Data);
                    await AddOptions.OnAddMessageReceived(botClient, callbackQuery.Message, ChatHistory, CurrentLineInfo);
                    break;
                }

            case EngReplies.TimeCalendarAsking:
                {
                    ChatHistory.Add("timelineStart", callbackQuery.Data);
                    await AddOptions.OnAddMessageReceived(botClient, callbackQuery.Message, ChatHistory, CurrentLineInfo);
                    break;
                }

            case EngReplies.SelectForDeleting:
                {
                    await DialogSender.SelectForDeleting(botClient, callbackQuery, CurrentLineInfo);
                    break;
                }
        }
        if (DialogChecker.IsCalendarOption(callbackQuery))
        {
            DatePosition = await DialogSender.CalendarSender(botClient, callbackQuery, DatePosition);
        }
        else if (DialogChecker.IsCalendarConfirmation(callbackQuery))
        {
            DatePosition = await DialogSender.CalendarDateSelector(botClient, callbackQuery, DatePosition, CurrentLineInfo);
            await InitialOption.SendList(botClient, callbackQuery, DatePosition);
        }
    }
    public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);

        // Cooldown in case of network connection error
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }
}
