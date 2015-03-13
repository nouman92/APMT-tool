<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.Risk>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
    Risk List
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $('#riskListTab').show('blind', 700);
        });

        $(function () {
            $("#displayRiskListTable").tablesorter();
        });

        $('#organizationTabs').tabs({
            load: function (event, ui) {
                $("a[name='ajaxLink']", ui.panel).click(function () {
                    $(ui.panel).load(this.href);
                    return false;
                });
            }
        });

    </script>

    <div id="riskListTab">

        <span style="float:right;" title="Add new risk in the Risk List">[ <%: Html.ActionLink("Add Risk", "AddRisk", "Organization", null, new { name = "ajaxLink"})%> ]</span>

        <table class="bodyTable" id="displayRiskListTable">
            <thead title="Click to Sort the List"> 
                <tr>
                    <th>
                        <h3>Risk</h3>
                    </th>
                    <th>
                        <h3>Category</h3>
                    </th>
                </tr>
                </thead>
                <tbody class="rowHover">
                <%foreach (var item in Model)
                    { %>
                    <tr>
                        <td class="tableColPadding">       
                                <%: item.Description%>
                        </td>
                        <td>
                            <%: item.RiskCategory.CategoryName%>
                        </td>
                        <td>
                                <%: Html.ActionLink("Edit", "EditRisk", "Organization", new { pRiskID = item.RiskID }, new { name = "ajaxLink" })%>
                        </td>
                    </tr>
                <% } %>
            </tbody>
        </table>
        <br />

    </div>

</asp:Content>

