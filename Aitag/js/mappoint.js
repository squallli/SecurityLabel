window.onload = function (){
	
	document.getElementById("COURSE_1").onmouseover = SHOWSHOW;
	document.getElementById("COURSE_2").onmouseover = SHOWSHOW;
	//document.getElementById("COURSE_3").onmouseover = SHOWSHOW;
	//document.getElementById("COURSE_4").onmouseover = SHOWSHOW;
	
	document.getElementById("COURSE_1").onmouseout = HIDEHIDE;
	document.getElementById("COURSE_2").onmouseout = HIDEHIDE;
	//document.getElementById("COURSE_3").onmouseout = HIDEHIDE;
	//document.getElementById("COURSE_4").onmouseout = HIDEHIDE;
	
	function SHOWSHOW(){
		var N = this.id.substr(7);
		document.getElementById("BOX_"+N).style.display = "block";
	}
	
	function HIDEHIDE(){
		var N = this.id.substr(7);
		document.getElementById("BOX_"+N).style.display = "none";
	}
	
}


//$(function(){
//    $('#COURSE').hover(function(){
//        $(this).find('#BOX_1').style.display = "block";
//    },function(){
//         $(this).find('#BOX_1').style.display = "none";
//    });
//});

//$(document).ready(function(){
//        
//    $("#COURSE").hover(function() {
//        $(this).find("#BOX_1").stop()
//        .css("display","block")
//    }, function() {
//        $(this).find("#BOX_1").stop()
//       .css("display","none")
//    });  
