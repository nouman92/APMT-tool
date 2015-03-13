<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.ProjAttValue>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Favorite Projects
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $('#favoriteProjectsTab').show('blind', 700);
        });

        $(function () {
            $("#favoriteprojectListTable").tablesorter();
        });

        $("#details3").dialog({
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
            $("#details3").load(link, function () { $(this).dialog('open'); });
            return false;
        });

    </script>

    <div id="favoriteProjectsTab">

        <table id="favoriteprojectListTable" class="bodyTable">
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
                        <%: Html.ActionLink("Details", "DisplayProject","Project" ,new { pProjectID = proj.ProjID }, new { name = "dAjaxLink" })%> |
                        <%: Html.ActionLink("More", "MoreActionsTabs", "Project", new { pProjectID = proj.ProjID, pProjectName = proj.Value }, null)%>
                    </td>
                </tr>
                 <% } %>
            <%}
            else
            {%>
                <tr>
                    <td>
                        <h3><i>You have no favorite project.</i><span class="helpSign" title="To add project in favorite list, please go to Project > Active/Show All > More"><b>?</b></span></h3> 
                    </td>
                </tr>
            <%} %>
            </tbody>
        </table>
        <br />    

    </div>

    <div id="details3">
    </div>

</asp:Content>

