using System;

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
