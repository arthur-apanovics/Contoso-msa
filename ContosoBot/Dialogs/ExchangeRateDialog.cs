using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ContosoData.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;

namespace ContosoBot.Dialogs
{
    public class ExchangeRateDialog : IDialog
    {
        private EntityProps _entityProps;

        public ExchangeRateDialog(LuisResult luisResult)
        {
            _entityProps = new EntityAssigner().AssignEntities(luisResult);
        }

        public Task StartAsync(IDialogContext context)
        {
            throw new NotImplementedException();
        }
    }
}