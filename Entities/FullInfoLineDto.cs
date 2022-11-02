using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueBotAzureFunction.Entities
{
    public class FullInfoLineDto
    {
        public string Id { get; set; }

        public string LineName { get; set; }

        public string LineType { get; set; }

        public long TelegramUserId { get; set; }

        public DateTimeOffset LineStart { get; set; }

        public DateTimeOffset LineFinish { get; set; }

        public long PositionPeriod { get; set; }

        public IList<Position> Positions { get; set; }
    }
}
