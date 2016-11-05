<%@ Page Language="C#" AutoEventWireup="true" CodeFile="NoticeEdit.aspx.cs" Inherits="Sys_NoticeEdit"
    EnableEventValidation="false" ValidateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>公告信息</title>
    <script src="../../js/jquery-1.5.1.min.js" type="text/javascript"></script>
    <script src="../../js/jquery-ui-1.8.13.custom.min.js" type="text/javascript"></script>
    <script src="../../js/area.js" type="text/javascript"></script>
    <link href="../../css/smoothness/jquery-ui-1.8.13.custom.css" rel="stylesheet" type="text/css" />
    <script src="../../js/kindeditor-3.5-zh_CN/kindeditor.js" type="text/javascript"></script>
    <script type="text/javascript">
        KE.show({
            id: 'content1',
            imageUploadJson: '~/js/kindeditor-3.5-zh_CN/asp.net/upload_json.ashx',
            allowFileManager: true,
            afterCreate: function (id) {

                KE.event.ctrl(document,13,function () {
                    KE.util.setData(id);
                    document.forms['form1'].submit();
                });
                KE.event.ctrl(KE.g[id].iframeDoc,13,function () {
                    KE.util.setData(id);
                    document.forms['form1'].submit();
                });
            }
        });
    </script>
    <script language="javascript" type="text/javascript">

        function showdialog(t) {
            $("#dd").html(t);
            $("#dd").dialog({
                title: '标题',
                bgiframe: true,
                height: 140,
                modal: true,
                overlay: {
                    backgroundColor: '#000',
                    opacity: 0.5
                },
                buttons: {
                    '确定': function () {
                        $(this).dialog('close');
                    }
                }
            });
        }
        //检查上传文件大小
        function checkFileChange(obj) {
            var val=obj.value;
            if(val.lastIndexOf('.')!= -1) {
                var index=val.lastIndexOf('\\');
                var fileName=val.substring(index+1);
                $("#attachName").val(fileName.substring(0,fileName.lastIndexOf('.')));
            } else {
                alert('不支持该附件文件格式');
            }

            //            if(obj.value!="") {
            //                var Sys={};
            //                var filesize=0;
            //                if(navigator.userAgent.indexOf("MSIE")>0) {
            //                    getFileSize(obj.value);
            //                    Sys.ie=true;
            //                    var fileobject=new ActiveXObject("Scripting.FileSystemObject"); //获取上传文件的对象   
            //                    var file=fileobject.GetFile(obj.value); //获取上传的文件   
            //                    filesize=file.Size; //文件大小   
            //                }
            //                if(isFirefox=navigator.userAgent.indexOf("Firefox")>0) {
            //                    Sys.firefox=true;
            //                    filesize=obj.files[0].fileSize;
            //                }
            //                if(filesize>(4*1024)) {
            //                    alert('文件大小超出范围,最大4M');
            //                    obj.value='';
            //                }
            //            }
        }
        //        function getFileSize(filePath) {
        //            var image=new Image();
        //            image.dynsrc=filePath;
        //            alert(image.fileSize);
        //        }
        function rbtnlSel() {
            var eless=document.getElementsByName('rbtnlFujian');
            var value="";
            for(var i=0;i<eless.length;i++) {
                if(eless[i].checked) {
                    value=eless[i].value;
                    break;
                }
            }
            if(value=="2") {
                document.getElementById('trAdd').style.display="block";
            }
            else {
                document.getElementById('trAdd').style.display="none";
            }
        }
        function ValContent() {
            var IsOk=true;
            var titlevalue=$.trim($("#Title").val());
            var time1=$.trim($("#ExpirationDateSta").val());
            var time2=$.trim($("#ExpirationDateStp").val());
            var content=$.trim($("#content1").val());
            var txtfilename=$.trim($("#attachName").val());
            var txtfilevalue=$.trim($("#fup").val());
            if(titlevalue=="") {
                $("#span_Title").html("标题不能为空！");
                IsOk=false;
            }
            else if(titlevalue.length>50) {
                $("#span_Title").html("标题太长，请缩减到50字符（25个汉字）以内！");
                IsOk=false;
            } else {
                $("#span_Title").html("");
                IsOk=true;
            }

            if(time1==""||time2=="") {
                $("#span_Time").html("有效时间不能为空！");
                IsOk=false;
            }
            else {
                var _temptime1=new Date(($("#ExpirationDateSta").val()).replace(/-/g,"/"));
                var _temptime2=new Date(($("#ExpirationDateStp").val()).replace(/-/g,"/"));
                var dates=new Date();
                if(dates>_temptime1) {
                    $("#span_Time").html("起始时间不能小于当前时间！");
                    IsOk=false;
                }
                if(_temptime1>_temptime2) {
                    $("#span_Time").html("起始时间不能大于结束时间！");
                    IsOk=false;
                } else {
                    $("#span_Time").html("");
                    IsOk=true;
                }
            }

            if(content=="") {
                $("#span_content1").html("公告内容不能为空！");
                IsOk=false;
            } else {
                $("#span_content1").html("");
                IsOk=true;
            }
            return IsOk;

        }
    </script>
    <script src="../../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <link href="../../css/table.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .ke-container tr
        {
            height: auto;
        }
        .table_info1 th
        {
            background-color: #FFFFFF;
            border: medium none;
            color: #424242;
            font-size: 12px;
            line-height: 24px;
            margin: 0;
            text-align: right;
            width: 100px;
            font-weight: normal;
        }
        #table_info td
        {
            text-align: left;
        }
    </style>
