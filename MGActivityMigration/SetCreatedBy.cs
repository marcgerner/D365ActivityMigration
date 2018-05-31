using Microsoft.Xrm.Sdk;
using System;

namespace DeltaN.BusinessSolutions.ActivityMigration
{
    public class SetCreatedBy : IPlugin
    {
        #region Secure/Unsecure Configuration Setup
        private string _secureConfig = null;
        private string _unsecureConfig = null;

        public SetCreatedBy(string unsecureConfig, string secureConfig)
        {
            _secureConfig = secureConfig;
            _unsecureConfig = unsecureConfig;
        }
        #endregion
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);

            try
            {
                Entity entity = (Entity)context.InputParameters["Target"];
                tracer.Trace("Entiteit gevonden");

                if (entity.Contains("createdby") && entity.Contains("dnbs_overriddencreatedby"))
                {
                    tracer.Trace("dnbs_overriddencreatedby heeft als waarde: " + entity["dnbs_overriddencreatedby"]);

                    entity["createdby"] = entity["dnbs_overriddencreatedby"];
                    tracer.Trace("createdby overschreven met dnbs_overriddencreatedby");
                }
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }
    }
}
