$(function(){
	function getUrlParam(){
        //?city=%E7%9F%B3%E5%AE%B6%E5%BA%84&indate=2016-8-19&outdate=2016-8-24
        //?city=石家庄&indate=2016-8-19&outdate=2016-8-24
        var url=decodeURI(location.search);
        var str = url.substr(1);
        var arr = str.split('&');
        var obj={};
        arr.forEach(function(v,i){
            var arr2 = v.split('=');
            obj[arr2[0]] = arr2[1];
        })
     
        return obj;
   	}
   	var urlbulhome=getUrlParam();
   	var bulh=urlbulhome.buhome_id;
   
   	function public2(strActionName,arrActionParam){
    	var arrActionParam = {
	       	"strProductID":bulh
	    };
    	var strActionName = strActionName;
	    var strActionParam = JSON.stringify(arrActionParam);
	    var strRequest = GetVisitData(strActionName, strActionParam);
	    var datSubmit = { strRequest: strRequest };
	    return datSubmit;
    }

	imgUpload.method(public2('Index_GetProductDetail'),function (data) {
		console.log(data)
	});


})