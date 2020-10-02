using Microsoft.Xrm.Sdk;

using System;

namespace DeltaN.BusinessSolutions.ActivityMigration
{
    public class PostSetCreatedBy : IPlugin
    {
        #region Secure/Unsecure Configuration Setup
        private string _secureConfig = null;
        private string _unsecureConfig = null;

        public PostSetCreatedBy(string unsecureConfig, string secureConfig)
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
                if (context.InputParameters["Target"] is Entity entity && entity.Contains("text"))
                {
                    var text = (string)entity["text"];
                    if (text != null && text.StartsWith("{")) //JSON
                    {
                        var annotationDto = DataTransferObject.ParseJson(text);
                        tracer.Trace(text);
                        entity["createdby"] = new EntityReference("systemuser", annotationDto.createdby);
                        entity["modifiedby"] = new EntityReference("systemuser", annotationDto.modifiedby);
                        entity["modifiedon"] = annotationDto.modifiedon;
                        entity["text"] = annotationDto.originalfieldvalue;
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
