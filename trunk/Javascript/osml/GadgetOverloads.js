/**
* Overwrites existing functions with extensions to support OSML functionality.
*/

gadgets.views.oldRequestNavigateTo = gadgets.views.requestNavigateTo;

/**
* Update requestNavigateTo with support for subview navigation
*/
gadgets.views.requestNavigateTo = function(view, opt_params){

	//non-subview navigate request
	if(
	        (view.indexOf && view.indexOf(".") === -1) ||
	        (view.getName && view.getName().indexOf(".") === -1)){
		gadgets.views.oldRequestNavigateTo(view, opt_params);
		return;
	}
//_osmlContentBlock_appid166877__osmlContentBlock_2
	var appId = opensocial.Container.get().getMySpaceEnvironment().getApplication().id;
	var idBase ="_osmlContentBlock_appid" + appId + "__osmlContentBlock_"; //id template for content blocks
	var classnameBase = "xcontentblockview-"; //classname base - subview is appended to end
	var mainContentClass = classnameBase + "canvas";
	
	var max = 30;
	var targetClass = classnameBase + view;
	var tmpId;
	var splitNames;
	var elem = null;
	var tmpElem;
	var n, markHidden, isMainContentBlock;
	//hide all subviews
	if(opt_params && opt_params.dismissAll == true){
		targetClass = "";
	}
	for(var i=0; i < max; i++){
		tmpId = idBase + i;
		tmpElem = document.getElementById(tmpId);
		if(!tmpElem){
			break;
		}
		if(tmpElem.className){
			splitNames = tmpElem.className.split(' ');
			markHidden = true;
			for(n=0; n < splitNames.length; n++){
				if(splitNames[n]==""){
					continue;
				}
				if(splitNames[n] == mainContentClass){
					markHidden = false;
					break;;
				}
				else if(splitNames[n] == targetClass){
					markHidden = false;
					elem = tmpElem;
					elem.style.display="block";
					elem.style.visibility="visible";
					break;
				}
			}
			if(markHidden){
				tmpElem.style.display="none";
				tmpElem.style.visibility="hidden";
			}
		}
		/*
		if(elem != null){
			break;
		}
		*/
	}
	
}

gadgets.views.hideSubviews = function(view){
	gadgets.views.requestNavigateTo(view, {"dismissAll":true});
}

