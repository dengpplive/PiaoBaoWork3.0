// JScript 文件
var TotalHight=0;
function getHight()
{
    TotalHight=form1.document.body.scrollHeight;
}
function autoResize(v) 
{ 
    var o= window.parent.document.getElementById("mainFrame");
    try 
    { 
        TotalHight=TotalHight+v;
        o.style.height =TotalHight;
        
    }
    catch (e) { }
}

//创建特价信息
//        function $(Id){return document.getElementById(Id);}
        function addTD(star,end,price)
        {
//           autoResize(120);
           var Id = parseInt(document.getElementById("hiID").value);
           var newId=Id+1;
           document.getElementById("hiID").value = newId;
           var tdSpan = "";
           tdSpan = "";
           tdSpan+="<span id='hotel_"+Id+"' ><table width='100%' border='1'>";
           tdSpan+="<tr bgcolor='#F8FDFF' width='100%'><td width='15%' align='right' style='height: 26px'> 开始时间：</td>";
           tdSpan += "<td style='width: 20%; height: 26px;' ><input id='star_" + Id + "' type='text' value='" + star + "' readonly='readonly' runat='server' class='Wdate inputtxtdat' onfocus='WdatePicker({isShowClear:true})' style='width: 149px;' onblur='cli(this)'/><div id='errstar_" + Id + "' style='display:none;color:red;'>请输入正确的日期格式</div></td>";
           tdSpan+="<td style='width: 15%; height: 26px;' align='right' >结束时间：</td>";
           tdSpan += "<td style='width: 20%; height: 26px;' ><input id='end_" + Id + "' type='text' value='" + end + "' readonly='readonly' runat='server' class='Wdate inputtxtdat' onfocus='WdatePicker({isShowClear:true})' style='width: 149px;' onblur='cli(this)'/><div id='errend_" + Id + "' style='display:none;color:red;'>请输入正确的日期格式</div></td>";
           tdSpan+="<td style='width: 10%; height: 26px;'  align='right'>价格：</td>";
           tdSpan+="<td style='width: 20%; height: 26px;' ><input id='price_"+Id+"' value='"+price+"'  type='text' onblur='clip(this)'><div id='errprice_"+Id+"' style='display:none;color:red;'>请输入正常的价格格式</div></td>";
           tdSpan+="<td style='width:10%; height: 26px;' ><input id='dele' type='button' value='删除' onclick='Deltb("+Id+")' class='btn1'></td>";
           tdSpan+="</tr></table></span>";
           document.getElementById("trMore").innerHTML = document.getElementById("trMore").innerHTML + tdSpan;
        }
        //删除
        function Deltb(id)
        {
            if(confirm('是否确认删除？'))
            {
                document.getElementById("hotel_" + id + "").innerHTML = "";
//               autoResize(-120);
            }
            else
            {
                return false;
            }
        }
        //保存页面和获值
        function savecheck(form) {
            document.getElementById("hiTJ").value = "";
            var ts,te,tp;
            var elements = form.elements;
            for(var i=0;i<elements.length;i++)
            {
                if(elements[i].id.indexOf("star_")!=-1)
                {
                    ts=elements[i].value+"*";
                    document.getElementById("hiTJ").value += ts;
                }
                if(elements[i].id.indexOf("end_")!=-1)
                {
                    te=elements[i].value+"/";
                    document.getElementById("hiTJ").value += te;
                }
                if(elements[i].id.indexOf("price_")!=-1)
                {
                   tp=elements[i].value+"^";
                   document.getElementById("hiTJ").value += tp;
                }
            }
           document.getElementById("hiTJ").value = document.getElementById("hiTJ").value;
        }
        //页面验证
        function cli(vi){
        var dt=/^(19|20)\d{2}-(0?\d|1[012])-(0?\d|[12]\d|3[01])$/;
            if(!dt.test(vi.value)){
                document.getElementById("err"+vi.id).style.display="";
                return false;
            }else
            {
                document.getElementById("err"+vi.id).style.display="none"
            }
        }
        function clip(vi){
            var num=/^[0-9]+[.]?[0-9]+$/;
            if(!num.test(vi.value)){
                document.getElementById("err"+vi.id).style.display="";
                return false;
            }else
            {
                document.getElementById("err"+vi.id).style.display="none";
            }
        }
        /*---读取特价信息---*/
        function  readTj(){
            var tdspan = document.getElementById("hiTJ").value; //读取初始化数据值
          var time=new Array();       //定义时间集合
          var starTime=new Array();   //开始时间集合
          var stim=new Array();       //分割数据集合           
          var endTime=new Array();    //结束时间集合
          var price=new Array();      //价格集合
          var info=tdspan.split("^"); //得到数据值
          
          for (var i = 0; i < info.length; i++)
          {
            stim[i]=info[i].toString();
          }
          for (var i = 0; i < stim.length; i++)
          {
            for (var j = 0; j < stim[i].toString().split('/').length; j++)
            {
                if (j % 2 == 0)
                {
                    time[i]=stim[i].toString().split('/')[j].toString();
                }
                else 
                {
                    price[i]=stim[i].toString().split('/')[j].toString();
                }
            }
          }
           for (var i = 0; i < time.length; i++)
           {
               for (var j = 0; j < time[i].toString().split('*').length; j++)
               {
                  if (j % 2 == 0)
                  {
                      starTime[i]=time[i].toString().split('*')[j].toString();
                  }
                  else 
                  {
                      endTime[i]=time[i].toString().split('*')[j].toString();
                  }
               }
           }
           for(var i=0;i<price.length;i++){
                addTD(starTime[i].toString(),endTime[i].toString(),price[i].toString());
           }
        }
//        window.onload=function(){
//            getHight();
//            readTj();
//        }