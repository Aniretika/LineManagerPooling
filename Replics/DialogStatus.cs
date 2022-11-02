using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueBotAzureFunction.Replics
{
    public static class DialogStatus
    {
        public static List<string> AddPosReplics = new()
        {
            "Add to the queue",
            "How can I name you?",
            "Now send me description",

        };
        public static List<string> DeletionReplics = new()
        {
            "Delete from the line"
        };
    }
}
