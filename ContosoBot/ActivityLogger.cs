using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Bot.Builder.History;
using Microsoft.Bot.Connector;
using Activity = ContosoData.Model.Activity;

namespace ContosoBot
{
    public class ActivityLogger : IActivityLogger
    {
        Task IActivityLogger.LogAsync(IActivity activity)
        {
            IMessageActivity msg = activity.AsMessageActivity();
            using (ContosoData.ContosoDataContext dataContext = new ContosoData.ContosoDataContext())
            {
                var newActivity = Mapper.Map<IMessageActivity, Activity>(msg);
                if (string.IsNullOrEmpty(newActivity.Id))
                    newActivity.Id = Guid.NewGuid().ToString();
                dataContext.ActivityLogs.Add(newActivity);
                dataContext.SaveChanges();
            }
            return Task.CompletedTask;
        }
    }
}