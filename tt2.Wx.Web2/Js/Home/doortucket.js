$(function(){

	
	var doortotal;
	var door_count=0;
	var pagesize=3;
	var fal=true;
	function public2(strActionName,arrActionParam){
    	var arrActionParam = {
	       	"VarietyID":"01",
	       	"Skip":door_count,
	       	"Take":pagesize
	    };
    	var strActionName = strActionName;
	    var strActionParam = JSON.stringify(arrActionParam);
	    var strRequest = GetVisitData(strActionName, strActionParam);
	    var datSubmit = { strRequest: strRequest };
	    return datSubmit;
    }

	imgUpload.method(public2('Index_GetProductPageVariety'),function (data) {
		doortotal=data.Total;
		//console.log(doortotal)
	   	var entrance=data.List;
	   	/*if(data.Message=="成功获取！"){
	   		console.log(doortotal)
	   		console.log(door_count*pagesize)
	   	}*/
	   	var str="";
	   	for(var i in entrance){
	   		str +=	'<li door_id="'+entrance[i].TailorID+'">'+
					'<img src="'+entrance[i].PictureUrl+'" >'+
					'<p>'+entrance[i].Inaword+'</p>'+
					'<p><span>'+entrance[i].Name+'</span>/<span>'+entrance[i].CityName+'</span>·<span>'+entrance[i].Age+'</span>·<span>'+entrance[i].Sex+'</span>·<span>'+entrance[i].Occupation+'</span></p>'+
					'<span class="mnp">'+entrance[i].SellPrice+'</span>'+
					'<span class="hdi"><img src="'+entrance[i].PictureUrl+'" ></span>'+
					'<span class="hdn">'+entrance[i].Name+'</span></li>';
		
	   	}	
	   	$('.selling').html(str);
	  
	});
	

	//下滑加载

	var scrollHeight = $(document).height();
	$(window).bind("scroll",function () {
		var scrollTop= $(this).scrollTop();
		var scrollHeight = $(document).height();
		var windowHeight = $(this).height();
		if (Math.floor(scrollTop + windowHeight) >= scrollHeight) {
			if(fal==true){
				pullDownUpData();
			}
			//this.fals=true;

			/*if(door_count*pagesize<=doortotal && flag){
			 door_count++;
			 console.log(door_count)
			 flag=false;

			 }*/

		}
	});








	function pullDownUpData() {
		//fal=false;
		door_count=door_count+3;


		function public2(strActionName,arrActionParam){

			var arrActionParam = {
				"VarietyID":"01",
				"Skip":door_count,
				"Take":pagesize
			};

			var strActionName = strActionName;
			var strActionParam = JSON.stringify(arrActionParam);
			var strRequest = GetVisitData(strActionName, strActionParam);
			var datSubmit = { strRequest: strRequest };
			return datSubmit;
		}


		imgUpload.method(public2('Index_GetProductPageVariety'),function (data) {
			var entrance=data.List;
			if(data.Message=="成功获取！"){
				if(door_count>=doortotal){
					fal=false;
				}else{
					fal=true;
				}

			}
			var str="";
			for(var i in entrance){
				str +=	'<li door_id="'+entrance[i].TailorID+'">'+
						'<img src="'+entrance[i].PictureUrl+'" >'+
						'<p>'+entrance[i].Inaword+'</p>'+
						'<p><span>'+entrance[i].Name+'</span>/<span>'+entrance[i].CityName+'</span>·<span>'+entrance[i].Age+'</span>·<span>'+entrance[i].Sex+'</span>·<span>'+entrance[i].Occupation+'</span></p>'+
						'<span class="mnp">'+entrance[i].SellPrice+'</span>'+
						'<span class="hdi"><img src="'+entrance[i].PictureUrl+'" ></span>'+
						'<span class="hdn">'+entrance[i].Name+'</span></li>';

			}
			$('.selling').append(str);

		});





	}


	    function pullDownUpData() {
	          //fal=false;
	         door_count=door_count+3;


	           function public2(strActionName,arrActionParam){

			    	var arrActionParam = {
				       	"VarietyID":"01",
				       	"Skip":door_count,
				       	"Take":pagesize
				    };

			    	var strActionName = strActionName;
				    var strActionParam = JSON.stringify(arrActionParam);
				    var strRequest = GetVisitData(strActionName, strActionParam);
				    var datSubmit = { strRequest: strRequest };
				    return datSubmit;
			    }


	    	imgUpload.method(public2('Index_GetProductPageVariety'),function (data) {
			   	var entrance=data.List;
			   	if(data.Message=="成功获取！"){
			      if(door_count>=doortotal){
			      	fal=false;
			      }else{
                    fal=true;
			      }

			   	}
			   	var str="";
			   	for(var i in entrance){
			   		str +=	'<li door_id="'+entrance[i].TailorID+'">'+
							'<img src="'+entrance[i].PictureUrl+'" >'+
							'<p>'+entrance[i].Inaword+'</p>'+
							'<p><span>'+entrance[i].Name+'</span>/<span>'+entrance[i].CityName+'</span>·<span>'+entrance[i].Age+'</span>·<span>'+entrance[i].Sex+'</span>·<span>'+entrance[i].Occupation+'</span></p>'+
							'<span class="mnp">'+entrance[i].SellPrice+'</span>'+
							'<span class="hdi"><img src="'+entrance[i].PictureUrl+'" ></span>'+
							'<span class="hdn">'+entrance[i].Name+'</span></li>';

			   	}
			   	$('.selling').append(str);

			});





    }

	
	//跳转到详情页
	$('.selling').on('click','li',function(){
		var door_id = $(this).attr('door_id');
		location.href = 'bulterHome.html?'+'bulter_id='+door_id;
	})

})