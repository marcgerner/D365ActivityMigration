using Microsoft.Xrm.Sdk;
using System;

namespace DeltaN.BusinessSolutions.ActivityMigration
{
    public class SetModifiedBy : IPlugin
    {
        #region Secure/Unsecure Configuration Setup
        private string _secureConfig = null;
        private string _unsecureConfig = null;

        public SetModifiedBy(string unsecureConfig, string secureConfig)
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
                tracer.Trace("entity found");

                if (context.PreEntityImages.Contains("preImage"))
                {
                    Entity preImageEntity = context.PreEntityImages["preImage"];
                    tracer.Trace("preImage found");

                    if (preImageEntity.Contains("modifiedby") && preImageEntity.Contains("dnbs_overriddenmodifiedby"))
                    {
                        tracer.Trace("dnbs_overriddenmodifiedby has a value: " + preImageEntity["dnbs_overriddenmodifiedby"]);

                        entity["modifiedby"] = preImageEntity["dnbs_overriddenmodifiedby"];
                        tracer.Trace("modifiedby overwritten with dnbs_overriddenmodifiedby");
                    }
                }
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }
    }
}
