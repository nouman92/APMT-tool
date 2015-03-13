<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.ProjectRisk>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	<%: Session["projectName"] %> - Risks
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $('#projectRisksTab').show('blind', 700);
        });

        $('#moreActionsTabs').tabs({
            load: function (event, ui) {
                $("#projectRisksTab a[name='ajaxLink']", ui.panel).click(function () {
                    $(ui.panel).load(this.href);
                    return false;
                });
            }
        });

        $("#details2").dialog({
            autoOpen: false,
            height: 'auto',
            width: 800,
            modal: true,
            draggable: false,
            position: 'center',
            resizable: false,
            title: 'Risk Details',
            show: 'clip'
        });

        $("a[name='dAjaxLink']").click(function () {
            var link = $(this).attr('href');
            $("#details2").load(link, function () { $(this).dialog('open'); });
            return false;
        });

        $("#filterList2").click(function () {
            var validator = $("#riskExposure").validate();
            validator.resetForm();
            $("#_0").attr("value", "");
            $("#filterRiskListDialog").dialog('open');
            return false;
        });

        $("#filterRiskListDialog").dialog({
            autoOpen: false,
            height: 'auto',
            width: 650,
            modal: true,
            draggable: false,
            position: 'center',
            resizable: false,
            title: 'Filter Risk List',
            closeOnEscape: false,
            show: 'clip',
            open: function (event, ui) {
                $(".ui-dialog-titlebar-close").hide();
            }
        });

        $("#cancel6").click(function () {
            $("#filterRiskListDialog").dialog('close');
        });

        $("#riskExposure").validate();

    </script>

    <div id="projectRisksTab">
        <% if (Model.FirstOrDefault() != null)
        {
        %>
            <Span style="float:right;" title="Define new project specific risk"> [ <%: Html.ActionLink("New", "NewRisk", "Project", new { name = "ajaxLink" })%> ]</Span>
            <Span style="float:right;" title="Add more risks in the project"> [ <%: Html.ActionLink("Add", "AddRisks", "Project", new { name = "ajaxLink" })%> ]&nbsp</Span>
            <Span style="float:right;" title="Filter the list on the basis of Risk Exposure">[ <%: Html.ActionLink("Filter List", "#", null, new { id = "filterList2" })%> ]&nbsp</Span>
            <table id="projectRisksTable" class="bodyTable">
                <thead style="cursor:default">
                    <tr>
                        <th>
                            <% if (ViewData["filtered"] == null)
                               { %>
                                <h3 title="Sorted according to Risk Exposure">Project has the following <%: Model.Count() %> risk(s):</h3>
                            <%}
                               else
                               { %>
                                <h3 title="Sorted according to Risk Exposure (filtered list)">Following <%: Model.Count()%> risk(s) found:</h3>
                            <%} %>
                        </th>
                        </tr>
                    </thead>
                    <tbody class="rowHover">
                    <% foreach (var risk in Model)
                        { %>
                    <tr> 
                        <td class="tableColPadding">       
                            <%: risk.Risk.Description%>
                        </td>
                        <td>    
                            <%: Html.ActionLink("Details", "DisplayProjectRisk", "Project", new { pRiskID = risk.RiskID }, new { name = "dAjaxLink" })%> |
                            <%: Html.ActionLink("Edit", "EditProjectRisk", "Project", new { pRiskID = risk.RiskID }, new { name = "ajaxLink" })%>
                        </td>
                    </tr>
                        <% } %>
                </tbody>
            </table>
        <%}
        else if (ViewData["filtered"] == null)
        {%>
            <Span style="float:right;" title="Define new project specific risk"> [ <%: Html.ActionLink("New", "NewRisk", "Project", new { name = "ajaxLink" })%> ]</Span>
            <Span title="Add risk(s)" style="float:right;"> [ <%: Html.ActionLink("Add", "AddRisks", "Project", new { name = "ajaxLink" })%> ]&nbsp</Span>
            <h3><i>No risk is associated with the project. </i></h3>
        <%}
        else
        {%>
            <Span style="float:right;" title="Define new project specific risk"> [ <%: Html.ActionLink("New", "NewRisk", "Project", new { name = "ajaxLink" })%> ]</Span>
            <Span style="float:right;" title="Add more risks in the project"> [ <%: Html.ActionLink("Add More", "AddRisks", "Project", new { name = "ajaxLink"})%> ]</Span>
            <Span style="float:right;" title="Filter the list on the basis of risk exposure">[ <%: Html.ActionLink("Filter List", "#", null, new { id = "filterList2" })%> ]&nbsp</Span>
            <br />
            <h3><i>No risk is found.</i></h3>
        <%} %>
        <br />    
    </div>

    <div id="details2">
    </div>

    <div id="filterRiskListDialog">
    
        <% using (Html.BeginForm("FilterRiskList", "Project", FormMethod.Post, new { id = "riskExposure" }))
           { %>
            <br />
            <b>Risk Exposure</b> &nbsp &nbsp
            <%: Html.RenderControl(new ControlInfo()
                                        {
                                            ControlID = 0,
                                            CanNull = false,
                                            RegularExpression = @"^[1-9]{1}$|^[1-9]{1}[0-9]{1}$|^100$",
                                            ErrorMessage = "*Only 1-100 value is allowed.",
                                            Type = ControlType.TextField,
                                            Value = null
                                        }, false
                                        )
            %><span class="helpSign" title="Filter out the risks having Risk Exposure less than specifed value"><b>?</b></span>
            <br />
            <br />
            <input class="buttonDesign" type="submit" value="Filter" id="filterButton1" />
            <input class="buttonDesign2" type="button" value="Cancel" id="cancel6" />
        <% } %>
        
    </div>

</asp:Content>

