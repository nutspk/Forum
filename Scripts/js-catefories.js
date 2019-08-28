$(function () {
    LoadCategories()
});

function LoadCategories() {
    var html = "";
    var elem = "";

    $.getJSON("api/v1/Categories", function (data) {
        $('#question-mini-list').empty(); 

        $.each(data, function (idx, val) {
            var accepted = (val.Accepted == "Y") ? "answered-accepted" : ""; 
                html += "  <div class='question-summary narrow' id='question-summary" + val.QuestionID +"'>";
                html += "   <div onclick=\"window.location.href='../area/Questions/" + val.QuestionID + "'\" class='cp' >";
                html += "       <div class='votes'>";
                html += "           <div class='mini-counts'><span title='" + val.Likes + " like'>" + val.Likes + "</span></div> <div>ถูกใจ</div>";
                html += "       </div>";
                html += "       <div class='status " + accepted +"' title='answers'>";
                html += "           <div class='mini-counts'><span title='" + val.Answers + " answers'>" + val.Answers + "</span></div> <div>ตอบ</div>";
                html += "       </div>";
                html += "       <div class='views'>";
                html += "           <div class='mini-counts'><span title='" + val.Views + " views'>" + val.Views + "</span></div> <div>อ่าน</div>";
                html += "       </div>";
                html += "   </div >";

                html += "   <div class='summary'>";
                html += "       <h3><a href='../area/Questions/" + val.QuestionID + "' class='question-hyperlink'>" + val.QuestionName + "</a></h3>";
                html += "       <div class='tags'>";

                $.each(val.TagList, function (index, value) {
                    html += "<a href='../Tags/" + value.TagName + "' class='post-tag' title='tagged' rel='tag'>" + value.TagName + "</a>";
                });

                html += "       </div>";
                html += "       <div class='started'>";
                html += "           ตอบเมื่อ <span title='" + val.ActionDated + "' class='relativetime'>" + val.ActionDated + "</span>";
                html += "           โดย <a href='../Users/" + val.ActionBy + "'>" + val.ActionName + "</a>";
                html += "       </div>";
                html += "   </div>";
                html += " </div>";
            });

        $('#question-mini-list').append(html);
        });
}



