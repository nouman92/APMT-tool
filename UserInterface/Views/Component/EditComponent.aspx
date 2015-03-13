<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TabContent.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.CompAttValue>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Edit Component
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $("#editComponent").validate();
            
    </script>

    <h3>Edit Component</h3>
    <br />
    <% using (Html.BeginForm("EditComponent", "Component", FormMethod.Post, new { id = "editComponent" }))
       { %>
        <table class="bodyTable">
        <% foreach (var item in Model)
           { %>
            <tr>
                <td class="labelPortion">
                    <% // If the control is checkbox then don't display the Attribute name as it will be with checkbox. 
                       if ((ControlType)item.CompAttribute.FieldType != ControlType.CheckBox)
                       {%>
                           <b> <%: item.CompAttribute.CompAttName%> </b>
                    <%} %>
                </td>
                <td class="controlPortion">
                    <%: Html.RenderControl(new ControlInfo()
                                                {
                                                    ControlID = item.CompAttID,
                                                    // If the type is List then pick the list options from the ProjAttributes.
                                                    DefaultValue = ((ControlType)item.CompAttribute.FieldType == ControlType.List)? item.CompAttribute.DefaultValue : item.Value,
                                                    CanNull = item.CompAttribute.CanNull,
                                                    // This field can be null. So, if field is null then don't pick the value of expression
                                                    // from Regular Expression table. 
                                                    RegularExpression = (item.CompAttribute.RegularExpression != null) ? item.CompAttribute.RegularExpression1.Value : null,
                                                    ErrorMessage = (item.CompAttribute.RegularExpression != null) ? item.CompAttribute.RegularExpression1.Error : "",
                                                    Type = (ControlType)item.CompAttribute.FieldType,
                                                    Value = (TempData["__" + item.CompAttID] != null) ? TempData["__" + item.CompAttID].ToString() : item.Value,
                                                    ControlAttName = item.CompAttribute.CompAttName,
                                                    RegExpressionID = (item.CompAttribute.RegularExpression != null) ? item.CompAttribute.RegularExpression1.ExpressionID : -1
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
    <input class="buttonDesign" type="submit" value="Save" />
    <a class="linkButton2" href='<%: Url.Action("MoreActionsTabs", "Component") %>'> <span> Cancel </span></a>
    <br />
    <%} %>

</asp:Content>
