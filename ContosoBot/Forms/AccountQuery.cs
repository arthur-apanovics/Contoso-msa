using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ContosoData;
using ContosoData.Contollers;
using ContosoData.Model;
using Microsoft.Bot.Builder.FormFlow;

namespace ContosoBot.Forms
{
    [Serializable]
    public class AccountQuery
    {

        public AccountQuery()
        {
            Accounts = new AccountDataController().GetAccounts();
        }


        public IEnumerable<Account> Accounts { get; set; }
        [Prompt("select")]
        public string SelectedAccount { get; set; }


        public static IForm<AccountQuery> BuildForm()
        {
            return new FormBuilder<AccountQuery>()
                .Message("Select account")
                .Build();
        }


    }
}