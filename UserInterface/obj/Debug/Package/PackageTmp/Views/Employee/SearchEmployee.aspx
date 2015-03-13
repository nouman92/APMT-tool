<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Search User
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $('#searchTab').show('blind', 700);
            $('#searchValue').hide();
            $('#fieldPortion').hide();
            $("#searchEmployee").bind("submit", function () { return false; });
        });

        $("#searchOptions").change(function () {
            var id = $(this).val();

            if (id > -1) {
                $("#searchField").load("SearchField", { pFieldID: id });
                $('#searchValue').show();
                $('#fieldPortion').show();
            }
            else {
                $('#searchValue').hide();
                $('#fieldPortion').hide();
            }

        });

        $('#searchEmployee').validate();

        $('#searchButton').click(function () {
            if ($('#searchEmployee').validate().form()) {
                var fieldID = $('#searchOptions').val();
                var field = $('#searchField *:first');
                var fieldValue = "";

                if (field.attr('type') == 'checkbox') {
                    if (field.attr('checked') == true) {
                        fieldValue = "Yes";
                    }
                    else {
                        fieldValue = null;
                    }
                }
                else {
                    fieldValue = field.val();
                }
                $("#result").load("SearchEmployee", { pFieldID: fieldID, pSearchValue: fieldValue }, function () {

                    $("#details").dialog({
                        autoOpen: false,
                        height: 'auto',
                        width: 800,
                        modal: true,
                        draggable: false,
                        position: 'center',
                        resizable: false,
                        title: 'User Details',
                        show: 'clip'
                    });

                    $("#result a[name='dAjaxLink']").click(function () {
                        var link = $(this).attr('href');
                        $("#details").load(link, function () { $("#details").dialog('open'); });
                        return false;
                    });
                });
            }
        });

    </script>
    
    <br />
    <div id="searchTab">
        <% using (Html.BeginForm("SearchEmployee", "Employee", FormMethod.Post, new { id = "searchEmployee" }))
        { %>
            <span class="labelPortion">
                <b>Search By</b>
            </span>
            <span class="controlPortion">
                <%: Html.DropDownList("searchOptions", (IEnumerable<SelectListItem>)ViewData["searchOptions"], new { id = "searchOptions" })%>  
            </span>
            <br />
            <br />
            <br />
            <div id="searchValue">
                <span class="labelPortion">
                    <b>Search Value</b>
                </span>
            </div>
            <div id="fieldPortion">
                <span class="controlPortion" id="searchField">
                </span>
                <br />
                <br />
                <br />
                <input id="searchButton" class="buttonDesign" type="button" value="Search" />
            </div>
        <%} %>
        <br />
        <br />
        <br />
        <br />
    </div>

    <div id="result">    
    </div>

    <div id="details">
    </div>
</asp:Content>
