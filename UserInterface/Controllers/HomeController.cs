using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Repository.Abstract;
using UserInterface.Models;
using System.Web.Security;

namespace UserInterface.Controllers
{
    public class HomeController : BaseController
    {
        private IOrganizationRepository _orgRepository;
        private IIssueRepository _issueRepository;
        private IEmployeeRepository _employeeRepository;
        private IProjectRepository _projRepository;
        private ISprintRepository _sprintRepository;

        public HomeController(IOrganizationRepository pOrgRepository, IIssueRepository pIssueRepository, IEmployeeRepository pEmployeeRepository, IProjectRepository pProjectRepository, ISprintRepository pSprintRepository)
        {
            _orgRepository = pOrgRepository;
            _issueRepository = pIssueRepository;
            _employeeRepository = pEmployeeRepository;
            _projRepository = pProjectRepository;
            _sprintRepository = pSprintRepository;
        }

        public ActionResult LogIn(string ReturnUrl)
        {
            // If user is logged in then redirect to home
            // in case of access denied.
            if (base.UserID != -1)
            {
                base.ShowMessage("Access denied.", true);
                Session["selectedMenu"] = "Home";
                return RedirectToAction("HomeTabs");
            }

            return View();
        }

        [HttpPost]
        public ActionResult LogIn(LogInViewModel pLogInObj, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                long empId;
                string empName;
                string empDesignation;

                // Verifying user information.
                if (_employeeRepository.VerifyUser(pLogInObj.UserName, pLogInObj.Password, out empId, out empName, out empDesignation))
                {
                    // Creating user session. UserId is the key and the value is user id retrieved from DB
                    // and similarly UserName.
                    base.UserID = empId;
                    base.UserName = empName;
                    base.UserDesignation = empDesignation;
                    FormsAuthentication.SetAuthCookie(empDesignation, false);

                    // For look and feel.
                    Session["selectedMenu"] = "Home";
                    return RedirectToAction("HomeTabs");
                }

                else
                {
                    TempData["errorMessage"] = "*Invalid user name or password.";
                }
            }

            return View(pLogInObj);
        }

        [Authorize]
        public ActionResult HomeTabs()
        {
            return View();
        }

        [Authorize]
        public ActionResult LogOut()
        {
            Session.RemoveAll();
            FormsAuthentication.SignOut();

            return RedirectToAction("LogIn");
        }

        // Will return the list of all the issues assigned to the logged in user.
        [Authorize]
        public ActionResult UserAssignedIssues()
        {
            Session["homeTabNumber"] = 0;
            Session["showIEditButton"] = "";

            List<bool> isValidSprint = new List<bool>();
            List<bool> isSprintStarted = new List<bool>();
            var issues = _issueRepository.GetAssignedIssues(base.UserID);

            // Verifying the issues' sprint validaity. If sprint has been expired then 
            // do not show the "Effort" and "Execute" link.
            foreach (var issue in issues)
            {
                // Please see the definition of "IsSprintValid" in "SqlSprintRepository".
                isValidSprint.Add(_sprintRepository.IsSprintValid(0, issue.IssueID));                
                isSprintStarted.Add(_sprintRepository.IsSprintStarted(0, issue.IssueID));
            }

            ViewData["isValidSprint"] = isValidSprint;
            ViewData["isSprintStarted"] = isSprintStarted;
            return View(issues);
        }

        [Authorize]
        public ActionResult PersonalNotes()
        {
            Session["homeTabNumber"] = 1;

            return View(_employeeRepository.GetNotes(base.UserID));
        }

        [Authorize (Roles="3")]
        public ActionResult FavoriteProjects()
        {
            Session["homeTabNumber"] = 2;
            // For explanation, see "ProjectTabs" in "ProjectController".
            Session["showPEditButton"] = "";

            return View(_projRepository.GetFavoriteProjects(base.UserID));
        }
    }
}
