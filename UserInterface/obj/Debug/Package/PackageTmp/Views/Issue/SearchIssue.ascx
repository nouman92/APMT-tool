<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<script type="text/javascript">

    $(function () {
        $('#searchValue3').hide();
    });

    $("#searchOptions3").change(function () {
        var id = $(this).val();

        if (id > -1) {
            $("#searchField3").load('<%: Url.Action("SearchField", "Issue")  %>', { pFieldID: id });
            $('#searchValue3').show();
        }
        else {
            $('#searchValue3').hide();
        }
    });

    $('#searchIssue').validate();

    $('#searchButton3').click(function () {
        if ($('#searchIssue').validate().form()) {
            var fieldID = $('#searchOptions3').val();
            var field = $('#searchField3 *:first');
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
    <% using (Html.BeginForm("SearchIssue", "Issue", new { pBacklog = 0}, FormMethod.Post, new { id = "searchIssue" }))
       { %>
        <span>
            <b style="padding-left:10px;">Search By</b>&nbsp &nbsp &nbsp &nbsp &nbsp&nbsp
        </span>
        <span>
            <%: Html.DropDownList("searchOptions", (IEnumerable<SelectListItem>)ViewData["searchOptions"], new { id = "searchOptions3" })%>  
        </span>
        <br />
        <br />
        <br />
        <div id="searchValue3">
            <span >
                <b style="padding-left:10px;">Search Value</b>&nbsp &nbsp &nbsp
            </span>
            <span id="searchField3">
            </span>
            <br />
            <br />
            <br />
            <input style="margin-left:40%;" id="searchButton3" class="buttonDesign" type="submit" value="Search" />
        </div>
    <%} %>
    <br />
    <br />
</div>
