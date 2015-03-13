<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.CompAttValue>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	<%: Session["projectName"] %> - Components
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $('#projectComponentsTab').show('blind', 700);
        });

        $("#details4").dialog({
            autoOpen: false,
            height: 'auto',
            width: 800,
            modal: true,
            draggable: false,
            position: 'center',
            resizable: false,
            title: 'Component Details',
            show: 'clip'
        });

        $("a[name='dAjaxLink']").click(function () {
            var link = $(this).attr('href');
            $("#details4").load(link, function () { $(this).dialog('open'); });
            return false;
        });

        $(function () {
            $("#projectComponentTable").tablesorter();
        });

        $("#search1").click(function () {
            var link = $(this).attr('href');
            $("#searchDialog").load(link, function () { $(this).dialog('open'); });
            return false;
        });

        $("#searchDialog").dialog({
            autoOpen: false,
            height: 'auto',
            width: 720,
            modal: true,
            draggable: false,
            position: 'center',
            resizable: false,
            title: 'Search Component',
            show: 'clip'
        });

    </script>

    <div id="projectComponentsTab">
        
        <% if (Model != null)
        {
        %>
            <Span style="float:right;" title="Search the component">[ <%: Html.ActionLink("Search", "SearchComponent", "Component",null, new { id = "search1" })%> ]</Span>
            <table id="projectComponentTable" class="bodyTable">
                <thead>
                    <tr>
                        <th>
                            <% if (TempData["compAttID"] == null)
                               { %>
                                <h3 title="Click to Sort the List">Project has the following <%: Model.Count()%> component(s):</h3>
                            <%}
                               else
                               { %>
                                <h3 title="Click to Sort the List">Following <%: Model.Count()%> record(s) found:</h3>
                            <%} %>
                        </th>
                    </tr>
                </thead>
                <tbody class="rowHover">
                <% foreach (var comp in Model)
                    { %>
                    <tr> 
                        <td class="tableColPadding" style="width:80%;">
                            <%: comp.Value%>
                        </td>
                        <td style="width:20%;">
                            <%: Html.ActionLink("Details", "DisplayComponent", "Component", new { pCompID = comp.CompID }, new { name = "dAjaxLink" })%> |
                            <%: Html.ActionLink("More", "MoreActionsTabs", "Component", new { pComponentID = comp.CompID, pComponentName = comp.Value }, null)%>
                        </td>
                    </tr>
                    <% } %>
                </tbody>
            </table>
        <%}
           else if (TempData["compAttID"] == null)
        {%>
            <br />
            <h3><i>Project has no component. </i></h3>
        <%}
        else
        {%>
            <Span style="float:right;" title="Search the component">[ <%: Html.ActionLink("Search", "SearchComponent", "Component",null, new { id = "search1" })%> ]</Span>
            <br />
            <h3><i>No component is found.</i></h3>
        <%} %>
        <br />    
    </div>

    <div id="details4">
    </div>

    <div id="searchDialog">        
    </div>

</asp:Content>

