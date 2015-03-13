<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<script type="text/javascript">

    $(function () {
        $('#searchValue').hide();
    });

    $("#searchOptions").change(function () {
        var id = $(this).val();

        if (id > -1) {
            $("#searchField").load('<%: Url.Action("SearchField", "SubComponent")  %>', { pFieldID: id });
            $('#searchValue').show();
        }
        else {
            $('#searchValue').hide();
        }
    });

    $('#searchSubComp').validate();

    $('#searchButton').click(function () {
        if ($('#searchSubComp').validate().form()) {
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
        }
    });

</script>
    
<br />
<div>
    <% using (Html.BeginForm("SearchSubComponent", "SubComponent", FormMethod.Post, new { id = "searchSubComp" }))
    { %>
        <span>
            <b style="padding-left:10px;">Search By</b>&nbsp &nbsp &nbsp &nbsp &nbsp&nbsp
        </span>
        <span>
            <%: Html.DropDownList("searchOptions", (IEnumerable<SelectListItem>)ViewData["searchOptions"], new { id = "searchOptions" })%>  
        </span>
        <br />
        <br />
        <br />
        <div id="searchValue">
            <span >
                <b style="padding-left:10px;">Search Value</b>&nbsp &nbsp &nbsp
            </span>
            <span id="searchField">
            </span>
            <br />
            <br />
            <br />
            <input style="margin-left:40%;" id="searchButton" class="buttonDesign" type="submit" value="Search" />
        </div>
    <%} %>
    <br />
    <br />
</div>

