<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/Main.Master" AutoEventWireup="true" CodeBehind="User.aspx.cs" Inherits="Selectcon.Members.User" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://www.google.com/recaptcha/api.js?hl=th&render=<%= SiteKey%>"></script>
    <link type="text/css" href="../Content/summernote/summernote-bs4.css" rel="stylesheet">
    <script type="text/javascript" src="../Scripts/summernote/summernote-bs4.js"></script>


    <script type="text/javascript">    
        $(function () {
            $('#summernote').summernote();

            $("ol.nav-links > li").removeClass("youarehere");
            $('li.user').addClass('youarehere');

             grecaptcha.ready(function() {
                  grecaptcha.execute('<%= SiteKey%>', {action: 'posts'}).then(function(token) {
                      $('#reCaptchaToken').val(token);
                  });
            });

            $('#btnUpload').click(function (e) {
                $("[id$=FileUpload]").click();
            });

            $("[id$=FileUpload]").on('change', function (e) {
                UploadImage();
            });

            $('#btnSave').click(function (e) {
                SaveProfile();
            });

            $('#mainbar').css("width", "100%");
            LoadMyProfiles();
        });

        function LoadMyProfiles() {
            var url = "<%:@ResolveClientUrl("~/api/v1/Member/"+ UserID)%>";
            $.getJSON(url, function (data) {
                var obj = $.parseJSON(data);
                console.log(obj);
                $("#DisplayName").html(obj.DisplayName);
                $("[id$=Avatar]").attr('src', "<%:@ResolveClientUrl("~/Files/Upload/Avatar/")%>" + obj.Avatar);
                $("#CreateDate").html(obj);
                $('#LastLogin').html(obj.LastLogin)
                $('#Title').html(obj.Title)
                $('#Comment').html(obj.Comment)
                $('#Homepage').html(obj.Email);
                $('#Location').html(obj.Address);

                $("#txtDisplayName").val(obj.DisplayName);
                $("[id$=Avatar2]").attr('src', "<%:@ResolveClientUrl("~/Files/Upload/Avatar/")%>" + obj.Avatar);
                $("#txtTitle").val(obj.Title);
                $("#txtAddress").val(obj.Address);
                $('#txtEmail').val(obj.Email)
                $('#txtPassword').val('')
                $('#txtConfirmPassword').val('')
                $('.note-editable').html(obj.Comment);

            });
        }
        
        function SaveProfile() {
            var token = $('#reCaptchaToken').val();
            var pwd = $('#txtPassword').val();
            var pwd2 = $('#txtConfirmPassword').val();
            if (pwd != "" && (pwd != pwd2)) {

                var elemPwd = $('#txtConfirmPassword');
                var validPwd = $('.valid-pwd');

                elemPwd.addClass("has-error");
                validPwd.addClass("error-message");
                validPwd.removeClass("d-none");

                return false;
            }

            var myData = {
                'DisplayName': $('#txtDisplayName').val(),
                'Title': $('#txtTitle').val(),
                'Address': $('#txtAddress').val(),
                'Email': $('#txtEmail').val(),
                'Password': $('#txtPassword').val(),
                'Comment': $('.note-editable').html(),
            };

            $.ajax({
                type: "PUT",
                contentType: "application/x-www-form-urlencoded",
                dataType: "text",
                headers: { "Authorization": "Bearer " + token },
                url: '../api/v1/Member/Me',
                data: myData,
                success: function (data) {

                }, error: function (xhr, data) {
                    console.log(xhr);
                }
            }).then(function (data) {
                submit();

            });
        }


        function UploadImage() {
            $('[name=action]').val("UPLOAD");
            $("#main").submit();
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="C_mainbar" runat="server">
     <input type="hidden" name="reCaptchaToken" id="reCaptchaToken" />

        <div id="mainbar-full" class="">
          <ul class="nav nav-tabs ml-0" role="tablist">
            <li class="nav-item mt-0">
              <a class="nav-link active" href=".profile" role="tab" data-toggle="tab">Profile</a>
            </li>
            <li class="nav-item mt-0" id="liEdit" runat="server">
              <a class="nav-link" href=".edit" role="tab" data-toggle="tab">Edit Profile</a>
            </li>
          </ul>
    </div>

    <div class="tab-content  pt-3">
      <div role="tabpanel" class=" tab-pane fade in active show profile" id="profile" runat="server">
          <div id="user-card" class="mt24 user-card">
              <div class="grid gs24">
                <div class="grid--cell fl-shrink0 ws2 overflow-hidden">
                  <div id="avatar-card" class="profile-avatar s-card mb16 p12 bc-black-3 ta-center">
                    <div class="avatar mt16 mx-auto overflow-hidden">
                      <a class="d-block" href="#">
                        <div class="gravatar-wrapper-164">
                          <img id="Avatar" src="" alt="" width="164" height="164" class="avatar-user" onerror="imgError(this);" />
                        </div>
                      </a>
                    </div>
                  </div>
                </div>
    
                <div class="grid--cell fl1 wmn0">
                  <div class="grid gs16">
                    <div class="grid--cell fl1 overflow-x-hidden overflow-y-auto pr16 profile-user--about about">
                      <div class="grid fd-column gs8 gsy">
                        <div class="grid--cell">
                          <h2 class="grid gs12 fw-wrap ai-center fs-headline1 lh-xs fw-bold fc-dark wb-break-all profile-user--name">
                            <span id="DisplayName" class="grid--cell"></span>
                          </h2>
                        </div>
                        <h3 id="Title" class="grid--cell fs-body3 fc-light lh-sm profile-user--role current-position">
                        </h3>
                        <div id="Comment" class="grid--cell mt16 fs-body2 profile-user--bio">
                        </div>
                      </div>
                    </div>
        
                    <div class="grid--cell fl-shrink0 pr24">
                      <div class="ps-relative s-anchors s-anchors__inherit">
                        <ul class="list-reset grid gs8 gsy fd-column fc-medium">
                          <li class="grid--cell ow-break-word">
                            <div class="grid gs8 gsx ai-center">
                              <div class="grid--cell fc-black-350"><svg aria-hidden="true" class="svg-icon iconLocation" width="18" height="18" viewBox="0 0 18 18"><path d="M8.1 17.7S2 9.9 2 6.38A6.44 6.44 0 0 1 8.5 0C12.09 0 15 2.86 15 6.38 15 9.91 8.9 17.7 8.9 17.7c-.22.29-.58.29-.8 0zm.4-8.45a2.75 2.75 0 1 0 0-5.5 2.75 2.75 0 0 0 0 5.5z"></path></svg></div>
                              <div class="grid--cell fl1">
                                   <span id="Location" class="grid--cell"></span>
                              </div>
                            </div>
                          </li>
                          <li class="grid--cell ow-break-word">
                            <div class="grid gs8 gsx ai-center">
                              <div class="grid--cell fc-black-350"><svg aria-hidden="true" class="svg-icon iconLink" width="18" height="18" viewBox="0 0 18 18"><path d="M2.9 9c0-1.16.94-2.1 2.1-2.1h3V5H5a4 4 0 1 0 0 8h3v-1.9H5A2.1 2.1 0 0 1 2.9 9zM13 5h-3v1.9h3a2.1 2.1 0 1 1 0 4.2h-3V13h3a4 4 0 1 0 0-8zm-7 5h6V8H6v2z"></path></svg></div>
                              <div class="grid--cell fl1">
                                  <span id="Homepage" class="grid--cell"></span>
                              </div>
                            </div>
                          </li>
                          <li class="grid--cell ow-break-word">
                            <div class="grid gs8 gsx ai-center">
                              <div class="grid--cell fc-black-350"><svg aria-hidden="true" class="svg-icon iconHistory" width="19" height="18" viewBox="0 0 19 18"><path d="M3 9a8 8 0 1 1 3.73 6.77L8.2 14.3A6 6 0 1 0 5 9l3.01-.01-4 4-4-4h3zm7-4h1.01L11 9.36l3.22 2.1-.6.93L10 10V5z"></path></svg></div>
                              <div class="grid--cell fl1">
                                Member for <span id="CreateDate" title=""></span>
                              </div>
                            </div>
                          </li>
                          <li class="grid--cell ow-break-word">
                            <div class="grid gs8 gsx ai-center">
                              <div class="grid--cell fc-black-350"><svg aria-hidden="true" class="svg-icon iconClock" width="18" height="18" viewBox="0 0 18 18"><path d="M9 17A8 8 0 1 1 9 1a8 8 0 0 1 0 16zm0-2A6 6 0 1 0 9 3a6 6 0 0 0 0 12zM8 5h1.01L9 9.36l3.22 2.1-.6.93L8 10V5z"></path></svg></div>
                              <div class="grid--cell fl1">Last Login <span id="LastLogin" title="" class="relativetime"></span></div>
                            </div>
                          </li>
                        </ul>
                      </div>

                    </div>
                  </div>
                </div>
              </div>
            </div>

      </div>
      <div role="tabpanel" class="tab-pane fade edit" id="edit" runat="server">
          <h1 class="">Edit your profile</h1>
           <div id="user-card2" class="mt24 user-card">
              <div class="grid gs24">
                <div class="grid--cell fl-shrink0 ws2 overflow-hidden">
                  <div id="avatar-card2" class="profile-avatar s-card mb16 p12 bc-black-3 ta-center">
                    <div class="avatar mt16 mx-auto overflow-hidden">
                      <a class="d-block" href="#">
                        <div class="gravatar-wrapper-164">
                          <img id="Avatar2" src="" alt="" width="164" height="164" class="avatar-user" onerror="imgError(this);" />
                        </div>
                      </a>
                    </div>
                  </div>
                    <div>
                        <button id="btnUpload" type="button" class="btn s-btn__google">Upload image</button>
                        <asp:FileUpload ID="FileUpload" runat="server" CssClass="d-none" />
                    </div>
                </div>
    
                <div class="grid--cell fl1 wmn0">
                  <div class="grid gs16">
                    <div class="grid--cell fl1  pr16  about">
                      <div class="grid fd-column gs8 gsy">
                        <div class="grid--cell">
                          <div class="col-9">
                              <label class="col-4 text-right">Display name</label>
                              <input id="txtDisplayName" type="text" value="" maxlength="50" size="30" tabindex="1" />
                          </div>
                        </div>
                        <div class="col-9">
                              <label class="col-4 text-right">Title</label>
                              <input id="txtTitle" type="text" value=""  maxlength="30" size="30" tabindex="2" />
                        </div>

                        <div class="col-9">
                              <label class="col-4 text-right">Address</label>
                              <input id="txtAddress" type="text" value=""  maxlength="500" size="30" tabindex="3" />
                        </div>

                        <div class="col-9">
                              <label class="col-4 text-right">Email</label>
                              <input id="txtEmail" type="text" value=""  maxlength="40" size="30" tabindex="4" >
                        </div>

                        <div class="col-9">
                            <label class="col-4 text-right">Password</label>
                            <input id="txtPassword" type="password" value=""  maxlength="40" size="30" tabindex="5" >
                        </div>

                        <div class="col-9">
                            <label class="col-4 text-right">Confirm Password</label>
                            <input id="txtConfirmPassword" type="password" value=""  maxlength="40" size="30" tabindex="6" >
                            <svg aria-hidden="true" class="svg-icon s-input-icon js-alert-icon d-none iconAlertCircle valid-pwd" width="18" height="18" viewBox="0 0 18 18"><path d="M9 17A8 8 0 1 1 9 1a8 8 0 0 1 0 16zM8 4v6h2V4H8zm0 8v2h2v-2H8z"></path></svg>
                            <p class="s-input-message mb0 d-none valid-pwd">

                            </p>
                        </div>
                          
                          <br/>

                        <label class="col-12 text-left">About me</label>
                        <div id="summernote" class="ps-relative">
                
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>

          <div class="inner-container">
            <div class="row">
              <div class="col-12">
                <div class="form-error" data-title="Oops! There was a problem updating your profile:"></div>
                <div class="form-submit" id="form-submit">
                  <input id="btnSave"  type="button" value="Save Profile" tabindex="31">
                </div>
              </div>
            </div>
          </div>
      </div>
    </div>

    


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="C_sidebar" runat="server">
</asp:Content>
