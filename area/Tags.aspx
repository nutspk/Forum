<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/Main.Master" AutoEventWireup="true" CodeBehind="Tags.aspx.cs" Inherits="Selectcon.area.Tags" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        $("ol.nav-links > li").removeClass("youarehere");
        $('li.tag').addClass('youarehere');
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="C_mainbar" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="C_sidebar" runat="server">
</asp:Content>
