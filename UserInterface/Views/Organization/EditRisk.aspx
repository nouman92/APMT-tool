<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TabContent.Master" Inherits="System.Web.Mvc.ViewPage<Domain.Entities.Risk>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Edit Risk
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $("#editRisk").validate();
            
    </script>

    <h3>Edit Risk</h3>
    <br />
    <% using (Html.BeginForm("EditRisk", "Organization", new { pRiskID = Model.RiskID }, FormMethod.Post, new { id = "editRisk" }))
        { %>
        <table class="bodyTable">
            <tr>
                <td class="labelPortion">
                    <b>Risk Description</b>
                </td>
                <td class="controlPortion">
                    <%: Html.RenderControl(new ControlInfo()
                                                {
                                                    ControlID = 1,
                                                    CanNull = false,
                                                    RegularExpression = @"^[a-zA-Z''-'\s]*$",
                                                    ErrorMessage = "*Field contains invalid character(s).",
                                                    Type = ControlType.TextArea,
                                                    Value = (TempData["__1"] != null) ? TempData["__1"].ToString() : Model.Description
                                                }, true
                                                )
                    %>
                    <% // Display the error message if any. 
                        if (TempData["_1"] != null)
                        { %>
                            <span class="errorMessage"> <%: TempData["_1"]%> </span>
                    <% } %>
                </td>
            </tr>
            <tr>
                <td class="labelPortion">
                    <b>Risk Category</b>
                </td>
                <td class="controlPortion">
                    <%: Html.DropDownList("selectedValue", (IEnumerable<SelectListItem>)ViewData["riskCategories"])%>
                </td>
            </tr>
        </table>    
        <br />
        <input class="buttonDesign" type="submit" value="Save"/>
        <a class="linkButton2" href='<%: Url.Action("OrganizationTabs", "Organization") %>'> <span> Cancel </span></a>
    <% } %>
    <br />
    
</asp:Content>
