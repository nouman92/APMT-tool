<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Domain.Entities.Risk>>" %>

<script type="text/javascript">

    $(function () {
        $("#riskListTable").tablesorter();
    });

    $("#selectAll").change(function () {

        if ($(this).attr('checked')) {
            $("#riskListTable input[type='checkbox']").attr("checked", true);
        }
        else {
            $("#riskListTable input[type='checkbox']").attr("checked", false);
        }
    });

</script>

<table class="bodyTable" id="riskListTable">
    <% if (Model.FirstOrDefault() != null)
        {%>
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
            <tbody>
            <%foreach (var item in Model)
                { %>
                <tr>
                    <td class="controlPortion" style="width:78%;">
                        <%: Html.CheckBox("risk_" + item.RiskID, new { id = "risk_" + item.RiskID, value = "true"})%> <b><%: item.Description%></b>
                        <br /> &nbsp &nbsp &nbsp Probability &nbsp
                        <%: Html.DropDownList("prob_" + item.RiskID, new[] {
                                                                            new SelectListItem { Text = "1", Value = "1" },
                                                                            new SelectListItem { Text = "2", Value = "2" },
                                                                            new SelectListItem { Text = "3", Value = "3" },
                                                                            new SelectListItem { Text = "4", Value = "4" },
                                                                            new SelectListItem { Text = "5", Value = "5" },
                                                                            new SelectListItem { Text = "6", Value = "6" },
                                                                            new SelectListItem { Text = "7", Value = "7" },
                                                                            new SelectListItem { Text = "8", Value = "8" },
                                                                            new SelectListItem { Text = "9", Value = "9" },
                                                                            new SelectListItem { Text = "10", Value = "10" },
                                                                            }, new { id = "prob_" + item.RiskID })%>
                            &nbsp &nbsp &nbsp Impact  &nbsp
                        <%: Html.DropDownList("imp_" + item.RiskID, new[] {
                                                                            new SelectListItem { Text = "1", Value = "1" },
                                                                            new SelectListItem { Text = "2", Value = "2" },
                                                                            new SelectListItem { Text = "3", Value = "3" },
                                                                            new SelectListItem { Text = "4", Value = "4" },
                                                                            new SelectListItem { Text = "5", Value = "5" },
                                                                            new SelectListItem { Text = "6", Value = "6" },
                                                                            new SelectListItem { Text = "7", Value = "7" },
                                                                            new SelectListItem { Text = "8", Value = "8" },
                                                                            new SelectListItem { Text = "9", Value = "9" },
                                                                            new SelectListItem { Text = "10", Value = "10" },
                                                                            }, new { id = "imp_" + item.RiskID })%>
                        <br />
                        <br />
                        &nbsp &nbsp &nbsp Mitigation<br /> &nbsp &nbsp &nbsp<%: Html.TextArea("mit_" + item.RiskID, new { id = "mit_" + item.RiskID, rows = "4", cols = "50" })%>
                        <br />
                        <br />
                    </td>
                    <td>
                        <b><%: item.RiskCategory.CategoryName%></b>
                    </td>
                </tr>
            <% } %>
    </tbody>
</table>
<br />
<span class="controlPortion" ><input type="checkbox" id="selectAll" value="Select All" /><b>Select All</b></span>
<br />
<%}
else
{%>
    <br />
    <h3><i>No risks are available.</i><span class="helpSign" title="To add risks, please go to Risk List tab in Organization Menu"><b>?</b></span></h3>
<% } %>

