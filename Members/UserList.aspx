<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/Main.Master" AutoEventWireup="true" CodeBehind="UserList.aspx.cs" Inherits="Selectcon.Members.UserList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style> 
        #mainbar, .mainbar {
            width: 100% !important;
        }

        .user-img {
          float:left !important;
        }

        .user-detail {
            float:left !important;
            margin-left: 9px !important;
            width: calc(100% - 64px) !important;
            line-height: 1.3 !important;
        }

        .user-detail > a {
          display: inline-block !important;
          font-size: 14px !important;
        }

        .user-location {
            font-size: 12px !important;
            margin-bottom: 2px !important;
            display:block !important;
            clear:both !important;
        }

        .user-other {
            font-size: 12px !important;
            margin-left: 57px !important;
            clear:both !important;
        }
        .elip{
           width: calc(100% - 7px) !important;
            white-space:nowrap !important;
            overflow:hidden !important;
            text-overflow:ellipsis !important;
        }

        [type="radio"]:checked,
    [type="radio"]:not(:checked) {
        position: absolute;
        left: -9999px;
    }
    [type="radio"]:checked + label,
    [type="radio"]:not(:checked) + label
    {
        position: relative;
        padding-left: 28px;
        cursor: pointer;
        line-height: 20px;
        display: inline-block;
        color: #666;
    }
    [type="radio"]:checked + label:before,
    [type="radio"]:not(:checked) + label:before {
        content: '';
        position: absolute;
        left: 0;
        top: 0;
        width: 18px;
        height: 18px;
        border: 1px solid #ddd;
        border-radius: 100%;
        background: #fff;
    }
    [type="radio"]:checked + label:after,
    [type="radio"]:not(:checked) + label:after {
        content: '';
        width: 12px;
        height: 12px;
        background: #F87DA9;
        position: absolute;
        top: 3px;
        left: 3.5px;
        border-radius: 100%;
        -webkit-transition: all 0.2s ease;
        transition: all 0.2s ease;
    }
    [type="radio"]:not(:checked) + label:after {
        opacity: 0;
        -webkit-transform: scale(0);
        transform: scale(0);
    }
    [type="radio"]:checked + label:after {
        opacity: 1;
        -webkit-transform: scale(1);
        transform: scale(1);
    }
    </style>

    <script type="text/javascript">
        $(document).ready(function () {

            $("ol.nav-links > li").removeClass("youarehere");
            $('li.userlist').addClass('youarehere');


            $("#userfilter").on("keyup", function () {
                var value = $(this).val().toLowerCase();
                $('.user-hover:not(:contains(' + value + '))').hide(); $('.user-hover:contains("' + value + '")').show();
            });

            $('#btnSave').click(function () {
                SaveData();
            });

            $('[name=radio-group]').click(function () {
                if ($('#Banned').prop("checked")) {
                    $("#banned").removeClass("d-none");
                } else {
                    $("#banned").addClass("d-none");
                    $("#txtBanned").val("");
                }
            })

            LoadUser();
        });

        function LoadUser() {
            var html = "";
            var elem = "";

            $.getJSON("../api/v1/Member", function (data) {
                $('#user-list').empty();
                var obj = $.parseJSON(data);
                $.each(obj, function (idx, val) {
                    html = "";
                    html += "<div class='grid-layout--cell user-info  user-hover' onclick=\"OpenDetail('"+ val.UserID +"');\">";
                    html += "  <div class='user-img'>";
                    html += "    <img src='../Files/Upload/Avatar/" + val.Avatar + "' alt='' onerror='imgError(this);' width='48' height='48'>";
                    html += "  </div>";
                    html += "  <div class='user-detail'>";
                    html += "    <a href='#'>" + val.DisplayName + "</a>";
                    html += "    <span class='user-location'>" + val.RoleName + "</span>";
                    html += "    <span class='user-location elip'>" + val.Title + "</span>";
                    html += "  </div>";
                    html += "  <div class='user-other'>";
                    html += "    <div class='elip'>" + val.Comment + "</div>";
                    html += "  </div>";
                    html += "</div>";
                    $('#user-list').append(html);
                });

            });
        }


        function OpenDetail(uid) {
            $('#uid').val(uid);

            var url = "../api/v1/Member/" + uid;
            $.getJSON(url, function (data) {
                var obj = $.parseJSON(data);

                $("#DisplayName").html(obj.DisplayName);
                $('#Email').html(obj.Email);
                $('#roleId').val(obj.RoleID);
                $('#roleId').val(obj.RoleID);

       
                $("#Active").prop("checked", (obj.IsApproved == 1));

                if (obj.IsBanned == 1) {
                    $("#Banned").prop("checked", true);
                    $("#banned").removeClass("d-none");
                    $("#txtBanned").val(obj.BannedReason);
                } else {
                    $("#Banned").prop("checked", false);
                    $("#banned").addClass("d-none");
                    $("#txtBanned").val("");
                }
                
                
            });
        

            $('#mdlEditUser').modal({ backdrop: 'static', keyboard: false, show: true });
        }

        function SaveData() {
            var uid = $('#uid').val();
            var roleID = $('#roleId').val();
            var active;
            var banned;
            
            if ($("#Banned").prop("checked")) {
                banned = "1";
            } else {
                banned = "0";
            }

            if ($("#Active").prop("checked")) {
                active = "1";
            } else {
                active = "0";
            }

            var myData = {
                'UserID': uid,
                'RoleID': roleID,
                'IsApproved' : active,
                'IsBanned': banned,
                'BannedReason': $("#txtBanned").val(),
            };

            $.ajax({
                method: "PUT",
                contentType: "application/x-www-form-urlencoded",
                dataType: 'json',
                url: '../api/v1/User',
                data: myData,
                success: function (data) {
                    ClearData()
                    $("#main").submit();
                }, error: function (xhr, data) {
                    console.log(xhr);
                    if (xhr.status == 200) {
                        ClearData();
                        $("#main").submit();
                    }
                }
            });

            
        }

        function ClearData() {
            $('#uid').val("");
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="C_mainbar" runat="server">
    <h1 class="fs-headline1 mb24">
        Users
    </h1>

    <div class="grid fw-wrap ai-stretch">
      <div class="grid--cell mb12 ps-relative">
        <input id="userfilter" name="userfilter" class="s-input s-input__search h100" autocomplete="off" size="30" type="text" value="" placeholder="Filter by user">
        <svg aria-hidden="true" class="svg-icon s-input-icon s-input-icon__search iconSearch" width="18" height="18" viewBox="0 0 18 18">
          <path d="M12.86 11.32L18 16.5 16.5 18l-5.18-5.14v-.35a7 7 0 1 1 1.19-1.19h.35zM7 12A5 5 0 1 0 7 2a5 5 0 0 0 0 10z"></path>
        </svg>
      </div>
    </div>

    <div class="page-description mb12">
      <div class="grid jc-space-between">
        <div class="grid--cell ml-auto">
        </div>
      </div>
    </div>
    
    <div id="user-browser">
      <div id="user-list" class="grid-layout">

      </div>
    </div>

    <div class="modal fade" id="mdlEditUser" role="dialog">
        <div class="modal-dialog wmn8">
          <div class="modal-content">
            <div class="modal-header">
              
              <h4 id="DisplayName" class="modal-title"></h4>
                <button type="button" class="close" onclick="ClearData();" data-dismiss="modal">&times;</button>
            </div>
            <div class="modal-body">
                <input id="uid" type="hidden" />
                <div class="grid gs24">
                    <div class="grid--cell fl1 wmn0">
                      <div class="grid gs16">
                        <div class="grid--cell fl1  pr16  about">
                          <div class="grid fd-column gs8 gsy">
                            <div class="grid--cell">
                              <div class="col-9">
                                  <label class="col-4 text-right">Email address</label>
                                  <label id="Email" class="col-form-label-sm"></label>
                              </div> 
                            </div>

                            <div class="grid--cell">
                              <div class="col-9">
                                  <label class="col-4 text-right">สิทธิ์การใช้งาน</label>
                                  <select id="roleId" class="form-control-sm">
                                      <option value="1">Administrator</option>
                                      <option value="2">Super User</option>
                                      <option value="3">User</option>
                                  </select>
                              </div> 
                            </div>

                            <div class="grid--cell">
                                <div class="col-9">
                                    <label class="col-4 text-right">สถานะ</label>

                                    <label class="pr-2">
                                        <input type="radio" id="Active" name="radio-group" value="active">
                                        <label for="Active">เปิดใช้งาน</label>
                                      </label>
                                    <label class="pr-2">
                                        <input type="radio" id="Banned" name="radio-group" value="banned">
                                        <label for="Banned">ระงับการใช้งาน</label>
                                    </label>
                                </div>
                            </div>

                            <div id="banned" class="grid--cell d-none">
                                <div class="col-9">
                                    <label class="col-4 text-right">เหตุผลการระงับ</label>
                                    <textarea id="txtBanned" name="txtBanned" class="form-control-sm" cols="30" rows="5"></textarea>
                                </div>
                            </div>
                              
                            
                          </div>
                        </div>
                      </div>
                    </div>
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
</asp:Content>
