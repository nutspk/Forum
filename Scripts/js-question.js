$(function () {
    LoadNewestQuestion()
});

function LoadNewestQuestion() {
    var html = "";
    var elem = "";

    $.getJSON("../api/v1/Question", function (data) {
        $('#question-mini-list').empty();
        var obj = $.parseJSON(data);

        $.each(obj, function (idx, val) {
            console.log(val.AnswerCnt);
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
            html += "           โดย <a href='../Members/User?k=" + val.UserID + "'>" + val.UserUpdated + "</a>";
            html += "       </div>";
            html += "   </div>";
            html += " </div>";
        });

        $('#question-mini-list').append(html);
    });
}



