<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/Main.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Selectcon.Members.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

        $(function () {

            $('#txtPassword').keypress(function (event) {
                if (event.keyCode == 13) {
                    LogOn();
                }
            });
            
        });

        function LogOn() {
            var elemID = $('#txtUserName');
            var elemPwd = $('#txtPassword');
            var ID = elemID.val();
            var Password = elemPwd.val();
            var validID = $('.valid-id');
            var validPwd = $('.valid-pwd');

            var myData = {
                'UserName':  ID,
                'Password': Password
            };
            

            if (!IsEmpty(ID) && !IsEmpty(Password)) {
                $('input[name=action]').val("LOGIN");
                
                 $.ajax({
                    url: '../api/v1/User/Login',
                    dataType: 'json',
                    data: myData,
                     success: function (data) {
                         $('#main').submit();
                    },
                     error: function (data, a ,b) {
                       elemID.addClass("has-error");
                       validID.addClass("error-message");
                       validID.removeClass("d-none");
                       switch (data.status) {
                           case 401:
                               $('p.valid-id').html("username ยังไม่ได้รับยืนยันที่อยู่อีเมลที่คุณป้อนไว้");
                               break;
                           case 403:
                               $('p.valid-id').html("username หรือ password ไม่ถูกต้อง");
                               break;
                           case 404:
                               $('p.valid-id').html("ไม่สามารถเข้าระบบได้! " + data.responseText);
                               break;
                       }
                       
                    }
                  });

                
            } else {
                $('input').removeClass("has-error");
                $('[class*="valid-"]').addClass("d-none");
                
                if (IsEmpty(ID)) {
                    elemID.addClass("has-error");
                    validID.addClass("error-message");

                    validID.removeClass("d-none");
                    $('p.valid-id').html("กรุณากรอก username หรือ email address");
                } else {
                    
                }

                if (IsEmpty(Password)) {
                    elemPwd.addClass("has-error");
                    validPwd.addClass("error-message");

                    validPwd.removeClass("d-none");
                     $('p.valid-pwd').html("กรุณากรอก password");
                }

            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="C_mainbar" runat="server">
    <input type="hidden" name="action" />

    <div class="grid--cell">
      <div class="ta-center fs-title mx-auto mb24">
      </div>
      <div id="formContainer" class="wmx3 mx-auto mb24 p24 bg-white bar-lg auth-shadow">

        <div  class="grid fd-column gs12 gsy">
          <input type="hidden" name="fkey" value="eda908868f0ec06f1712e83466b921e0c38bfe1fa9d2ccaad2244bc86da07d1a">

          <div class="grid fd-column gs4 gsy ">
            <label class="grid--cell s-label" for="txtUserName">Username or Email address</label>
            <div class="grid ps-relative">
              <input class="s-input" id="txtUserName" type="text" size="30" maxlength="100" name="txtUserName" tabindex="1">
              <svg aria-hidden="true" class="svg-icon s-input-icon js-alert-icon d-none iconAlertCircle valid-id" width="18" height="18" viewBox="0 0 18 18"><path d="M9 17A8 8 0 1 1 9 1a8 8 0 0 1 0 16zM8 4v6h2V4H8zm0 8v2h2v-2H8z"></path></svg>
            </div>
            <p class="s-input-message mb0 d-none valid-id">

            </p>
          </div>

          <div class="grid fd-column gs4 gsy ">
            <div class="grid ai-center ps-relative jc-space-between">
              <label class="grid--cell s-label" for="txtPassword">Password</label>

              <a class="grid--cell s-link fs-caption" href="recovery">Forgot password?</a>
            </div>
            <div class="grid ps-relative">
              <input class="grid--cell s-input" type="password" autocomplete="off" name="txtPassword" id="txtPassword" tabindex="2">
              <svg aria-hidden="true" class="svg-icon s-input-icon js-alert-icon d-none iconAlertCircle valid-pwd" width="18" height="18" viewBox="0 0 18 18"><path d="M9 17A8 8 0 1 1 9 1a8 8 0 0 1 0 16zM8 4v6h2V4H8zm0 8v2h2v-2H8z"></path></svg>
            </div>
            <p class="s-input-message mb0 d-none valid-pwd">

            </p>


          </div>
          <button class="grid--cell s-btn s-btn__primary" id="btnSubmit" name="btnSubmit" type="button" onclick="LogOn();" tabindex="3">
              Log in
          </button>

        </div>
      </div>


      <div class="mx-auto ta-center fs-body1 p16 pb0 mb24 w100 wmx3">



        <div class="mt12">

        </div>
      </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="C_sidebar" runat="server">
</asp:Content>
