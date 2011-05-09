/**
 * Encapsulation of repeater data
 */
MyOpenSpace.TemplateProcessor.repeatNodeData = function(){
	this.instanceMarkup = "";
	this.repeatExpression = "";
	this.repeatOnData = {};	
	this.currentKey = "Cur";
	
	/**
	 * Sets the current repeater expression, stripping
	 * off any unused tokens
	 * @param {Object} expr
	 */
	this.setRepeatExpression = function(expr){
		if(!expr || expr == "") return;
		
		expr = expr.replace(MyOpenSpace.TemplateProcessor.Tokens.TOKEN_START, "");
		this.repeatExpression = expr.replace(MyOpenSpace.TemplateProcessor.Tokens.TOKEN_END, "");
	}
	
	/**
	 * Create an instance of the loop data
	 */
	this.createLoopInstance = function(){
		var inst = document.createElement("span");
		inst.innerHTML = this.instanceMarkup;
		
		return inst;
	}
}
