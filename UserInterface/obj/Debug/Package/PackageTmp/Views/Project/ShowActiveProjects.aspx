<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.ProjAttValue>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Active Projects
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $('#activeProjectsTab').show('blind', 700);
        });

        $(function () {
            $("#projectListTable").tablesorter();
        });

        $("#details").dialog({
            autoOpen: false,
            height: 'auto',
            width: 800,
            modal: true,
            draggable: false,
            position: 'center',
            resizable: false,
            title: 'Project Details',
            show: 'clip'
        });

        $("a[name='dAjaxLink']").click(function () {
            var link = $(this).attr('href');
            $("#details").load(link, function () { $(this).dialog('open'); });
            return false;
        });

    </script>

    <div id="activeProjectsTab">
        <table id="projectListTable" class="bodyTable">
             <% if (Model != null)
               {
             %>
             <thead title="Click to Sort the List"> 
                <tr>
                    <th>
                        <h3>Project Name</h3> 
                    </th>
                 </tr>
             </thead>
             <tbody class="rowHover">
                <% foreach (var proj in Model)
                   { %>
                <tr> 
                    <td class="tableColPadding">       
                           <%: proj.Value %>
                    </td>
                    <td>
                           <%: Html.ActionLink("Details", "DisplayProject", new { pProjectID = proj.ProjID }, new { name = "dAjaxLink" })%> |
                           <%: Html.ActionLink("More", "MoreActionsTabs", new { pProjectID = proj.ProjID, pProjectName = proj.Value }, null)%>
                    </td>
                </tr>
                 <% } %>
            <%}
            else
            {%>
                <tr>
                    <td>
                        <h3><i>No active project.</i></h3>
                    </td>
                </tr>
            <%} %>
            </tbody>
        </table>
        <br />    
    </div>

    <div id="details">
    </div>

</asp:Content>