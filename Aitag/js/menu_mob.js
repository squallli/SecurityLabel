$(function(){
	
	$("#MENU2").on("click", SHOWSHOW );
	
	function SHOWSHOW(){
		
		$("#SUBMENU").removeClass("CLOSE").addClass("OPEN");
			
		$("#XX").on("click", HIDEHIDE );
		
	}
	
	function HIDEHIDE(){
		
		$("#SUBMENU").removeClass("OPEN").addClass("CLOSE");
				
		$("#XX").off("click", HIDEHIDE );
		
	}
	$(window).on("resize",CLEARSTYLE);
	
	function CLEARSTYLE(){
		if($(window).innerWidth()>736){
			$("#SUBMENU").attr("style","");
		}
	}
});
