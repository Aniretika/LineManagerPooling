using System;

namespace QueueBotAzureFunction.Entities
{
    public class Position
    {
        public string Id { get; set; }

        public long TelegramRequesterId { get; set; }

        public int NumberInTheQueue { get; set; }

        public string Requester { get; set; }

        public long? BotId { get; set; }

        public string DescriptionText { get; set; }

        public DateTime TimelineStart { get; set; }

        public DateTime TimelineFinish { get; set; }

    }
}

