using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
            //TODO: Come up with a cleaner way of checking that enough info has been supplied
            if (entities.DateRange == null &&
                entities.Currency == 0 &&
                string.IsNullOrEmpty(entities.Encyclopedia) &&
                string.IsNullOrEmpty(entities.OrdinalTense))
                    entities = null;

            var sql = new QueryStringBuilder().TransactionBuilder(account, entities);
            return _context.Transactions.SqlQuery(sql)
                .OrderBy(t => t.DateTime);
        }

        public static bool PerformInternalTransfer(Account activeAccount, EntityProps entities)
        {
            try
            {
                var accountToDeduct = _context.Accounts
                    .FirstOrDefault(a => a.Id == activeAccount.Id);
                var accountToAdd = _context.Accounts
                    .FirstOrDefault(a => a.Id == entities.Account.Id);

                accountToDeduct.Balance -= entities.Currency;
                accountToAdd.Balance += entities.Currency;

                _context.SaveChanges();

                return true;
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}
