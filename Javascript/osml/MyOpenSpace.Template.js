/**
 * @author jreyes
 */
opensocial = typeof opensocial === 'undefined'? {} : opensocial;
opensocial.template = typeof opensocial.template === 'undefined' ? {} : opensocial.template;
var ost = opensocial.template;

ost.log = function(msg) {
  var console = window['console'];
  if (console && console.log) {
    console.log(msg); 
  }
};

/**
 * Reprocess all template instances
 */
ost.process = function(){
	throw "Not implemented";
}

/**
 * Ignore
 */
ost.disableAutoProcessing = function(){
	throw "Not implemented";
}

ost.getTemplate = function(tag){
	if(!tag) return null;
	//TODO Verify that the template exist.
	return new MyOpenSpace.Template(tag);
}

MyOpenSpace = MyOpenSpace || {};

/**
 * Client side object representing a tag template.  Templates are emitted by the server
 * and are registed with the client via the call:
 * opensocial.template.registerTemplate(domNode)
 * x-mst-tag
 * @param {Object} templateDomNode
 */
MyOpenSpace.Template = function(tag) {

	//closure reference
	var me = this;
	this.tag = tag;

	/**
	* Renders an instance of the template with supplied JSON data 
	* and returns the created DOM node 
	* @param {Object} data
	*/
	this.render = function(data) {
		var container = document.createElement('div');
		MyOpenSpace.template.TemplateProcessor.parseTemplate(me.tag, data, container);
		return container;
	}

	/**
	* Renders template instance into the specified element
	* @param {DOMElement or string} element DOM element or ID of DOM element
	* @param {Object} data
	* @param {bool} append true to append to current contents, false/not defined to replace
	*/
	this.renderInto = function(element, data, append) {
		if (typeof (element) === "string") {
			element = document.getElementById(element);
		}
		if (!element || !element.innerHTML) {
			opensocial.template.log("Error resolving element for template processing");
		}
		if (append !== true) element.innerHTML = '';
		MyOpenSpace.template.TemplateProcessor.parseTemplate(me.tag, data, element);
	}
};