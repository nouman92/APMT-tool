<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.SprintAttribute>>" %>


<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Create Sprint
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $('#newSprintTab').show('blind', 700);
            $("#createSprint").bind("submit", function () {
                if ($('#createSprint').validate().form()) {
                    var cDate = new Date();
                    var sDate = new Date($("#_2").val() + " " + cDate.toString().split(' ')[4] + 1);
                    var eDate = new Date($("#_3").val());

                    if (sDate < cDate) {
                        showError("<span style='color: Red;'>The date '" + $("#_2").val() + "' has been expired.</span>");
                        return false;
                    }
                    else if (eDate <= (new Date($("#_2").val()))) {
                        showError("<span style='color: Red;'>Invalid sprint duration.</span>");
                        return false;
                    }
                }
            });
        });

        $("#createSprint").validate();

    </script>

    <div id="newSprintTab">

        <h5>
            <i>* You cannot modify the sprint <b>Start</b> or <b>End</b> date later.</i>
            <br />
        </h5>

        <h3>Please provide the following information:</h3>
        <br />
        <% using (Html.BeginForm("CreateSprint", "Sprint", FormMethod.Post, new { id = "createSprint" }))
            {
        %>
            <table class="bodyTable">
            <% foreach (var item in Model) { %>
                <tr>
                    <td class="labelPortion">
                        <% // If the control is checkbox then don't display the Attribute name as it will be with checkbox. 
                            if ((ControlType)item.FieldType != ControlType.CheckBox)
                            {%>
                                <b><%: item.SprintAttName %></b>
                        <%} %>
                    </td>
                    <td class="controlPortion">
                        <%: Html.RenderControl(new ControlInfo ()
                                                    {
                                                        ControlID = item.SprintAttID,
                                                        DefaultValue = item.DefaultValue,
                                                        CanNull = item.CanNull, 
                                                        // This field can be null. So, if field is null then don't pick the value of expression
                                                        // from Regular Expression table. 
                                                        RegularExpression = (item.RegularExpression != null)?  item.RegularExpression1.Value: null,
                                                        ErrorMessage = (item.RegularExpression != null) ? item.RegularExpression1.Error : "",
                                                        Type = (ControlType)item.FieldType,
                                                        Value = (TempData["__" + item.SprintAttID] != null) ? TempData["__" + item.SprintAttID].ToString() : null,
                                                        ControlAttName = item.SprintAttName,
                                                        RegExpressionID = (item.RegularExpression != null) ? item.RegularExpression1.ExpressionID : -1
                                                    }, true
                                                    )
                        %>
                        <% // Display the error message if any. 
                            if (TempData["_" + item.SprintAttID] != null)
                            { %>
                                <span class="errorMessage"> <%: TempData["_" + item.SprintAttID]%> </span>
                        <% } %>
                    </td>
                </tr>
            <% } %>
            </table>
            <br />
            <input class="buttonDesign" type="submit" value="Create" id="create"/>
            <a class="linkButton2" href='<%: Url.Action("MoreActionsTabs", "Project") %>'> <span> Cancel </span> </a>
            <br />
            <br />
        <%} %>
    </div>

</asp:Content>

