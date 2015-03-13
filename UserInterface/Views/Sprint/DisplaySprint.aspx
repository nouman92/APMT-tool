<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TabContent.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.SprintAttValue>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Display Sprint
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $('#sMoreActionsTabs').tabs({
            load: function (event, ui) {
                $("a[name='ajaxLink']", ui.panel).click(function () {
                    $(ui.panel).load(this.href);
                    return false;
                });
            }
        });

        $(function () {
            $("#progressbar").progressbar({
                value: <%:ViewData["loadPercent"]%>
            });
            $("#progressbar").css({ 'background': '#c9c9c9' });

            var value = $( "#progressbar" ).progressbar( "option", "value" );
            if (value < 50)
               $("#progressbar > div").css({ 'background': 'Orange' });
            else if (value < 85)
               $("#progressbar > div").css({ 'background': 'Yellow' });
            else if (value < 101)
               $("#progressbar > div").css({ 'background': 'Green' });
            else 
               $("#progressbar > div").css({ 'background': 'Red' });
            
        });

    </script>

    <div id="sprintDetails">

        <div>
            <br />
            <b title="Working days">&nbsp &nbsp &nbsp Sprint Duration: </b>&nbsp &nbsp <%:ViewData["tDays"]%> <%: (ViewData["tDays"].ToString() == "1") ? "day" : "days" %>
            <br />
            <br />
            <b title="Working days">&nbsp Remaining Day(s): </b>&nbsp &nbsp <%:ViewData["rDays"]%>
            <table>
                <tr>
                    <td>
                        <br />
                        <b> &nbsp &nbsp&nbsp Sprint Workload: </b>&nbsp &nbsp                         
                    </td>
                    <td>
                        <br />
                        <div title='<%:ViewData["load"]%>' id="progressbar" style="width:170px; height:20px;"></div>
                    </td>
                    <td>
                        <br />
                        &nbsp <span title="Loaded hour(s)"><%:ViewData["wHours"]%> hr</span> / <span title="Available hour(s)"><%:ViewData["tHours"]%> hr</span>
                    </td>
                </tr>
            </table>
        </div>
        <br />
        <br />
        
        <table class="bodyTable">
            <tr>
                <td class="labelPortion">
                    <b> Sprint ID</b>
                </td>
                <td class="controlPortion">
                    <%: Model.First().SprintID %>        
                </td>
            </tr>
            <% foreach (var item in Model) { %>
            <tr>
                <td class="labelPortion">
                    <b> <%: item.SprintAttribute.SprintAttName %> </b>
                </td>
                <td class="controlPortion">
                    <% // If check box is not checked then display the value "No".
                       if ((ControlType)item.SprintAttribute.FieldType == ControlType.CheckBox && item.Value == null)
                       {%>
                        <span>No</span>
                    <%} else { %>
                        <%: item.Value %>
                    <%} %>
                </td>
            </tr>
        <% } %>
        </table>
        <br />

        <%
            if (Session["showSEditButton"] != "")
          {  
        %>
            <script type="text/javascript">
                $(function () {
                    $('#sprintDetails').show('blind', 700);
                });
            </script>
            <a name = "ajaxLink" title="Edit Sprint Details" class="linkButton2" href='<%: Url.Action("EditSprint", "Sprint") %>'> <span> Edit </span></a>
            <br />
        <%} %>

    </div>

</asp:Content>

