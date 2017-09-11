using System.Web.Http;
using Autofac;
using ContosoData.Contollers;
using ContosoData.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;

namespace ContosoBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Microsoft.Bot.Connector.IMessageActivity, Activity>()
                    .ForMember(dest => dest.FromId, opt => opt.MapFrom(src => src.From.Id))
                    .ForMember(dest => dest.RecipientId, opt => opt.MapFrom(src => src.Recipient.Id))
                    .ForMember(dest => dest.FromName, opt => opt.MapFrom(src => src.From.Name))
                    .ForMember(dest => dest.RecipientName, opt => opt.MapFrom(src => src.Recipient.Name));
            });

            var builder = new ContainerBuilder();

            //TODO: Uncomment to enable logging.
            //builder.RegisterType<ActivityLogger>().AsImplementedInterfaces().InstancePerDependency();

            builder.RegisterModule(new ReflectionSurrogateModule());
            builder.RegisterModule<GlobalMessageHandlersBotModule>();
            builder.Update(Conversation.Container);
        }
    }
}