</head>
<body>
    <form id="formUpLoad" runat="server" method="post" enctype="multipart/form-data">
    <div id="dd">
    </div>
    <div class="infomain">
        <div class="mainPanel">
            <table width="100%" border="0" cellpadding="0" cellspacing="0" id="table_info1" class="table_info1">
                <tr>
                    <th>
                        公告标题：
                    </th>
                    <td>
                        <asp:TextBox ID="Title" CssClass=" inputtxtdat" runat="server" MaxLength="450" Width="318px"></asp:TextBox>
                        <span id="span_Title" style="color: Red;"></span>
                    </td>
                </tr>
                <tr>
                    <th>
                        有效期：
                    </th>
                    <td>
                        <%-- <asp:TextBox ID="ExpirationDateSta" runat="server" class="Wdate inputtxtdat" EnableViewState="False"
                            onfocus="WdatePicker({isShowWeek:true,minDate:'%y-%M-%d'})" Style="width: 100px;"></asp:TextBox>
                        --%>
                        <input type="text" id="ExpirationDateSta" style="width: 100px;" readonly="true" runat="server"
                            class="inputBorder" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'%y-%M-%d',autoPickDate:true,dateFmt:'yyyy-MM-dd'})" />
                        至<%--<asp:TextBox ID="ExpirationDateStp" runat="server" class="Wdate inputtxtdat" EnableViewState="False"
                            Style="width: 100px;" onfocus="WdatePicker({isShowWeek:true,minDate:'%y-%M-{%d+1}'})"></asp:TextBox>--%>
                        <input type="text" id="ExpirationDateStp" style="width: 100px;" readonly="true" runat="server"
                            class="inputBorder" onfocus="WdatePicker({isShowClear:false,isShowWeek:false,minDate:'%y-%M-{%d+1}',autoPickDate:true,dateFmt:'yyyy-MM-dd'})" />
                        <span id="span_Time" style="color: Red;"></span>
                    </td>
                </tr>
                <tr>
                    <th>
                        公告类型：
                    </th>
                    <td id="Td1">
                        <asp:RadioButtonList ID="IsInternal" runat="server" RepeatDirection="Horizontal">
                            <asp:ListItem Selected="True" Value="1">内部</asp:ListItem>
                            <asp:ListItem Value="2">外部</asp:ListItem>
                            <asp:ListItem Value="3">全部</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <th>
                        紧急标识：
                    </th>
                    <td id="Td3">
                        <asp:RadioButtonList ID="Emergency" runat="server" RepeatDirection="Horizontal">
                            <asp:ListItem Selected="True" Value="1">紧急</asp:ListItem>
                            <asp:ListItem Value="2">不紧急</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <th>
                        是否滚动：
                    </th>
                    <td id="Td2">
                        <asp:RadioButtonList ID="rbisroll" runat="server" RepeatDirection="Horizontal">
                            <asp:ListItem Value="2">是</asp:ListItem>
                            <asp:ListItem Selected="True" Value="1">否</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <th>
                        公告内容：
                    </th>
                    <td>
                        <textarea id="content1" cols="100" rows="8" class="inputtxtdat" style="width: 650px;
                            height: 200px; visibility: hidden;" runat="server"></textarea>
                        <span id="span_content1" style="color: Red;"></span>
                    </td>
                </tr>
                <tr id="trUpdate" runat="server">
                    <th>
                        附件：
                    </th>
                    <td>
                        <div>
                            <asp:RadioButtonList ID="rbtnlFujian" CssClass="inputtxtdat" runat="server" RepeatColumns="5"
                                RepeatDirection="Horizontal" onclick="rbtnlSel()" Height="52px" Width="258px">
                                <asp:ListItem Value="1" Selected="True">保留原有附件</asp:ListItem>
                                <asp:ListItem Value="0">取消附件</asp:ListItem>
                                <asp:ListItem Value="2">重置附件</asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </td>
                </tr>
                <tr id="trAdd">
                    <th>
                        添加附件:
                    </th>
                    <td>
                        <div>
                            附件名：
                            <input type="text" id="attachName" runat="server" />
                            <input type="file" name="fup" id="fup" runat="server" contenteditable="false" onpropertychange="checkFileChange(this)" />
                            <span id="span_file" style="color: Red;"></span>最大4M
                        </div>
                    </td>
                </tr>
                <tr>
                    <th>
                    </th>
                    <td height="35" class="btni" align="center">
                        <span class="btn btn-ok-s">
                            <asp:Button ID="btsave" runat="server" Text="保  存" OnClientClick="return ValContent();"
                                OnClick="btsave_Click" />
                        </span>&nbsp; &nbsp;&nbsp;&nbsp; <span class="btn btn-ok-s">
                            <asp:Button ID="btnBack" runat="server" Text="返  回" />
                        </span>
                        <asp:Label ID="lblid" runat="server" Visible="false" Text=""></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    </form>
</body>
</html>
