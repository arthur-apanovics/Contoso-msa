using System;
using Microsoft.Bot.Builder.FormFlow;

namespace ContosoBot.Models
{
    [Serializable]
    public class TransactionHistoryRangeQuery
    {
        [Prompt("Please select start date, e.g. 01/01/2017 or yesterday")]
        public DateTime DateStart { get; set; }
        [Prompt("Please select end date, e.g. 01/02/2017 or today")]
        public DateTime DateEnd { get; set; }
    }
}