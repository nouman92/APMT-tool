using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ninject;
using Ninject.Modules;
using System.Web.Routing;
using Domain.Repository.Abstract;
using Domain.Repository.Concrete;

namespace UserInterface.Controllers.ControllerFactory
{   
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        // A Ninject "kernel" is the thing that can supply object instances.
        private IKernel kernel = new StandardKernel(new ResolveDependency());

        // ASP.NET MVC calls this to get the controller for each request.
        protected override IController GetControllerInstance(RequestContext context, Type controllerType)
        {
            if (controllerType == null)
                return null;

            return (IController)kernel.Get(controllerType);
        }

        // Configures how interfaces are mapped to concrete implementations.
        private class ResolveDependency : NinjectModule
        {
            public override void Load()
            {
                Bind<IOrganizationRepository>().To<SqlOrganizationRepository>();
                Bind<IProjectRepository>().To<SqlProjectRepository>();
                Bind<IIssueRepository>().To<SqlIssueRepository>();
                Bind<IEmployeeRepository>().To<SqlEmployeeRepository>();
                Bind<IComponentRepository>().To<SqlComponentRepository>();
                Bind<ISubComponentRepository>().To<SqlSubComponentRepository>();
                Bind<ISprintRepository>().To<SqlSprintRepository>();
                Bind<ISearchFilterRepository>().To<SqlSearchFilterRepository>();
            }
        }
    }
}