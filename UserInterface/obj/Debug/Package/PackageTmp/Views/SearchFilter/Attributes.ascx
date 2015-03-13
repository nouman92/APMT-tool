<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<%: Html.RenderControl(new ControlInfo ()
                                        {
                                            ControlID = (int)ViewData["attributeRowID"]+200,
                                            DefaultValue = (string)ViewData["attributesOptions"],
                                            Type = (ControlType)4,
                                            ControlAttName = "attribute" + ViewData["attributeRowID"]
                                        }, false
                    )
%>