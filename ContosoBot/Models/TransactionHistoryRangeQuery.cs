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
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public int AccountId { get; set; }
    }
}