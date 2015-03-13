<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.SubCompAttValue>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	<%: Session["componentName"]%> - Sub Components
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $('#subComponentsTab').show('blind', 700);
        });

        $(function () {
            $("#subComponentsTable").tablesorter();
        });

        $('#cMoreActionsTabs').tabs({
            load: function (event, ui) {
                $("a[name='ajaxLink']", ui.panel).click(function () {
                    $(ui.panel).load(this.href);
                    return false;
                });
            }
        });

        $("#details").dialog({
            autoOpen: false,
            height: 'auto',
            width: 800,
            modal: true,
            draggable: false,
            position: 'center',
            resizable: false,
            title: 'Sub-Component Details',
            show: 'clip'
        });

        $("a[name='dAjaxLink']").click(function () {
            var link = $(this).attr('href');
            $("#details").load(link, function () { $(this).dialog('open'); });
            return false;
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
            title: 'Search Sub-Component',
            show: 'clip'
        });

    </script>

    <div id="subComponentsTab">

        <% if (Model != null)
        {%>
            <Span style="float:right;" title="Search the sub-component">[ <%: Html.ActionLink("Search", "SearchSubComponent", "SubComponent",null, new { id = "search1" })%> ]</Span>
            <table class="bodyTable" id="subComponentsTable">
                <thead title="Click to Sort the List"> 
                    <tr>
                        <th>
                            <% if (TempData["subCompAttID"] == null)
                               { %>
                                <h3 title="Click to Sort the List">Component has the following <%: Model.Count()%> sub-component(s):</h3>
                            <%}
                               else
                               { %>
                                <h3 title="Click to Sort the List">Following <%: Model.Count()%> record(s) found:</h3>
                            <%} %>
                        </th>
                    </tr>
                </thead>
                <tbody class="rowHover">
                <% foreach (var subComp in Model)
                    { %>
                    <tr> 
                        <td class="tableColPadding" style="width:80%;">
                            <%: subComp.Value%>
                        </td>
                        <td style="width:20%;">
                            <%: Html.ActionLink("Details", "DisplaySubComponent", "SubComponent", new { pSubCompID = subComp.SubCompID }, new { name = "dAjaxLink" })%> |
                            <%: Html.ActionLink("Edit", "EditSubComponent", "SubComponent", new { pSubCompID = subComp.SubCompID }, new { name = "ajaxLink" })%>
                        </td>
                    </tr>
                    <% } %>
                </tbody>
            </table>
        <%}
           else if (TempData["subCompAttID"] == null)
        {%>
            <br />
            <h3><i>Component has no sub-component. </i></h3>
        <%}
        else
        {%>
            <Span style="float:right;" title="Search the component">[ <%: Html.ActionLink("Search", "SearchSubComponent", "SubComponent",null, new { id = "search1" })%> ]</Span>
            <br />
            <h3><i>No sub-component is found.</i></h3>
        <%} %>
        <br />
    </div>

    <div id="details">
    </div>

    <div id="searchDialog">        
    </div>

</asp:Content>

