<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/Main.Master" AutoEventWireup="true" CodeBehind="Confirm.aspx.cs" Inherits="Selectcon.Members.Confirm" ValidateRequest="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://www.google.com/recaptcha/api.js?hl=th&render=<%= SiteKey%>"></script>

    <script>
        $(function () {
            grecaptcha.ready(function() {
                  grecaptcha.execute('<%= SiteKey%>', {action: 'posts'}).then(function(token) {
                      $('#reCaptchaToken').val(token);
                      $('input[id$=btnCheck]').click();
                  });
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="C_mainbar" runat="server">
    <input type="hidden" name="reCaptchaToken" id="reCaptchaToken" />
    <asp:Button ID="btnCheck" runat="server" OnClick="btnCheck_Click" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="C_sidebar" runat="server">
</asp:Content>
