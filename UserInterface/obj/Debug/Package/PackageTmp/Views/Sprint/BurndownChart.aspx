<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Sprint Burndown Chart
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $('#burnDownTab').show('blind', 700);
        });

    </script> 

    <div id="burnDownTab" class="burndown">
        <img src="<%: TempData["Link"] %>" alt="" />
    </div>

</asp:Content>
