
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	User Home
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $("#homeTabs").tabs({ selected: parseInt($("#homeTabNumber").val(), 10) });
        });

    </script>

    <input type="hidden" id="homeTabNumber" name="homeTabNumber" value='<%: Session["homeTabNumber"] %>' />

    <div id="homeTabs">
         <ul>
             <li><a href='<%: Url.Action("UserAssignedIssues", "Home") %>'><span>Assigned Issues</span></a></li>
             <li><a href='<%: Url.Action("PersonalNotes", "Home") %>'><span>Personal Notes</span></a></li>
             <% if (Roles.IsUserInRole(Session["userDesignation"].ToString(), "3"))
                { %>
             <li><a href='<%: Url.Action("FavoriteProjects", "Home") %>'><span>Favorite Projects</span></a></li>
             <%} %>
         </ul>
    </div>

</asp:Content>
