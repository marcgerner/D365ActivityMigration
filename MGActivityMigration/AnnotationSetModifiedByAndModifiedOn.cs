using System;
using Microsoft.Xrm.Sdk;

namespace DeltaN.BusinessSolutions.ActivityMigration
{
    public class AnnotationSetModifiedByAndModifiedOn : IPlugin
    {
        #region Secure/Unsecure Configuration Setup
        private string _secureConfig = null;
        private string _unsecureConfig = null;

        public AnnotationSetModifiedByAndModifiedOn(string unsecureConfig, string secureConfig)
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
                if (context.InputParameters["Target"] is Entity entity && entity.LogicalName == "annotation" && entity.Contains("subject"))
                {
                    var subject = (string)entity["subject"];
                    if (subject != null && subject.StartsWith("{")) //JSON
                    {
                        var annotationDto = DataTransferObject.ParseJson(subject);
                        tracer.Trace(subject);
                        entity["modifiedby"] = new EntityReference("systemuser", annotationDto.modifiedby);
                        entity["modifiedon"] = annotationDto.modifiedon;
                        entity["subject"] = annotationDto.originalfieldvalue;
                    }
                }
            }
            catch (Exception exception)
            {
                throw new InvalidPluginExecutionException(exception.Message, exception);
            }
        }
    }

    public class AnnotationSetUpdatedByAndUpdatedOn : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Only for Backward Compability. Against failures when importing the solution because of the missing type "AnnotationSetUpdatedByAndUpdatedOn"
        }
    }
}