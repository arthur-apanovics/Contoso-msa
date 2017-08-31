using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContosoBot.Models
{
    /// <summary>
    /// Holds properties for LUIS datetimev2 resolutions
    /// </summary>
    [Serializable]
    public class DateRange
    {
        public string Timex { get; set; }
        public string Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}