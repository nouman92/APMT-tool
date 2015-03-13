<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	User - More Actions
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $("#moreActionsTabs").tabs({ selected: parseInt($("#empMoreActionsTabNumber").val(), 10) });
        });

    </script>

    <input type="hidden" id="empMoreActionsTabNumber" name="empMoreActionsTabNumber" value='<%: Session["empMoreActionsTabNumber"] %>' />
    
    <div style="margin-top:9px;">
        <span title="Selected User" class="selectedItem"><%:Session["employeeName"]%></span>
    </div>

    <div id="topNavigation" class="topNavigation">
        <a title="Add new User" class="topLinkButton" href='<%: Url.Action("AddEmployeeTabs", "Employee") %>'><span>Add</span></a>
        <a title="Go back to Users Tabs" class="topLinkButton" href='<%: Url.Action("EmployeeTabs", "Employee") %>'><span>Users</span></a>
    </div>
    
    <div id="moreActionsTabs" class="spacing">
         <ul>
             <li><a href='<%: Url.Action("DisplayEmployee", "Employee", new { pEmployeeID = (long)Session["employeeID"] }) %>'><span>User Details</span></a></li>
             <li><a href='<%: Url.Action("ResetUserPassword", "Employee") %>'><span>Reset Password</span></a></li>
         </ul>
    </div>

</asp:Content>
