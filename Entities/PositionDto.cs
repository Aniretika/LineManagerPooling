using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueBotAzureFunction.Entities
{
    public class PositionDto
    {
        public int NumberInTheQueue { get; set; }

        public string Id { get; set; }

        public string DescriptionText { get; set; }
    }
}
