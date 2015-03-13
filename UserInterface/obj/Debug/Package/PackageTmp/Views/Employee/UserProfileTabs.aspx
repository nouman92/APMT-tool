<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	User Profile
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $("#userProfileTabs").tabs({ selected: parseInt($("#userAccountTabNumber").val(), 10) });
        });

    </script>

    <input type="hidden" id="userAccountTabNumber" name="userAccountTabNumber" value='<%: Session["userAccountTabNumber"] %>' />
    
    <div id="userProfileTabs" >
         <ul>
             <li><a href='<%: Url.Action("ViewProfile", "Employee") %>'><span>Personal Info</span></a></li>
             <li><a href='<%: Url.Action("ChangePassword", "Employee") %>'><span>Change Password</span></a></li>
         </ul>
    </div>

</asp:Content>

