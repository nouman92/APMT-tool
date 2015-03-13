<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.SearchFilter>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
    Display Search Filter
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <script type="text/javascript">
        $(function () {
            $('#filterTab0').show('blind', 700);
        });

        $("a[name='AjaxLink']").click(function () {
            var link1 = $(this).attr('href');
            $("#result1").load(link1, function () {
                $("#details1").dialog({
                    autoOpen: false,
                    height: 'auto',
                    width: 800,
                    modal: true,
                    draggable: false,
                    position: 'center',
                    resizable: false,
                    title: 'Details',
                    show: 'clip'
                });

                $("#result1 a[name='dAjaxLink']").click(function () {
                    var link = $(this).attr('href');
                    $("#details1").load(link, function () { $("#details1").dialog('open'); });
                    return false;
                });

            });
            return false;
        });
        
    </script>
    <br />
    <div id="filterTab0">
        <table id="Table00" class="bodyTable">
            <% if (Model.FirstOrDefault() != null)
               {%>
            <thead title="Click to Sort the List">
                <tr>
                    <th>
                        <h3>
                            Filter Name</h3>
                    </th>
                </tr>
            </thead>
            <tbody class="rowHover">
                <% foreach (var filter in Model)
                   { %>
                <tr>
                    <td class="tableColPadding" style="width: 80%;">
                        <%: filter.FilterName %>
                    </td>
                    <td style="width: 20%;">
                        <%: Html.ActionLink("Execute", "ExecuteFilter", new { pFilterValue = filter.Value }, new { name = "AjaxLink" })%>|
                        <%: Html.ActionLink("Remove", "RemoveFilter", new { pFilterID = filter.FilterID, pEmpID = filter.EmpID })%>
                    </td>
                </tr>
                <% } %>
            </tbody>
            <%}
               else
               {%>
            <tr>
                <td>
                    <h3>
                        <i>No Filter is available.</i></h3>
                </td>
            </tr>
            <%} %>
        </table>
        <br />
    </div>
    <div id="result1">
    </div>
    <div id="details1">
    </div>
</asp:Content>
