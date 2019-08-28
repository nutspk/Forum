<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/Main.Master" AutoEventWireup="true" CodeBehind="Ask.aspx.cs" Inherits="Selectcon.Ask" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://www.google.com/recaptcha/api.js?hl=th&render=<%= SiteKey%>"></script>
    <link type="text/css" href="../Content/summernote/summernote-bs4.css" rel="stylesheet">
    <script type="text/javascript" src="../Scripts/summernote/summernote-bs4.js"></script>

    <link type="text/css" href="../Content/jquery.tag-editor.css" rel="stylesheet">
    <script type="text/javascript" src="../Scripts/jquery.caret.min.js"></script>
    <script type="text/javascript" src="../Scripts/jquery.tag-editor.js"></script>

    <link href="../Content/jquery.mentionsInput.css" rel="stylesheet" />
    <script src="../Scripts/underscore-min.js"></script>
    <script src="../Scripts/jquery.events.input.js"></script>
    <script src="../Scripts/jquery.elastic.js"></script>
    <script src="../Scripts/jquery.mentionsInput.js"></script>

    <style type="text/css">
    fieldset.group  { 
        margin: 0; 
        padding: 0; 
        margin-bottom: 1.25em; 
        padding: .125em; 
    } 

    ul.checkbox  { 
        margin: 0; 
        padding: 0; 
        margin-left: 20px; 
        list-style: none; 
    } 

    ul.checkbox li input { 
        margin-right: .25em; 
    } 

    ul.checkbox li {
    border: 1px transparent solid;
    float: left;
    min-width: 200px;
    }

    ul.checkbox li label { 
        margin-left: 0; 
    }

    ul.draggable{
	    list-style:none;
    }

    .post-tag{
         font-size:14px;
        color: #39739d;
        background-color: #E1ECF4;
        border-color: #E1ECF4;
        position: relative;
        display: inline-block;
        padding: .4em .5em;
        margin: 2px 2px 2px 0;
        font-size: 11px;
        line-height: 1;
        white-space: nowrap;
        text-decoration: none;
        text-align: center;
        border-width: 1px;
        border-style: solid;
        border-radius: 3px;
        transition: all .15s ease-in-out;
    }


    </style>

    <script>    
        $(function () {

           

            $("ol.nav-links > li").removeClass("youarehere");
            $('li.category').addClass('youarehere');

            $('#summernote').summernote();

            $('#submit-button').click(function () {
                if (checkData()) {
                    Post();
                }
            });

            InitCtrl();

             grecaptcha.ready(function() {
                  grecaptcha.execute('<%= SiteKey%>', {action: 'posts'}).then(function(token) {
                      $('#reCaptchaToken').val(token);
                  });
            });

             
        });

        function InitCtrl() {
            var html = "";

            $.getJSON("../api/v1/Member/", function (result) {
                var obj = $.parseJSON(result);

                $.each(obj, function(index, item) {
                    item.id = item.UserID;
                    delete item.UserID;

                    item.name = item.DisplayName;
                    delete item.DisplayName;

                    item.type = 'contact';
                 });

                var data;
                console.log(obj);
                $('textarea.mention').mentionsInput({
                    
                    onDataRequest: function (mode, query, callback) {
                        if (query == "") {
                            data = obj;
                        } else {
                            data = _.filter(obj, function (item) {
                                var name = item.name.toLowerCase();
                                return name.indexOf(query.toLowerCase()) > -1;
                            });
                        }
                        callback.call(this, data);
                    },
                    minChars: 0
                });
            });
           

            //$('#txtTags').tagEditor({ 
            //    initialTags: [] ,
            //      beforeTagDelete: function(field, editor, tags, val) {
            //          $("li[data-tag='" + val + "' i]").removeClass('d-none');
            //          console.log(val)
            //          return true;
            //      }
            //});
  
            //  $("#pnlTag").droppable({
            //      drop: function (event, ui) {
            //          var val = $(ui.draggable).attr('data-tag');
            //        $('#txtTags').tagEditor('addTag', val);
            //        $(ui.draggable).addClass('d-none');
            //      }
            //  });

            //$.getJSON("../api/v1/Tags", function (data) {
            //    var elem2 = $('#tagList');
            //    var obj = $.parseJSON(data);
            //    var html2 = "";
            //    $.each(obj, function (idx, val) {
            //        html2 += "<li id='" + val.TagID + "' class='post-tag' data-tag='" + val.TagID + ":" + val.TagDesc + "'>" + val.TagDesc + "</li>";
            //    });
            //    elem2.append(html2);
            //}).then(function() {
            //  $('ul.draggable > li').draggable({
            //      revert: false,
            //      helper: 'clone'
            //    });
            //});

            $.getJSON("../api/v1/Categories", function (data) {
                var elem = $('#ddlCategory');
                var obj = $.parseJSON(data);
                var html = "";

                $.each(obj, function (idx, val) {
                    if (val.CategoryID == "<%=CategoryID%>") {
                        html += " <option value='" + val.CategoryID + "' selected>" + val.CategoryName + "</option> ";
                        elem.prop('disabled', 'disabled');
                    } else {
                        html += " <option value='"+ val.CategoryID +"' >"+ val.CategoryName +"</option> ";
                    }
                });
                elem.append(html);
            });
        }

        function checkData() {
            var err = "";
            var elemTitle = $('#title');
            var elemCate = $('#ddlCategory');
            var elemUser = $('#txtUser');
            var elemBody = $('.note-editor');
            var Title = elemTitle.val();
            var Cate = elemCate.val();
            var User = elemUser.val();
            var Body = $('.note-editable').html();
            var validTitle = $('.valid-title');
            var validCate = $('.valid-cate');
            var validUser = $('.valid-user');
            var validBody = $('.valid-body');

            $('.has-error').removeClass("has-error");
            $('[class*="valid-"]').addClass("d-none");

            if (Title == "") {
                elemTitle.addClass("has-error");
                validTitle.addClass("error-message");
                validTitle.removeClass("d-none");
                err = "a";
            }

            if (Body == "") {
                elemBody.addClass("has-error");
                validBody.addClass("error-message");
                validBody.removeClass("d-none");
                err = "a";
            }

            if (Cate == "") {
                elemCate.addClass("has-error");
                validCate.addClass("error-message");
                validCate.removeClass("d-none");
                err = "a";
            }

             if (!$('#chkPublic').prop('checked')) {
                 if (User == "") {
                    elemUser.addClass("has-error");
                    validUser.addClass("error-message");
                    validUser.removeClass("d-none");
                    err = "a";
                }
            }

            return (err == "");
        }

        function Post() {
            var token = $('#reCaptchaToken').val();
            var isPublic = $('#chkPublic').prop('checked');
            //var tagList = $('#txtTags').tagEditor('getTags')[0].tags;
            var UserList;

            //var result = tagList.map((val) => {
            //    return val.split(':')[0];
            //})

            //var myTag = result.map(tag => ({ TagID: tag }));

            if (!isPublic) {
                $('textarea.mention').mentionsInput('getMentions', function(data) {
                    UserList = $.map(data, function(v){
                        return {UserID: v.id};
                    });
                });
            }

            var myData = {
                'QuestionTitle': $('#title').val(),
                'CategoryID': $('#ddlCategory').val(),
                'QuestionDesc': $('.note-editable').html(),
                'IsPublic':  (isPublic) ? 1 : 0 ,
                'TagList': null,
                'ActiveFlag': "1",
                'UserList': UserList
            };

            $.ajax({
                type: "POST",
                contentType: "application/x-www-form-urlencoded",
                dataType: "text",
                headers: {"Authorization": "Bearer "+ token},
                url: '../api/v1/Question/',
                data: myData,  
                success: function (data) {
                    
                }, error: function (xhr, data) {
                    console.log(xhr);
                }
            }).then(function (data) {
                var obj = $.parseJSON(data);
                if (obj.QuestionID != "") {
                    window.location.href = "../area/Question?k="+ obj.QuestionID;
                }
                
            });

        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="C_mainbar" runat="server">
    <input type="hidden" name="reCaptchaToken" id="reCaptchaToken" />
    <div id="ask-form">
        <div class="question-context-title"> สร้างกระทู้ </div>
        
        <div class="" id="post-Cate" style="position: relative;">
            <div class="form-item ask-title">
                <div class="js-wz-element" data-wz-state="2,256">
                    <div class="grid gs8 gsx">
                        <div class="grid--cell fl1 grid fd-column js-stacks-validation">
                            <label class="s-label mb4" for="title">หมวดหมู่</label>
                            <div class="fl1 ps-relative">
                                <div class="select">
                                    <select name="sort" id="ddlCategory" class="form-control">
                                        <option value='' >เลือกหมวดหมู่</option> 
                                    </select>
                                    <svg aria-hidden="true" class="svg-icon s-input-icon d-none iconAlertCircle valid-cate" width="18" height="18" viewBox="0 0 18 18"><path d="M9 17A8 8 0 1 1 9 1a8 8 0 0 1 0 16zM8 4v6h2V4H8zm0 8v2h2v-2H8z"></path></svg>
                                </div>
                            </div>
                            <div class="s-input-message mt4 d-none valid-cate">
                                กรุณาเลือกหมวดหมู่
                            </div>
                        </div>
                    </div>
                </div>
                <div class="" data-wz-state="4">

                </div>
            </div>
        </div>

        <div class="" id="post-title" style="position: relative;">
            <div class="form-item ask-title">
                <div class="js-wz-element" data-wz-state="2,256">
                    <div class="grid gs8 gsx">
                        <div class="grid--cell fl1 grid fd-column js-stacks-validation">
                            <label class="s-label mb4" for="title">หัวข้อ</label>
                            <div class="fl1 ps-relative">
                                <input id="title" name="title" type="text" maxlength="300" tabindex="100" placeholder="" class="s-input h100 mt0 ask-title-field" data-min-length="15" data-max-length="150" autocomplete="off">
                                 <svg aria-hidden="true" class="svg-icon s-input-icon d-none iconAlertCircle valid-title" width="18" height="18" viewBox="0 0 18 18"><path d="M9 17A8 8 0 1 1 9 1a8 8 0 0 1 0 16zM8 4v6h2V4H8zm0 8v2h2v-2H8z"></path></svg>
                            </div>
                            <div class="s-input-message mt4 d-none valid-title">
                                กรุณาเลือกกรอกหัวข้อ
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="post-editor" class="post-editor js-post-editor js-wz-element" data-wz-state="8,16,32,64,128,256">
            <div class="ps-relative">
                <label class="s-label mb4 d-block" for="wmd-input">Body</label>
                <div id="summernote" class="ps-relative">
                
                </div>
                 <div class="s-input-message mt4 d-none valid-body">
                                กรุณาเลือกเนื้อหา
                 </div>
            </div>

            <div id="draft-saved" class="draft-saved community-option fl" style="height:24px; display:none;">draft saved</div>
            <div id="draft-discarded" class="draft-discarded community-option fl" style="height:24px; display:none;">draft discarded</div>

            <div class="edit-block">
                <input id="fkey" name="fkey" type="hidden" value="cba927a71a90d6ce5b2c68b81378ef3d62e1d3347e02d7d28b51aeff9d35d2a3">
                <input id="author" name="author" type="text">
            </div>
        </div>

        <div class="js-wz-element ps-relative" id="tag-editor" data-wz-state="1,256">
            <div class="ps-relative">
                <div class="form-item p1 js-stacks-validation d-none">
                    <label for="tageditor-replacing-tagnames--input" class="s-label mb4 d-block">แท็ก </label>
                    
                    <div class="ps-relative">
                        <div id="pnlTag">
                            
                          <input id="txtTags" type="text" class="drag-share" />
                            <span class="small">ดึง tag จากด้านล่างมาใส่ในช่อง</span>
                            <ul id="tagList" class="draggable">
                            </ul>
                        </div>
                    </div>

                    <div class="s-input-message mt4 d-none js-stacks-validation-message"></div>
                </div>
            </div>

            <div class="grid gs8 gsx">
                <div class="grid--cell fl1 grid fd-column js-stacks-validation">
                    <div class="mb8"></div>
                    <div class="fl1 ps-relative">
                        <label for="chkPublic" class="s-label mb4">สาธารณะ</label>
                        <input id="chkPublic" name="chkDisplay" type="radio" checked="checked"/>

                        <label for="chkPrivate" class="s-label mb4 ">ส่วนตัว</label>
                        <input id="chkPrivate" name="chkDisplay" type="radio"/>
                        <script type="text/javascript">
                            $('#chkPublic').change(function () {
                                $('#pnlPrivate').addClass("d-none");
                            });

                             $('#chkPrivate').change(function () {
                                $('#pnlPrivate').removeClass("d-none");
                            });
                        </script>
                    </div>
                </div>
            </div>

            <div class="d-none" id="pnlPrivate">
                <div class="form-submit cbt grid gsx gs4 p0 mt32">
                    <label class="s-label mb4" for="title">กำหนดผู้ใช้</label>
                </div>

                <div class="ps-relative js-tags">
                    <div id="pnlUser">
                        <textarea id="txtUser" class="form-control mention" style="height: 42px;"> </textarea>
                    </div>
                </div>

            </div>

            <div class="js-wz-element" id="question-only-section" data-wz-state="256">
                <div class="form-submit cbt grid gsx gs4 p0 mt32">
                    <button id="submit-button" class="grid--cell s-btn s-btn__primary s-btn__icon js-submit-button" type="button" tabindex="120" >
                    สร้างกระทู้ </button>
                </div>
            </div>
        </div>   
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="C_sidebar" runat="server">
</asp:Content>
