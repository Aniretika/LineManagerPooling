using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotFunction.Replics
{
    public static class EngReplies
    {

        public const string Greeting = "Welcome to Queue Manager Bot!";
        public const string Switch = "What we will do?";

        //option: add to the queue
        public const string AddToQueue = "Add to the queue";
        public const string NameAsking = "How can I name you?";
        public const string DescriptionAsking = "Now send me description";
        public const string UsersTurn = "Status of your posititons:\n";

        //option: delete from the queue
        public const string DeleteFromQueue = "Delete from the line";
        public const string SelectForDeleting = "Select item to delete:";
        public const string DeleteFirstUserPosition = "I end work";

        //option: check status of the line
        public const string AskPositions = "Show my positions";
        public const string UserPositionsStatus = "Your positions:\n";

        //server status
        public const string AddPositionServerOk = "You have successfully joined the queue!";
        public const string AddPositionServerFailed = "Server connection error. Please, try again later.";

        //inline keyboard
        public const string ClendarAsking = "Would you like to choose date?";

        //inline choose the line keyboard
        public const string ChoseLineType = "Welcome to Queue Manager Bot!\nChoose the line type where you want to sign up:";
        public const string ChoseLine = "Select the line where you want to sign up:";
        public const string DateAsking = "Select date:";
        public const string TimeAsking = "Select session time:";
        public const string TimeCalendarAsking = "Select position time:";

        //Line
        public const string ChangeLine = "Change Line";
        public const string EndWork = "I have completed my job: get out of the line";
        public const string GetOutOfLineSuccess = "You have successfully exited the queue.";

        //Calendar
        public const string AddMonth = "Select the month:";
        public const string AddYear = "Select the month:";
        public const string WrongDate = "Sorry, but you cant select time in past";

        //Error Section
        public const string TryAgain = "Try again";
        public const string NotFirstPosition = "Your position in this queue is not the first. If you want to delete your position, use the \"Delete from the line\" option";

        //choose time
        public const string SelectTime = "Select time:";
        public const string NoAvailableTime = "Sorry, there is no available time in this line.";

        //initial inline keyboard
        public const string NormalLine = "Normal line";
        public const string SteamLine = "Steam line";
        public const string NormalLineCallback = "plain";
        public const string SteamLineCallback = "steam";

        //initial date
        public const string SelectDate = "Select the date for your future position";
        public const string WithoutLineOption = "Continue without time limits";
        public const string WithoutLineOptionCallback = "notimelimits";
    }
}
