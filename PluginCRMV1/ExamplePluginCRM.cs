using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;

namespace PluginCRMV1
{
    public class ExamplePluginCRM : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the tracing service atau Get Tracing Service
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.
            // Service provider == Crm Sales???
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                // get target entity from the input parameters.
                Entity entity = (Entity)context.InputParameters["Target"];

                // Obtain the organization service reference which you will need for  
                // web service calls. 
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                try
                {
                    // Businis Logic nya di sini 
                    Entity followup = new Entity("task");

                    followup["subjet"] = "Send email to the new customer.";
                    followup["descripton"] = "Isi emailnya";
                    followup["scheduledStart"] = DateTime.Now.AddDays(7);
                    followup["scheduledEnd"] = DateTime.Now.AddDays(7);
                    followup["category"] = context.PrimaryEntityName;

                    dynamic id = context.OutputParameters.Contains("id").ToString();

                    if (context.OutputParameters.Contains("id"))
                    {

                        Guid regardingobjectid = new Guid(context.OutputParameters["id"].ToString());
                        string regardingobjectidType = "account";

                        followup["regardingobjectid"] = new EntityReference(regardingobjectidType, regardingobjectid);
                    }

                    tracingService.Trace("Folloup pulgin: creating task activity. == ", id);
                    service.Create(followup);

                }
                catch (Exception e)
                {
                    tracingService.Trace("FollowUpPlugin : {0}", e.Message.ToString());
                    throw;
                }
            }
        }
    }
}
