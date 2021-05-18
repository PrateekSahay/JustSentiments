using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetworkDashboard.Models
{
    public class AverageSentimentModel 
    {
        public List<string> Tweets { get; set; }

        public string Location { get; set; }

        public float AverageSentimentValue { get; set; }
    }
}
