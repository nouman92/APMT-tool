<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
    <%
        if (Session["userID"] != null)
        {
    %>
         <a class="logoutLink" href='<%: Url.Action("LogOut", "Home") %>'>[ Logout ]</a>
         <b class="userName"> <%: Session["userName"] %>, <%: Session["userDesignation"] %> </b> 
    <%
        }
    %>



