<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<script type="text/javascript">

    $(function () {
        $('#searchValue2').hide();
    });

    $("#searchOptions2").change(function () {
        var id = $(this).val();

        if (id > -1) {
            $("#searchField2").load('<%: Url.Action("SearchField", "Sprint")  %>', { pFieldID: id });
            $('#searchValue2').show();
        }
        else {
            $('#searchValue2').hide();
        }
    });

    $('#searchSprint').validate();

    $('#searchButton2').click(function () {
        if ($('#searchSprint').validate().form()) {
            var fieldID = $('#searchOptions2').val();
            var field = $('#searchField2 *:first');
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
    <% using (Html.BeginForm("SearchSprint", "Sprint", FormMethod.Post, new { id = "searchSprint" }))
    { %>
        <span>
            <b style="padding-left:10px;">Search By</b>&nbsp &nbsp &nbsp &nbsp &nbsp&nbsp
        </span>
        <span>
            <%: Html.DropDownList("searchOptions", (IEnumerable<SelectListItem>)ViewData["searchOptions"], new { id = "searchOptions2" })%>  
        </span>
        <br />
        <br />
        <br />
        <div id="searchValue2">
            <span >
                <b style="padding-left:10px;">Search Value</b>&nbsp &nbsp &nbsp
            </span>
            <span id="searchField2">
            </span>
            <br />
            <br />
            <br />
            <input style="margin-left:40%;" id="searchButton2" class="buttonDesign" type="submit" value="Search" />
        </div>
    <%} %>
    <br />
    <br />
</div>
