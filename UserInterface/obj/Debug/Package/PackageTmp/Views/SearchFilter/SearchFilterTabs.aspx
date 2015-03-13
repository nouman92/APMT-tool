<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Search Filters    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $("#filtersTabs").tabs({ selected: parseInt($("#filtersTabNumber").val(), 10) });
        });

    </script>

    <input type="hidden" id="filtersTabNumber" name="filtersTabNumber" value='<%: Session["filtersTabNumber"] %>' />

    <div id="filtersTabs">
        <ul>
             <li><a href='<%: Url.Action("DisplayFilters", "SearchFilter") %>'><span>Filters</span></a></li>
             <li><a href='<%: Url.Action("CreateFilter", "SearchFilter") %>'><span>Create Filter</span></a></li>
        </ul>
    </div>

</asp:Content>

