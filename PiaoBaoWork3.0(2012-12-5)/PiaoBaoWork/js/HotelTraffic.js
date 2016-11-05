// JScript 文件
/*var TotalHight=0;
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
 function $(Id){return document.getElementById(Id);}
        function addTD(name,range)
        {
           autoResize(120);
           var Id=parseInt($("hiID").value);
           var newId=Id+1;
           $("hiID").value=newId;
           var tdSpan="";
           tdSpan+="<span id='hotel_"+Id+"' ><table width='100%' border='1'>";
           tdSpan+="<tr bgcolor='#F8FDFF' width='100%'><td width='15%' align='right' style='height: 26px'> 地点名称：</td>";
           tdSpan+="<td style='width:30%; height: 26px;' ><input id='name_"+Id+"' value='"+name+"'  type='text'></td>";
           tdSpan+="<td style='width:15%; height: 26px;' align='right' >离酒店距离：</td>";
           tdSpan+="<td style='width:30%; height: 26px;' ><input id='range_"+Id+"' value='"+range+"'  type='text'></td>";
           tdSpan+="<td style='width:10%; height: 26px;' ><input id='dele' type='button' value='删除' onclick='Deltb("+Id+")' class='aniu'></td>";
           tdSpan+="</tr></table></span>";
           $("trJT").innerHTML=$("trJT").innerHTML+tdSpan;
        }
        //删除
        function Deltb(id)
        {
            if(confirm('是否确认删除？'))
            {
               $("hotel_"+id+"").innerHTML="";
               autoResize(-120);
            }
            else
            {
                return false;
            }
        }
        //保存页面和获值
        function savecheck(form)
        {
            $("hiJT").value="";
            var ts,te;
            var elements = form.elements;
            for(var i=0;i<elements.length;i++)
            {
                if(elements[i].id.indexOf("name_")!=-1)
                {
                    ts=elements[i].value+"/";
                    $("hiJT").value+=ts;
                }
                if(elements[i].id.indexOf("range_")!=-1)
                {
                    te=elements[i].value+"^";
                    $("hiJT").value+=te;
                }
            }
                $("hiJT").value=$("hiJT").value;
        }
        //页面验证
        
        function  readTj(){
          var tdspan =$("hiJT").value;//读取初始化数据值
          var name=new Array();       //定义距离集合
          var range=new Array();       //定义距离集合
          var tarm=new Array();
          var info=tdspan.split("^"); //得到数据值
          for(var i=0;i<info.length;i++){
            tarm[i]=info[i].toString();
          }
          for(var i=0;i<tarm.length;i++){
            for(var j=0;j<tarm[i].toString().split('/').length;j++){
                if(j%2==0){
                  name[i]=tarm[i].toString().split('/')[j].toString();
                }else{
                  range[i]= tarm[i].toString().split('/')[j].toString();  
                }
            }
          }
           for(var i=0;i<name.length;i++){
                addTD(name[i].toString(),range[i].toString());
           }
        }
        window.onload=function(){
            getHight();
            readTj();
        }
*/