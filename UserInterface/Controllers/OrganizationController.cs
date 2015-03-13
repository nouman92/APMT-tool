using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Repository.Abstract;
using UserInterface.Models;
using Domain.Entities;

namespace UserInterface.Controllers
{
    public class OrganizationController : BaseController
    {
        private IOrganizationRepository _orgRepository;

        public OrganizationController(IOrganizationRepository pOrgRepository)
        {
            _orgRepository = pOrgRepository;
        }

        [Authorize(Roles = "1")]
        public ActionResult OrganizationTabs()
        {
            return View();
        }

        [Authorize(Roles = "1")]
        public ActionResult OrganizationInfo()
        {
            Session["orgTabNumber"] = 0;
            return View(_orgRepository.GetOrgInfo());
        }

        [Authorize(Roles = "1")]
        public ActionResult EditOrganizationInfo()
        {
            // Clearing the lists.
            ClearLists(1);
            return View(_orgRepository.GetOrgInfo());
        }

        [Authorize(Roles = "1")]
        [HttpPost]
        public ActionResult EditOrganizationInfo(FormCollection pFormData)
        {
            string[] data = base.CollectData(pFormData);
            bool flag = false;

            if (base.IsValidData)
            {
                if (_orgRepository.EditOrgInfo(data, RenderedControlsInfo.L1ControlsID.ToArray()))
                {
                    base.ShowMessage("Organization information updated successfully.", false);
                }

                else
                {
                    base.ShowMessage("Organization information did not update successfully.", true);
                }

                flag = true;
            }

            ClearLists(1);

            if (flag)
            {
                return RedirectToAction("OrganizationTabs");
            }

            else
            {
                return RedirectToAction("EditOrganizationInfo");
            }
        }

        public string GetOrganizationName()
        {
            return _orgRepository.GetOrganizationName();
        }

        [Authorize(Roles = "1")]
        public ActionResult RiskList()
        {
            return View(_orgRepository.RiskList());
        }

        [Authorize(Roles = "1")]
        public ActionResult DisplayRiskList()
        {
            Session["orgTabNumber"] = 1;
            return View(_orgRepository.RiskList());
        }

        [Authorize(Roles = "1")]
        public ActionResult AddRisk()
        {
            int index = 0;

            // Creating risk category list.
            SelectListItem[] categories = new SelectListItem[_orgRepository.RiskCategories().Count()];

            foreach (var item in _orgRepository.RiskCategories())
            {
                categories[index++] = new SelectListItem { Text = "" + item.CategoryName, Value = "" + item.CategoryID };
            }

            ViewData["categories"] = categories;

            // Clearing the lists.
            ClearLists(1);
            return View();
        }

        [Authorize(Roles = "1")]
        [HttpPost]
        public ActionResult AddRisk(FormCollection pFormData)
        {
            string[] data = base.CollectData(pFormData);
            bool flag = false;

            if (base.IsValidData)
            {
                if (_orgRepository.AddRisk(data[0], int.Parse(pFormData[1]), true))
                {
                    base.ShowMessage("Risk added successfully.", false);
                }

                else
                {
                    base.ShowMessage("Risk did not add successfully.", true);
                }

                flag = true;
            }

            ClearLists(1);

            if (flag)
            {
                return RedirectToAction("OrganizationTabs");
            }

            else
            {
                return RedirectToAction("AddRisk");
            }
        }

        [Authorize(Roles = "1")]
        public ActionResult EditRisk(int pRiskID)
        {
            // Clearing the lists.
            ClearLists(1);
            var risk = _orgRepository.GetRiskByID(pRiskID);
            // Creating risk category list.
            SelectListItem[] riskCategories = new SelectListItem[_orgRepository.RiskCategories().Count()];
            int index = 0;

            foreach (var item in _orgRepository.RiskCategories())
            {
                riskCategories[index++] = new SelectListItem { Text = item.CategoryName, Value = "" + item.CategoryID };
            }

            ViewData["riskCategories"] = riskCategories;
            ViewData["selectedValue"] = risk.Category;

            return View(risk);
        }

