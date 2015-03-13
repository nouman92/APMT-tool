<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TabContent.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.CompAttValue>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Component Result
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">
        $(function () {
            $("#searchResultTable").tablesorter();
        }); 

    </script>
    
    <hr />
     <% if (Model != null)
      {
     %>
    <table id="searchResultTable" class="bodyTable">
        <thead title="Click to Sort the List"> 
            <tr>
                <th>
                    <h3>Following <%: Model.Count() %> record(s) found:</h3> 
                </th>
             </tr>
         </thead>
         <tbody class="rowHover">
             <% foreach (var comp in Model)
             { %>
             <tr> 
                 <td class="tableColPadding">       
                     <%: comp.Value %>
                 </td>
                 <td>
                     <%Session["showCEditButton"] = ""; %>
                     <%: Html.ActionLink("Details", "DisplayComponent", "Component", new { pCompID = comp.CompID }, new { name = "dAjaxLink" })%> |
                     <%: Html.ActionLink("More", "MoreActionsTabs", "Component", new {pProjectID = comp.Component.ProjID, pComponentID = comp.CompID, pComponentName = comp.Value }, null)%>
                 </td>
             </tr>
             <% } %>
         </tbody>
     </table>
     <%}
     else
     {%>
        <h3><i>No record found.</i></h3>
     <%} %>
     <br />

</asp:Content>

