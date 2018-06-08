 $(function(){
	
	$(window).scroll(function(){
		
		var HEIGHT = $(window).scrollTop() + $(window).innerHeight()-100;
		
		if( $(window).scrollTop() > 200 ){
			
			$("#TOP").stop().animate({top:HEIGHT});
			
		}else{
			
			$("#TOP").stop().animate({top:-100});
			
		}
		
	});
	
	$("#TOP").click(function goTop(){
		
		$("html,body").animate({scrollTop:0},1000);
		
	});
		 
 });