
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<UserInterface.Models.LogInViewModel>" %>

<asp:Content ID="content1" ContentPlaceHolderID="titleContent" runat="server">
	APM Tool - Log In
</asp:Content>

<asp:Content ID="mainContent" ContentPlaceHolderID="body" runat="server">

    <br />
    <br />
    <div class="login">
        <% Html.EnableClientValidation(); %>
        <% using (Html.BeginForm())
           { %>
            <br />
            <b>Log In to APM Tool Account</b>
            <br />
            <br />
            <br />
            User Name:<%: Html.TextBoxFor(y => y.UserName, new { id = "UserName", name = "UserName"})%>
            <br />
            <%: Html.ValidationMessageFor(x => x.UserName)%>
            <br />
            <br />
            Password:<%: Html.PasswordFor(y => y.Password, new { id = "Password", name = "Password"})%>
            <br />
            <%: Html.ValidationMessageFor(x => x.Password)%>
            <br />
            <asp:Label ID="messageLabel" runat="server" Font-Size="Medium" ForeColor="Red"><%: TempData["errorMessage"]%> </asp:Label>
            <br />
            <span class="loginButton"><input class="buttonDesign" type="submit" value="Log In"/></span>
        <% } %>        
    </div>
 
</asp:Content>
