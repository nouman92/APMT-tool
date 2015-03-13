<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TabContent.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.SubCompAttValue>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Edit Sub-Component
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $("#editSubComp").validate();
            
    </script>

    <h3>Edit Sub-Component</h3>
    <br />
    <% using (Html.BeginForm("EditSubComponent", "SubComponent", new { pSubCompID = Model.First().SubCompID }, FormMethod.Post, new { id = "editSubComp" }))
       { %>
        <table class="bodyTable">
        <% foreach (var item in Model)
           { %>
            <tr>
                <td class="labelPortion">
                    <% // If the control is checkbox then don't display the Attribute name as it will be with checkbox. 
        if ((ControlType)item.SubCompAttribute.FieldType != ControlType.CheckBox)
        {%>
                           <b> <%: item.SubCompAttribute.SubCompAttName%> </b>
                    <%} %>
                </td>
                <td class="controlPortion">
                    <%: Html.RenderControl(new ControlInfo()
                                                {
                                                    ControlID = item.SubCompAttID,
                                                    // If the type is List then pick the list options from the ProjAttributes.
                                                    DefaultValue = ((ControlType)item.SubCompAttribute.FieldType == ControlType.List) ? item.SubCompAttribute.DefaultValue : item.Value,
                                                    CanNull = item.SubCompAttribute.CanNull,
                                                    // This field can be null. So, if field is null then don't pick the value of expression
                                                    // from Regular Expression table. 
                                                    RegularExpression = (item.SubCompAttribute.RegularExpression != null) ? item.SubCompAttribute.RegularExpression1.Value : null,
                                                    ErrorMessage = (item.SubCompAttribute.RegularExpression != null) ? item.SubCompAttribute.RegularExpression1.Error : "",
                                                    Type = (ControlType)item.SubCompAttribute.FieldType,
                                                    Value = (TempData["__" + item.SubCompAttID] != null) ? TempData["__" + item.SubCompAttID].ToString() : item.Value,
                                                    ControlAttName = item.SubCompAttribute.SubCompAttName,
                                                    RegExpressionID = (item.SubCompAttribute.RegularExpression != null) ? item.SubCompAttribute.RegularExpression1.ExpressionID : -1
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
    <input class="buttonDesign" type="submit" value="Save" />
    <a class="linkButton2" href='<%: Url.Action("MoreActionsTabs", "Component") %>'> <span> Cancel </span></a>
    <br />
    <%} %>

</asp:Content>


