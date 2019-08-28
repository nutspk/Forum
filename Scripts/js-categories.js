$(function () {
    LoadCategories()
});

function LoadCategories() {
    var html = "";
    var elem = "";

    $.getJSON("../api/v1/Categories", function (data) {
        $('#category-list').empty(); 
        var obj = $.parseJSON(data);
        $.each(obj, function (idx, val) {
            
            var q = val.LatestQuestion;
            html += "<div class='summary'>";
            html += "    <div class='question-summary narrow' id='category-"+ val.CategoryID +"'>";
            html += "        <div class='summary col-sm-12 col-md-8 pl-md-3'>";
            html += "            <h3>";
            html += "                <a href='' class='question-hyperlink'>";
            html += "                    " + val.CategoryName;
            html += "                </a>";
            html += "            </h3>";
            html += "            <div class='tags t-html t-css'>";
            html += "                <span class='small'>"+ val.CategoryDesc +"</span> ";
            html += "            </div>";

            html += "        </div>";
            html += "        <div class='col-sm-12 col-md-4'>";
            html += "            <div class='float-right started-link'>";
            html += "                 <div class='wrap'>";
            html += "                    <a>" + !IsEmpty(q.QuestionDesc) ? q.QuestionDesc : "" + "</a>";
            html += "                </div>";

            if (!IsEmpty(q.UserID)) {
                html += "                โดย <a href='../Members?k=" + q.UserID + "'>" + q.UserUpdated + "</a>";
                html += "                <br/>";
                html += "                "+ q.DateUpdated;
            }
            html += "            </div>";

            html += "        </div>";
            html += "    </div> ";
            html += "</div>";
        });

        $('#category-list').append(html);
    });
}



