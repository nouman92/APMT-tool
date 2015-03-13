<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TabContent.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.IssueAttValue>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Issue Results
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
             <% foreach (var issue in Model)
             { %>
             <tr> 
                 <td class="tableColPadding">       
                     <%: issue.Value %>
                 </td>
                 <td>
                     <%Session["showIEditButton"] = ""; %>
                     <%: Html.ActionLink("Details", "DisplayIssue","Issue", new { pIssueID = issue.IssueID }, new { name = "dAjaxLink" })%> |
                     <%: Html.ActionLink("More", "MoreActionsTabs", "Issue", new { pIssueID = issue.IssueID, pIssueName = issue.Value, pProjectID=1 }, null)%>
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

