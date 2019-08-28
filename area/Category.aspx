<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/Main.Master" AutoEventWireup="true" CodeBehind="Category.aspx.cs" Inherits="Selectcon.Category" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>

    </style>
    <script>
        $(function () {
            $("ol.nav-links > li").removeClass("youarehere");
            $('li.category').addClass('youarehere');

            LoadCategories();
        })

        function CheckData() {
           var title = $('[id$=txtTitle]').val();

           if (title == "") {
               $('[id$=txtTitle]').addClass("has-error");
               $(".valid-title").addClass("error-message");
               $(".valid-title").removeClass("d-none");
               return false;
           } else {
               return true;
           }

        }

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
                    html += "                <a href='../area/QuestionList?k="+ val.CategoryID +"' class='question-hyperlink'>";
                    html += "                    " + val.CategoryName;
                    html += "                </a>";
                    html += "            </h3>";
                    html += "            <div class='tags t-html t-css'>";
                    html += "                <span class='small'>"+ val.CategoryDesc +"</span> ";
                    html += "            </div>";

                    html += "        </div>";
                    html += "        <div class='col-sm-12 col-md-4'>";
                    html += "           <div class='m-0 post-menu small' style='position: absolute;'>";

                    <% if (IsAdmin) { %>
                    html += "           <a href='#' onclick=\"openModal('" + val.CategoryID + "','" + val.CategoryName + "','" + val.CategoryDesc + "')\"> แก้ไข</a>    <br/></div>";
                    <% } %>
                    
                    html += "            <div class='float-right started-link'>";
                    html += "                 <div class='wrap'>";

                    if (!IsEmpty(q.QuestionID)) {
                        html += "                    <a href='../area/Question?k="+ q.QuestionID +"'>" + q.QuestionTitle + "</a>";
                    }
                    
                    html += "                </div>";

                    if (!IsEmpty(q.UserID)) {
                        html += "                โดย <a href='../Members/User?k=" + q.UserID + "'>" + q.UserUpdated + "</a>";
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

        function openModal(id, name, desc) {
            $("#CateId").val(id);
            $("#txtCateTitle").val(name);
            $("#txtCateDesc").val(desc);
            
            $("#btnDel").click(function () {
                editData(true);
            });

            $("#btnSave").click(function () {
                editData(false);
            });

            $('#mdlEditCate').modal("show");
        }

        function editData(del) {
            var myData = {
                'CategoryID': $("#CateId").val(),
                'CategoryName': $("#txtCateTitle").val(),
                'CategoryDesc': $("#txtCateDesc").val(),
                'ActiveFlag': (del) ? "0" : "1"
            };

            $.ajax({
                type: "PUT",
                contentType: "application/x-www-form-urlencoded",
                dataType: "text",
                url: '../api/v1/Category/',
                data: myData,
                success: function (data) {
                    $('[id$=main]').submit();
                }, error: function (xhr, data) {
                    $('.error-msg').removeClass('d-none');
                    $('#error-msg').html('แก้ไขหมวดหมู่ผิดพลาด กรุณาลองใหม่อีกครั้ง');
                }
            });

            $("#txtCateTitle").val("");
            $("#txtCateDesc").val("");
            $("#CateId").val("");
        }

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="C_mainbar" runat="server">
    <div class="grid">
    <h1 class="grid--cell fl1 fs-headline1"> หมวดหมู่ </h1>

    <div class="pl8 aside-cta grid--cell" role="navigation" aria-label="ask new question">
      <button id="NewCate" runat="server" type="button" class="d-inline-flex ai-center ws-nowrap s-btn s-btn__primary"
          data-toggle='modal' data-target='#mdlCate' data-backdrop='static' data-keyboard='false'
          > เพิ่มหมวดหมู่</button>
    </div>

  </div>
  <div class="grid ai-center mb16 ">
    <div class="grid--cell fl1 fs-body3"></div>
    <div class="grid--cell d-none">
      <div class="grid tabs-filter s-btn-group">
        <a class="grid--cell s-btn s-btn__muted s-btn__outlined py8 ws-nowrap" href="#where_Month"  title="กระทู้รายสัปดาห์" > สัปดาห์</a>
      </div>
    </div>
  </div>


  <div class="flush-left">
    <div id="category-list">

    </div>
  </div>


 <div class="modal fade" id="mdlCate" role="dialog">
        <div class="modal-dialog wmn8">
          <div class="modal-content">
            <div class="modal-header">
              <h4 class="modal-title">เพิ่มหมวดหมู่</h4>
              <button type="button" class="close" data-dismiss="modal">&times;</button>
             
            </div>
            <div class="modal-body">
                <div class="grid gs24">
                    <div class="grid--cell fl1 wmn0">
                      <div class="grid gs16">
                        <div class="grid--cell fl1  pr16  about">
                          <div class="grid fd-column gs8 gsy">
                            <div class="grid--cell">
                              <div class="col-11">
                                  <label class="col-3 text-right">หัวเรื่อง</label>
                                  <asp:TextBox ID="txtTitle" CssClass="form-control form-control-sm w-75" MaxLength="250" AutoCompleteType="none" TabIndex="1" runat="server"></asp:TextBox>
                                  <svg aria-hidden="true" class="svg-icon s-input-icon js-alert-icon d-none iconAlertCircle valid-title" width="18" height="18" viewBox="0 0 18 18"><path d="M9 17A8 8 0 1 1 9 1a8 8 0 0 1 0 16zM8 4v6h2V4H8zm0 8v2h2v-2H8z"></path></svg>  
                                  <p class="s-input-message mb0 d-none valid-title">กรุณากรอก หัวเรื่อง*</p>
                              </div> 
                            </div>
                            <div class="col-11">
                                <label class="col-3 text-right">คำอธิบาย</label>
                                <asp:TextBox ID="txtDesc" CssClass="form-control form-control-sm w-75" MaxLength="250" AutoCompleteType="none" TabIndex="2" runat="server"></asp:TextBox>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer bg-white p-2">
               <asp:Button ID="btnAddCate" CssClass="btn btn-default" Text="สร้าง" runat="server" OnClick="btnAddCate_Click" OnClientClick="if (!CheckData()) { return false;};"  />
            </div>
          </div>
        </div>
  </div>


    <div class="modal fade" id="mdlEditCate" role="dialog">
        <div class="modal-dialog wmn8">
          <div class="modal-content">
            <div class="modal-header">
              
              <h4 class="modal-title">แก้ไขหมวดหมู่</h4>
                <button type="button" class="close" data-dismiss="modal">&times;</button>
            </div>
            <div class="modal-body">
                <input id="CateId" type="hidden" />
                <div class="form-row">
                    <div class="col-12 float-left">
                        หัวเรื่อง
                    </div>
                    <div class="col-12 float-left">
                        <input type="text" id="txtCateTitle" name="txtDesc" maxlength="100" class="form-control form-control-sm" />
                    </div>
                </div>

                <div class="form-row">
                    <div class="col-12 float-left">
                        คำอธิบาย
                    </div>
                    <div class="col-12 float-left">
                        <input type="text" id="txtCateDesc" name="txtDesc" maxlength="100" class="form-control form-control-sm" />
                    </div>
                </div>
            </div>
            <div class="modal-footer bg-white p-0">
                <button id="btnDel" data-id="" type="button" class="mr-auto s-btn s-btn__facebook s-btn__icon">Delete</button>

              <button id="btnSave" data-id="" type="button" class="s-btn s-btn__facebook s-btn__icon">Save</button>
            </div>
          </div>
        </div>
  </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="C_sidebar" runat="server">
</asp:Content>
