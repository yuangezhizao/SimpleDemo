<%@ Page Title="" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="SpiderProSet.aspx.cs" Inherits="WebAppClinet.SpiderProSet" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="assets/css/chosen.css" />


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">
        <div class="col-xs-12">
            <form class="form-horizontal" role="form" runat="server">
                <div class="form-group">
                    <label class="col-sm-3 control-label no-padding-right" for="caseName">方案名称 </label>
                    <div class="col-sm-9">

                        <asp:TextBox ID="txtCatName" runat="server" CssClass="col-xs-10 col-sm-5"></asp:TextBox>
                    </div>
                </div>
                
                <div class="form-group">
                    
                    <label class="col-sm-3 control-label no-padding-right" for="ddlCaseType">方案类型 </label>
                    <div class="col-sm-9">
                        <asp:DropDownList ID="ddlCaseType" runat="server"></asp:DropDownList>
                    </div>
                </div>
                <div class="form-group">
                    
                    <label class="col-sm-3 control-label no-padding-right" for="ddlCaseType">排序方式 </label>
                    <div class="col-sm-9">
                        <asp:DropDownList ID="ddlSort" runat="server"></asp:DropDownList>
                    </div>
                </div>
                    <div class="form-group">
                    
                    <label class="col-sm-3 control-label no-padding-right" for="chkType">状态 </label>
                    <div class="col-sm-9">
                      <asp:CheckBox ID="chkType" runat="server" Checked="True" />
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-sm-3 control-label no-padding-right" for="caseName">分类 </label>
                    <div id="sitetree" class="col-sm-9">
                        <asp:TreeView ID="catTree" runat="server" OnTreeNodeCheckChanged="catTree_TreeNodeCheckChanged" ShowCheckBoxes="All"></asp:TreeView>
                        
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-sm-3 control-label no-padding-right" for="siteInfo">商城 </label>
                    <div class="col-sm-9">
                        <select multiple="" class="width-40 chosen-select" id="siteInfo" data-placeholder="选择商城...">
                            <option value="">&nbsp;</option>
                            <%=SiteList %>
                        </select>
                       <span class="help-button" data-rel="popover" data-trigger="hover" data-placement="left" data-content="默认为全部商城" title="" data-original-title="提示">?</span>
                        <input type="hidden" id="sites" name="sites" />
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-sm-3 control-label no-padding-right" for="areas">地区 </label>
                    <div class="col-sm-9">

                        <select multiple="" class="width-40 chosen-select" id="areas" data-placeholder="选择地区...">
                            <option value="">&nbsp;</option>
                            <%=AreaList %>
                        </select>
                         <span class="help-button" data-rel="popover" data-trigger="hover" data-placement="left" data-content="默认为全部地区" title="" data-original-title="提示">?</span>
                        <input type="hidden" id="hidareas" name="hidareas" />
                    </div>
                </div>
                <div class="form-group">
                        <label class="col-sm-3 control-label no-padding-right" for="txtStartTime">开始时间 </label>
                    <div class="col-sm-9">

                        <asp:TextBox ID="txtStartTime" runat="server" CssClass="col-xs-10 col-sm-5" TextMode="DateTime"></asp:TextBox>
                    </div>
                    </div>
                <div class="form-group">
                        <label class="col-sm-3 control-label no-padding-right" for="txtStartTime">截止时间 </label>
                    <div class="col-sm-9">
                        <asp:TextBox ID="txtStopTime" runat="server" CssClass="col-xs-10 col-sm-5" TextMode="DateTime"></asp:TextBox>
                    </div>
                    </div>
                <div class="form-group">
                        <label class="col-sm-3 control-label no-padding-right" for="txtTimeSpan">间隔时间 </label>
                   
                    
                    <div class="col-sm-9">
                         <asp:TextBox ID="txtTimeSpan" runat="server" CssClass="col-xs-10 col-sm-5" TextMode="Number"></asp:TextBox>

											<span class="help-inline col-xs-12 col-sm-7">
												<span class="middle">单位：秒</span>
											</span>
										</div>

                    </div>
                <div class="form-group">
                        <label class="col-sm-3 control-label no-padding-right" for="txtTimeSpan">最大页数 </label>
                    <div class="col-sm-9">
                        <asp:TextBox ID="txtMaxPage" runat="server" CssClass="col-xs-10 col-sm-5" TextMode="Number"></asp:TextBox>
                    </div>
                    </div>
                <div class="form-group">
                        <label class="col-sm-3 control-label no-padding-right" for="txtqz">权重 </label>
                    <div class="col-sm-9">
                        <asp:TextBox ID="txtqz" runat="server" CssClass="col-xs-10 col-sm-5"></asp:TextBox>
                    </div>
                    </div>
                   <div class="form-group">
                        <label class="col-sm-3 control-label no-padding-right" for="txtRemark">备注 </label>
                    <div class="col-sm-9">
                        <asp:TextBox  ID="txtRemark" runat="server" CssClass="col-xs-10 col-sm-5" TextMode="MultiLine"></asp:TextBox>
                    </div>
                    </div>
                
                
                    <div class="clearfix form-actions">
                        <div class="col-md-offset-3 col-md-9">
                      
                             <asp:Button ID="btnOk" CssClass="btn btn-info" runat="server" Text="提交" OnClick="btnOk_Click" />
                            &nbsp; &nbsp; &nbsp;
										<button class="btn" type="reset">
                                            <i class="ace-icon fa fa-undo bigger-110"></i>
                                            重置
                                        </button>
                        </div>
                    </div>
                
            </form>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="server">


    <script src="assets/js/chosen.jquery.min.js"></script>
    <script>

        function postBackByObject() {
            var o = window.event.srcElement;
            if (o.tagName == "INPUT" && o.type == "checkbox") {
                __doPostBack("", "");
            }
        }


        jQuery(function ($) {

            $("#siteInfo").chosen();
            $("#areas").chosen();
            
            $("#siteInfo").change(function (a, b) {
                $("#sites").val(chose_get_text('#siteInfo'));
            });
            $("#areas").change(function (a, b) {
                $("#hidareas").val(chose_get_text('#areas'));
            });

            function chose_get_text(select) {
                var res = "";
                $(select + " option:selected").each(function (a, b) {
                    res += b.value + ",";
                });

                return res;
            }
        });
        $('[data-rel=popover]').popover({ container: 'body' });

    </script>
</asp:Content>
