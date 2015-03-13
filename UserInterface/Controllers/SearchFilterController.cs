using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Repository.Abstract;
using UserInterface.HtmlHelpers;
using UserInterface.Models;
using System.Text;
using Domain.Entities;

namespace UserInterface.Controllers
{
    [Authorize(Roles = "7")]
    public class SearchFilterController : BaseController
    {
        private ISearchFilterRepository _filterRepository;
        // List Search Filter Categories. 
        private List<string> Filter_Name;
        // Array containing correct possible relation between Search Filter categories.
        private int[,] SearchByFilter;

        public SearchFilterController(ISearchFilterRepository pFilterRepository)
        {
            _filterRepository = pFilterRepository;
            Filter_Name = new List<string>();
            SearchByFilter = new int[,] {  { 1, 1, 1, 1, 0, 0, 0 }, { 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 0, 0, 0 },{ 1, 1, 1, 1, 0, 0, 0 },
                                            { 0, 1, 0, 0, 1, 1, 0 }, { 0, 1, 0, 0, 1, 1, 0 } };
            Filter_Name.Add("Employee");
            Filter_Name.Add("Project");
            Filter_Name.Add("Sprint");
            Filter_Name.Add("Issue");
            Filter_Name.Add("Component");
            Filter_Name.Add("SubComponent");
            Filter_Name.Add("Risk");
        }

        public ActionResult SearchFilterTabs(int pTabNumber = -1)
        {
            if (pTabNumber != -1)
            {
                Session["filtersTabNumber"] = pTabNumber;
            }

            return View();
        }

        public ActionResult DisplayFilters()
        {
            Session["filtersTabNumber"] = 0;
            return View(_filterRepository.GetFilters(base.UserID));
        }

        public ActionResult RemoveFilter(long pFilterID, long pEmpID)
        {
            if (_filterRepository.RemoveFilter(pFilterID, pEmpID))
            {
                base.ShowMessage("Search Filter is removed successfully.", false);
            }
            
            else
            {
                base.ShowMessage("Search Filter is not remove successfully.", true);
            }
            
            return RedirectToAction("SearchFilterTabs", new { pTabNumber = 0 });
        }

        public ActionResult ExecuteFilter(string pFilterValue)
        {
            string[] filterData = pFilterValue.Split(':');
            return TestFilter(filterData[0], filterData[1], filterData[2], filterData[3], filterData[4], filterData[5], filterData[6]);
        }
        
        public ActionResult CreateFilter()
        {
            SelectListItem[] searchOptions = new SelectListItem[6];
            
            for (int i = 0; i < Filter_Name.Count - 1; i++)
            {
                searchOptions[i] = new SelectListItem { Text = Filter_Name[i], Value = i.ToString() };
            }
            
            ViewData["searchOptions"] = searchOptions;
            Session["filtersTabNumber"] = 1;
            ClearLists(1);
            return View();
        }

        public string Validation(string pValue, string pRegExp, string pErrMsg, int pValueID)
        {
            if (pValue=="" || pRegExp == "" || base.VerifyInput(pValue, pRegExp))
            {
                return (pValueID+",No");
            }
            
            return (pValueID+","+pErrMsg);
        }

        [HttpPost]
        public ActionResult CreateFilter(FormCollection pFormData)
        {
            return RedirectToAction("SearchFilterTabs", new { pTabNumber=0});
        }

