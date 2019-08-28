<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/Main.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="Selectcon.Members.Register" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

        function validateEmail(email) {
            var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            return re.test(String(email).toLowerCase());
        }

        function CheckData() {
            $('input').removeClass("has-error");
            $('[class*="valid-"]').addClass("d-none");

            var err = "";
            var elem1 = $('#txtDisplayName');
            var val1 = elem1.val();
            var valid1 = $('.valid-displayname');
              
            var elem2 = $('#txtUserName');
            var val2 = elem2.val();
            var valid2 = $('.valid-username');

            var elem3 = $('#txtEmail');
            var val3 = elem3.val();
            var valid3 = $('.valid-email');

            var elem4 = $('#txtPassword');
            var val4 = elem4.val();
            var valid4 = $('.valid-pwd');

            if (val1 == "" || val1.length < 5) {
                elem1.addClass("has-error");
                valid1.addClass("error-message d-block");

                err = "a";
            }

            if (val2 == "" || val2.length < 6) {
                elem2.addClass("has-error");
                valid2.addClass("error-message d-block");
                err = "a";
            }

            if (val3 == "" || !validateEmail(val3)) {
                elem3.addClass("has-error");
                valid3.addClass("error-message d-block");
                err = "a";
            }

            if (val4 == "" || val4.length < 6) {
                elem4.addClass("has-error");
                valid4.addClass("error-message d-block");
                err = "a";
            }

            return (err == "")
        }

        function Register() {

            if (!CheckData()) {
                return false;
            }

            var myData = {
                'DisplayName': $('[id=txtDisplayName]').val(),
                'UserName': $('[id=txtUserName]').val(),
                'Email': $('[id=txtEmail]').val(),
                'Password': $('[id=txtPassword]').val(),
            };

           $.ajax({
                type: "POST",
                contentType: "application/x-www-form-urlencoded",
                dataType: "json",
                url: '../api/v1/Member/regis',
                data: myData,  
                success: function (data) {
                    $(".success").removeClass("d-none");
                    $(".register").addClass("d-none");
                    $("#email").html(data.Email);
                    $(".error-msg").addClass("d-none");
               }, error: function (data) {
                    var obj = data.responseJSON;
               if (obj === undefined || obj == null) {
                   $("#error-msg").html("ลงทะเบียนผิดพลาด กรุณาลองใหม่อีกครั้ง");
                   $(".error-msg").removeClass("d-none");
               } else if (IsEmpty(obj.UserName)) {
                   $("#error-msg").html("ลงทะเบียนผิดพลาด Username นี้มีอยู่ในระบบแล้ว");
                   $(".error-msg").removeClass("d-none");
               } else if (IsEmpty(obj.Email)) {
                   $("#error-msg").html("ลงทะเบียนผิดพลาด Email นี้มีอยู่ในระบบแล้ว");
                   $(".error-msg").removeClass("d-none");
               } else {
                    $(".success").removeClass("d-none");
                    $(".register").addClass("d-none");
                    $("#email").html(obj.Email);
                }
               }
           }).then(function (data) {
               
           });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="C_mainbar" runat="server">
     <input type="hidden" name="action" />


    <div class="grid--cell register">
      <div class="ta-center fs-title mx-auto mb24">
          <h1 class="grid--cell fl1 fs-headline1">Register Now.</h1>
          <span>Create your personal account</span>
      </div>
      <div id="formContainer" class="wmx3 mx-auto mb24 p24 bg-white bar-lg auth-shadow">

        <div  class="grid fd-column gs12 gsy">
          <input type="hidden" name="fkey" value="eda908868f0ec06f1712e83466b921e0c38bfe1fa9d2ccaad2244bc86da07d1a">

            <div class="grid fd-column gs4 gsy ">
            <label class="grid--cell s-label" for="txtUserName">Display name <span class="error-message">*</span></label>
            <div class="grid ps-relative">
              <input class="s-input" id="txtDisplayName" type="text" maxlength="50" name="txtDisplayName">
                <svg aria-hidden="true" class="svg-icon s-input-icon js-alert-icon d-none iconAlertCircle valid-displayname" width="18" height="18" viewBox="0 0 18 18"><path d="M9 17A8 8 0 1 1 9 1a8 8 0 0 1 0 16zM8 4v6h2V4H8zm0 8v2h2v-2H8z"></path></svg>
            </div>
            <p class="s-input-message mb0 d-none valid-displayname">
                กรอกชื่อที่แสดง
            </p>
          </div>

          <div class="grid fd-column gs4 gsy ">
            <label class="grid--cell s-label" for="txtUserName">Username <span class="error-message">*</span></label>
            <div class="grid ps-relative">
              <input class="s-input" id="txtUserName" type="text" maxlength="30" name="txtUserName">
              <svg aria-hidden="true" class="svg-icon s-input-icon js-alert-icon d-none iconAlertCircle valid-username" width="18" height="18" viewBox="0 0 18 18"><path d="M9 17A8 8 0 1 1 9 1a8 8 0 0 1 0 16zM8 4v6h2V4H8zm0 8v2h2v-2H8z"></path></svg>
            </div>
            <p class="s-input-message mb0 d-none valid-username">
                username ควรมีอย่างน้อย 6 ตัวอักษร
            </p>
          </div>

            <div class="grid fd-column gs4 gsy ">
            <label class="grid--cell s-label" for="txtUserName">Email address <span class="error-message">*</span></label>
            <div class="grid ps-relative">
              <input class="s-input" id="txtEmail" type="text"  maxlength="150" name="txtEmail">
              <svg aria-hidden="true" class="svg-icon s-input-icon js-alert-icon d-none iconAlertCircle valid-email" width="18" height="18" viewBox="0 0 18 18"><path d="M9 17A8 8 0 1 1 9 1a8 8 0 0 1 0 16zM8 4v6h2V4H8zm0 8v2h2v-2H8z"></path></svg>
            </div>
            <p class="s-input-message mb0 d-none valid-email">
                Email ไม่ถูกต้อง
            </p>
          </div>

          <div class="grid fd-column gs4 gsy ">
            <div class="grid ai-center ps-relative jc-space-between">
              <label class="grid--cell s-label" for="txtPassword">Password<span class="error-message">*</span></label>
            </div>
            <div class="grid ps-relative">
              <input class="grid--cell s-input" type="password" autocomplete="off" name="txtPassword" id="txtPassword">
              <svg aria-hidden="true" class="svg-icon s-input-icon js-alert-icon d-none iconAlertCircle valid-pwd" width="18" height="18" viewBox="0 0 18 18"><path d="M9 17A8 8 0 1 1 9 1a8 8 0 0 1 0 16zM8 4v6h2V4H8zm0 8v2h2v-2H8z"></path></svg>
            </div>
            <p class="s-input-message mb0 d-none valid-pwd">
                รหัสผ่านควรมีอย่างน้อย 6 ตัวอักษร
            </p>
          </div>

          <button class="grid--cell s-btn s-btn__primary btn-success" id="btnSubmit" name="btnSubmit" type="button" onclick="Register();">
              Create an account
          </button>

        </div>
      </div>


      <div class="mx-auto ta-center fs-body1 p16 pb0 mb24 w100 wmx3">
          By clicking “Create an account” below, you agree to our terms of service and privacy statement. We’ll occasionally send you account related emails.


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
                Account recovery email sent to <span id="email"></span>
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
