<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TabContent.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.AccessRight>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Update Role Access Rights
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <br />
    <% using (Html.BeginForm("EditRole", "Organization", FormMethod.Post, new { id = "editRole" }))
        { 
    %>
        <% foreach (var item in Model) { %>
            <%: Html.RenderControl(new ControlInfo ()
                                        {
                                            ControlID = item.RightID,
                                            DefaultValue = "",
                                            CanNull = true, 
                                            // This field can be null. So, if field is null then don't pick the value of expression
                                            // from Regular Expression table. 
                                            RegularExpression =null,
                                            ErrorMessage = null,
                                            Type = (ControlType)5,
                                            Value = item.RightID<0?"Yes":null,
                                            ControlAttName = item.Description,
                                            RegExpressionID = -1
                                        }, true
                                        )
            %>
            <br />            
        <% } %>
        <br />
        <input class="buttonDesign" type="submit" value="Update" id="update"/>
        <br />
    <%} %>

</asp:Content>
