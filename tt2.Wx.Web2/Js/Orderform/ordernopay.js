$(function(){
	function getUrlParam(){
        var url=decodeURI(location.search);
        var str = url.substr(1);
        var arr = str.split('&');
        var obj={};
        arr.forEach(function(v,i){
            var arr2 = v.split('=');
            obj[arr2[0]] = arr2[1];
        });
     
        return obj;
   	}
   	var urlbulhome=getUrlParam();
   	var doodt=urlbulhome.goodt_id;
    //定义仿消费码的数组
    var arr1=[];

   	function public2(strActionName,arrActionParam){
        var arrActionParam = {
            "OrderID":doodt
        };
        var strActionName = strActionName;
        var strActionParam = JSON.stringify(arrActionParam);
        var strRequest = GetVisitData(strActionName, strActionParam);
        var datSubmit = { strRequest: strRequest };
        return datSubmit;
    }

    imgUpload.method(public2('Order_GetOrderGoods'),function (data) {
    	var datam=data.Model;
        var datagoodlist=datam.GoodsList;
        
 		var str='',stratus='';
        //判断状态
 		if(datam.Status==0){
            status='待支付';
        }else if(datam.Status==2){
            status='部分消费';
        }else if(datam.Status==3){
            status='已消费';
        }else if(datam.Status==8){
            status='部分完成';
        }else if(datam.Status==9){
            status='已完成';
        }else if(datam.Status==-1){
            status='已取消';
        }else if(datam.Status==-2){
            status='部分退订';
        }else if(datam.Status==-3){
            status='已退订';
        }else if(datam.Status==-10){
            status='已关闭';
        }else if(datam.Status==1){
            status='已付款';
        }


        for(var i in datagoodlist){
            str+='<div class="specialty_list"><div><dl>'+
                    '<dt><img src="'+datagoodlist[i].PictureUrl+'" ></dt>'+
                    '<dd>'+
                        '<h4>'+datagoodlist[i].Name+'</h4>'+
                        '<p>'+datagoodlist[i].VarietyName+'</p>'+
                        '<p>￥<span>'+datagoodlist[i].SellPrice+'</span></p>'+
                        '<p class="speprice">×<span>'+datagoodlist[i].Quantity+'</span></p>'+
                    '</dd></dl></div>'+
                '</div>';

            if(status=="已付款"){
                str+='<div class="dimensional_code">'+
                    '<p>消费码：<span class="cus">'+datagoodlist[i].Consume+'</span></p>'+
                    '<div id="code'+i+'" class="code"></div>'+
                '</div>';
            }
            arr1.push(datagoodlist[i].Consume);

        }

        str+='<ul>'+
            '<li>订单信息</li>'+
            '<li><span>订&nbsp;&nbsp;单&nbsp;&nbsp;号：</span><span>'+datam.OrderID+'</span></li>'+
            '<li><span>下单时间：</span><span>'+datam.CreateTime+'</span></li>'+
            '<li><span>联&nbsp;&nbsp;系&nbsp;&nbsp;人：</span><span>'+datam.Name+'</span></li>'+
            '<li><span>手机号码：</span><span>'+datam.Mobile+'</span></li>'+
            '<li><span>应付金额：</span><span>'+datam.Payable+'</span></li>'+
        '</ul>'+
        '<ul class="manager">'+
            '<li>管家信息</li><dl>'+
            '<dt><img src="'+datam.TailorHead+'" ></dt><dd>'+
            '<p>'+datam.TailorName+'</p>'+
            '<p>'+datam.TailorOccupation+'</p></dd></dl>'+
            '<li><i></i><span>'+datam.TailorMobile+'</span></li>'+
        '</ul>';
        $('.C_content').html(str);


        $.each(arr1,function(i,v){

            $("#code"+i).qrcode({
                render: "table", //table方式
                width: 120, //宽度
                height:120, //高度
                text:v  //任意内容
            });
        })



    });






});