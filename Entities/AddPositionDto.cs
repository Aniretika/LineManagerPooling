using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueBotAzureFunction.Entities
{
    public class AddPositionDto
    {

        public DateTime TimelineStart { get; set; }

        public long TelegramRequesterId { get; set; }

        public int NumberInTheQueue { get; set; }

        public string Requester { get; set; }

        public long? BotId { get; set; }

        public string DescriptionText { get; set; }

    }
}