        [Authorize(Roles = "1")]
        [HttpPost]
        public ActionResult EditRisk(FormCollection pFormData, int pRiskID)
        {
            string[] data = base.CollectData(pFormData);
            bool flag = false;

            if (base.IsValidData)
            {
                if (_orgRepository.EditRisk(pRiskID, data[0], int.Parse(pFormData[1])))
                {
                    base.ShowMessage("Risk description updated successfully.", false);
                }

                else
                {
                    base.ShowMessage("Risk description did not update successfully.", true);
                }

                flag = true;
            }

            ClearLists(1);

            if (flag)
            {
                return RedirectToAction("OrganizationTabs");
            }

            else
            {
                return RedirectToAction("EditRisk", new { pRiskID = pRiskID });
            }
        }

        [Authorize(Roles = "1")]
        public int AddRiskCategory(string pCategoryName)
        {
            if (base.VerifyInput(pCategoryName, @"^[a-zA-Z\s]*$"))
            {
                return _orgRepository.AddRiskCategory(pCategoryName);
            }

            else
            {
                return -2;
            }
        }

        [Authorize(Roles = "1")]
        public ActionResult Roles()
        {
            Session["orgTabNumber"] = 2;
            return View(_orgRepository.RoleList());
        }

        [Authorize(Roles = "1")]
        public ActionResult AddRole()
        {
            // Clearing the lists.
            ClearLists(1);
            return View(_orgRepository.AccessRightList());
        }

        [Authorize(Roles = "1")]
        [HttpPost]
        public ActionResult AddRole(FormCollection pFormData)
        {
            string[] data = base.CollectData(pFormData);

            if (base.IsValidData)
            {
                int result = _orgRepository.AddRole(RenderedControlsInfo.L1ControlsID.ToArray().ToArray(), data);

                if (result == 1)
                {
                    base.ShowMessage("Role Added successfully.", false);
                }

                else if (result == -1)
                {
                    base.ShowMessage("Role did not add successfully.", true);
                }

                else
                {
                    base.ShowMessage("Role Name already exists.", true);
                }

                ClearLists(1);
                return RedirectToAction("OrganizationTabs");
            }

            else
            {
                return RedirectToAction("AddRole");
            }
        }

        [Authorize(Roles = "1")]
        public bool IsRoleNameExist(string pRoleName)
        {
            return _orgRepository.IsRoleNameExist(pRoleName);
        }

        [Authorize(Roles = "1")]
        public ActionResult EditRole(int pRoleID)
        {
            List<int> rights = _orgRepository.AccessRightListByID(pRoleID);
            IEnumerable<AccessRight> rightList = _orgRepository.AccessRightList();
            base.SelectedRoleID = pRoleID;

            if (rights.Count > 0)
            {
                foreach (AccessRight r in rightList)
                {
                    if (rights.Contains(r.RightID))
                        r.RightID *= -1;
                }
            }

            // Clearing the lists.
            ClearLists(1);
            return View(rightList);
        }

        [Authorize(Roles = "1")]
        [HttpPost]
        public ActionResult EditRole(FormCollection pFormData)
        {
            string[] data = base.CollectData(pFormData);

            if (base.IsValidData)
            {
                if (_orgRepository.EditRole(data, RenderedControlsInfo.L1ControlsID.ToArray(), base.SelectedRoleID))
                {
                    base.ShowMessage("Role rights updated successfully.", false);
                }

                else
                {
                    base.ShowMessage("Role rights did not update successfully.", true);
                }

                ClearLists(1);
                return RedirectToAction("OrganizationTabs");
            }

            else
            {
                ClearLists(1);
                return RedirectToAction("OrganizationTabs");
            }
        }

        public string GetAnnouncement()
        {
            string message = _orgRepository.GetAnnouncement();

            if (message == "" || message == null)
            {
                message = "There is no announcement.";
            }

            return message;
        }

        [Authorize(Roles = "1")]
        public ActionResult Announcement()
        {
            Session["orgTabNumber"] = 3;
            ViewData["message"] = _orgRepository.GetAnnouncement();
            return View();
        }

        [Authorize(Roles = "1")]
        public bool UpdateAnnouncement(string pMessage)
        {
            return _orgRepository.UpdateAnnouncement(pMessage);
        }
    }
}
