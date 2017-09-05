using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContosoData.Model;

namespace ContosoData.Contollers
{
    [Serializable]
    public static class AccountDataController
    {
        public static IEnumerable<Account> Accounts
        {
            get
            {
                //TODO: Detect change and only update after changes have been made.
                // fetch account info on each get
                Accounts = GetAccounts();
                return _accounts;
            }
            set { _accounts = value; }
        }

        private static ContosoDataContext _context;
        private static IEnumerable<Account> _accounts;

        static AccountDataController()
        {
            _context = new ContosoDataContext();
        }
        private static IEnumerable<Account> GetAccounts()
        {
            return _context.Accounts;
        }

        public static IEnumerable<Transaction> GetTransactionsFromEntities(Account account, EntityProps entities)
        {
            var sql = new QueryStringBuilder().TransactionBuilder(account, entities);
            return _context.Transactions.SqlQuery(sql)
                .OrderBy(t => t.DateTime);
        }
    }
}
