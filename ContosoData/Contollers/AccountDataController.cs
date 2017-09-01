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

        //TODO: Change account from int back to Account
        public static IEnumerable<Transaction> GetTransactionRangeByDate(Account account, DateTime dateStart, DateTime dateEnd)
        {
            return _context.Transactions.Where(t =>
                t.DateTime >= dateStart &&
                t.DateTime <= dateEnd &&
                t.AccountId == account.Id); //TODO: change back to Account from int
        }
    }
}
