using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContosoData.Model;

namespace ContosoData.Contollers
{
    public class AccountDataController
    {
        private ContosoDataContext _context;
        public AccountDataController()
        {
            _context = new ContosoDataContext();
        }
        public IEnumerable<Account> GetAccounts()
        {
            return _context.Accounts;
        }

        //TODO: Change account from int back to Account
        public IEnumerable<Transaction> GetTransactionRangeByDate(Account account, DateTime dateStart, DateTime dateEnd)
        {
            return _context.Transactions.Where(t =>
                t.DateTime >= dateStart &&
                t.DateTime <= dateEnd &&
                t.AccountId == account.Id); //TODO: change back to Account from int
        }
    }
}
