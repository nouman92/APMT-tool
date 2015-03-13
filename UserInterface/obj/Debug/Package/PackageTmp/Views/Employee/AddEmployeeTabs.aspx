<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>"%>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
    Add User
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $("#addEmployeeTabs").tabs({ selected: parseInt($("#addEmployeeTabNumber").val(), 10) });
        });

    </script>

    <input type="hidden" id="addEmployeeTabNumber" name="addEmployeeTabNumber" value='<%: TempData["addEmployeeTabNumber"] %>' />

    <div id="topNavigation">
        <span class="topLinkButton2">Add</span>
        <a title="Go back to Users Tabs" class="topLinkButton" href='<%: Url.Action("EmployeeTabs") %>'><span>Users</span></a>
    </div>

    <div id="addEmployeeTabs" class="spacing">
         <ul>
             <li><a href='<%: Url.Action("AddEmployee", "Employee") %>'><span>New User</span></a></li>
             <li><a href='<%: Url.Action("CustomFields", "Employee") %>'><span>Custom Fields</span></a></li>
             <li><a href='<%: Url.Action("CreateCustomField", "Employee") %>'><span>New Custom Field</span></a></li>
         </ul>
    </div>

</asp:Content>
