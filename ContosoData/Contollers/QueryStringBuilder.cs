using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContosoData.Model;

namespace ContosoData.Contollers
{
    class QueryStringBuilder
    {
        //TODO: Use StringBuilder instead of string concatenation
        public string TransactionBuilder(Account account, EntityProps entities)
        {
            //if no usable entities supplied by user, return latest 5 transaction query string
            if (entities == null)
                return "SELECT TOP 5 * FROM Transactions WHERE DateTime = (SELECT max(DateTime) FROM Transactions)";

            //start building query string
            string query = "SELECT ";

            //if query contains the word 'last' or 'first'
            if (!string.IsNullOrEmpty(entities.OrdinalTense))
            {
                //ordianl tense (last/first N transactions)
                //TODO: add support for 'last 2/4/6/etc.'
                switch (entities.OrdinalTense)
                {
                    case "first": //todo: add support for 'first'
                    case "last":
                        //if user specified recepient name, fetch latest transaction to that recepient
                        if (!string.IsNullOrEmpty(entities.Encyclopedia))
                            query += $"* FROM Transactions WHERE DateTime IN (SELECT max(DateTime) FROM Transactions WHERE RecepientName LIKE '%{entities.Encyclopedia}%') AND RecepientName LIKE '%{entities.Encyclopedia}%'";
                        else
                            query += "TOP 1 * FROM Transactions WHERE DateTime = (SELECT max(DateTime) FROM Transactions)";
                        
                        //further additions to the query will not change the result. Return the query and done.
                        return query;
                }
            }
            else if (entities.DateRange != null)
            {
                query += "* FROM Transactions WHERE ";

                //dates
                //TODO: check collisions if OrdinalTense is set as well
                switch (entities.DateRange.Type)
                {
                    case "daterange":
                    case "datetimerange":
                        query +=
                            $"DateTime >= '{entities.DateRange.Start:yyyy-MM-dd}' AND DateTime <= '{entities.DateRange.End:yyyy-MM-dd}' AND ";
                        break;
                    case "date":
                        query += $"DateTime = '{entities.DateRange.Start:yyyy-MM-dd}' AND ";
                        break;
                }
            }
            else
            {
                query += "* FROM Transactions WHERE ";
            }

            //recepient
            //DONE: Support partial names
            query += string.IsNullOrEmpty(entities.Encyclopedia) ? "" : $"RecepientName LIKE '%{entities.Encyclopedia}%' AND ";

            //currency (amount of transaction)
            if (entities.MoneyAmount != 0)
            {
                query += "Amount ";

                //check if user specified if transaction amount is 'over' or 'under' certain number
                switch (entities.ComparisonOperator)
                {
                    case "lessThan":
                        query += "< ";
                        break;
                    case "moreThan":
                        query += "> ";
                        break;
                    default:
                        query += "= ";
                        break;
                }

                query += entities.MoneyAmount;
            }

            //return transactions that mach account id
            query += query.EndsWith("AND ") ? $"AccountId = {account.Id}" : $" AND AccountId = {account.Id}";

            //check if sql query ends with 'AND '
            return query;
        }
    }
}
