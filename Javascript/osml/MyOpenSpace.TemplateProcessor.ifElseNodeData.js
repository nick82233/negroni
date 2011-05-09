
/**
 * Encapsulation of an If/Else node set
 */
MyOpenSpace.TemplateProcessor.ifElseNodeData = function(node){
	this.ifConditionExpression = "";
	
	/**
	 * Remembers if this evaluated to true or false
	 */
	this.isIfTrue = null;
	
	/**
	 * Flags if this statement has an else component
	 */
	this.hasElseBlock = null;
	
	/**
	 * Actual DOM element nodes for this block
	 */
	this.domNodes = {
		ifNode : null,
		elseNode: null
	}
	
	/**
	 * InnerHTML string of the nodes, since evaluation is destructive.
	 */
	this.domNodeContents = {
		ifContent: null,
		elseContent: null
	}
	
	/**
	 * Sets the current repeater expression, stripping
	 * off any unused tokens
	 * @param {Object} expr
	 */
	this.testIfStatement = function(data){
		if(!this.ifConditionExpression || this.ifConditionExpression == "") return;
		var expr = this.ifConditionExpression;
		
		expr = expr.replace(MyOpenSpace.TemplateProcessor.Tokens.TOKEN_START, "");
		this.ifConditionExpression = expr.replace(MyOpenSpace.TemplateProcessor.Tokens.TOKEN_END, "");
		//TODO: NAUGHTY, NAUGHTY - REDESIGN THIS
		var val = opensocial.data.deepGetValue(this.ifConditionExpression, data);
		var result = false;
		if(val){
			try{
				result = window.eval(val);
			}
			catch(ex){}
		}
		this.isIfTrue = result;
		return this.isIfTrue;
	}
	
	/**
	 * Loads a DOM element node
	 * @param {DOMElement} node
	 */
	this.loadNode = function(node){
		if(!node) return;
		this.ifConditionExpression = node.getAttribute("condition");
		
		this.domNodes.ifNode = node;
		this.domNodeContents.ifContent = node.innerHTML;
		
//		debugger;
		var xnode = node;
		var nextNode = node.nextSibling;
		var i=0;
		
		for(i=0; i<100; i++){
//		while(nextNode != null){
//		while(xnode.nextSibling != null){
//var n=0;
//while(n++ < 4){
			if(nextNode.nodeType == 1){
				if(nextNode.nodeName.toUpperCase() == "OSX:ELSE"){
					this.hasElseBlock = true;
					this.domNodes.elseNode = nextNode;
					this.domNodeContents.elseContent = nextNode.innerHTML;
					break;
				}
				else{
					break;
				}
			}
			else{
				if(nextNode.nextSibling == null){
					break;
				}
				else{
					nextNode = nextNode.nextSibling;
				}				
			}
		}
		
	}
	
	if(node){
		this.loadNode(node);
	}
	
}


