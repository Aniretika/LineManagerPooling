using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueBotAzureFunction.Entities
{
    public class SessionLine
    {
        public Guid SessionLineId { get; set; } = Guid.NewGuid();

        public bool IsPaused { get; set; } = false;

        public string SessionName { get; set; }

        public DateTime TimelineStart { get; set; }

        public DateTime TimelineFinish { get; set; }

        public TimeSpan PeriodDayStart { get; set; }

        public TimeSpan PeriodDayFinish { get; set; }
    }
}
