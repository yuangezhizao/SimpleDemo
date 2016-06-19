<%@ Page Title="" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="BindCat.aspx.cs" Inherits="WebAppClinet.BindCat" %>
<%@ MasterType VirtualPath="~/Admin.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<link href="Plugin/bootstrap/css/select2.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="foot" runat="server">
<script src="Plugin/bootstrap/js/select2.min.js"></script>
<script src="Plugin/bootstrap/js/select2_locale_zh-CN.js"></script>
<script src="/Plugin/bootstrap/js/bootstrap-treeview.js"></script>
    <script>
        $(document).ready(function() {
            $("#ContentPlaceHolder1_ddlParen").select2({
                placeholder:"请选择",dropdownAutoWidth:true
                
            });
            $("#ContentPlaceHolder1_ddlParen").on("select2-selecting", function (e) {
                var siteid = e.val;
         
                location.href = "?sid=" + siteid;
            });
            var data = <%=SiteData%>;
            var catData = <%=CatData%>;
            if (data!="") {
                $('#sitetree').treeview({
                    data: data, levels: 1,showTags:true,
                    enableLinks: true
                });
            }
            if (catData!="") {
                $('#cattree').treeview({
                    data: catData, levels: 10,showTags:true,
                    enableLinks: true
                });
            }
        });

        function Bing(classid) {
            var siteClassId = $('#hidSiteCatId').val();
            location.href = "?Catid=" + classid + "&SiteCat=" + siteClassId+"&sid="+  $("#ContentPlaceHolder1_ddlParen").val();
        }

        function bindCat(id,catName) {
            $('#txtBindClass').val(catName);
            $('#hidSiteCatId').val(id);
            
            var same;
            var same1;
            var same2;
            $("#cattree li>a").removeClass("red");
            $("#cattree li>a").each(function(a, b) {
              
                if (b.text == catName) {
                    same = b;
                }
                else if (b.text.indexOf(catName) > -1) {
                    same1 = b;
                }
                else if (catName.indexOf(b.text) > -1) {
                    same2 = b;
                }

            });

            if (same) {
                same.focus();
                $(same).addClass("red");
            }
            else if(same1) {
                same1.focus();
                $(same1).addClass("red");
            }
            else if(same2) {
                same2.focus();
                $(same2).addClass("red");
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">
          <div class="col-xs-12">
             <%=Msg %>
          </div>
    </div>
    <div class="row">
            <div id="sitetree" class="col-xs-5"></div>
            <div class="col-xs-2" runat="server"> <form id="form1" runat="server"> <asp:DropDownList ID="ddlParen" runat="server"></asp:DropDownList> </form>
                <textarea name="txtBindClass" rows="2" id="txtBindClass"></textarea>

            </div>
            <div id="cattree" class="col-xs-5"></div>
        <input type="hidden" id="hidSiteCatId" />
       
    </div>
    
    

</asp:Content>

