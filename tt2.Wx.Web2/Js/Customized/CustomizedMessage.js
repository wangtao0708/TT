$(function(){
    //var mineuser = window.localStorage.getItem("MemberID");
    //主题
    var themeArr=[];
    $(".tourism li").click(function () {
        //console.log(themeArr.indexOf($(this).text()))
        if(themeArr.indexOf($(this).attr("ThemeID"))<0){
            if(themeArr.length<3 ){
                //themeArr.push($(this).text());
                themeArr.push($(this).attr("ThemeID"));
                $(this).addClass("tourismbg");
            }else {
                alert("最多只能选择三个主题");
            }
        }else {
            //themeArr.remove($(this).text());
            //themeArr.splice($(this).attr("ThemeID"),1);
            for(var i=0;i<=themeArr.length-1;i++){
                if(themeArr[i]==$(this).attr("ThemeID")){
                    themeArr.splice(i,1);
                }
            }
            $(this).removeClass("tourismbg");
        }
        $("#theme").val(themeArr);
    });
    Array.prototype.remove = function(val) {
        var index = this.indexOf(val);
        if (index > -1) {
            this.splice(index, 1);
        };
        return this;
    };
    //星级选项
    function setStar(){
        $("input.starCheck").each(function () {
            if (this.checked) {
                $(this).prev("img").attr("src", "../../Images/Customized/lightStar.jpg");
            }
            else {
                $(this).prev("img").attr("src", "../../Images/Customized/grayStar.jpg");
            }
        });
    }
    
    $("input.starCheck").click(function () {
        var initStar = 1;
        var index = $("input.starCheck").index(this);
        var elems = document.getElementsByName("starCheck");
        for (var i = index+1; i <=elems.length - 1; i++) {
            elems[i].checked = false;
        }
        for (var j = 0; j < index;j++)
        {
            elems[j].checked = true;
            initStar++;
        }
        if (this.checked)
        {
            initStar++;
        }
        setStar();
        $("#star").val(initStar);
    });



   //日期插件
    var currYear = (new Date()).getFullYear();  
    var opt={};
    opt.date = {preset : 'date'};
    opt.datetime = {preset : 'datetime'};
    opt.time = {preset : 'time'};
    opt.default = {
        theme: 'android-ics light', //皮肤样式
        display: 'modal', //显示方式 
        mode: 'scroller', //日期选择模式
        dateFormat: 'yyyy-mm-dd',
        lang: 'zh',
        showNow: true,
        nowText: "今天",
        startYear: currYear - 50, //开始年份
        endYear: currYear + 10 //结束年份
    };

    $("#USER_AGE").mobiscroll($.extend(opt['date'], opt['default']));
    //获取当前日期
    function show(){
       var mydate = new Date();
       var str = "" + mydate.getFullYear() + "-";
       str += (mydate.getMonth()+1) + "-";
       str += mydate.getDate() + "-";
       return str;
    }
    
    $("#USER_AGE").change(function(){
        var usernum=$(this).val();
        if(usernum<=show()){
            alert("出游日期不正确");
            return false;
        }
    })

    $('#adultgrown').on('click',function(){
        $('.adultgrown').toggle();
        $('.children').hide();
    })
    $('.adultgrown').on('click','li',function(){
        var livalue=$(this).val();
        $(".adultgrownnum").html(livalue);
        $('.adultgrown').hide()
    })

    $('#children').on('click',function(){
        $('.children').toggle();
        $('.adultgrown').hide();
    })
    $('.children').on('click','li',function(){
        var livalue=$(this).val();
        $(".childrennum").html(livalue);
        $('.children').hide()
    })

    //点击立即定制
    $(".btn").click(function () {
        if ($("#theme").val()=="")
        {
            alert("请选择旅游主题");
            return false;
        }
        else if ($("#set").val()=="")
        {
            alert("请填写出发城市");
            return false;
        }
        else if($("#destination").val()=="")
        {
            alert("请填写目的地");
            return false;
        }
        else if($("#USER_AGE").val()==""){
            alert("请填写出游日期");
            return false;
        }
        else if($("#USER_AGE").val()<=show()){
            alert("出游日期不正确");
            return false;
        }
        else if($(".adultgrownnum").text()==0 && $(".childrennum").text()==0){
            alert("请选择出行人数");
            return false;
        }
        else if($("#Travel").val()=="")
        {
            alert("请填写出行天数");
            return false;
        }
        else if($("#budget").val()==""){
            alert("请填写人均预算");
            return false;
        }else{

            $(".firstStep").hide();
            $(".secondStep").show();
            //如果未登录就短信验证
            if(jsgMemberID==""){
                $(".Verification").show();
                $(".daojishi").on("click",function(){
                    function public2(strActionName,arrActionParam){
                        var arrActionParam = {
                            "Way":0,
                            "Mobile": $("#mobilenum").val()
                        };
                        var strActionName = strActionName;
                        var strActionParam = JSON.stringify(arrActionParam);
                        var strRequest = GetVisitData(strActionName, strActionParam);
                        var datSubmit = { strRequest: strRequest };
                        return datSubmit;
                    }

                    imgUpload.method(public2('Com_SendSmsCode'),function (data) {
                      alert(data.Message);
                        if(data.Result!=1){
                          alert("请输入正确的手机号");
                          return false;
                        }else if(data.Result==1){
                            $(".daojishi").addClass("on");
                            settime($(".daojishi").get(0));
                            
                        }
                    })

                });
            }
        }
    });
    var countdown=60; 
    function settime(obj) { 
        if (countdown == 0) { 
            obj.removeAttribute("disabled");    
            obj.value="免费获取验证码"; 
            countdown = 60; 
            return;
        } else { 
            obj.setAttribute("disabled", true); 
            $(".daojishi").removeClass("on");
            obj.value="重新发送(" + countdown + ")"; 
            countdown--; 
        } 
      setTimeout(function() { 
        settime(obj) }
        ,1000) 
    }
    //留言字数
    $("#demand").keydown(function(){
        var demandnum=$(this).val().length;
        if(demandnum<=500){
            $(".inputword").text((500-demandnum));
            //b.innerHTML='您还可以输入'+(20-c)+'个字符';
        }else{
            //a.value=a.value.slice(0,20);
            $(".inputword").text(0);
            $(this).val($(this).val().slice(0,500));
        }
    })

    $(".submit").click(function () {
        var mobileReg = /(^1[3578][01379]\d{8}$)|(^1[34578][01256]\d{8}$)|(^(134[012345678]\d{7}|1[34578][012356789]\d{8})$)/g;//验证手机号码的正则表达式
        var phoneReg = /^0\d{2,3}-\d{7,8}$/g;//固定电话正则表达式验证
        var loginReg = /^[a-zA_Z]\w{6,}$/g;//账号验证表达式
        var emailReg = /^(\w-*\.*)+@(\w-?)+(\.\w{2,})+$/g;//邮箱验证表达式
        var chineseReg = /^[\u4E00-\u9FA5]+$/;//中文验证
        var uservalue=$("#user").val();
        var mobilenum=$("#mobilenum").val();
        var callemial=$("#callemial").val();
        var requirement=$(".weui_textarea").val();
        
        if(jsgMemberID==''){
            var codenum=$(".codenum").val();
            if(uservalue==''){
                alert("请输入联系人");
                return false;
            }else if(mobilenum==''){
                alert("请输入手机号码");
            }else if(callemial==''){
                alert("请输入邮箱号码");
                return false;
            }else if(codenum==""){
                alert("请输入验证码");
                return false;
            }else if(!mobileReg.test(mobilenum)){
                alert("请输入正确的手机号码");
                return false;
            }else if(!emailReg.test(callemial)){
                alert("请输入正确的邮箱号码");
            }else{
                function public2(strActionName,arrActionParam){
                    var arrActionParam = {
                        "TravelerID":mineuser,
                        "Theme":$("#theme").val(),
                        "Star":$("#star").val(),
                        "From":$("#set").val(),
                        "Destination":$("#destination").val(),
                        "Start":$("#USER_AGE").val(),
                        "Adult":$(".adultgrownnum").text(),
                        "Children":$(".childrennum").text(),
                        "Demand":requirement,
                        "SmsCode":codenum,
                        "Name":uservalue,
                        "Phone":mobilenum,
                        "Email":callemial,
                        "Budget":$("#budget").val(),
                        "Day":$("#Travel").val(),
                        "UnionID":unid,
                        "OpenID":opid
                    };
                    console.log(arrActionParam)
                    var strActionName = strActionName;
                    var strActionParam = JSON.stringify(arrActionParam);
                    var strRequest = GetVisitData(strActionName, strActionParam);
                    var datSubmit = { strRequest: strRequest };
                    return datSubmit;
                }

                imgUpload.method(public2('Custom_AddCustom'),function (data) {
                    
                    if(data.Result==1){
                        alert("提交成功");
                        location.href="CustomizedList.html";
                        var datam=data.Model;
                        alert(window.localStorage.setItem("jsgMemberID", "datam.TravelerID"));
                        window.localStorage.setItem("jsgMemberID", datam.TravelerID);
                        window.localStorage.setItem("jsgMemberMobile", datam.TravelerMobile);
                        window.localStorage.setItem("jsgMemberI", datam.Identity);
                        window.localStorage.setItem("jsgMemberAdress", datam.Address);
                        window.localStorage.setItem("jsgMemberGuide", datam.Guide);
                    }else{
                        console.log(data.Message)
                    }
                });
            }
        }else{
            if(uservalue==''){
                alert("请输入联系人");
                return false;
            }else if(mobilenum==''){
                alert("请输入手机号码");
            }else if(callemial==''){
                alert("请输入邮箱号码");
                return false;
            }else if(!mobileReg.test(mobilenum)){
                alert("请输入正确的手机号码");
                return false;
            }else if(!emailReg.test(callemial)){
                alert("请输入正确的邮箱号码");
            }else{

                function public2(strActionName,arrActionParam){
                    var arrActionParam = {
                        "TravelerID":mineuser,
                        "Theme":$("#theme").val(),
                        "Star":$("#star").val(),
                        "From":$("#set").val(),
                        "Destination":$("#destination").val(),
                        "Start":$("#USER_AGE").val(),
                        "Adult":$(".adultgrownnum").text(),
                        "Children":$(".childrennum").text(),
                        "Demand":requirement,
                        "SmsCode":"",
                        "Name":uservalue,
                        "Phone":mobilenum,
                        "Email":callemial,
                        "Budget":$("#budget").val(),
                        "Day":$("#Travel").val()
                    };
                    console.log(arrActionParam)
                    var strActionName = strActionName;
                    var strActionParam = JSON.stringify(arrActionParam);
                    var strRequest = GetVisitData(strActionName, strActionParam);
                    var datSubmit = { strRequest: strRequest };
                    return datSubmit;
                }

                imgUpload.method(public2('Custom_AddCustom'),function (data) {
                    
                    if(data.Result==1){
                        alert("提交成功");
                        location.href="CustomizedList.html";
                    }else{
                        alert(data.Message)
                    }
                });
            }

        }
        

        
    });
   
})