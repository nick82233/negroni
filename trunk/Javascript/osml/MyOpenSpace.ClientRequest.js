
/**
 * Object encapsulating data or render request parameters that will be 
 * transformed and executed as makeRequest statements.
 * These are usually loaded from OSML tags
 * 
 * @param {String} url Target url
 * @param {String} responseTarget How to handle response. Valid values are 'data' or 'render'
 * @param {String or DOM element} target DOM ID or DOM element reference
 * @param {Object} opt_params parameters to pass on to makeRequest
 */
MyOpenSpace.ClientRequest = function(url, responseTarget, target, opt_params){
	
	/**
	* Url being requested
	*/
	this.url = url
	
	/**
	* Type of response processing to use. Valid types are 'data' or 'render'
	*/
	this.responseTarget = responseTarget;
	

	/**
	* Target element ID or DOM reference where results will be rendered if
	* responseTarget == 'render' or the Data Context key if responseTarget == 'data'
	*/
	this.target = target;
	
	/**
	 * Parameters passed into the gadget.io.makeRequest as opt_params
	 */
	this.requestParams = opt_params || {};
}
