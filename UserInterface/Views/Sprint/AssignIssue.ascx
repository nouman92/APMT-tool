<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Domain.Entities.EmpAttValue>>"  %>

<script type="text/javascript">

    $(function () {
        $("[title]").tooltip({
            open: function () {
                var tooltip = $(this).tooltip("widget");
                $(document).mousemove(function (event) {
                    tooltip.position({
                        my: "left center",
                        at: "right center",
                        offset: "25 25",
                        of: event
                    });
                })
                // trigger once to override element-relative positioning
				.mousemove();
            },
            close: function () {
                $(document).unbind("mousemove");
            }
        });
    });

    $(function () {
        $("#empsListTable").tablesorter();
    });

    function setBar(id, val) {
        var selector = "#__" + id;
        $(selector).progressbar({
            value: val
        });
        
         $(selector).css({ 'background': '#c9c9c9' });

        if (val < 50)
            $(selector + " > div").css({ 'background': 'Orange' });
        else if (val < 85)
            $(selector + " > div").css({ 'background': 'Yellow' });
        else if (val < 101)
            $(selector + " > div").css({ 'background': 'Green' });
        else 
            $(selector + " > div").css({ 'background': 'Red' });
            
    }

</script>
<% 
    int index = 0;
    List<double> loadedHoursList = (List<double>)ViewData["loadedHoursList"];
    List<double> loadPercentageList = (List<double>)ViewData["loadPercentageList"];
 %>

<% List<string> depList =(List<string>) ViewData["depList"];
   double manHours = double.Parse(ViewData["manHours"].ToString());
    if (depList.Count > 0 || manHours == 0)  {%>
        <h3>Warning(s)</h3>
        <%if(depList.Count > 0) {%>
            <b>* Issue has the following dependenc<%: (depList.Count() > 1) ? "ies" : "y"%> unresolved:</b>
            <ul>
            <%foreach(string item in depList){ %>
                  <li><%:item%></li>
            <%} %>
            </ul>
        <%} %>
        <%if(manHours == 0){ %>
            <b>* Issue has the <b>0</b> man hours.</b>
            <br />
        <%} %>
        <hr />
    <%} %>

    <% using (Html.BeginForm("AssignIssue", "Sprint", new { pIssueID = ViewData["issueID"]}))
       {
    %>
            <% if (Model.Count() > 0)
               {%>
            <Span style="float:right;" title="Issue man hours"><b> Man Hours: </b><%:manHours %></Span>
            <table id="empsListTable" style="width:100%">
                <thead title="Click to Sort the List"> 
                    <tr>
                        <th>
                            <h3>Select the user(s) for issue.</h3>
                        </th>
                    </tr>
                    </thead>
                    <tbody class="rowHover">
                    <%
                    foreach (var item in Model)
                    { %>
                        <tr>
                            <td style="width:65%;">
                            <br />
                                <%: Html.CheckBox("_" + index, new { id = "_" + index, value = "" + item.EmpID  })%> <%: item.Value%>
                                , <b>ID: </b><%:item.EmpID%>
                            </td>
                            <td>
                                <div id="__<%:item.EmpID %>" style="width:130px; height:18px;"></div>
                                <script type="text/javascript">
                                    setBar(<%:item.EmpID %>,<%:loadPercentageList[index] %> );
                                </script>
                            </td>
                            <td>
                                &nbsp <span title="Loaded hour(s)"><%:loadedHoursList[index++]%> hr</span> / <span title="Available hour(s)"><%:ViewData["availableManHours"]%> hr</span>
                            </td>
                        </tr>
                <% } %>
            </tbody>
        </table>
        <br />
        <br />
        <input class="buttonDesign" type="submit" value="Assign" />
        <input class="buttonDesign2" type="button" value="Cancel" id="cancel8" />
        <br />
        <%}
        else
        {%>
            <h3><i>No users are available.</i></h3>
            <br />
            <br />
            <input class="buttonDesign" type="button" value="Close" id="cancel8"/>
            <br />
        <% } %>
    <% } %>
    <br />
