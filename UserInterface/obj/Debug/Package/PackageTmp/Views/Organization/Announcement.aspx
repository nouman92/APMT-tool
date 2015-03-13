<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Announcement
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    
    <script type="text/javascript">

        $(function () {
            $('#announcementTab').show('blind', 700);
        });

        $("#update").click(function () {
            $.get('<%: Url.Action("UpdateAnnouncement", "Organization")  %>', { pMessage: $("#_1").val() }, function (data) {
                if (data) {
                    showNotification("Announcement message updated successfully.");
                }
                else {
                    showError("<span style='color: Red;'>Announcement message did not update successfully.</span>");
                }
            });
        });

    </script>
    
    <div id="announcementTab">

        <br />
        <table class="bodyTable">
            <tr>
                <td class="labelPortion">
                    <b>Announcement Message</b>
                </td>
                <td class="controlPortion">
                    <%: Html.RenderControl(new ControlInfo()
                                                {
                                                    ControlID = 1,
                                                    CanNull = false,
                                                    RegularExpression = null,
                                                    ErrorMessage = null,
                                                    Type = ControlType.TextArea,
                                                    Value = ViewData["message"].ToString()
                                                }, false
                                             )
                    %>
                </td>
            </tr>
        </table>    
        <br />
        <input class="buttonDesign" type="button" value="Update" id="update"/>
        <br />

    </div>

</asp:Content>
