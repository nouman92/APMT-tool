<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<UserInterface.Models.NavigationLink>>" %>

<% foreach (var link in Model)
   {
       if (Session["userID"] != null)
       {%>
            <%: Html.RouteLink(link.Text, link.RouteValues, new Dictionary<string, object> {
            { "class", link.IsSelected ? "selected" : null }}) %>
     <%} %>
<% } %>
