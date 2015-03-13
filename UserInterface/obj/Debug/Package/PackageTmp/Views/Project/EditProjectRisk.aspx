<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TabContent.Master" Inherits="System.Web.Mvc.ViewPage<Domain.Entities.ProjectRisk>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Edit Project Risk
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">
        $("#editProjectRisk").validate();
    </script>

    <h3>Edit Risk - <%:Model.Risk.Description %></h3>
    <br />
    <% using (Html.BeginForm("EditProjectRisk", "Project", new { pRiskID = Model.RiskID }, FormMethod.Post, new { id = "editProjectRisk" }))
        { %>
        <table class="bodyTable">
            <tr>
                <td class="controlPortion">
                    <br /><span style="padding-left:25%;">Probability</span>&nbsp
                    <%: Html.DropDownList("currentProbValue", new[] {
                                                            new SelectListItem { Text = "1", Value = "1" },
                                                            new SelectListItem { Text = "2", Value = "2" },
                                                            new SelectListItem { Text = "3", Value = "3" },
                                                            new SelectListItem { Text = "4", Value = "4" },
                                                            new SelectListItem { Text = "5", Value = "5" },
                                                            new SelectListItem { Text = "6", Value = "6" },
                                                            new SelectListItem { Text = "7", Value = "7" },
                                                            new SelectListItem { Text = "8", Value = "8" },
                                                            new SelectListItem { Text = "9", Value = "9" },
                                                            new SelectListItem { Text = "10", Value = "10" },
                                                            }, new { id = "prob_" })%>
                       <span style="padding-left:5%;">Impact</span>&nbsp
                    <%: Html.DropDownList("currentImpValue", new[] {
                                                            new SelectListItem { Text = "1", Value = "1" },
                                                            new SelectListItem { Text = "2", Value = "2" },
                                                            new SelectListItem { Text = "3", Value = "3" },
                                                            new SelectListItem { Text = "4", Value = "4" },
                                                            new SelectListItem { Text = "5", Value = "5" },
                                                            new SelectListItem { Text = "6", Value = "6" },
                                                            new SelectListItem { Text = "7", Value = "7" },
                                                            new SelectListItem { Text = "8", Value = "8" },
                                                            new SelectListItem { Text = "9", Value = "9" },
                                                            new SelectListItem { Text = "10", Value = "10" },
                                                            }, new { id = "imp_" })%>
                    <br />
                    <br />
                    <span style="padding-left:25%;">Mitigation</span><br/>
                    <span style="padding-left:25%;"></span><%: Html.RenderControl(new ControlInfo()
                                                {
                                                    ControlID = 2,
                                                    CanNull = true,
                                                    RegularExpression = null,
                                                    ErrorMessage = null,
                                                    Type = ControlType.TextArea,
                                                    Value = Model.Mitigation
                                                }, false
                                                )
                    %>
                    <br />
                    <br />
                </td>
            </tr>
        </table>    
        <br />
        <input class="buttonDesign" type="submit" value="Save"/>
        <a class="linkButton2" href='<%: Url.Action("MoreActionsTabs", "Project") %>'> <span> Cancel </span></a>
    <% } %>
    <br />

</asp:Content>
