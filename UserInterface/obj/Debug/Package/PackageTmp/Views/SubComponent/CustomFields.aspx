<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.SubCompAttribute>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Sub-Component - Custom Fields
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $('#customFieldsTab').show('blind', 700);
        });

        $('#cancel2').click(function () {
            $('#createSubCompTabs').tabs('select', 0);
            return false;
        });

        $(function () {
            $("#customFieldsTable").tablesorter();
        });

        $('#createSubCompTabs').tabs({
            load: function (event, ui) {
                $("a[name='ajaxLink']", ui.panel).click(function () {
                    $(ui.panel).load(this.href);
                    return false;
                });
            }
        });

        $("#selectAll").change(function () {

            if ($(this).attr('checked')) {
                $("#customFieldsTable input[type='checkbox']").attr("checked", true);
            }
            else {
                $("#customFieldsTable input[type='checkbox']").attr("checked", false);
            }
        });

        $("#details").dialog({
            autoOpen: false,
            height: 'auto',
            width: 800,
            modal: true,
            draggable: false,
            position: 'center',
            resizable: false,
            title: 'Field Details',
            show: 'clip'
        });

        $("a[name='dAjaxLink']").click(function () {
            var link = $(this).attr('href');
            $("#details").load(link, function () { $(this).dialog('open'); });
            return false;
        });

    </script>

    <div id="customFieldsTab">

        <% using (Html.BeginForm("AddCustomFields", "SubComponent"))
           { 
        %>
            <table class="bodyTable" id="customFieldsTable">
                <% if (Model.FirstOrDefault() != null)
                   {%>
                    <thead title="Click to Sort the List"> 
                        <tr>
                            <th>
                                <h3>Select the Custom Field(s) for the Sub-Component being created.</h3>
                           </th>
                        </tr>
                     </thead>
                     <tbody class="rowHover">
                        <%foreach (var item in Model)
                           { %>
                            <tr>
                                <td class="controlPortion">
                                    <%: Html.RenderControl(new ControlInfo()
                                                            {
                                                                ControlID = item.SubCompAttID,
                                                                DefaultValue = null,
                                                                CanNull = false,
                                                                Type = ControlType.CheckBox,
                                                                // If the field was selected then keep that selected.
                                                                Value = (CustomControlsInfo.CustomControlsID.Contains(item.SubCompAttID))?"Yes":null,
                                                                ControlAttName = item.SubCompAttName
                                                            }, true
                                                         )
                                    %>
                                </td>
                                <td>
                                    <%: Html.ActionLink("Details", "DisplayFieldInfo", new { pFieldID = item.SubCompAttID }, new { name = "dAjaxLink" })%> |
                                    <%: Html.ActionLink("Edit", "EditFieldInfo", new { pFieldID = item.SubCompAttID }, new { name = "ajaxLink" })%>
                                </td>
                           </tr>
                    <% } %>
                </tbody>
            </table>
            <br />
            <span class="controlPortion" ><input type="checkbox" id="selectAll" value="Select All" /><b>Select All</b></span>
            <br />
            <br />
            <input class="buttonDesign" type="submit" value="Add" />
            <input class="buttonDesign2" type="button" value="Cancel" id="cancel2" />        
            <%}
            else
            {%>
                <br />
                <h3><i>No Custom Field is created.</i></h3>        
            <% } %>
        <% } %>
        <br />
    </div>

    <div id="details">
    </div>

</asp:Content>

