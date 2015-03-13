<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<%: Html.RenderControl(new ControlInfo ()
                                        {
                                            ControlID = (int)ViewData["operatorRowID"] + 400,
                                            DefaultValue = (string)ViewData["operatorOptions"],
                                            Type = (ControlType)4,
                                            ControlAttName = "operator" + ViewData["operatorRowID"]
                                        }, false
                    )
%>