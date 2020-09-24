using Microsoft.Xrm.Sdk;

using System;

namespace DeltaN.BusinessSolutions.ActivityMigration
{
    public class SetModifiedOn : IPlugin
    {
        #region Secure/Unsecure Configuration Setup
        private string _secureConfig = null;
        private string _unsecureConfig = null;

        public SetModifiedOn(string unsecureConfig, string secureConfig)
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
                tracer.Trace("Plugin started");
                
                if (context.InputParameters["Target"] is Entity entity)
                {
                    if (context.PreEntityImages.Contains("preImage"))
                    {
                        Entity preImageEntity = context.PreEntityImages["preImage"];

                        var attributeName = preImageEntity.Attributes.GetAttributeNameThatEndsBy(tracer, "_overriddenmodifiedon");

                        if (attributeName != null && preImageEntity.Contains(attributeName))
                        {
                            tracer.Trace($"{attributeName} contains no data, {preImageEntity[attributeName]}");

                            entity["modifiedon"] = preImageEntity[attributeName];
                            tracer.Trace($"modifiedon overwritten with {attributeName}");
                        }
                    }
                    else
                    {
                        var attributeName = entity.Attributes.GetAttributeNameThatEndsBy(tracer, "_overriddenmodifiedon");

                        if (attributeName != null)
                        {
                            if (entity.Attributes.Contains(attributeName) &&
                                entity.Attributes.Contains("modifiedon") == false)
                            {
                                tracer.Trace("modifiedon contains no data");

                                entity.Attributes.Add("modifiedon", entity[attributeName]);
                                tracer.Trace($"modifiedon filled with {attributeName}, {entity[attributeName]}");
                            }
                            else if (entity.Attributes.Contains(attributeName) && entity.Attributes.Contains("modifiedon"))
                            {
                                tracer.Trace("modifiedon already contains data");

                                entity["modifiedon"] = entity[attributeName];
                                tracer.Trace($"modifiedon overwritten with {attributeName}, {entity[attributeName]}");
                            }
                        }
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
