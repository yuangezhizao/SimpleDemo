<%@ Page Title="" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="SiteClass.aspx.cs" Inherits="WebAppClinet.SiteClass" %>
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

          

          var data = <%=siteData%>;
            if (data!="") {
                $('#sitetree').treeview({
                    data: data, levels: 1,showTags:true,
                    enableLinks: true
                });
            }
        });
        function UnbindCat(classid) {
            var siteid = $("#ContentPlaceHolder1_ddlParen").val();
            location.href = "?catid=" + classid + "&sid=" + siteid;
        }
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">
        <form id="form1" runat="server">
            <asp:DropDownList ID="ddlParen" runat="server"></asp:DropDownList>
            <div id="sitetree" class="col-xs-6"></div>
        </form>
    </div>
    
    

</asp:Content>


