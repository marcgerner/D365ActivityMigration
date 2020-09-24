using Microsoft.Xrm.Sdk;

using System;
using System.Linq;

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
                if (context.InputParameters["Target"] is Entity entity && context.PreEntityImages.Contains("preImage"))
                {
                    Entity preImageEntity = context.PreEntityImages["preImage"];

                    var attributeName = preImageEntity.Attributes.GetAttributeNameThatEndsBy(tracer, "_overriddenmodifiedby");
                    
                    if (attributeName != null && preImageEntity.Contains("modifiedby") && preImageEntity.Contains(attributeName))
                    {
                        tracer.Trace($"{attributeName} has value: {preImageEntity[attributeName]}");
                        entity["modifiedby"] = preImageEntity[attributeName];
                        tracer.Trace($"modifiedby overwritten with {attributeName}");
                    }
                }
            }
            catch (Exception exception)
            {
                throw new InvalidPluginExecutionException(exception.Message, exception);
            }
        }
    }
}
