<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<%: Html.RenderControl(new ControlInfo ()
                                        {
                                            ControlID = (int)ViewData["matchRowID"] + 500,
                                            DefaultValue = (string)ViewData["matchOptions"],
                                            Type = (ControlType)4,
                                            ControlAttName = "match" + ViewData["matchRowID"]
                                        }, false
                    )
%>
