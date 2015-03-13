<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Issue
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $("#issueTabs").tabs({ selected: parseInt($("#issueTabNumber").val(), 10) });
        });

    </script>

    <input type="hidden" id="issueTabNumber" name="issueTabNumber" value='<%: Session["issueTabNumber"] %>' />

    <div id="issueTabs">
         <ul>
             <li><a href='<%: Url.Action("WorkFlow", "Issue") %>'><span>Issue Work-Flow</span></a></li>
         </ul>
    </div>

</asp:Content>
