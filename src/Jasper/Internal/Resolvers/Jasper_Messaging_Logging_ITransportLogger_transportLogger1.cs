using Lamar.IoC;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Jasper.Internal.Resolvers
{
    // START: Jasper_Messaging_Logging_ITransportLogger_transportLogger1
    public class Jasper_Messaging_Logging_ITransportLogger_transportLogger1 : Lamar.IoC.Resolvers.SingletonResolver<Jasper.Messaging.Logging.ITransportLogger>
    {
        private readonly Microsoft.Extensions.Logging.ILoggerFactory _loggerFactory477442712;
        private readonly Lamar.IoC.Scope _topLevelScope;

        public Jasper_Messaging_Logging_ITransportLogger_transportLogger1([Lamar.Named("loggerFactory2")] Microsoft.Extensions.Logging.ILoggerFactory loggerFactory477442712, Lamar.IoC.Scope topLevelScope) : base(topLevelScope)
        {
            _loggerFactory477442712 = loggerFactory477442712;
            _topLevelScope = topLevelScope;
        }



        public override Jasper.Messaging.Logging.ITransportLogger Build(Lamar.IoC.Scope scope)
        {
            var nulloMetrics = new Jasper.Messaging.Logging.NulloMetrics();
            return new Jasper.Messaging.Logging.TransportLogger(_loggerFactory477442712, nulloMetrics);
        }

    }

    // END: Jasper_Messaging_Logging_ITransportLogger_transportLogger1
    
    
}
