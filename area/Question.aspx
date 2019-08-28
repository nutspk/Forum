<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/Main.Master" AutoEventWireup="true" CodeBehind="Question.aspx.cs" Inherits="Selectcon.Question" EnableViewState="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://www.google.com/recaptcha/api.js?hl=th&render=<%= SiteKey%>"></script>
    <link type="text/css" href="../Content/summernote/summernote-bs4.css" rel="stylesheet">
    <script type="text/javascript" src="../Scripts/summernote/summernote-bs4.js"></script>

    <link type="text/css" href="../Content/lightbox/ekko-lightbox.css" rel="stylesheet" />
    <script type="text/javascript" src="../Scripts/lightbox/ekko-lightbox.js"></script>
    
    <script type="text/javascript">
        $(function () {

            grecaptcha.ready(function() {
                  grecaptcha.execute('<%= SiteKey%>', {action: 'posts'}).then(function(token) {
                      $('#reCaptchaToken').val(token);
                  });
            });

            //$(document).on("click", '[data-toggle="lightbox"]', function (event) {
            //    event.preventDefault();
            //    $(this).ekkoLightbox();
            //});

            $(document).on("click", '.suggest-edit-post', function (e) {
                $('.modal-body .note-editable').html($(this).data('desc'));
                $(".modal-footer #btnSave").attr('data-id', $(this).data('id'));
            });

            $('#btnSave').click(function (e) {
                EditAnswer($(this).data('id'));
            });

            $('#btnSaveGallery').click(function (e) {
                CreateGallery();
            });


            $("ol.nav-links > li").removeClass("youarehere");
            $('li.category').addClass('youarehere');

            $('#summernote').summernote();
            $('#txtEdit').summernote();

            $('#question-header').removeClass('d-none');
            if ($('input[id$=qID]').val() == "") {
                window.location.href = "../area/Home";
            }
            LoadQuestion();
            LoadAnswer();
            $('[id$=btnPost]').click(function () {
                Post();
            });

            InitFileUpload();
        });


        function LoadQuestion() {
            var html = "";
            var html2 = "";
            var elem = "";

            $.getJSON("../api/v1/Question/"+ $('input[id$=qID]').val(), function (data) {
                var obj = $.parseJSON(data);
                $.each(obj, function (idx, val) {
                var like = val.LikeCnt;
                $('#question-header').removeClass('d-none');
                $('#question-title').html(val.QuestionTitle);
                $('.js-desc').html(val.QuestionDesc);
                $('.js-dateUpdate').html(val.DateUpdated);
                $('.js-dateUpdate').attr('title',val.DateUpdated);
                $('#btnQLike').attr('data-id', val.QuestionID);

                if (!IsEmpty(like)) {
                    $('.like-q-count').html(like);
                    var star = $('#btnQLike');
                    star.addClass(star.attr("data-selected-classes"));
                }
                

                html2 = "<a href='../Members/User?k=" + val.UserID + "'>";
                html2 += "<div class='gravatar-wrapper-32'><img src='../Files/Upload/Avatar/"+ val.Avatar +"' alt='' width='32' height='32' onerror='imgError(this);'></div>";
                html2 += "</a>";
                $('.js-avatar').html(html2);

                var html3 = "";
                    <% if (Owner) {%>
                    html3 += "<span class='lsep'>|</span> <a href='../area/Posted?k=" + val.QuestionID + "' class='small suggest-edit-post' title='แก้ไข'>แก้ไขโพส</a>";
                    <% } %>

                    <% if (Owner || IsAdmin) {%>
                    html3 += "&nbsp;&nbsp;<span class='lsep'>|</span> <a href='#' onclick='DeleteQuestion(\"" + val.QuestionID + "\")' class='small suggest-edit-post' title='แก้ไข'>ลบโพส</a>";
                    <% } %>

                    $('.post-menu').html(html3);

                <%--if ("<%=UserID%>" == val.UserID && val.QuestionID != null) {
                    html2 = "<span class='lsep'>|</span> <a href='../area/Posted?k="+ val.QuestionID +"' class='suggest-edit-post' title='แก้ไข'>แก้ไขโพส</a>";
                    $('.post-menu').html(html2);
                }--%>
                
                html2 = "<a href='../Members/User?k=" + val.UserID + "'>" + val.UserUpdated + "</a><span class='d-none'>" + val.UserUpdated + "</span>";
                html2 += "<div class='-flair'> <span class='ipAddress' title='ipAddress' dir='ltr'>"+ val.IpAddress +"</span> ";
                html2 += "</div>";
                $('.js-author').html(html2);

                // loop
                if (val.TagList != null && val.TagList.length > 0) {
                    $.each(val.TagList, function (index, value) {
                        var htmlT = "<a href='../area/tagged?k="+ value.TagID +"' class='post-tag' title='show questions tagged '"+ value.TagDesc +"' rel='tag'>"+ value.TagDesc +"</a>";
                        $('.js-tagged').append(htmlT);
                    });
                }

                    // loop
                if (val.GalleyList != null && val.GalleyList.length > 0) {
                    $.each(val.GalleyList, function (index, value) {
                        
                        var img = value.ImgList;
                        var htmlG = "";
                        htmlG += "<div class='col-md-12'>";
                        htmlG += "  <div class='card mb-4 box-shadow rounded-0 m-1'>";
                        htmlG += "    <div class='img-stock'>";
                        
                        $.each(img, function (index2, val2) {
                            var hidden = (val2.ShowFlag == 0) ? "" : "";
                            var IsImg = true ;
                            var url = "../Files/Upload/Images/" + val2.ImageUrl;
                            if (val2.ImageUrl != "") {
                                if (val2.ImageUrl.toLowerCase().indexOf(".mp4") >= 0) {
                                    IsImg=false;
                                }else{
                                    IsImg = true;
                                }
                            }
                            console.log(val2);
                            if (IsImg) {
                                htmlG += "      <a href='#' onclick='openImage(this, 0);' data-id='" + val2.ImageID + "' data-url='" + url + "' data-toggle='lightbox'  class='" + hidden + "' data-title='" + val.DateUpdated + " by " + val.UserUpdated + "'>";
                                htmlG += "        <img src='../Files/Upload/Images/" + val2.ImageUrl + "' class='card-img-top img-fluid rounded-0 " + hidden + " img-thumbnail w-25'>";
                                //htmlG += "      <a href='" + val2.ImageUrl + "' data-toggle='lightbox' data-gallery='gallery' class='" + hidden + "' data-title='" + val2.DateUpdated + " by " + val2.UserUpdated + "'>";
                                //htmlG += "        <img src='" + val2.ImageUrl + "' class='card-img-top img-fluid rounded-0 " + hidden + "'>";
                                htmlG += "      </a>";
                            } else {
                                htmlG += "      <a href='#' onclick='openImage(this, 1);' data-id='" + val2.ImageID + "' data-url='" + url + "' data-toggle='lightbox'  class='" + hidden + "' data-title='" + val.DateUpdated + " by " + val.UserUpdated + "'>";
                                htmlG += "<video preload='auto' poster='../Content/Images/video_poster.png' class='card-img-top img-fluid rounded-0 " + hidden + " img-thumbnail w-50'>";
                                    htmlG += "<source src='" + url + "' type='video/mp4'>";
                                    htmlG += "</video>";
                                htmlG += "      </a>";
                            }
                            
                        });

                        htmlG += "    </div>";
                        htmlG += "  </div>";

                        htmlG += "  <div class='card-body img-desc bottom pt-0 d-none'>";
                        htmlG += "    <div class='img-text wrap-3 w-100 p-3'>";
                        htmlG += "      <span class='card-text'>";
                        htmlG += "        " + value.GalleryDesc;
                        htmlG += "      </span>";
                        htmlG += "      <br/>";
                        htmlG += "      <small class='float-left'>";
                        htmlG += "          " + value.DateUpdated + " by " + value.UserUpdated + " <span class='pl-4 bold'> " + img.length + " ภาพ</span>";
                        htmlG += "      </small>";
                        htmlG += "      <div class='float-right'>";
                        <% if (Owner) { %>
                            htmlG += "        <i  class='fas fa-edit i-btn btnEditGallery' data-toggle='modal' data-target='#mdlAlbum' data-backdrop='static' data-keyboard='false'></i>";
                        <%} %>
                        
                        htmlG += "      </div>";
                        htmlG += "    </div>";
                        htmlG += "  </div>";

                        htmlG += "</div>";

                        $('.pnl-gallery').append(htmlG);
                    });
                }
                
            });
          });
        }

        function LoadAnswer() {
            var html = "";
            var html2 = "";
            var elem = "";
            $('.post-answer').empty();

            $.ajax({
                url: '../api/v1/Answer/'+ $('input[id$=qID]').val(),
                dataType: 'json',
                success: function( data ) {
                    var obj = $.parseJSON(data);
 
                $('.ansCnt').html(obj.length + " Answer");

                $.each(obj, function (idx, val) {
                    if (val.ActiveFlag == 0) return true;
                    html += "<div id='answer-"+ val.AnswerID +"' class='answer' data-answerid='"+val.AnswerID+"'>";
                    html += "  <div class='post-layout'>";
                    html += "    <div class='votecell post-layout--left'>";
                    html += "      <div class='grid fd-column ai-stretch gs4 fc-black-200' data-post-id='55214088'>";
                    if ("<%=UserID %>" == val.UserID <% if (IsAdmin) Response.Write("|| true");%>) {
                        html += "        <button type='button' data-id='" + val.AnswerID + "' onclick='Likes(this);' class='s-btn s-btn__unset c-pointer py8 btn-like d-none' aria-pressed='false' aria-label='like' data-selected-classes='fc-yellow-600'>";
                        html += "                        <svg aria-hidden='true' class='svg-icon iconStar' width='18' height='18' viewBox='0 0 18 18'>";
                        html += "                          <path d='M9 12.65l-5.29 3.63 1.82-6.15L.44 6.22l6.42-.17L9 0l2.14 6.05 6.42.17-5.1 3.9 1.83 6.16z'></path>";
                        html += "                        </svg>";
                        html += "                        <div class='like-count mt8' data-value=''></div>";
                        html += "                    </button>";
                    }
                    
                    html += "      </div>";
                    html += "    </div>";


                    html += "    <div class='answercell post-layout--right'>";
                    html += "      <div class='post-text' itemprop='text'>";
                    html += "      " + val.AnswerDesc;      
                    html += "      </div>";
                    html += "      <div class='grid mb0 fw-wrap ai-start jc-end gs8 gsy'>";
                    html += "        <div class='grid--cell mr16' style='flex: 1 1 100px;'>";

                    html += "      <div class='post-menu'>";
                    html += "          <span class='lsep'>|</span>";
                    if ("<%=UserID%>" == val.UserID <% if (IsAdmin) Response.Write("|| true");%>) {
                        html += "          <a href='#' data-id='" + val.AnswerID + "' data-desc='" + val.AnswerDesc + "' data-toggle='modal' data-target='#mdlEditAns' data-backdrop='static' data-keyboard='false' class='small suggest-edit-post' title='แก้ไข'>แก้ไขโพส</a>";
                    }

                    if ("<%=UserID%>" == val.UserID <% if (IsAdmin) Response.Write("|| true");%>) {
                        html += "&nbsp;&nbsp;<span class='lsep'>|</span> <a href='#' onclick='DeleteAnswer(\"" + val.AnswerID + "\")' class='small suggest-edit-post' title='แก้ไข'>ลบโพส</a>";
                    }



                    html += "      </div>";
                   

                    html += "        </div>"
                    html += "        <div class='post-signature grid--cell fl0'>";
                    html += "          <div class='user-info '>";
                    html += "            <div class='user-action-time'>";
                    //html += "              <a href='/posts/55214117/revisions' title='show all edits to this post'>edited <span title='2019-03-18 02:52:48Z' class='relativetime'>23 hours ago</span></a>";
                    html += "            </div>";
                    html += "            <div class='user-gravatar32'>";
                    html += "            </div>";
                    html += "            <div class='user-details'>";
                    html += "              <div class='-flair'>";
                    html += "              </div>";
                    html += "            </div>";
                    html += "          </div>";
                    html += "        </div>";
                    html += "        <div class='post-signature owner grid--cell fl0'>";
                    html += "          <div class='user-info user-hover'>";
                    html += "            <div class='user-action-time'>";
                    html += "              เมื่อ <span title='"+ val.DateUpdated +"' class='relativetime'>"+ val.DateUpdated +"</span>";
                    html += "            </div>";
                    html += "            <div class='user-gravatar32'>";
                    html += "              <a href='/Members/users?k="+ val.UserID +"'>";
                    html += "                          <div class='gravatar-wrapper-32'><img src='../Files/Upload/Avatar/"+ val.Avatar +"' alt='' width='32' height='32' onerror='imgError(this);'></div>";
                    html += "                        </a>";
                    html += "            </div>";
                    html += "            <div class='user-details'>";
                    html += "              <a href='/Members/users?k=" + val.UserID + "'>" + val.UserUpdated + "</a><span class='d-none' itemprop='name'>" + val.UserUpdated + "</span>";
                    html += "               <div class='-flair'> <span class='ipAddress' title='ipAddress' dir='ltr'>"+ val.IpAddress +"</span> ";
                    html += "            </div>";
                    html += "          </div>";
                    html += "        </div>";
                    html += "      </div>";
                    html += "    </div>";
                    html += "  </div>";
                    html += "</div>";
                    html += "</div>";

                });

                $('.post-answer').html(html);
                    }
            });

            $.getJSON("../api/v1/Answer/" + $('input[id$=qID]').val(), function (data) {
                
            });
        }

        function Post() {
            var questionID = $('input[id$=qID]').val();

            if ($('.note-editable').html() == "") {
                $('[id$=main]').submit();
            }

            var myData = {
                'QuestionID':  $('input[id$=qID]').val(),
                'AnswerDesc': $('.note-editable').html()
            };

           $.ajax({
                type: "POST",
                contentType: "application/x-www-form-urlencoded",
                dataType: "text",
                url: '../api/v1/Answer',
                data: myData,  
                success: function (data) {
                    $('[id$=main]').submit();
               }, error: function (xhr, data) {
                   $('.error-msg').removeClass('d-none');
                   $('#error-msg').html('โพสผิดพลาด! กรุณาลองใหม่อีกครั้ง');
               }
            });
        }

        function EditAnswer(id) {
            var questionID = $('input[id$=qID]').val();
            var token = $('#reCaptchaToken').val();
            var myData = {
                'QuestionID' : questionID,
                'AnswerID': id,
                'OriginalDesc': $('.modal-body .note-editable').html()
            };

           $.ajax({
                type: "POST",
                contentType: "application/x-www-form-urlencoded",
                dataType: "text",
                headers: { "Authorization": "Bearer " + token },
                url: '../api/v1/edit/',
                data: myData,  
                success: function (data) {
                    $('[id$=main]').submit();
                }, error: function (xhr, data) {
                    $('.error-msg').removeClass('d-none');
                    $('#error-msg').html('แก้ไขโพสผิดพลาด กรุณาลองใหม่อีกครั้ง');
               }
            });
        }

        function DeleteAnswer(aid) {
            var questionID = $('input[id$=qID]').val();
            var token = $('#reCaptchaToken').val();
            var myData = {
                'QuestionID' : questionID,
                'AnswerID': aid
            };

           $.ajax({
                type: "POST",
                contentType: "application/x-www-form-urlencoded",
                dataType: "text",
                headers: { "Authorization": "Bearer " + token },
                url: '../api/v1/del/',
                data: myData,  
                success: function (data) {
                    $('[id$=main]').submit();
                }, error: function (xhr, data) {
                    $('.error-msg').removeClass('d-none');
                    $('#error-msg').html('ลบโพสผิดพลาด กรุณาลองใหม่อีกครั้ง');
               }
            });
        }

        function CreateGallery() {
            $('#reCaptchaToken').val();

            $('[name=action]').val("CREATE");
            $('[id$=main]').submit();
        }

        function InitFileUpload() {
            var IsImg = true;
            if (window.File && window.FileList && window.FileReader) {
                var filesInput = $('[id$=files]').get(0);

                filesInput.addEventListener("change", function (event) {
                    var files = event.target.files;
                    var output = document.getElementById("result");
                    output.innerHTML = "";

                    for (var i = 0; i < files.length; i++) {
                        var file = files[i];

                        if (!file.type.match('image'))
                            IsImg = false;

                        var picReader = new FileReader();

                        picReader.addEventListener("load", function (event) {
                            var picFile = event.target;
                            var div = document.createElement("div");
                            if (IsImg) {
                                div.innerHTML = "<img class='thumbnail border border-dark rounded' src='" + picFile.result + "' title='" + picFile.name + "'/>";
                            } else {
                                var Vhtml = "<video preload='metadata' controls='controls' class='thumbnail border border-dark rounded'>";
                                Vhtml += "<source src='" + picFile.result + "' type='video/mp4'>";
                                Vhtml += "</video>";
                                div.innerHTML = Vhtml;
                            }
                            

                            output.insertBefore(div, null);
                        });
                        picReader.readAsDataURL(file);
                    }
                });
            }
            else {
                console.log("Your browser does not support File API");
            }
        }

        function openImage(elem, type) {

            $('#modalIMG').modal("show");

            if (type == 1) {
                $('#showVideo').attr('src', $(elem).data("url"));
                $('#showVideo').removeClass("d-none");
            } else {
                $('#showImage').attr('src', $(elem).data("url"));
                $('#showImage').removeClass("d-none");
            }
            
            

            $('#ImageId').val($(elem).data("id"));
        }

        function DeleteQuestion(qid) {
            <% if (Owner || IsAdmin) { %>
                $.ajax({
                    type: "DELETE",
                    contentType: "application/x-www-form-urlencoded",
                    dataType: "json",
                    url: '../api/v1/Question/' + qid,
                    success: function (data) {
                        window.history.go(-1)
                    }, error: function (xhr, data) {
                        if (xhr.status == 200) {
                            console.log("200");
                            $('[id$=main]').submit();
                        } else {
                            console.log("400");
                        }
                        
                    }
                });
            <%}%>
        }

        function DeleteImage() {
            var imgId = $('#ImageId').val();

            <% if (Owner || IsAdmin) { %>
                $.ajax({
                    type: "POST",
                    contentType: "application/x-www-form-urlencoded",
                    dataType: "json",
                    url: '../api/v1/Gallery/' + imgId,
                    success: function (data) {
                        $('[id$=main]').submit();
                    }, error: function (xhr, data) {
                        if (xhr.status == 200) {
                            console.log("200");
                            $('[id$=main]').submit();
                        } else {
                            $('#modalIMG').modal("hide");
                            console.log("400");
                        }
                        
                    }
                });
            <%}%>
           
        }

    </script>
    <style>
        header h1{
            font-size:12pt;
            color: #fff;
            background-color: #1BA1E2;
            padding: 20px;

        }
        article
        {
            width: 80%;
            margin:auto;
            margin-top:10px;
        }


        .thumbnail{
            width: 150px;
            height: 150px;
            margin: 10px;    
        }

        .modal-dialog {
            width:90% !important;
            margin: 1.75rem auto;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="C_mainbar" runat="server">
 <input id="qID" type="hidden" runat="server">
 <input id="aID" type="hidden">
    <input type="hidden" name="reCaptchaToken" id="reCaptchaToken" />
    <div class="question question-page" data-questionid="" id="question">
      <div class="post-layout">
        <div class="votecell post-layout--left">
          <div class="grid fd-column ai-stretch gs4 fc-black-200" data-post-id="55214088">
            <button id="btnQLike" onclick="Likes(this);" type="button" class="s-btn s-btn__unset c-pointer py8" aria-pressed="false" aria-label="like" data-selected-classes="fc-yellow-600">
              <svg aria-hidden="true" class="svg-icon iconStar" width="18" height="18" viewBox="0 0 18 18">
                <path d="M9 12.65l-5.29 3.63 1.82-6.15L.44 6.22l6.42-.17L9 0l2.14 6.05 6.42.17-5.1 3.9 1.83 6.16z"></path>
              </svg>
              <div class="like-q-count mt8" data-value=""></div>
            </button>
          </div>
        </div>


        <div class="postcell post-layout--right">
          <div class="post-text js-desc" itemprop="text">
              
          </div>
            
          <div class="post-taglist grid gs4 gsy fd-column">
            <div class="grid ps-relative d-block js-tagged">
              <%--<a href="/area/tagged?k=arrays" class="post-tag" title="show questions tagged 'arrays'" rel="tag">arrays</a> --%>
            </div>
          </div>

          <div class="mb0 ">
            <div class="mt16 pt4 grid gs8 gsy fw-wrap jc-end ai-start">
              <div class="grid--cell mr16" style="flex: 1 1 100px;">
                  <div class="post-menu">
                      
                  </div>
              </div>

                <div class="user-info user-hover">
                    <div class="user-action-time d-none">
                        มีการแก้ไขเมื่อ <span title="" class="relativetime"></span>
                    </div>
                </div>

              <div class="post-signature owner grid--cell">
                <div class="user-info ">
                  <div class="user-action-time">
                    ปรับปรุงเมื่อ <span title="" class="relativetime js-dateUpdate"></span>
                  </div> 
                  <div class="user-gravatar32 js-avatar">
                   
                  </div>
                  <div class="user-details js-author">
                  </div>
                </div>
              </div>

            </div>
          </div>
        </div>
      </div>
    </div>

    <div id="answers">
        <a name="tab-top"></a>
        <div id="answers-header">
            <div class="subheader answers-subheader">
                <h2 class="ansCnt"></h2>
                <script>
                    
                </script>
            </div>
        </div>
        <div class="post-answer">

        </div>

        <div id="divpost" class="post-form" runat="server">
            <input type="hidden" id="post-id" value="55214088">
            <input type="hidden" id="qualityBanWarningShown" name="qualityBanWarningShown" value="false">
            <input type="hidden" name="referrer" value="">
            <h2 class="space">Your Answer</h2>
            <div id="post-editor" class="post-editor">
              <div id="summernote" class="ps-relative">
                
              </div>

              <div class="edit-block">
                  
              </div>
            </div>

            <div class="ps-relative">
            </div>

            <div class="form-submit cbt grid gsx gs4">
              <button id="btnPost" class="grid--cell s-btn s-btn__primary s-btn__icon" type="submit" tabindex="120" runat="server">
                Post Your Answer </button>
              <button id="btnDiscard" class="grid--cell s-btn s-btn__danger discard-answer dno" runat="server">
                Discard
              </button>
            </div>
          </div>

    </div>

    <div aria-hidden="true" aria-labelledby="myModalLabel" class="modal fade" id="modalIMG" role="dialog" tabindex="-1">
	<div class="modal-dialog wmn11">
		<div class="modal-content">
            <div class="modal-header">
              <button type="button" class="close" data-dismiss="modal">&times;</button>
            </div>
			<div class="modal-body mb-0 p-0 mx-auto">
                <input type="hidden" id="ImageId" name="ImageId" />
				<img id="showImage" class="d-none" src="" alt="" onerror='imgError(this);' />
                <video id="showVideo" preload='auto' controls="controls" class='card-img-top img-fluid rounded-0 img-thumbnail w-100 d-none'>
                    <source src='' type='video/mp4'>
                 </video>
			</div>
			<div class="modal-footer bg-white p-2">
                <% if (Owner || IsAdmin) { %>
                <button class="btn btn-rounded btn-warning" type="button" onclick="DeleteImage();">ลบ</button>
                <% } %>
				
			</div>
		</div>
	</div>
</div>

    <%--<div class="modal fade" id="mdlAlbum" role="dialog">
        <div class="modal-dialog wmn11">
          <div class="modal-content">
            <div class="modal-header">
              <button type="button" class="close" data-dismiss="modal">&times;</button>
            </div>
            <div class="modal-body">
                
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
            </div>
          </div>
        </div>
  </div>--%>

    <div class="modal fade" id="mdlAdd" role="dialog">
        <div class="modal-dialog wmn11">
          <div class="modal-content">
            <div class="modal-header">
                <h4 class="modal-title">สร้าง Gallery</h4>
                <button type="button" class="close" data-dismiss="modal">&times;</button>
            </div>
            <div class="modal-body">
                <div class="form-row d-none">
                    <div class="col-12 float-left">
                        คำอธิบาย
                    </div>
                    <div class="col-12 float-left">
                        <input type="text" id="txtDesc" name="txtDesc" maxlength="100" class="form-control form-control-sm" />
                    </div>
                </div>

                <div class="form-row">
                    <div class="col-12 float-left">
                        <input id="files" type="file" name="fileUpload" multiple="multiple" runat="server"/>
                    </div>
                    <div class="col-12 float-left">
                        <p class="small text-danger">สามารถอัพโหลดไฟล์นามสกุล .jpg .jpeg .png .gif .mp4 ขนาดไม่เกิน 30mb เท่านั้น</p>
                    </div>
                </div>

                <div class="form-row">
                    <article>
                        <output id="result" class="form-row" >

                        </output>
                    </article>
                </div>
            </div>
            <div class="modal-footer bg-white p-0">
              <button id="btnSaveGallery" type="button" class="s-btn s-btn__facebook s-btn__icon">สร้าง</button>
            </div>
          </div>
        </div>
  </div>



    <div class="modal fade" id="mdlEditAns" role="dialog">
        <div class="modal-dialog wmn8">
          <div class="modal-content">
            <div class="modal-header">
              
              <h4 class="modal-title">แก้ไขโพส</h4>
                <button type="button" class="close" data-dismiss="modal">&times;</button>
            </div>
            <div class="modal-body">
              <div id="txtEdit" class="ps-relative">
                
              </div>
            </div>
            <div class="modal-footer bg-white p-0">
              <button id="btnSave" data-id="" type="button" class="s-btn s-btn__facebook s-btn__icon">Save</button>
            </div>
          </div>
        </div>
  </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="C_sidebar" runat="server">
    <% if (Owner && IsPrivate) { %>
    <div class="card-gallery pb-2">
        <button id="btnAddGallery" type="button" class="btn btn-confirm btn-block" data-toggle='modal' data-target='#mdlAdd' data-backdrop='static' data-keyboard='false'> New Gallery</button>
    </div>
        <i id="btnAddGallery2" class="fas fa-plus-circle i-btn gallery text-success float-right d-none" data-toggle='modal' data-target='#mdlAdd' data-backdrop='static' data-keyboard='false'></i>
    <% } %>

    <div class="card card-gallery" style="border-top: 3px solid #ef4e4e;">
        <div class="card-header d-none" style="border-bottom: 1px solid #d3d3d3;background-color: #fafafa;"> 
            <h4 class="m-0">Image Gallery
                
            </h4>
            
        </div>
        <div class="card-box">
            <div class="form-row pnl-gallery">
              
            </div>
        </div>
    </div>


    <script type="text/javascript">

        function LoadLike() {

        }

        function Likes(elem) {
            let id = $(elem).data('id');
            let LikeCnt = +$(".like-q-count").html();
            let star = $('#btnQLike');
            let className = star.attr("data-selected-classes");
            let state = (star.hasClass(className)) ? "0": "1";
            

            let myData = {
                'QuestionID': id,
                'Likes': state
            };

            if (!!id) {
                $.ajax({
                    type: "POST",
                    contentType: "application/x-www-form-urlencoded",
                    dataType: "text",
                    url: '../api/v1/Like/',
                    data: myData,
                    success: function (data) {
                        if (!star.hasClass(className)) {
                            star.addClass(className);
                            LikeCnt += 1;
                        } else {
                            LikeCnt -= 1;
                            star.removeClass(className);
                        }
                        $(".like-q-count").html(LikeCnt);
                    }, error: function (xhr, data) {
                    }
                });
            }
        }
    </script>

</asp:Content>


