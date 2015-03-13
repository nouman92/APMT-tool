<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Users    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $("#employeeTabs").tabs({ selected: parseInt($("#employeeTabNumber").val(), 10) });
        });

    </script>

    <input type="hidden" id="employeeTabNumber" name="employeeTabNumber" value='<%: Session["employeeTabNumber"] %>' />

    <div id="topNavigation">
        <a title="Add new User" class="topLinkButton" href='<%: Url.Action("AddEmployeeTabs", "Employee") %>'><span>Add</span></a>
        <span class="topLinkButton2">Users</span>
    </div>
    
    <div id="employeeTabs" class="spacing">
         <ul>
             <li><a href='<%: Url.Action("ShowAllEmployees", "Employee") %>'><span>All Users</span></a></li>
             <li><a href='<%: Url.Action("SearchEmployee", "Employee") %>'><span>Search</span></a></li>
         </ul>
    </div>

</asp:Content>

