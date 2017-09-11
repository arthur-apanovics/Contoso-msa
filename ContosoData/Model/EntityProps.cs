using System;
using System.Collections.Generic;

namespace ContosoData.Model
{
    /// <summary>
    /// Holds entites passed in through the query
    /// </summary>
    [Serializable]
    public class EntityProps
    {
        public Account Account { get; set; }
        public string Encyclopedia { get; set; }
        //TODO: Create seperate class to hold MoneyAmount and MoneyCurrency
        public float MoneyAmount { get; set; }
        public List<string> MoneyCurrency { get; set; }
        public int Ordinal { get; set; }
        public float Percentage { get; set; }
        public string OrdinalTense { get; set; }
        public DateRange DateRange { get; set; }
        public string ComparisonOperator { get; set; }

        public EntityProps()
        {
            //DateRange = new DateRange();
            MoneyCurrency = new List<string>();
        }
    }
}