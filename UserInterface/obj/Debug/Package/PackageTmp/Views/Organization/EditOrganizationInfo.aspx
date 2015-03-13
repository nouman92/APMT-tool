<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.OrgAttValue>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Edit Organization Info
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    
    <script type="text/javascript">

        $(function () {
            $('#editInfoTab').show('blind', 700);
        });

        $("#editOrgInfo").validate();
            
    </script>
    
    <div id="editInfoTab">
        <br />
        <% using (Html.BeginForm("EditOrganizationInfo", "Organization", FormMethod.Post, new { id = "editOrgInfo" }))
           { %>
            <table class="bodyTable">
            <% foreach (var item in Model) { %>
                <tr>
                    <td class="labelPortion">
                        <% // If the control is checkbox then don't display the Attribute name as it will be with checkbox. 
                           if ((ControlType)item.OrgAttribute.FieldType != ControlType.CheckBox)
                           {%>
                                <b><%: item.OrgAttribute.OrgAttName%></b>
                        <%} %>
                    </td>
                    <td class="controlPortion">
                        <%: Html.RenderControl(new ControlInfo()
                                                    {
                                                        ControlID = item.OrgAttID,
                                                        // If the type is List then pick the list options from the ProjAttributes and append the selected option with last one with ":" separated.
                                                        DefaultValue = ((ControlType)item.OrgAttribute.FieldType == ControlType.List)? item.OrgAttribute.DefaultValue : item.Value,
                                                        CanNull = item.OrgAttribute.CanNull,
                                                        // This field can be null. So, if field is null then don't pick the value of expression
                                                        // from Regular Expression table. 
                                                        RegularExpression = (item.OrgAttribute.RegularExpression != null) ? item.OrgAttribute.RegularExpression1.Value : null,
                                                        ErrorMessage = (item.OrgAttribute.RegularExpression != null) ? item.OrgAttribute.RegularExpression1.Error : "",
                                                        Type = (ControlType)item.OrgAttribute.FieldType,
                                                        Value = (TempData["__" + item.OrgAttID] != null) ? TempData["__" + item.OrgAttID].ToString() : item.Value,
                                                        ControlAttName = item.OrgAttribute.OrgAttName,
                                                        RegExpressionID = (item.OrgAttribute.RegularExpression != null) ? item.OrgAttribute.RegularExpression1.ExpressionID : -1
                                                    }, true
                                                 )
                        %>
                        <% // Display the error message if any. 
                           if (TempData["_" + item.OrgAttID] != null)
                           { %>
                               <span class="errorMessage"> <%: TempData["_" + item.OrgAttID]%> </span>
                        <% } %>
                    </td>
                </tr>
            <% } %>
        </table>
        <br />
        <input class="buttonDesign" type="submit" value="Save" />
        <a class="linkButton2" href='<%: Url.Action("OrganizationTabs", "Organization") %>'> <span> Cancel </span></a>
    <%} %>
        <br />
    </div>

</asp:Content>