        [HttpPost]
        public ActionResult TestFilter(string pSearch, string pSearchBy, string pAttribute, string pValue, string pOperator, string pMatch, string pValueType)
        {
            // All value are samicolon ';' sprated, so remove the samicolon. 
            pSearchBy = pSearchBy.Substring(0, pSearchBy.Length - 1);
            pAttribute = pAttribute.Substring(0, pAttribute.Length - 1);
            pValue = pValue.Substring(0, pValue.Length - 1);
            pOperator = pOperator.Substring(0, pOperator.Length - 1);
            pMatch = pMatch.Substring(0, pMatch.Length - 1);
            pValueType = pValueType.Substring(0, pValueType.Length - 1);

            string[] matchArray = pMatch.Split(';');
            string[] operatorArray = pOperator.Split(';');
            string[] valueTypeArray = pValueType.Split(';');
            int[] operators = new int[pOperator.Length];
            int[] match = new int[pMatch.Length];
            int[] valueType = new int[pMatch.Length];

            for (int i = 0; i < operatorArray.Length; i++)
            {
                operators[i] = operatorArray[i] == "OR" ? 0 : 1;
                valueType[i] = Convert.ToInt32(valueTypeArray[i]);

                switch (matchArray[i])
                {
                    case "EqualTo":
                    case "Exactly":
                        match[i] = 0;
                        break;
                    
                    case "LessThan":
                    case "Contain":
                        match[i] = 1;
                        break;
                    
                    case "GreaterThan":
                    case "StartWith":
                        match[i] = 2;
                        break;
                    
                    case "EndWith":
                        match[i] = 3;
                        break;
                }
            }

            switch (pSearch)
            {
                case "Employee":
                    return View("EmployeeResult", _filterRepository.SearchEmployee(pSearchBy.Split(';'), pAttribute.Split(';'), pValue.Split(';'), operators, match, valueType));
                
                case "Project":
                    return View("ProjectResult", _filterRepository.SearchProject(pSearchBy.Split(';'), pAttribute.Split(';'), pValue.Split(';'), operators, match, valueType));
                
                case "Sprint":
                    return View("SprintResult", _filterRepository.SearchSprint(pSearchBy.Split(';'), pAttribute.Split(';'), pValue.Split(';'), operators, match, valueType));
                
                case "Issue":
                    return View("IssueResult", _filterRepository.SearchIssue(pSearchBy.Split(';'), pAttribute.Split(';'), pValue.Split(';'), operators, match, valueType));
                
                case "Component":
                    return View("ComponentResult", _filterRepository.SearchComponent(pSearchBy.Split(';'), pAttribute.Split(';'), pValue.Split(';'), operators, match, valueType));
                
                default:
                    return View("SubComponentResult", _filterRepository.SearchSubComponent(pSearchBy.Split(';'), pAttribute.Split(';'), pValue.Split(';'), operators, match, valueType));
            }
        }

        public bool SaveFilter(string pFilterName, string pSearch, string pSearchBy, string pAttribute, string pValue, string pOperator, string pMatch, string pValueType)
        {
            string searchFilterData = pSearch + ":" + pSearchBy + ":" + pAttribute + ":" + pValue + ":" + pOperator + ":" + pMatch + ":" + pValueType;
            return _filterRepository.CreateFilter(pFilterName, searchFilterData, base.UserID);
        }

        public bool IsFilterNameExist(string pFilterName)
        {
            return _filterRepository.IsFilterNameExist(pFilterName, base.UserID);
        }

        // "SearchBy" seach on what basis eg Employee or Project etc.
        public ActionResult SearchBy(int pSelectedID, int pSearchByRowID)
        {
            string searchByOptions = "";
            
            for (int i = 0; i < 7; i++)
                if (SearchByFilter[pSelectedID, i] == 1)
                    searchByOptions += Filter_Name[i] + ";";
            
            searchByOptions = searchByOptions.Substring(0, searchByOptions.Length - 1);
            ViewData["searchByOptions"] = searchByOptions;
            ViewData["searchByRowID"] = pSearchByRowID;
            return View();
        }

