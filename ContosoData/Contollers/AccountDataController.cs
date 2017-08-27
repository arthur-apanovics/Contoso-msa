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
    }
}
