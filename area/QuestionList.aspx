<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/Main.Master" AutoEventWireup="true" CodeBehind="QuestionList.aspx.cs" Inherits="Selectcon.QuestionList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
    <script type="text/javascript">
        $("ol.nav-links > li").removeClass("youarehere");
        $('li.category').addClass('youarehere');

        $(function () {
            LoadQuestion()
        });

        function LoadQuestion() {
            var html = "";
            var elem = "";

            $('[id$=NewAsk]').attr('href', '../area/ask?k=' + $('input[id$=cID]').val());

            $.getJSON("../api/v1/Categories/" + $('input[id$=cID]').val(), function (data) {
                $('#question-list').empty();
                var obj = $.parseJSON(data);

                $.each(obj, function (idx, val) {

                    if (idx == 0) $('.categoryTitle').html(val.CategoryName);

                    var accepted = (val.AnswerCnt != null && val.AnswerCnt > 0) ? "answered-accepted" : "";
                    html += "  <div class='question-summary narrow' id='question-summary" + val.QuestionID + "'>";
                    html += "   <div onclick=\"window.location.href='../area/Question?k=" + val.QuestionID + "'\" class='cp' >";
                    html += "       <div class='votes'>";
                    html += "           <div class='mini-counts'><span title='" + val.LikeCnt + " like'>" + val.LikeCnt + "</span></div> <div>ถูกใจ</div>";
                    html += "       </div>";
                    html += "       <div class='status " + accepted + "' title='answers'>";
                    html += "           <div class='mini-counts'><span title='" + val.AnswerCnt + " answers'>" + val.AnswerCnt + "</span></div> <div>ตอบ</div>";
                    html += "       </div>";
                    html += "       <div class='views'>";
                    html += "           <div class='mini-counts'><span title='" + val.ViewCnt + " views'>" + val.ViewCnt + "</span></div> <div>อ่าน</div>";
                    html += "       </div>";
                    html += "   </div >";

                    html += "   <div class='summary'>";
                    html += "       <h3><a href='../area/Question?k=" + val.QuestionID + "' class='question-hyperlink'>" + val.QuestionTitle + "</a></h3>";
                    html += "       <div class='tags'>";
            
                    if (val.TagList != null && val.TagList.length > 0) {
               
                        $.each(val.TagList, function (index, value) {
                            html += "<a href='../area/tagged?k=" + value.TagID +"' class='post-tag' title='tagged' rel='tag'>" + value.TagDesc + "</a>";
                        });
                    }
            

                    html += "       </div>";
                    html += "       <div class='started started-link'>";
                    html += "           เมื่อ <span title='" + val.DateUpdated + "' class='relativetime'>" + val.DateUpdated + "</span>";
                    html += "           โดย <a href='../Users/" + val.UserUpdated + "'>" + val.UserUpdated + "</a>";
                    html += "       </div>";
                    html += "   </div>";
                    html += " </div>";
                });

                $('#question-list').append(html);
            });
        }




    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="C_mainbar" runat="server">
    <input id="cID" type="hidden" runat="server">

    <div class="grid mb16">
    <h1 class="grid--cell fl1 fs-headline1 categoryTitle"> </h1>

    <div class="pl8 aside-cta grid--cell" role="navigation" aria-label="ask new question">
      <a id="NewAsk" runat="server" href="#" class="d-inline-flex ai-center ws-nowrap s-btn s-btn__primary"> ตั้งกระทู้ </a>
    </div>

  </div>



  <div id="qlist-wrapper" class="flush-left">
    <div id="question-list">
    </div>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="C_sidebar" runat="server">
</asp:Content>
