using System;
using System.Collections.Generic;

namespace QueueBotAzureFunction.Entities
{
    public class Line
    {
        public Guid Id { get; set; }

        public bool IsSteamAccount { get; set; }

        public string LineName { get; set; }

        public string LineType { get; set; }

        public long TelegramUserId { get; set; }

        public DateTimeOffset LineStart { get; set; }

        public DateTimeOffset LineFinish { get; set; }

        public IList<Position> Positions { get; set; }
    }
}
