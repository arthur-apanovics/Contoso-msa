﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContosoBot.Models
{
    /// <summary>
    /// Holds entites passed in through the query
    /// </summary>
    [Serializable]
    public class EntityProps
    {
        public string Encyclopedia { get; set; }
        public float Currency { get; set; }
        public int Ordinal { get; set; }
        public string OrdinalTense { get; set; }
        public DateRange DateRange { get; set; }
        public string ComparisonOperator { get; set; }
    }
}