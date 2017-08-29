using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;

namespace ContosoData.Model
{
    [Serializable]
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Number { get; set; }
        public float Balance { get; set; }
        public int? OverdraftLimit { get; set; }
    }
}
