<%@ Page Title="" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="SpiderConfigList.aspx.cs" Inherits="WebAppClinet.SpiderConfigList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">
        <div class="col-xs-12">
            <div class="table-responsive">
                <form class="form-horizontal" role="form" runat="server">
                    <asp:Repeater ID="repConfigList" runat="server">
                        <HeaderTemplate>
                            <table id="sample-table-1" class="table table-striped table-bordered table-hover">
                                <thead>
                                    <tr>
                                        <th class="hidden-480">方案名称</th>
                                        <th>开始时间</th>
                                        <th>截至时间</th>
                                        <th>间隔时间</th>
                                        <th>权重</th>
                                          <th>状态</th>
                                        <th class="hidden-480">说明</th>
                                      <th>备注</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="hidden-480"><%# Eval("TaskName")%></td>
                                <td><%# Eval("StartTime","{0:yyyy-MM-dd HH:mm:ss}")%></td>
                                <td><%# Eval("StopTime","{0:yyyy-MM-dd HH:mm:ss}")%></td>
                                <td><%# Eval("TimeSpan")%></td>
                                <td><%# Eval("Qzsort")%></td>
                                 <td><%# (bool)Eval("IsAlive")?"启用":"停用"%></td>
                                <td class="hidden-480"><%# Eval("Detial")%></td>
                                <td><%# Eval("TaskRemark")%></td>
                            </tr>
                       </ItemTemplate>
                        <FooterTemplate>
                        </tbody>
                        </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </form>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="foot" runat="server">
</asp:Content>
