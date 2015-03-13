using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Repository.Abstract;
using System.Web.Routing;
using UserInterface.Models;

namespace UserInterface.Controllers
{
    [Authorize]
    public class NavigationController : BaseController
    {
        private IEmployeeRepository _empRepository;

        public NavigationController(IEmployeeRepository pEmpRepository)
        {
            _empRepository = pEmpRepository;
        }

        public ActionResult RenderMenu()
        {
            Func<string, NavigationLink> makeLink = menuName => new NavigationLink
            {
                Text = menuName,
                RouteValues = new RouteValueDictionary(new
                {
                    controller = "Navigation",
                    action = menuName.Replace(" ", ""),
                }),

                IsSelected = menuName.Equals(Session["selectedMenu"])
            };

            // Putting Home link at the top.
            List<NavigationLink> navigationLinks = new List<NavigationLink>();
            navigationLinks.Add(makeLink("Home"));

            // Adding other links according to the user rights.
            foreach (string menuName in _empRepository.RightsList(base.UserID))
            {
                navigationLinks.Add(makeLink(menuName));
            }

            return View(navigationLinks);
        }

        public ActionResult Home()
        {
            // For look and feel.
            Session["selectedMenu"] = "Home";
            return RedirectToAction("HomeTabs", "Home");
        }

        [Authorize (Roles="3")]
        public ActionResult Project()
        {
            Session["selectedMenu"] = "Project";
            Session["projectTabNumber"] = 0;
            return RedirectToAction("ProjectTabs", "Project");
        }

        [Authorize(Roles = "4")]
        public ActionResult WorkFlow()
        {
            Session["selectedMenu"] = "Work Flow";
            return RedirectToAction("IssueTabs", "Issue");
        }

        [Authorize(Roles = "6")]
        public ActionResult UserProfile()
        {
            Session["selectedMenu"] = "User Profile";
            return RedirectToAction("UserProfileTabs", "Employee");
        }

        [Authorize(Roles = "2")]
        public ActionResult Users()
        {
            Session["selectedMenu"] = "Users";
            Session["employeeTabNumber"] = 0;
            return RedirectToAction("EmployeeTabs", "Employee");
        }

        [Authorize(Roles = "1")]
        public ActionResult Organization()
        {
            Session["selectedMenu"] = "Organization";
            return RedirectToAction("OrganizationTabs", "Organization");
        }

        [Authorize(Roles = "7")]
        public ActionResult SearchFilter()
        {
            Session["filtersTabNumber"] = 0;
            Session["selectedMenu"] = "Search Filter";
            return RedirectToAction("SearchFilterTabs", "SearchFilter");
        }
    }
}
