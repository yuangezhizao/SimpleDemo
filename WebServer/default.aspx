<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="WebServer._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>京东热卖</title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="padding: 200px;text-align:center">
    <input style="margin-right: 5px; width: 400px" class="searchinput" maxlength="512" aria-haspopup="true" role="combobox" id="SearchTxt" name="SearchTxt" title="请输入京东id 或者url地址" /><input type="submit" class="search" value="去购买" />
        <br/>     <br/>     <br/>

        请输入 京东商城 网页地址如：https://item.jd.com/3133853.html
             <br/>     <br/>     <br/>
          <asp:Label ID="lblmsg" style="color: red" runat="server" Text=""></asp:Label>
    </div>
      
    </form>
</body>
</html>
