<%@ Page Title="" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="ClassList.aspx.cs" Inherits="WebAppClinet.ClassList" %>
<%@ MasterType VirtualPath="~/Admin.Master" %>
<asp:Content ID="Content2" ContentPlaceHolderID="foot" runat="server">

    <script src="/Plugin/bootstrap/js/bootstrap-treeview.js"></script>
    
    <script>
        $(document).ready(function() {
          
            var data = <%=siteData%>;
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
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div id="sitetree" class="col-xs-12"></div>
</asp:Content>
