
/**
 * Client request processor
 */
MyOpenSpace.ClientRequestProcessor = {

	/**
	* Array of MyOpenSpace.ClientRequest objects
	* that will be transformed into makeRequest statements
	*/
	tagRequests: [],
	
	/**
	 * Add a new render request to the MyOpenSpace.TemplateProcessor.tagRenderRequests queue.
	 * 
	 * @param {String} url target URL
	 * @param {String} responseTarget How to handle response. Valid values are 'data' or 'render'
	 * @param {String or DOM element} target DOM element reference to render contents into or DataContext key
	 * @param {Object} opt_params parameters passed on to makeRequest
	 */
	addRequest: function(url, responseType, target, opt_params){
		this.tagRequests.push(new MyOpenSpace.ClientRequest(url, responseType, target, opt_params));
	},
	
	/**
	 * Executes all render requests currently registered with 
	 * MyOpenSpace.TemplateProcessor.tagRenderRequests.
	 * Deletes requests as they are processed
	 */
	resolveRequests: function(){
		var requestQ = this.tagRequests;
		if(!requestQ || requestQ.length == 0){
			return;
		}
		var i, req;
		for(i= requestQ.length-1; i >= 0; i--){
			req = requestQ.pop();
			this.executeRenderRequest(req);
		}	
	},
	
	/**
	 * Construct the appropriate makeRequest and callback to interpret the rendered results.
	 * @param {Object} request
	 */
	executeRenderRequest: function(request){
		var params = request.requestParams || {};
		
		var respType = request.responseTarget;
		if(respType == ''){
			respType = 'render';
		}
		var callback;
		
		if(respType == 'data'){
			//assume JSON if not specified
			if(!params[gadgets.io.RequestParameters.CONTENT_TYPE]){
				params[gadgets.io.RequestParameters.CONTENT_TYPE] = gadgets.io.ContentType.JSON;
			}
			var key = request.target;
			callback = function(obj){
				var newResultObj = {};
				//debugger;
				if ((obj != null && obj !== undefined) && (typeof (obj.errorCode) == 'undefined' || (!obj.errors || obj.errors.length == 0))) {
					newResultObj.result = {
						content: null,
						status: "200"
					};
					
					try{
						if(obj.data){
							newResultObj.result.content = obj.data;
						}
						else{
							newResultObj.result.content = obj.text;
						}
						opensocial.data.getDataContext().putDataSet(key, newResultObj);
					
					}
					catch(ex){
						if(gadgets.warn){
							gadgets.warn("Error registering tag request result");
						}
					}
				}
				else{
					newResultObj.error = {};
					
					if(obj.error && obj.error.length > 0){
						newResultObj.error = {
							message: obj.error[0]
						};
						if(obj.errorCode){
							newResultObj.error.errorCode = obj.errorCode;
						}
						else{
							newResultObj.error.errorCode = obj.error[0];						
						}
					}
					else{				
						newResultObj.error = {
							code: obj.errorCode,
							message: obj.errorMessage
						};
					}
					if(obj.text && obj.text.length > 0){
						newResultObj.error.data = {content: obj.text};
					}
					
					if(gadgets.warn){
						gadgets.warn("Error processing url: " + request.url + "\n" + obj.errorMessage);
					}
					else if(gadgets.log){
						gadgets.warn("Error processing url: " + request.url + "\n" + obj.errorMessage);
					}
				}
			}
		}		
		else if(respType == 'render'){
			//default to GET
			if(!params[gadgets.io.RequestParameters.METHOD]){
				params[gadgets.io.RequestParameters.METHOD] = gadgets.io.MethodType.GET;
			}
			//force as text
			params[gadgets.io.RequestParameters.CONTENT_TYPE] = gadgets.io.ContentType.TEXT;
			//resolve element
			var elem = request.target;
			if(typeof(elem) === "string"){
				elem = document.getElementById(elem);
			}
			if(!elem || elem.innerHTML === undefined){
				return;
			}
			
			callback = function(obj){
				if(typeof(obj.errorCode) == 'undefined'){
					elem.innerHTML =obj.text;
				}else{
					elem.innerHTML =obj.errorMessage;
				}		
			}
		}
		
		
		gadgets.io.makeRequest(request.url, callback, params);	
	}	
}
