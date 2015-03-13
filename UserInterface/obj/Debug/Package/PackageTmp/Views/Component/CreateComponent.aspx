<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.CompAttribute>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Create Component
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $('#newComponentTab').show('blind', 700);
        });

        $("#createComponent").validate();

    </script>

    <div id="newComponentTab">

        <h3>Please provide the following information:</h3>
        <% using (Html.BeginForm("CreateComponent", "Component", FormMethod.Post, new { id = "createComponent" }))
            {
        %>
            <table class="bodyTable">
            <% foreach (var item in Model) { %>
                <tr>
                    <td class="labelPortion">
                        <% // If the control is checkbox then don't display the Attribute name as it will be with checkbox. 
                            if ((ControlType)item.FieldType != ControlType.CheckBox)
                            {%>
                                <b><%: item.CompAttName %></b>
                        <%} %>
                    </td>
                    <td class="controlPortion">
                        <%: Html.RenderControl(new ControlInfo ()
                                                    {
                                                        ControlID = item.CompAttID,
                                                        DefaultValue = item.DefaultValue,
                                                        CanNull = item.CanNull, 
                                                        // This field can be null. So, if field is null then don't pick the value of expression
                                                        // from Regular Expression table. 
                                                        RegularExpression = (item.RegularExpression != null)?  item.RegularExpression1.Value: null,
                                                        ErrorMessage = (item.RegularExpression != null) ? item.RegularExpression1.Error : "",
                                                        Type = (ControlType)item.FieldType,
                                                        Value = (TempData["__" + item.CompAttID] != null)? TempData["__" + item.CompAttID].ToString() : null,
                                                        ControlAttName = item.CompAttName,
                                                        RegExpressionID = (item.RegularExpression != null) ? item.RegularExpression1.ExpressionID : -1
                                                    }, true
                                                    )
                        %>
                        <% // Display the error message if any. 
                            if (TempData["_" + item.CompAttID] != null)
                            { %>
                                <span class="errorMessage"> <%: TempData["_" + item.CompAttID]%> </span>
                        <% } %>
                    </td>
                </tr>
            <% } %>
            </table>
            <br />
            <input class="buttonDesign" type="submit" value="Create" />
            <a class="linkButton2" href='<%: Url.Action("MoreActionsTabs", "Project") %>'> <span> Cancel </span> </a>
            <br />
            <br />
        <%} %>
    </div>

</asp:Content>

