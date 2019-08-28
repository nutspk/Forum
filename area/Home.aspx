<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/Main.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="Selectcon.Home" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="../Scripts/js-question.js"></script>
    <script>
        $("ol.nav-links > li").removeClass("youarehere");
        $('li.home').addClass('youarehere');
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="C_mainbar" runat="server">
    <div class="grid mb16">
    <h1 class="grid--cell fl1 fs-headline1"> กระทู้ล่าสุด </h1>

    <div class="pl8 aside-cta grid--cell" role="navigation" aria-label="ask new question">
      <a id="NewAsk" runat="server" href="../area/ask" class="d-inline-flex ai-center ws-nowrap s-btn s-btn__primary"> ตั้งกระทู้ </a>
    </div>

  </div>



  <div id="qlist-wrapper" class="flush-left">
    <div id="question-mini-list">
    </div>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="C_sidebar" runat="server">
</asp:Content>
