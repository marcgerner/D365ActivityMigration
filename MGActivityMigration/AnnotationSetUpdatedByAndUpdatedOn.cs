using System;
using Microsoft.Xrm.Sdk;

namespace DeltaN.BusinessSolutions.ActivityMigration
{
    public class AnnotationSetUpdatedByAndUpdatedOn : IPlugin
    {
        #region Secure/Unsecure Configuration Setup
        private string _secureConfig = null;
        private string _unsecureConfig = null;

        public AnnotationSetUpdatedByAndUpdatedOn(string unsecureConfig, string secureConfig)
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
                if (context.InputParameters["Target"] is Entity entity && entity.LogicalName == "annotation" && entity.Contains("notetext"))
                {
                    var notetext = (string)entity["notetext"];
                    if (notetext != null && notetext.StartsWith("{")) //JSON
                    {
                        var annotationDto = AnnotationDto.ParseJson(notetext);
                        tracer.Trace(notetext);
                        entity["modifiedby"] = new EntityReference("systemuser", annotationDto.modifiedby);
                        entity["modifiedon"] = annotationDto.modifiedon;
                        entity["notetext"] = annotationDto.notetext;
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