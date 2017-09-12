using System;
using System.Data.Entity;
using ContosoData.Model;

namespace ContosoData
{
    [Serializable]
    public class ContosoDataContext : DbContext
    {
        public ContosoDataContext()
            : base("ContosoDataContextConnectionString")
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Activity> ActivityLogs { get; set; }
    }
}