        // "Attributes" seach on what attributes eg EmployeeName or ProjectID etc.
        public ActionResult Attributes(string pSelected, int pAttributeRowID)
        {
            string attributesOptions = "";
            List<string> attributeList = new List<string>();
            
            switch (pSelected)
            {
                case "Employee":
                    attributeList = _filterRepository.GetEmployeeAttributes();
                    attributesOptions = "Employee ID;";
                    break;
                
                case "Project":
                    attributeList = _filterRepository.GetProjectAttributes();
                    attributesOptions = "Project ID;";
                    break;
                
                case "Sprint":
                    attributeList = _filterRepository.GetSprintAttributes();
                    attributesOptions = "Sprint ID;";
                    break;
                
                case "Issue":
                    attributeList = _filterRepository.GetIssueAttributes();
                    attributesOptions = "Issue ID;";
                    break;
                
                case "Component":
                    attributeList = _filterRepository.GetComponentAttributes();
                    attributesOptions = "Component ID;";
                    break;
                
                case "SubComponent":
                    attributeList = _filterRepository.GetSubComponentAttributes();
                    attributesOptions = "SubComponent ID;";
                    break;
                
                case "Risk":
                    attributeList.Add("Category");
                    attributeList.Add("Risk Exposure");
                    attributesOptions = "Risk ID;";
                    break;
            }

            foreach (var item in attributeList)
            {
                attributesOptions += item + ";";
            }

            ViewData["attributesOptions"] = attributesOptions.Substring(0, attributesOptions.Length - 1);
            ViewData["attributeRowID"] = pAttributeRowID;

            return View();
        }

