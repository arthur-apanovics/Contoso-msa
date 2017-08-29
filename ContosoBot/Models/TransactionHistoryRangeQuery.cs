using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;

namespace ContosoBot.Models
{
    [Serializable]
    public class TransactionHistoryRangeQuery
    {
        [Prompt("Please type in the start date (e.g. 15/07/2017)")]
        public DateTime DateStart { get; set; }
        [Prompt("Please type in the end date(e.g. 17/07/2017)")]
        public DateTime DateEnd { get; set; }
    }
}