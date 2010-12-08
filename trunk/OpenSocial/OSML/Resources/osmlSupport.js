
/*
 * OSML Template support library.
 * Included in all rendered templates
 */



window.OSML = {
	
	/**
	 * Empty client side context object initialized
	 */
	DataContext: {},
	
	
	
	
	/**
	 * Internally used functions
	 */
	Internals_:{
		/**
		 * Gets a reference to the DOM element representing a named view
		 * defined in an OSML template
		 * 
		 * @param {String} viewName      Name of the view as defined in the template
		 */
		getViewElement: function(viewName){
			var vname = "osmlView_" + viewName;
			
			return document.getElementById(vname);		
		},
		
		hideElem: function(elem){
			elem = OSML.Internals_.confirmIsDomElem(elem);
			elem.style.visibility = "hidden";
			elem.style.display = "none";
		},
		
		showElem: function(elem){
			elem = OSML.Internals_.confirmIsDomElem(elem);
			elem.style.visibility = "visible";
			elem.style.display = "block";
		},
		confirmIsDomElem: function(elem){
			if(typeof(elem) === "string"){
				elem = document.getElementById(elem);
			}
			return elem;
		},
		
		hideAllViews: function(){
			OSML.Internals_.hideAllViewsBut();
		},
		
		hideAllViewsBut: function(nonHiddenView){
			if(window.OsmlViews && window.OsmlViews.length > 0){
				var view, i;
				for(i=0; i < OsmlViews.length; i++){
					view = OSML.Internals_.confirmIsDomElem(OsmlViews[i]);
					if(view && OsmlViews[i] != nonHiddenView){
						OSML.Internals_.hideElem(view);
					}
				}
				 
			}
		}
		
		
	},
	
	
	navToView: function(viewName){
		var elem = OSML.Internals_.getViewElement(viewName);
		if(!elem) return;
		OSML.Internals_.hideAllViewsBut(elem.id);
		OSML.Internals_.showElem(elem);
	}
	
	
	
	
	
	
	
}