        // "ValueField" search value of attributes e.g. EmployeeName=Shaban or ProjectID=1 etc.
        public ActionResult ValueField(string pSelected, string pSelectedAttribute, int pValueFieldRowID)
        {
            ControlInfo CI = new ControlInfo();
            
            if (pSelectedAttribute == (pSelected + " ID") || pSelectedAttribute == "Risk Exposure")
            {
                CI.ControlID = pValueFieldRowID + 300;
                CI.CanNull = false;
                CI.DefaultValue = "";
                CI.RegularExpression = @"^\d+$";
                CI.ErrorMessage = "*Invalid Numeric Value";
                CI.Type = (ControlType)1;
                CI.ControlAttName = "ID";
                CI.RegExpressionID = -1;
            }
            
            else if (pSelectedAttribute == "Category" && pSelectedAttribute=="Risk")
            {
                CI.ControlID = pValueFieldRowID + 300;
                CI.CanNull = false;
                CI.DefaultValue = _filterRepository.GetRiskCategories();
                CI.RegularExpression = null;
                CI.ErrorMessage = "";
                CI.Type = (ControlType)4;
                CI.ControlAttName = pSelectedAttribute;
                CI.RegExpressionID = -1;
            }
            
            else
            {
                switch (pSelected)
                {
                    case "Employee":
                        EmpAttribute emp = _filterRepository.GetEmployeeField(pSelectedAttribute);
                        CI.ControlID = pValueFieldRowID + 300;
                        CI.CanNull =emp.CanNull;
                        CI.DefaultValue = ((ControlType)emp.FieldType == ControlType.List) ? emp.DefaultValue : "";
                        CI.RegularExpression = (emp.RegularExpression != null) ? emp.RegularExpression1.Value : null;
                        CI.ErrorMessage = (emp.RegularExpression != null) ? emp.RegularExpression1.Error : "";
                        CI.RegExpressionID = (emp.RegularExpression != null) ? emp.RegularExpression1.ExpressionID : -1;
                        CI.ControlAttName = emp.EmpAttName;
                        CI.Type = (ControlType)emp.FieldType;
                        break;
                    
                    case "Project":
                        ProjAttribute proj = _filterRepository.GetProjectField(pSelectedAttribute);
                        CI.ControlID = pValueFieldRowID + 300;
                        CI.CanNull = proj.CanNull;
                        CI.DefaultValue = ((ControlType)proj.FieldType == ControlType.List) ? proj.DefaultValue : "";
                        CI.RegularExpression = (proj.RegularExpression != null) ? proj.RegularExpression1.Value : null;
                        CI.ErrorMessage = (proj.RegularExpression != null) ? proj.RegularExpression1.Error : "";
                        CI.RegExpressionID = (proj.RegularExpression != null) ? proj.RegularExpression1.ExpressionID : -1;
                        CI.ControlAttName = proj.ProjAttName;
                        CI.Type = (ControlType)proj.FieldType;
                        break;
                    
                    case "Sprint":
                        SprintAttribute sprint = _filterRepository.GetSprintField(pSelectedAttribute);
                        CI.ControlID = pValueFieldRowID + 300;
                        CI.CanNull = sprint.CanNull;
                        CI.DefaultValue = ((ControlType)sprint.FieldType == ControlType.List) ? sprint.DefaultValue : "";
                        CI.RegularExpression = (sprint.RegularExpression != null) ? sprint.RegularExpression1.Value : null;
                        CI.ErrorMessage = (sprint.RegularExpression != null) ? sprint.RegularExpression1.Error : "";
                        CI.RegExpressionID = (sprint.RegularExpression != null) ? sprint.RegularExpression1.ExpressionID : -1;
                        CI.ControlAttName = sprint.SprintAttName;
                        CI.Type = (ControlType)sprint.FieldType;
                        break;
                    
                    case "Issue":
                        IssueAttribute issue = _filterRepository.GetIssueField(pSelectedAttribute);
                        CI.ControlID = pValueFieldRowID + 300;
                        CI.CanNull = issue.CanNull;
                        CI.DefaultValue = ((ControlType)issue.FieldType == ControlType.List) ? issue.DefaultValue : "";
                        CI.RegularExpression = (issue.RegularExpression != null) ? issue.RegularExpression1.Value : null;
                        CI.ErrorMessage = (issue.RegularExpression != null) ? issue.RegularExpression1.Error : "";
                        CI.RegExpressionID = (issue.RegularExpression != null) ? issue.RegularExpression1.ExpressionID : -1;
                        CI.ControlAttName = issue.IssueAttName;
                        CI.Type = (ControlType)issue.FieldType;
                        break;
                    
                    case "Component":
                        CompAttribute comp = _filterRepository.GetComponentField(pSelectedAttribute);
                        CI.ControlID = pValueFieldRowID + 300;
                        CI.CanNull = comp.CanNull;
                        CI.DefaultValue = ((ControlType)comp.FieldType == ControlType.List) ? comp.DefaultValue : "";
                        CI.RegularExpression = (comp.RegularExpression != null) ? comp.RegularExpression1.Value : null;
                        CI.ErrorMessage = (comp.RegularExpression != null) ? comp.RegularExpression1.Error : "";
                        CI.RegExpressionID = (comp.RegularExpression != null) ? comp.RegularExpression1.ExpressionID : -1;
                        CI.ControlAttName = comp.CompAttName;
                        CI.Type = (ControlType)comp.FieldType;
                        break;
                    
                    default:// "SubComponent":
                        SubCompAttribute subComp = _filterRepository.GetSubComponentField(pSelectedAttribute);
                        CI.ControlID = pValueFieldRowID + 300;
                        CI.CanNull = subComp.CanNull;
                        CI.DefaultValue = ((ControlType)subComp.FieldType == ControlType.List) ? subComp.DefaultValue : "";
                        CI.RegularExpression = (subComp.RegularExpression != null) ? subComp.RegularExpression1.Value : null;
                        CI.ErrorMessage = (subComp.RegularExpression != null) ? subComp.RegularExpression1.Error : "";
                        CI.RegExpressionID = (subComp.RegularExpression != null) ? subComp.RegularExpression1.ExpressionID : -1;
                        CI.ControlAttName = subComp.SubCompAttName;
                        CI.Type = (ControlType)subComp.FieldType;
                        break;
                }
            }

            return View(CI);
        }

        // "Operator" how data is treated of two searchfilters e.g. Union(OR) or Intersection(AND) etc.
        public ActionResult Operator(int pOperatorRowID)
        {
            string operatorOptions = "OR;AND";
            ViewData["operatorOptions"] = operatorOptions;
            ViewData["operatorRowID"] = pOperatorRowID;
            return View();
        }

        // "Match" how search value compare with actual value e.g. Excatly or Contains etc.
        public ActionResult Match(string pSelectedAttribute, int pMatchRowID)
        {
            string matchOptions = "Exactly;Contain;StartWith;EndWith";
            
            if (pSelectedAttribute == "Risk Exposure" || pSelectedAttribute.EndsWith(" ID"))
                matchOptions = "EqualTo;LessThan;GreaterThan";
            
            ViewData["matchOptions"] = matchOptions;
            ViewData["matchRowID"] = pMatchRowID;
            return View();
        }
    }
}
