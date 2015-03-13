<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.SubCompAttribute>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Create Sub Component
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $('#newSubCompTab').show('blind', 700);
        });

        $("#createSubComp").validate();

    </script>

    <div id="newSubCompTab">

        <h3>Please provide the following information:</h3>
        <% using (Html.BeginForm("CreateSubComponent", "SubComponent", FormMethod.Post, new { id = "createSubComp" }))
            {
        %>
            <table class="bodyTable">
            <% foreach (var item in Model) { %>
                <tr>
                    <td class="labelPortion">
                        <% // If the control is checkbox then don't display the Attribute name as it will be with checkbox. 
                            if ((ControlType)item.FieldType != ControlType.CheckBox)
                            {%>
                                <b><%: item.SubCompAttName %></b>
                        <%} %>
                    </td>
                    <td class="controlPortion">
                        <%: Html.RenderControl(new ControlInfo ()
                                                    {
                                                        ControlID = item.SubCompAttID,
                                                        DefaultValue = item.DefaultValue,
                                                        CanNull = item.CanNull, 
                                                        // This field can be null. So, if field is null then don't pick the value of expression
                                                        // from Regular Expression table. 
                                                        RegularExpression = (item.RegularExpression != null)?  item.RegularExpression1.Value: null,
                                                        ErrorMessage = (item.RegularExpression != null) ? item.RegularExpression1.Error : "",
                                                        Type = (ControlType)item.FieldType,
                                                        Value = (TempData["__" + item.SubCompAttID] != null)? TempData["__" + item.SubCompAttID].ToString() : null,
                                                        ControlAttName = item.SubCompAttName,
                                                        RegExpressionID = (item.RegularExpression != null) ? item.RegularExpression1.ExpressionID : -1
                                                    }, true
                                                    )
                        %>
                        <% // Display the error message if any. 
                            if (TempData["_" + item.SubCompAttID] != null)
                            { %>
                                <span class="errorMessage"> <%: TempData["_" + item.SubCompAttID]%> </span>
                        <% } %>
                    </td>
                </tr>
            <% } %>
            </table>
            <br />
            <input class="buttonDesign" type="submit" value="Create" />
            <a class="linkButton2" href='<%: Url.Action("MoreActionsTabs", "Component") %>'> <span> Cancel </span> </a>
            <br />
            <br />
        <%} %>
    </div>

</asp:Content>

