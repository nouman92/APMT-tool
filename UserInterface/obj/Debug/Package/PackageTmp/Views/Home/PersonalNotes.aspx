<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.PersonalNote>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Personal Notes
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $('#pNotesTab').show('blind', 700);
        });

        $(function () {
            $("#pNotesTable").tablesorter();
        });

        $('#homeTabs').tabs({
            load: function (event, ui) {
                $("a[name='ajaxLink']", ui.panel).click(function () {
                    $(ui.panel).load(this.href);
                    return false;
                });
            }
        });

        $("#details2").dialog({
            autoOpen: false,
            height: 'auto',
            width: 800,
            modal: true,
            draggable: false,
            position: 'center',
            resizable: false,
            title: 'Note Details',
            show: 'clip'
        });

        $("a[name='dAjaxLink']").click(function () {
            var link = $(this).attr('href');
            $("#details2").load(link, function () { $(this).dialog('open'); });
            return false;
        });

    </script>

    <div id="pNotesTab">

        <span style="float:right;" title="Create new Note">[ <%: Html.ActionLink("Create", "CreateNote", "Employee", null, new { name = "ajaxLink"})%> ]</span>
        <br />
        <% if (Model.FirstOrDefault() != null)
            {%>
            <table class="bodyTable" id="pNotesTable">
                <thead title="Click to Sort the List"> 
                    <tr>
                        <th>
                            <h3>Subject</h3>
                        </th>
                        <th>
                            <h3>Creation Date</h3>
                        </th>
                    </tr>
                    </thead>
                    <tbody class="rowHover">
                    <%foreach (var item in Model)
                        { %>
                        <tr>
                            <td class="tableColPadding" style="width:55%;">       
                                    <%: item.Subject%>
                            </td>
                            <td style="width:25%;">
                                <%: item.CreationDate.ToString("dd MMMM yyyy")%>
                            </td>
                            <td style="width:20%;">
                                    <%: Html.ActionLink("Details", "DisplayNote", "Employee", new { pNoteID = item.NoteID }, new { name = "dAjaxLink" })%> | 
                                    <%: Html.ActionLink("Edit", "EditNote", "Employee", new { pNoteID = item.NoteID }, new { name = "ajaxLink" })%> | 
                                    <%: Html.ActionLink("Delete", "DeleteNote", "Employee", new { pNoteID = item.NoteID }, null)%>
                            </td>
                        </tr>
                    <% } %>
                </tbody>
            </table>
        <%}
        else
        {%>
            <br />
            <h3><i>No personal notes are available.</i></h3>
        <% } %>
        <br />

    </div>

    <div id="details2">
    </div>

</asp:Content>

