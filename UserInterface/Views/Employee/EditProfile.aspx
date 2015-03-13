<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TabContent.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.EmpAttValue>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Edit - User Profile
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $("#editUserProfile").validate();
            
    </script>

    <h3>Edit Profile</h3>
    <br />
    <% using (Html.BeginForm("EditProfile", "Employee", FormMethod.Post, new { id = "editUserProfile" }))
       { %>
        <table class="bodyTable">
        <% foreach (var item in Model)
           { %>
            <tr>
                <td class="labelPortion">
                    <% // If the control is checkbox then don't display the Attribute name as it will be with checkbox. 
                       if ((ControlType)item.EmpAttribute.FieldType != ControlType.CheckBox)
                       {%>
                           <b> <%: item.EmpAttribute.EmpAttName%> </b>
                    <%} %>
                </td>
                <td class="controlPortion">
                    <%: Html.RenderControl(new ControlInfo()
                                                {
                                                    ControlID = item.EmpAttID,
                                                    // If the type is List then pick the list options from the EmpAttributes.
                                                    DefaultValue = ((ControlType)item.EmpAttribute.FieldType == ControlType.List)? item.EmpAttribute.DefaultValue : item.Value,
                                                    CanNull = item.EmpAttribute.CanNull,
                                                    // This field can be null. So, if field is null then don't pick the value of expression
                                                    // from Regular Expression table. 
                                                    RegularExpression = (item.EmpAttribute.RegularExpression != null) ? item.EmpAttribute.RegularExpression1.Value : null,
                                                    ErrorMessage = (item.EmpAttribute.RegularExpression != null) ? item.EmpAttribute.RegularExpression1.Error : "",
                                                    Type = (ControlType)item.EmpAttribute.FieldType,
                                                    Value = (TempData["__" + item.EmpAttID] != null) ? TempData["__" + item.EmpAttID].ToString() : item.Value,
                                                    ControlAttName = item.EmpAttribute.EmpAttName,
                                                    RegExpressionID = (item.EmpAttribute.RegularExpression != null) ? item.EmpAttribute.RegularExpression1.ExpressionID : -1
                                                }, true
                                             )
                    %>
                    <% // Display the error message if any. 
                       if (TempData["_" + item.EmpAttID] != null)
                       { %>
                           <span class="errorMessage"> <%: TempData["_" + item.EmpAttID]%> </span>
                    <% } %>
                </td>
            </tr>
        <% } %>
    </table>
    <br />
    <input class="buttonDesign" type="submit" value="Save" />
    <a class="linkButton2" href='<%: Url.Action("UserProfileTabs", "Employee") %>'> <span> Cancel </span></a>
    <br />
    <%} %>

</asp:Content>
