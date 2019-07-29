﻿using Microsoft.Xrm.Sdk;
using System;
using Xrm.Models.Crm;
using Xrm.Models.Interfaces;

namespace Xrm.Plugin.Base
{
    public class LocalPluginContext
    {
        internal IServiceProvider ServiceProvider { get; }

        internal IOrganizationServiceWrapper OrgServiceWrapper { get; }

        internal IPluginExecutionContext PluginExecutionContext { get; }

        internal ITracingService TracingService { get; }

        #region XrmProjectTemplate
        public Entity GetTarget()
        {
            return (Entity)this.PluginExecutionContext.InputParameters["Target"];
        }

        public EntityReference GetTargetReference()
        {
            return (EntityReference)this.PluginExecutionContext.InputParameters["Target"];
        }

        public T GetTarget<T>() where T : Entity
        {
            Entity target = GetTarget();
            return target.ToEntity<T>();
        }
        #endregion


        internal LocalPluginContext(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvider");
            }

            // Obtain the execution context service from the service provider.
            this.PluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the tracing service from the service provider.
            this.TracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the Organization Service factory service from the service provider
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            // Use the factory to generate the Organization Service.
            IOrganizationService orgService = factory.CreateOrganizationService(this.PluginExecutionContext.UserId);
            IOrganizationService orgServiceAsSystem = factory.CreateOrganizationService(null);

            OrgServiceWrapper = new OrganizationServiceWrapper(orgService, orgServiceAsSystem);
        }

        internal void Trace(string message)
        {
            if (string.IsNullOrWhiteSpace(message) || this.TracingService == null)
            {
                return;
            }

            if (this.PluginExecutionContext == null)
            {
                this.TracingService.Trace(message);
            }
            else
            {
                this.TracingService.Trace(
                    "{0}, Correlation Id: {1}, Initiating User: {2}",
                    message,
                    this.PluginExecutionContext.CorrelationId,
                    this.PluginExecutionContext.InitiatingUserId);
            }
        }
    }
}