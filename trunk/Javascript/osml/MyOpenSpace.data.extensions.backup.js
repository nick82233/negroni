/**
 * An extension of opensocial.data.DataContext
 * 
 * NOTE:
 * Extensions may be pushed into spec next version
 */
opensocial = opensocial || {}; 
opensocial.data = opensocial.data || {};
opensocial.data.a_ = 'a_';
opensocial.data.with_ = 'with (a_) return ';

opensocial.data.expressionCache_ = {};

/**
 * Does a smarter get of data than default method.
 * Adding new method to opensocial.data
 * @param {String} expression Dot-notation expression of value to get
 */
opensocial.data.deepGetValue = function(expression, data){
	
	
	if(!data){
	    var dc = opensocial.data.getDataContext(); //this object
		data = dc.getData();
	}
    
    var expressionFunction = this.expresionToFunction(expression);
    if (expressionFunction === null){
        return "error"; //NOTE jorge Not sure what to return.
    }
    try{
	    var value = expressionFunction(data);
	    return value;
	} catch (e) {
	    return "error";//NOTE jorge Not sure what to return.
	}
}

opensocial.data.expresionToFunction = function(expr) {
    if (!this.expressionCache_[expr]) {
        try {
            // NOTE: The Function constructor is faster than eval().
            this.expressionCache_[expr] =
            new Function(this.a_, this.with_ + expr);
        } catch (e) {
            return null;
        }
    }
    return this.expressionCache_[expr];
}


/**
* Reprocesses an existing item in the data context with new parameters.
*
*/
opensocial.data.DataContext.reprocess = function(key, params){
	if(!key || key.length == 0){
		return;
	}
	debugger;
	var dc = opensocial.data.DataContext;
	//get original tag
	var data = dc.getDataSet(key);
	if(!data || !data.Request || !data.Request.callingTag) return;
	
	var newParams = {};
	if(params){
		for(var key in params){
			//
			data.Request[key] = params[key];
			switch(key.toLowerCase()){
				case "startindex":
					newParams["first"] = params[key];
					break;
			}
		}
	}
	
	var callingRequest = data.Request;
	var closureKey = key;
	var req = null;
	
	var t = data.Request.callingTag.toLowerCase();
	switch(t){
		case "os:peoplerequest":
			req = osapi.people.get(newParams);
			break;
	}
	
	if(req != null){
	debugger;
		req.execute(function(result){
			if(result.result){
				result = result.result;
			}
			if(!result.error){
				dc.putDataSet(closureKey, result);
			}
		});
	}	

}
