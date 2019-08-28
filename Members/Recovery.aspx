<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/Main.Master" AutoEventWireup="true" CodeBehind="Recovery.aspx.cs" Inherits="Selectcon.Members.Recovery" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <script type="text/javascript">

         function SendMail() {
             var elemEmail = $('#txtEmail');
             var email = elemEmail.val();
             var validEmail = $('.valid-email');

             var myData = { 'email':  email };

             if (!IsEmpty(email)) {

                 $.ajax({
                    type: "POST",
                    contentType: "application/x-www-form-urlencoded",
                    url: '../api/v1/Recovery',
                    data: myData,
                    success: function (data) {
                        var obj = data;
                        if (obj.success) {
                            recovery.addClass("d-none")
                            $('.success').removeClass("d-none");
                            elemEmail.val("");
                        } else {
                            elemEmail.addClass("has-error");
                            validEmail.addClass("error-message");
                            validEmail.removeClass("d-none");
                            $('p.valid-email').html("ไม่พบ email address นี้ในระบบ");
                        }
                    }, error: function (xhr, data) {
                        elemEmail.addClass("has-error");
                        validEmail.addClass("error-message");
                        validEmail.removeClass("d-none");
                        $('p.valid-email').html("ไม่พบ email address นี้ในระบบ");
                    }
                });  
            } else {
                $('input').removeClass("has-error");
                $('[class*="valid-"]').addClass("d-none");
                
                if (IsEmpty(ID)) {
                    elemID.addClass("has-error");
                    validID.addClass("error-message");

                    validID.removeClass("d-none");
                    $('p.valid-id').html("กรุณากรอก email address");
                }
             }

         }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="C_mainbar" runat="server">
    
     <div class="grid--cell recovery">
      <div class="ta-center fs-title mx-auto mb24">
      </div>
      <div id="formContainer" class="wmx3 mx-auto mb24 p24 bg-white bar-lg auth-shadow">
          <p>ไม่สามารถเข้าสู่ระบบใช่หรือไม่ เราจะส่งข้อมูลไปยัง email address ที่ท่านสมัครสมาชิกไว้</p>
        <div  class="grid fd-column gs12 gsy">
          

          <div class="grid fd-column gs4 gsy ">
            <label class="grid--cell s-label" for="txtEmail">Email address</label>
            <div class="grid ps-relative">
              <input class="s-input" id="txtEmail" type="text" size="30" maxlength="100" name="txtEmail" tabindex="1">
              <svg aria-hidden="true" class="svg-icon s-input-icon js-alert-icon d-none iconAlertCircle valid-email" width="18" height="18" viewBox="0 0 18 18"><path d="M9 17A8 8 0 1 1 9 1a8 8 0 0 1 0 16zM8 4v6h2V4H8zm0 8v2h2v-2H8z"></path></svg>
            </div>
            <p class="s-input-message mb0 d-none valid-email">

            </p>
          </div>

         
          <button class="grid--cell s-btn s-btn__primary" id="btnSubmit" name="btnSubmit" type="button" onclick="SendMail();" tabindex="3">
                    Send recovery email
          </button>

        </div>
      </div>


      <div class="mx-auto ta-center fs-body1 p16 pb0 mb24 w100 wmx3">
        <div class="mt12">
        </div>
      </div>
    </div>

    <div class="w100 wmx5 mx-auto s-notice s-notice__success grid sm:fd-column sm:ai-center success d-none">
        <div class="grid--cell mtn4 mr8 sm:mr0 sm:mb8">
            <svg aria-hidden="true" class="svg-icon iconCheckmarkLg" width="36" height="36" viewBox="0 0 36 36"><path d="M6 14l8 8L30 6v8L14 30l-8-8z"></path></svg>
        </div>
        <div class="grid fd-column">
            <div class="grid--cell fs-body3 mb4">
                Account recovery email sent to a@mail.com
            </div>
            <div class="grid--cell fs-body1">
                <p>ถ้าหากคุณยังไม่ได้รับ email ให้คุณตรวจสอบใน 'โฟลเดอร์เมลขยะ'
                และถ้าหากภายใน 15 นาที กรุณาติดต่อเราที่นี่ <a href="#" onclick="window.open('https://selectcon.com/ContactUs.asp', '_blank')"> คลิ้กที่นี่ </a></p>
            </div>
        </div>
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="C_sidebar" runat="server">
</asp:Content>
