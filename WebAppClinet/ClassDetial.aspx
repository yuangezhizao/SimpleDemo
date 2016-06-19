<%@ Page Title="" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="ClassDetial.aspx.cs" Inherits="WebAppClinet.ClassDetial" %>

<%@ MasterType VirtualPath="~/Admin.Master" %>
<asp:Content ID="Content2" ContentPlaceHolderID="foot" runat="server">

    <script src="/Plugin/bootstrap/js/bootstrap-treeview.js"></script>
    <script>
        $(document).ready(function() {
          
            var data = <%=SiteData%>;
              if (data!="") {
                  $('#sitetree').treeview({
                      data: data, levels: 1,showTags:true,
                      enableLinks: true
                  });
              }
        });
        function delCat(catid) {
            if (!confirm("确定要删除此分类和其子分类吗？"))
                return;
            location.href = "?delId="+catid;
        }
    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server" >
    <form class="form-horizontal" id="addform" method="post" role="form" <%=addform %>>

        <div class="row">
            <div class="col-xs-12">
                <!-- PAGE CONTENT BEGINS -->
                <form class="form-horizontal" role="form">
                    <div class="form-group">
                        <label class="col-sm-3 control-label no-padding-right" for="catName">分类名称 </label>

                        <div class="col-sm-9">
                            <input type="text" id="catName" name="catName" class="col-xs-10 col-sm-5">
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label no-padding-right" for="parCatName">父类名称 </label>

                        <div class="col-sm-9">

                            <select id="parCatName" name="parCatName" class="col-xs-10 col-sm-5">
                                <option value="0"></option>
                                <%=ParaCatInfo %>
                            </select>
                        </div>
                    </div>

                    <div class="space-4"></div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label no-padding-right" for="seoKey">Seo 关键词 </label>

                        <div class="col-sm-9">
                            <input type="text" id="seoKey" name="seoKey" class="col-xs-10 col-sm-5">

                            <span class="help-inline col-xs-12 col-sm-7">
                                <span class="middle">seo 优化</span>
                            </span>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label no-padding-right" for="catSort">排序 </label>

                        <div class="col-sm-9">
                            <input type="text" id="catSort" name="catSort" value="0" class="col-xs-10 col-sm-5">

                            <span class="help-inline col-xs-12 col-sm-7">
                                <span class="middle">正整数，数字越大越前面</span>
                            </span>
                        </div>
                    </div>

                    <div class="space-4"></div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label no-padding-right" for="chkHid">操作 </label>

                        <div class="col-sm-9">

                            <span class="help-inline col-xs-12 col-sm-7">
                                <label class="middle">
                                    <input class="ace" name="chkHid" type="checkbox" id="chkHid">
                                    <span class="lbl">隐藏</span>
                                </label>
                            </span>
                        </div>
                    </div>

                    <div class="clearfix form-actions">
                        <div class="col-md-offset-3 col-md-9">
                            <button class="btn btn-info" type="submit">
                                <i class="ace-icon fa fa-check bigger-110"></i>
                                提交
                            </button>
                            &nbsp; &nbsp; &nbsp;
										<button class="btn" type="reset">
                                            <i class="ace-icon fa fa-undo bigger-110"></i>
                                            重置
                                        </button>
                        </div>
                    </div>



                </form>


            </div>
            <!-- /.col -->
        </div>

    </form>


    <form class="form-horizontal" id="bathform" method="post" role="form"<%=bathform %>>
        <div class="row">
            <h3>批量添加分类</h3>
            <div class="form-group">
                <label class="col-sm-3 control-label no-padding-right" for="parCatName">父类名称 </label>

                <div class="col-sm-9">

                    <select id="parCatName" name="parCatName" class="col-xs-10 col-sm-5">
                        <option value="0"></option>
                        <%=ParaCatInfo %>
                    </select>
                </div>
            </div>
            <div class="form-group">
                <label class="col-sm-3 control-label no-padding-right" for="parCatName">商城分类 </label>
                <div class="col-sm-9">
                    <input type="text" id="SiteCatID" name="SiteCatID" value="<%=SiteCatName %>" class="col-xs-10 col-sm-5">
                </div>
            </div>
  
            <div class="clearfix form-actions">
                <div class="col-md-offset-3 col-md-9">
                    <button class="btn btn-info" type="submit">
                        <i class="ace-icon fa fa-check bigger-110"></i>
                        提交
                    </button>


                </div>
            </div>

        </div>
    </form>
    
    
    <form class="form-horizontal" id="updataform" method="post" role="form" <%=updateform %>>

        <div class="row">
            <div class="col-xs-12">
                <!-- PAGE CONTENT BEGINS -->
                <form class="form-horizontal" role="form">
                    <div class="form-group">
                        <label class="col-sm-3 control-label no-padding-right" for="catName1">分类名称 </label>

                        <div class="col-sm-9">
                            <input type="text" id="catName1" value="<%=oldCatName %>" name="catName" class="col-xs-10 col-sm-5">
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label no-padding-right" for="parCatName1">父类名称 </label>

                        <div class="col-sm-9">

                            <select id="parCatName1" name="parCatName" class="col-xs-10 col-sm-5">
                                <option value="0"></option>
                                <%=ParaCatInfo %>
                            </select>
                        </div>
                    </div>

                    <div class="space-4"></div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label no-padding-right" for="seoKey1">Seo 关键词 </label>

                        <div class="col-sm-9">
                            <input type="text" id="seoKey1" name="seoKey" value="<%=oldSeo %>" class="col-xs-10 col-sm-5">

                            <span class="help-inline col-xs-12 col-sm-7">
                                <span class="middle">seo 优化</span>
                            </span>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label no-padding-right" for="catSort1">排序 </label>

                        <div class="col-sm-9">
                            <input type="text" id="catSort1" name="catSort" value="<%=oldSort %>" class="col-xs-10 col-sm-5">

                            <span class="help-inline col-xs-12 col-sm-7">
                                <span class="middle">正整数，数字越大越前面</span>
                            </span>
                        </div>
                    </div>

                    <div class="space-4"></div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label no-padding-right" for="chkHid">操作 </label>

                        <div class="col-sm-9">

                            <span class="help-inline col-xs-12 col-sm-7">
                                <label class="middle">
                                    <input class="ace" name="chkHid" type="checkbox" <%=oldHide %> id="chkHid">
                                    <span class="lbl">隐藏</span>
                                </label>
                            </span>
                        </div>
                    </div>

                    <div class="clearfix form-actions">
                        <div class="col-md-offset-3 col-md-9">
                            <button class="btn btn-info" type="submit">
                                <i class="ace-icon fa fa-check bigger-110"></i>
                                提交
                            </button>
             
                        </div>
                    </div>



                </form>


            </div>
            <!-- /.col -->
        </div>
    <div id="sitetree" class="col-xs-12"></div>
    </form>


</asp:Content>


