<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Organization
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $("#organizationTabs").tabs({ selected: parseInt($("#orgTabNumber").val(), 10) });
        });

    </script>

    <input type="hidden" id="orgTabNumber" name="orgTabNumber" value='<%: Session["orgTabNumber"] %>' />

    <div id="organizationTabs">
         <ul>
             <li><a href='<%: Url.Action("OrganizationInfo", "Organization") %>'><span>Organization Info</span></a></li>
             <li><a href='<%: Url.Action("DisplayRiskList", "Organization") %>'><span>Risk List</span></a></li>
             <li><a href='<%: Url.Action("Roles", "Organization") %>'><span>Role List</span></a></li>
             <li><a href='<%: Url.Action("Announcement", "Organization") %>'><span>Announcement</span></a></li>
        </ul>
    </div>

</asp:Content>

