<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TabContent.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.SubCompAttValue>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Search Result
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
             <% foreach (var subComp in Model)
             { %>
             <tr> 
                 <td class="tableColPadding">       
                     <%: subComp.Value%>
                 </td>
                 <td>
                     <%: Html.ActionLink("Details", "DisplaySubComponent", "SubComponent", new { pSubCompID = subComp.SubCompID }, new { name = "dAjaxLink" })%> |
                     <%: Html.ActionLink("More", "MoreActionsTabs", "Component", new { pProjectID = subComp.SubComponent.Component.ProjID, pComponentID = subComp.SubComponent.CompID }, null)%>
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

