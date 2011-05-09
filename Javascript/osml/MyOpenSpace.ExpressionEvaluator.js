/**
 * @author Jorge Reyes
 */
var MyOpenSpace = MyOpenSpace || {};


//Todo Centralize in utility class
MyOpenSpace.log = function (msg) {
	var console = window.console;
	if (console && console.log) {
		console.log(msg);
	}
};

MyOpenSpace.ExpressionEvaluator ={
	"operatorPrecedence":{
		"empty" : {"p": 5,"a": "r", "f": "empty", "t" : "f"},
		"toNumber" : {"p": 5,"a": "r", "f": "toNumber", "t" : "f"},
		"toBoolean" : {"p": 5,"a": "r", "f": "toBoolean", "t" : "f"},
		"toString" : {"p": 5,"a": "r", "f": "toString", "t" : "f"},
		"mathRound" : {"p": 5,"a": "r", "f": "mathRound", "t" : "f"},
		"mathFloor" : {"p": 5,"a": "r", "f": "mathFloor", "t" : "f"},
		"mathCeil" : {"p": 5,"a": "r", "f": "mathCeil", "t" : "f"},
		"htmlEncode" : {"p": 5,"a": "r", "f": "htmlEncode", "t" : "f"},
		"htmlDecode" : {"p": 5,"a": "r", "f": "htmlDecode", "t" : "f"},
		"urlEncode" : {"p": 5,"a": "r", "f": "urlEncode", "t" : "f"},
		"urlDecode" : {"p": 5,"a": "r", "f": "urlDecode", "t" : "f"},
		"jsStringEscape" : {"p": 5,"a": "r", "f": "jsStringEscape", "t" : "f"},
		
		
		"~" : {"p": 5,"a": "r", "f": "neg", "t" : "u"},
		"!" : {"p": 5,"a": "r", "f": "not", "t" : "u"},
	
		"*" : {"p": 15,"a": "l", "f": "mul", "t" : "b"},
		"/" : {"p": 15,"a": "l", "f": "div", "t" : "b"},
		"%" : {"p": 15,"a": "l", "f": "mod", "t" : "b"},
		
		"+" : {"p": 20,"a": "l", "f": "add", "t" : "b"},
		"-" : {"p": 20,"a": "l", "f": "sub", "t" : "b"},
		
		">" : {"p": 25,"a": "l", "f": "gt", "t" : "b"},
		"<" : {"p": 25,"a": "l", "f": "lt", "t" : "b"},
		">=" : {"p": 25,"a": "l", "f": "ge", "t" : "b"},
		"<=" : {"p": 25,"a": "l", "f": "le", "t" : "b"},
		
		"==" : {"p": 30,"a": "l", "f": "eq", "t" : "b"},
		"!=" : {"p": 30,"a": "l", "f": "ne", "t" : "b"},

		"&&" : {"p": 35,"a": "l", "f": "and", "t" : "b"},
		"||" : {"p": 40,"a": "l", "f": "or", "t" : "b"}
	},
	"cache" : {}
};

MyOpenSpace.ExpressionEvaluator.parse = function(expression){
	if (this.cache[expression]){ return this.cache[expression]; }
	var output = [];
	var stack = [];

	var token;
	var lastTokenType = "s";
	var parentesisCount = 0;
	var braketsCount = 0;
	var bpOrder = [];
	var prev;
	
	var parseFunctionParams = function(expression, i){
		
		while(i < expression.length){
			i++;
			var c = expression[i];
			if (c === " " || c === "\t" || c === "\n" || c === "\r"){
				continue;
			}
			else if (c !== "("){
				throw "Invalid token at position " + i;
			}
			else{
				break;
			}
		}
		if (c !== "("){
			throw "Invalid token at position " + i;
		}
		i++;
		var parentCount = 1;
		var param = "";
		var params = [];
		while(parentCount > 0 && i < expression.length){
			c = expression[i];
			if (c === ")"){
				parentCount --;
				if (parentCount === 0){
					param = param.replace(/^\s+|\s+$/g,"");
					if (param === "" && params.length > 0){
						throw "Invalid token parameter at " + i;
					}
					else{
						params.push(MyOpenSpace.ExpressionEvaluator.parse(param));
					}
					continue;
				}
			}
			else if(c === "("){
				parentCount++;
			}
			else if (c === ","){
				param = param.replace(/^\s+|\s+$/g,"");
				if (param === ""){
					throw "Invalid token parameter at " + i;
				}
				else{
					params.push(MyOpenSpace.ExpressionEvaluator.parse(param));
				}
				i++;
				param = "";
				continue;
			}
			param += c;
			i++;
		}
		if (parentCount > 0){
			throw "Invalid Expression \")\" expected.";
		}
		
		return {p:params, i:i};
	};
	
	// expression for
	for (var i =0; i < expression.length; i+=1){
		var c = expression[i];
		if (c === "?"){
			bpOrder.push("?");
			this.validateToken_(lastTokenType, c, i);
			lastTokenType = c;
			token = {type:lastTokenType, value : c};
		}
		else if (c === ":"){
			if (bpOrder.length === 0){
				throw "Invalid token at position " + i;
			}
			else{
				prev = bpOrder.pop();
				if (prev !== "?"){
					throw "Invalid token at position " + i;
				}
			}
			this.validateToken_(lastTokenType, c, i);
			lastTokenType = c;
			token = {type:c, value : c};
		}
		else if (c === ")" || c === "(") {
			if (c === ")"){
				if (bpOrder.length === 0){
					throw "Invalid token at position " + i;
				}
				else{
					prev = bpOrder.pop();
					if (prev !== "("){
						throw "Invalid token at position " + i;
					}
				}
			}
			else{
				bpOrder.push(c);
			}
			parentesisCount += (c === "(")?1:-1;
			this.validateToken_(lastTokenType, c, i);
			lastTokenType = c;
			token = {type:lastTokenType, value:c};
		}
		else if (c === "]" || c === "[") {
			if (c === "]"){
				if (bpOrder.length === 0){
					throw "Invalid token at position " + i;
				}
				else{
					prev = bpOrder.pop();
					if (prev !== "["){
						throw "Invalid token at position " + i;
					}
				}
			}
			else{
				bpOrder.push(c);
			}
			braketsCount += (c === "[")?1:-1;
			this.validateToken_(lastTokenType, c, i);
			lastTokenType = c;
			token = {type: lastTokenType, value:c};
		}
		else if (c.match(new RegExp("[+\\/*%]", "gi"))){
			this.validateToken_(lastTokenType, "b", i);
			lastTokenType = "b";
			token = {type : "b", value: c};
		}
		else if (c === "-"){
			if (lastTokenType === "[" || lastTokenType === "(" || lastTokenType === "u" || lastTokenType === "s" || lastTokenType === "b") {
				lastTokenType = "u";
				token = {type : lastTokenType, value:"~"};
			}
			else {
				this.validateToken_(lastTokenType, "b", i);
				lastTokenType = "b";
				token = {type: lastTokenType, value:"c"};
			}
		}
		else if (c === " " || c === "\r" || c === "\t" || c === "\n"){
			continue;
		}
		else if (c === "\"" || c === "'"){
			this.validateToken_(lastTokenType, "o", i);
			var strVal = c;
			var completed =false;
			for (var k = i+1; k < expression.length; k+=1){
				i+=1;
				if (expression[k] === "\\"){
					strVal += "\\" + expression[k + 1];
					k+=1;
					i+=1;
				}
				else if (expression[k] === c){
					strVal += c;
					completed = true;
					break;
				}
				else{
					strVal += expression[k];
				}
			}
			if (completed === false) {
				this.cache[expression] = {"error":"Unterminated string at position " + i};
				throw "Unterminated string at position " + i;
			}
			lastTokenType = "o";
			token = {type : lastTokenType, value : strVal};
			
		}
		else if (c.match(/[0-9.]/)){
			this.validateToken_(lastTokenType, "o", i);
			var numStr = c;
			var decimalVal = c === ".";
			var e = false;
			for (var numIndex = i+1; numIndex < expression.length; numIndex+=1){
				i+=1;
				if (expression[numIndex] === "."){
					if (decimalVal) {
						this.cache[expression] = {"error":"Invalid number at position " + i};
						throw "Invalid number at position " + i;
					}
					numStr += ".";
				}
				else if(expression[numIndex].match(/[0-9]/)){
					numStr += expression[numIndex];
				}
				else if((expression[numIndex] === "e" || expression[numIndex] === "E") && !e && expression.length > numIndex + 1){
					if (expression[numIndex + 1] === "-" && expression.length > numIndex + 2 && expression[numIndex + 2].match(/[0-9]/)){
						e = true;
						decimalVal = true;
						numStr += "e-" + expression[numIndex + 2];
						numIndex += 2;
						i += 2;
					}
					else if (expression[numIndex + 1] === "+" && expression.length > numIndex + 2 && expression[numIndex + 2].match(/[0-9]/)){
						e = true;
						decimalVal = true;
						numStr += "e+" + expression[numIndex + 2];
						numIndex += 2;
						i += 2;
					}
					else if (expression[numIndex + 1].match(/[0-9]/)){
						e = true;
						decimalVal = true;
						numStr += "e" + expression[numIndex + 1];
						numIndex += 1;
						i += 1;
					}
					else{
						i-=1;
						break;
					}
				}
				else {
					i-=1;
					break;
				}
			}
			lastTokenType = "o";
			token = {type : lastTokenType, value: Number(numStr)};
		}
		else if (c === "l" && expression.substr(i, 3) === "lt "){
			this.validateToken_(lastTokenType, "b", i);
			lastTokenType = "b";
			token = {type: lastTokenType, value : "<"};
			i += 2;
		}
		else if (c === "g" && expression.substr(i, 3) === "gt "){
			this.validateToken_(lastTokenType, "b", i);
			lastTokenType = "b";
			token = {type : lastTokenType, value : ">"};
			i += 2;
		}
		else if (c === "l" && expression.substr(i, 3) === "le "){
			
			this.validateToken_(lastTokenType, "b", i);
			lastTokenType = "b";
			token = {type : lastTokenType, value : "<="};
			i += 2;
		}
		else if (c === "g" && expression.substr(i, 3) === "ge "){
			this.validateToken_(lastTokenType, "b", i);
			lastTokenType = "b";
			token = {type : lastTokenType, value : ">="};
			i += 2;
		}
		else if (c === "d" && expression.substr(i, 4) === "div "){
			this.validateToken_(lastTokenType, "b", i);
			lastTokenType = "b";
			token = {type : lastTokenType, value : "/"};
			i += 3;
		}
		else if (c === "m" && expression.substr(i, 4) === "mod "){
			this.validateToken_(lastTokenType, "b", i);
			lastTokenType = "b";
			token = {type : lastTokenType, value : "%"};
			i += 3;
		}
		else if (c === "=" && expression.substr(i, 2) === "=="){
			this.validateToken_(lastTokenType, "b", i);
			lastTokenType = "b";
			token = { type : lastTokenType , value : "=="};
			i += 1;
		}
		else if (c === "e" && expression.substr(i, 3) === "eq "){
			this.validateToken_(lastTokenType, "b", i);
			lastTokenType = "b";
			token = {type : lastTokenType, value : "=="};
			i += 2;
		}
		else if (c === "!" && expression.substr(i, 2) === "!="){
			this.validateToken_(lastTokenType, "b", i);
			lastTokenType = "b";
			token = { type : lastTokenType, value : "!="};
			i += 1;
		}
		else if (c === "!"){
			this.validateToken_(lastTokenType, "u", i);
			lastTokenType = "u";
			token = { type : lastTokenType , value : "!"};
		}
		else if (c === "n" && expression.substr(i, 4) === "not "){
			this.validateToken_(lastTokenType, "u", i);
			lastTokenType = "u";
			token = { type : lastTokenType, value : "!"};
			i += 3;
		}
		else if (c === "n" && expression.substr(i, 3) === "ne "){
			this.validateToken_(lastTokenType, "b", i);
			lastTokenType = "b";
			token = {type : lastTokenType, value : "!="};
			i += 2;
		}
		else if (c === "a" && expression.substr(i, 4) === "and "){
			this.validateToken_(lastTokenType, "b", i);
			lastTokenType = "b";
			token = { type : lastTokenType, value : "&&"};
			i += 3;
		}
		else if (c === "&" && expression.substr(i, 2) === "&&"){
			this.validateToken_(lastTokenType, "b", i);
			lastTokenType = "b";
			token = {type : lastTokenType, value : "&&"};
			i += 1;
		}
		else if (c === "&" && expression.substr(i, 10) === "&amp;&amp;"){
			this.validateToken_(lastTokenType, "b", i);
			lastTokenType = "b";
			token = {type : lastTokenType, value : "&&"};
			i += 9;
		}
		else if (c === "o" && expression.substr(i, 3) === "or "){
			this.validateToken_(lastTokenType, "b", i);
			lastTokenType = "b";
			token = { type: lastTokenType, value : "||"};
			i += 2;
		}
		else if (c === "|" && expression.substr(i, 2) === "||"){
			this.validateToken_(lastTokenType, "b", i);
			lastTokenType = "b";
			token = { type : lastTokenType, value : "||"};
			i += 1;
		}
// ----------------- Functions-------------------------
		else if (c === "e" && (expression.substr(i, 6) === "empty " || expression.substr(i, 6) === "empty(")){
			this.validateToken_(lastTokenType, "f", i);
			lastTokenType = "f";
			var params = parseFunctionParams(expression, i + 4);
			i = params.i;
			token = { type: lastTokenType , value : "empty", params : params.p};
		}
		else if (c === "t" && (expression.substr(i, 9) === "toNumber " || expression.substr(i, 9) === "toNumber(")){
			this.validateToken_(lastTokenType, "f", i);
			lastTokenType = "f";
			var params = parseFunctionParams(expression, i + 7);
			i = params.i;
			token = {type : lastTokenType, value : "toNumber", params : params.p};
		}
		else if (c === "t" && (expression.substr(i, 10) === "toBoolean " || expression.substr(i, 10) === "toBoolean(")){
			this.validateToken_(lastTokenType, "f", i);
			lastTokenType = "f";
			var params = parseFunctionParams(expression, i + 8);
			i = params.i;
			token = {type : lastTokenType, value : "toBoolean", params : params.p};
		}
		else if (c === "t" && (expression.substr(i, 9) === "toString " || expression.substr(i, 9) === "toString(")){
			this.validateToken_(lastTokenType, "f", i);
			lastTokenType = "f";
			var params = parseFunctionParams(expression, i + 7);
			i = params.i;
			token = {type : lastTokenType, value : "toString", params : params.p};
		}
		else if (c === "m" && (expression.substr(i, 10) === "mathRound " || expression.substr(i, 10) === "mathRound(")){
			this.validateToken_(lastTokenType, "f", i);
			lastTokenType = "f";
			var params = parseFunctionParams(expression, i + 8);
			i = params.i;
			token = {type: lastTokenType, value : "mathRound", params : params.p};
		}
		else if (c === "m" && (expression.substr(i, 9) === "mathCeil " || expression.substr(i, 9) === "mathCeil(")){
			this.validateToken_(lastTokenType, "f", i);
			lastTokenType = "f";
			var params = parseFunctionParams(expression, i + 7);
			i = params.i;
			token = {type : lastTokenType, value: "mathCeil", params : params.p};
		}
		else if (c === "m" && (expression.substr(i, 10) === "mathFloor " || expression.substr(i, 10) === "mathFloor(")){
			this.validateToken_(lastTokenType, "f", i);
			lastTokenType = "f";
			var params = parseFunctionParams(expression, i + 8);
			i = params.i;
			token = {type : lastTokenType, value : "mathFloor", params : params.p};
		}
		else if (c === "o" && (expression.substr(i,14) === "os:htmlEncode " || expression.substr(i,14) === "os:htmlEncode(")){
			this.validateToken_(lastTokenType, "f", i);
			lastTokenType = "f";
			var params = parseFunctionParams(expression, i + 12);
			i = params.i;
			token = {type : lastTokenType, value : "htmlEncode", params : params.p};
		}
		else if (c === "o" && (expression.substr(i,14) === "os:htmlDecode " || expression.substr(i,14) === "os:htmlDecode(")){
			this.validateToken_(lastTokenType, "f", i);
			lastTokenType = "f";
			var params = parseFunctionParams(expression, i + 12);
			i = params.i;
			token = {type : lastTokenType, value : "htmlDecode", params : params.p};
		}
		else if (c === "o" && (expression.substr(i,13) === "os:urlEncode " || expression.substr(i,13) === "os:urlEncode(")){
			this.validateToken_(lastTokenType, "u", i);
			lastTokenType = "f";
			var params = parseFunctionParams(expression, i + 11);
			i = params.i;
			token = {type: lastTokenType, value : "urlEncode", params : params.p};
		}
		else if (c === "o" && (expression.substr(i,13) === "os:urlDecode " || expression.substr(i,13) === "os:urlDecode(")){
			this.validateToken_(lastTokenType, "f", i);
			lastTokenType = "f";
			var params = parseFunctionParams(expression, i + 11);
			i = params.i;
			token = {type : lastTokenType, value : "urlDecode", params : params.p};
		}
		else if (c === "o" && (expression.substr(i,18) === "os:jsStringEscape " || expression.substr(i,18) === "os:jsStringEscape(")){
			this.validateToken_(lastTokenType, "f", i);
			lastTokenType = "f";
			var params = parseFunctionParams(expression, i + 16);
			i = params.i;
			token = {type : lastTokenType, value : "jsStringEscape", params : params.p};
		}
//--------------------------------------------
		else if (c.match(/[a-zA-Z_"]/)) {
			this.validateToken_(lastTokenType, "o", i);
			var varStr = c;
			var prevChar = c;
			for (var indexChar = i + 1; indexChar < expression.length; indexChar+=1) {
				i+=1;
				if (expression[indexChar].match(/[a-zA-Z0-9_.]/)){
					varStr += expression[indexChar];
					if (expression[indexChar] === "." && prevChar === "."){
						this.cache[expression] = {"error":"Invalid Variable name at position " + i};
						throw "Invalid Variable name at position " + i;
					}
					prevChar = expression[indexChar];
				}
				else{
					i-=1;
					break;
				}
			}
			
			lastTokenType = "v";
			if (varStr === "true" || varStr === "false"){
				lastTokenType = "o";
			}
			
			token = {type : lastTokenType, "value" : varStr};
		}
		else{
			this.cache[expression] = {"error":"Invalid character at position " + i};
			throw "Invalid character at position " + i;
		}
		this.processToken_(token, output,stack);
	}
	
	// end expression for

	try{
		this.validateToken_(lastTokenType, "e", i);
	}
	catch (tokenTypeError){
		this.cache[expression] = {"error":tokenTypeError};
		throw tokenTypeError;
	}
	if (braketsCount !== 0){
		this.cache[expression] = {"error":"Invalid number of []"};
		throw "Invalid number of []";
	}
	if (parentesisCount !== 0) {
		this.cache[expression] = {"error":"Invalid number of parentesis"};
		throw "Invalid number of parentesis";
	}
	for (var stackIndex = stack.length - 1; stackIndex >= 0; stackIndex-=1) {
		var lastOperator = stack[stackIndex];
		output.push(lastOperator);
		stack.pop();
	}
	this.cache[expression] = output;
	return output;
};

MyOpenSpace.ExpressionEvaluator.processToken_ = function(token, output,stack){
	var tokenType = token.type;
	var k, lastOperator;
	switch(tokenType){
		case "?":
			for (k = stack.length - 1; k >= 0; k-=1) {
				lastOperator = stack[k];
				if (lastOperator.type !== "("){
					output.push(stack.pop());
				}
				else{
					break;
				}
			}
			stack.push(token);
			output.push(token);
			break;
		case ":":
			for (k = stack.length - 1; k >= 0; k-=1) {
				lastOperator = stack[k];
				if (lastOperator.type !== "?"){
					output.push(lastOperator);
					stack.pop();
				}
				else{
					stack.pop();
					break;
				}
			}
			output.push(token);
			break;
		case "o":
		case "v":
			output.push(token);
			break;
		case "f":
		case "u":
		case "b":
//			if (lastOperator === "empty"){
//				stack.push(token);
//				break;
//			}
			for (k = stack.length - 1; k >= 0; k-=1) {
				lastOperator = stack[k].value;
				if (lastOperator === "(" || lastOperator === "[") {
					break;
				}
				if ((this.operatorPrecedence[token.value].a === "l" && this.operatorPrecedence[token.value].p >= this.operatorPrecedence[lastOperator].p) ||
					(this.operatorPrecedence[token.value].a === "r" && this.operatorPrecedence[token.value].p > this.operatorPrecedence[lastOperator].p)) {
					output.push(lastOperator);
					stack.pop();
				}
				else{
					break;
				}
			}
			stack.push(token);
			break;
		case "(":
			output.push(token);
			stack.push(token);
			break;
		case ")":
			for (k = stack.length - 1; k >= 0; k-=1) {
				lastOperator = stack[k];
				
				if (lastOperator.type === "(") {
					stack.pop();
					break;
				}
				output.push(lastOperator);
				stack.pop();
			}
			output.push(token);
			break;
		case "[" :
			//output.push(token);
			stack.push(token);
			break;
		case "]" :
			for (k = stack.length - 1; k >= 0; k-=1) {
				lastOperator = stack[k];
				
				if (lastOperator.type === "[") {
					stack.pop();
					break;
				}
				output.push(lastOperator);
				stack.pop();
			}
			output.push(token);
			break;
	}
};

MyOpenSpace.ExpressionEvaluator.validateToken_ = function(lastTokenType, tokenType, position){
	var isValid = false;
	switch (lastTokenType){
		case "[":
			isValid = (tokenType === "f" || tokenType === "u" || tokenType === "o" || tokenType === "v" || tokenType === "(");
			break;
		case "]":
			isValid = (tokenType === ")" || tokenType === "b" || tokenType === "e" || tokenType === "[" || 
				tokenType === "]" || tokenType === ":" || tokenType === "?");
			break;
		case "s":
		case "b":
		case "?": 
		case ":":
			isValid = (tokenType === "f" || tokenType === "o" || tokenType === "v" || tokenType === "(" || tokenType === "u");
			break;
		case "u":
			isValid = (tokenType === "f" || tokenType === "(" || tokenType === "o" || tokenType === "v" || tokenType === "u");
			break;
		case "o":
			isValid = (tokenType === "b" || tokenType === "e" || tokenType === ")" || tokenType === "]" || 
				"?" || ":");
			break;
		case "v":
			isValid = (tokenType === "b" || tokenType === "e" || tokenType === ")" || tokenType === "[" || 
				tokenType === "?" || tokenType === ":");
			break;
		case "(":
			isValid = (tokenType === "f" || tokenType === "u" || tokenType === "(" || tokenType === "o");
			break;
		case ")":
		case "f":
			isValid = (tokenType === ")" || tokenType === "e" || tokenType === "b" || tokenType === "?" || tokenType === ":");
			break;
	}
	if (!isValid){
		throw "Invalid token at " + position;
	}
};

MyOpenSpace.ExpressionEvaluator.EvalFunctions = {};

MyOpenSpace.ExpressionEvaluator.EvalFunctions.getStringValue = function(object, errorMessage){
	if (object === null){
		return "null";
	}
	else if (typeof object === "string" || typeof object === "number" || typeof object === "boolean"){
		return "" + object;
	}
	else{
		throw {exceptionType:"ELException", message:errorMessage};
	}
}

MyOpenSpace.ExpressionEvaluator.EvalFunctions.add = function(o1, o2) {
	if (typeof o1 === "string" || typeof o2 === "string") {
		if (typeof o1 === "string") {

			return o1 + MyOpenSpace.ExpressionEvaluator.EvalFunctions.getStringValue(o2, "Cannot concatenate objects.");
		}
		else {
			return MyOpenSpace.ExpressionEvaluator.EvalFunctions.getStringValue(o1, "Cannot concatenate objects.") + o2;
		}
	}
	if ( ((typeof o1 === 'object' && o1 !== null) || typeof o1 === 'undefined') ||
		((typeof o2 === 'object' && o2 !== null) || typeof o2 === 'undefined')
		) {
		throw { exceptionType: "ELException", message: "Cannot add objects." };
	}
	var result = o1 + o2;
	if (isNaN(result)) {
		throw { exceptionType: "ELException", message: "Cannot add -Infinity and Infinity." };
	}
	return result;
};

MyOpenSpace.ExpressionEvaluator.EvalFunctions.sub = function(o1, o2) {
	var o1Num = MyOpenSpace.ExpressionEvaluator.EvalFunctions.toNumber([o1]);
	var o2Num = MyOpenSpace.ExpressionEvaluator.EvalFunctions.toNumber([o2]);

	var result = o1Num - o2Num;
	if (isNaN(result)) {
		throw { exceptionType: "ELException", message: "Cannot substract Infinity and Infinity or -Infinity and -Infinity." };
	}
	return result;
};

MyOpenSpace.ExpressionEvaluator.EvalFunctions.mul = function(o1, o2){
	var o1Num = MyOpenSpace.ExpressionEvaluator.EvalFunctions.toNumber([o1]);
	var o2Num = MyOpenSpace.ExpressionEvaluator.EvalFunctions.toNumber([o2]);
	var result = o1Num * o2Num;
	if (isNaN(result)){
		throw {exceptionType:"ELException", message:"Cannot multiply (+/-)Infinity by 0."};
	}
	return result;
};

MyOpenSpace.ExpressionEvaluator.EvalFunctions.div = function(o1, o2){
	var o1Num = MyOpenSpace.ExpressionEvaluator.EvalFunctions.toNumber([o1]);
	var o2Num = MyOpenSpace.ExpressionEvaluator.EvalFunctions.toNumber([o2]);
	var result = o1Num / o2Num;
	if (isNaN(result)){
		throw {exceptionType:"ELException", message:"Cannot divide (+/-)Infinity by (+/-)Infinity."};
	}
	return result;
};

MyOpenSpace.ExpressionEvaluator.EvalFunctions.mod = function(o1, o2){
	var o1Num = MyOpenSpace.ExpressionEvaluator.EvalFunctions.toNumber([o1]);
	var o2Num = MyOpenSpace.ExpressionEvaluator.EvalFunctions.toNumber([o2]);
	var result = o1Num % o2Num;
	if (isNaN(result)){
		throw {exceptionType:"ELException", message:"Invalid Modulo values."};
	}
	return result;
};

MyOpenSpace.ExpressionEvaluator.EvalFunctions.eq = function(o1, o2){
	return o1 === o2;
};

MyOpenSpace.ExpressionEvaluator.EvalFunctions.ne = function(o1, o2){
	return o1 !== o2;
};

MyOpenSpace.ExpressionEvaluator.EvalFunctions.lt = function(o1, o2){
	if (o1 === null || o2 === null){
		throw {exceptionType:"ELException", message:"Cannot compare null values."};
	}
	if (typeof o1 === "string" || typeof o2 === "string"){
		if (typeof o1 === "string"){
			return o1 < MyOpenSpace.ExpressionEvaluator.EvalFunctions.getStringValue(o2, "Cannot compare objects.");
		}
		else{
			return MyOpenSpace.ExpressionEvaluator.EvalFunctions.getStringValue(o1, "Cannot compare objects.") < o2;
		}
	}
	if (typeof o1 !== "number" || typeof o2 !== "number"){
		throw {exceptionType:"ELException", message:"Cannot compare not numeric."};
	}
	return o1 < o2;
};

MyOpenSpace.ExpressionEvaluator.EvalFunctions.gt = function(o1, o2){
	if (o1 === null || o2 === null){
		throw {exceptionType:"ELException", message:"Cannot compare null values."};
	}
	if (typeof o1 === "string" || typeof o2 === "string"){
		if (typeof o1 === "string"){
			return o1 > MyOpenSpace.ExpressionEvaluator.EvalFunctions.getStringValue(o2, "Cannot compare objects.");
		}
		else{
			return MyOpenSpace.ExpressionEvaluator.EvalFunctions.getStringValue(o1, "Cannot compare objects.") > o2;
		}
	}
	if (typeof o1 !== "number" || typeof o2 !== "number"){
		throw {exceptionType:"ELException", message:"Cannot compare not numeric."};
	}
	return o1 > o2;
};



MyOpenSpace.ExpressionEvaluator.EvalFunctions.ge = function(o1, o2){
	if (o1 === null || o2 === null){
		throw {exceptionType:"ELException", message:"Can not compare null values."};
	}
	if (typeof o1 === "string" || typeof o2 === "string"){
		if (typeof o1 === "string"){
			return o1 >= MyOpenSpace.ExpressionEvaluator.EvalFunctions.getStringValue(o2, "Can not compare objects.");
		}
		else{
			return MyOpenSpace.ExpressionEvaluator.EvalFunctions.getStringValue(o1, "Can not compare objects.") >= o2;
		}
	}
	if (typeof o1 !== "number" || typeof o2 !== "number"){
		throw {exceptionType:"ELException", message:"Can not compare not numeric."};
	}
	return o1 >= o2;
};

MyOpenSpace.ExpressionEvaluator.EvalFunctions.le = function(o1, o2){
	if (o1 === null || o2 === null){
		throw {exceptionType:"ELException", message:"Can not compare null values."};
	}
	if (typeof o1 === "string" || typeof o2 === "string"){
		if (typeof o1 === "string"){
			return o1 <= MyOpenSpace.ExpressionEvaluator.EvalFunctions.getStringValue(o2, "Can not compare objects.");
		}
		else{
			return MyOpenSpace.ExpressionEvaluator.EvalFunctions.getStringValue(o1, "Can not compare objects.") <= o2;
		}
	}
	if (typeof o1 !== "number" || typeof o2 !== "number"){
		throw {exceptionType:"ELException", message:"Can not compare not numeric."};
	}
	return o1 <= o2;
};


MyOpenSpace.ExpressionEvaluator.EvalFunctions.and = function(o1, o2){
	if (typeof o1 !== "boolean" || typeof o2 !== "boolean"){
		throw {exceptionType:"ELException", message:"Can not apply and to not booleans."};
	}
	return o1 && o2;
};

MyOpenSpace.ExpressionEvaluator.EvalFunctions.or = function(o1, o2){
	if (typeof o1 !== "boolean" || typeof o2 !== "boolean"){
		throw {exceptionType:"ELException", message:"Can not apply or to not booleans."};
	}
	return o1 || o2;
};



MyOpenSpace.ExpressionEvaluator.EvalFunctions.empty = function(params){
	if (params.length !== 1){
		throw {exceptionType:"ELException", message:"Invalid number of parameters for function empty."};
	}
	var o1 = params[0]
	if (o1 === '' || o1 === null || typeof o1 === 'undefined'){
		return true;
	}
	if (typeof o1.length !== 'undefined' && o1.length === 0){
		return true;
	}
	return false;
};

MyOpenSpace.ExpressionEvaluator.EvalFunctions.toNumber = function(params){
	if (params.length !== 1){
		throw {exceptionType:"ELException", message:"Invalid number of parameters for function toNumber."};
	}
	var o1 = params[0];
	if (o1 === null){
		return 0;
	}
	else if (typeof o1 === 'number'){
		return o1;
	}
	else if (typeof o1 === 'string'){
		if (o1 === "Infinity"){
			return 1/0;
		}
		else if (o1 === "-Infinity"){
			return -1/0
		}
		var numberMatch = /^\d*\.?\d+(?:e[+-]?\d)?/gi;
		var match = o1.match(numberMatch);
		if (match === null){
			throw {exceptionType:"ELException", message:"String [" + o1 + "] cannot be converted to number."};
		}
		return Number(match[0]);
	}
	else if (typeof o1 === 'boolean'){
		if (o1 === true){
			return 1;
		}
		else{
			return 0;
		}
	}
	else{
		throw {exceptionType:"ELException", message:"Objects cannot be converted to number."};
	}
};

MyOpenSpace.ExpressionEvaluator.EvalFunctions.toBoolean = function(params){
	if (params.length !== 1){
		throw {exceptionType:"ELException", message:"Invalid number of parameters for function toBoolean."};
	}
	var o1 = params[0];
	if (o1 === null){
		return false;
	}
	else if (o1 === 'true'){
		return true;
	}
	else if (o1 === 'false'){
		return false;
	}
	else if (typeof o1 === 'string'){
		throw {exceptionType:"ELException", message:"String cannot be converted to boolean."};
	}
	else if (typeof o1 === 'boolean'){
		return o1;
	}
	else if (typeof o1 === 'number'){
		if (o1 === 0){
			return false;
		}
		else{
			return true;
		}
	}
	else{
		throw {exceptionType:"ELException", message:"Objects cannot be converted to number."};
	}
};

MyOpenSpace.ExpressionEvaluator.EvalFunctions.toString = function(params){
	if (params.length !== 1){
		throw {exceptionType:"ELException", message:"Invalid number of parameters for function toString."};
	}
	var o1 = params[0];
	if (o1 === null || typeof o1 === 'undefined'){
		return "null";
	}
	else if(typeof o1 === 'boolean' || typeof o1 === 'number' || typeof o1 === 'string'){
		return '' + o1;
	}
	else if(typeof o1.length !== 'undefined'){
		return 'array';
	}
	else{
		return 'object'
	}
	
};

MyOpenSpace.ExpressionEvaluator.EvalFunctions.mathRound = function(params) {
	if (params.length !== 1) {
		throw { exceptionType: "ELException", message: "Invalid number of parameters for function mathRound." };
	}

	var o1 = MyOpenSpace.ExpressionEvaluator.EvalFunctions.toNumber([params[0]]);
	var sing = o1 < 0 ? -1 : 1;
	//This is because difference on behavior between server and client.
	return sing * Math.round(Math.abs(o1));
	return Math.round(o1);

};

MyOpenSpace.ExpressionEvaluator.EvalFunctions.mathCeil = function(params){
	if (params.length !== 1){
		throw {exceptionType:"ELException", message:"Invalid number of parameters for function mathCeil."};
	}
	return Math.ceil(MyOpenSpace.ExpressionEvaluator.EvalFunctions.toNumber([params[0]]));
};

MyOpenSpace.ExpressionEvaluator.EvalFunctions.mathFloor = function(params){
	if (params.length !== 1){
		throw {exceptionType:"ELException", message:"Invalid number of parameters for function mathFloor."};
	}
	return Math.floor(MyOpenSpace.ExpressionEvaluator.EvalFunctions.toNumber([params[0]]));
};

MyOpenSpace.ExpressionEvaluator.EvalFunctions.htmlEncode = function(params) {
	if (params.length !== 1) {
		throw { exceptionType: "ELException", message: "Invalid number of parameters for function os:htmlEncode." };
	}
	var o1 = params[0];
	if ((typeof o1 === 'object' || typeof o1 === 'undefined') && o1 !== null) {
		throw { exceptionType: "ELException", message: "Cannot htmlEncode objects" };
	}

	return MyOpenSpace.Util.HtmlEncoder.htmlEncode("" + o1);
};

MyOpenSpace.ExpressionEvaluator.EvalFunctions.htmlDecode = function(params){
	if (params.length !== 1){
		throw {exceptionType:"ELException", message:"Invalid number of parameters for function os:htmlDecode."};
	}
	var o1 = params[0];
	if ((typeof o1 === 'object' || typeof o1 === 'undefined') && o1 !== null) {
		throw { exceptionType: "ELException", message: "Cannot htmlDecode objects." };
	}
	return MyOpenSpace.Util.HtmlEncoder.htmlDecode("" + o1);
};

MyOpenSpace.ExpressionEvaluator.EvalFunctions.urlEncode = function(params) {
	if (params.length !== 1) {
		throw { exceptionType: "ELException", message: "Invalid number of parameters for function os:urlEncode." };
	}
	var o1 = params[0];
	if ((typeof o1 === 'object' || typeof o1 === 'undefined') && o1 !== null) {
		throw { exceptionType: "ELException", message: "Cannot urlEncode objects." };
	}
	return encodeURIComponent("" + o1);
};

MyOpenSpace.ExpressionEvaluator.EvalFunctions.urlDecode = function(params){
	if (params.length !== 1){
		throw {exceptionType:"ELException", message:"Invalid number of parameters for function os:urlDecode."};
	}
	var o1 = params[0];
	if ((typeof o1 === 'object' || typeof o1 === 'undefined') && o1 !== null) {
		throw { exceptionType: "ELException", message: "Cannot urlDecode objects." };
	}
	return decodeURIComponent("" + o1);
};

MyOpenSpace.ExpressionEvaluator.EvalFunctions.jsStringEscape = function(params){
	if (params.length < 1 || params.length > 2){
		throw {exceptionType:"ELException", message:"Invalid number of parameters for function os:jsStringEscape."};
	}
	
	var o1, o2;
	o1 = params[0];
	if ((typeof o1 === 'object' || typeof o1 === 'undefined') && o1 !== null) {
		throw { exceptionType: "ELException", message: "Cannot jsStringEscape objects." };
	}
	o2 = false;
	if (params.length == 2){
		o2 = (params[1] === true);
	}
	if (o2){
		return ("" + o1).replace(/\\'/gi, "\\\\'").replace(/'/gi, "\\'");
	}
	else{
		return ("" + o1).replace(/\\"/gi, '\\\\"').replace(/"/gi, '\\"');
	}
};

MyOpenSpace.ExpressionEvaluator.EvalFunctions.neg = function(o1){
	if (typeof o1 !== "number"){
		throw {exceptionType:"ELException", message:"Can not apply unitary - operator to not numbers."};
	}
	return -o1;
};

MyOpenSpace.ExpressionEvaluator.EvalFunctions.not = function(o1){
	if (typeof o1 !== "boolean"){
		throw {exceptionType:"ELException", message:"Can not apply not operator to not booleans."};
	}
	return !o1;
};



MyOpenSpace.ExpressionEvaluator.EvalFunctions.toBool = function(o1){
	if (typeof o1 === 'undefined') {
		return false;
	}
	if (o1 === false || o1 === 0 || o1 === null || o1 === ""){
		return false;
	}
	if (typeof o1 === 'string' && o1.toLowerCase() === "false"){
		return false;
	}
	if (typeof o1 === 'object' &&
		o1.constructor.toString().indexOf("Array") !== -1 && 
		o1.length === 0) {
			return false;
		}

	return true;
};

MyOpenSpace.ExpressionEvaluator.EvalData ={
//	expressionCache_ : {},
//	data_ : 'data_',
//	my_ : 'my_',
//	cur_ : 'cur_',
//	context_: 'context_',
//	top_ : 'top_',
//	with_ : 'with (top_) with (context_) with (cur_) with (my_) with (data_) return '
};

//MyOpenSpace.ExpressionEvaluator.EvalData.expresionToFunction = function(expr){
//	if (!this.expressionCache_[expr]) {
//		try {
//			this.expressionCache_[expr] =
//			new Function(this.top_, this.context_, this.cur_, this.my_, this.data_, this.with_ + expr);
//		} catch (e) {
//			return null;
//		}
//	}
//	return this.expressionCache_[expr];
//};

MyOpenSpace.ExpressionEvaluator.EvalData.evalValue = function(expression, data){
	if (expression === "true"){
		return true;
	}
	else if (expression === "false"){
		return false;
	}
	else if (expression === "null"){
		return null;
	}
	else if (typeof expression === "number"){
		return expression;
	}
	else if (expression.indexOf('"') === 0 || expression.indexOf("'") === 0){
		return eval(expression);
	}
	var elements = expression.split(".");
	var variable;
	var properties = [];
	switch(elements[0]){
		case "My":
		case "Top":
		case "Cur":
		case "Context":
			if (elements.length === 1){
				variable = data[elements[0]];
			}
			else{
				variable = data[elements[0]][elements[1]];
				if (typeof variable === 'undefined'){
					MyOpenSpace.log("Unable to evaluate value for expression ["+ expression +"]");
					return undefined;
				}
				for (var j = 2; j < elements.length; j++){
					properties.push(elements[j]);
				}
			}
			break;
		default:
			if (typeof data.Cur[elements[0]] !== 'undefined'){
				variable = data.Cur[elements[0]];
			}
			else if (typeof data.My[elements[0]] !== 'undefined'){
				variable = data.My[elements[0]];
			}
			else if (typeof data.Top[elements[0]] !== 'undefined'){
				variable = data.Top[elements[0]];
			}
			else{
				MyOpenSpace.log("Unable to evaluate value for expression ["+ expression +"]");
				return undefined;
			}
			for (var j = 1; j < elements.length; j++){
				properties.push(elements[j]);
			}
		break;
	}
	
	variable = MyOpenSpace.ExpressionEvaluator.EvalData.mapVariable(variable, properties, expression);
	
	if (properties.length === 0 || variable === null || typeof variable === 'undefined'){
		return variable;
	}
	
	return MyOpenSpace.ExpressionEvaluator.EvalData.evalProperties(variable, properties, expression);
};

MyOpenSpace.ExpressionEvaluator.EvalData.mapVariable = function(variable, properties, expression){
	if (variable === null || typeof variable === 'undefined'){
		return variable;
	}
	if (properties.length > 0){
		if (typeof variable[properties[0]] !== 'undefined'){
			return variable;
		}
	}
	else{
		if (typeof variable.result === 'undefined' || typeof variable.result.content === 'undefined'){
			return variable;
		}
	}
	// It may be a make request call
	if (typeof variable.result !== 'undefined'){
		if (typeof variable.result.content !== 'undefined'){
			return variable.result.content;
		}
		else{
			MyOpenSpace.log("Error processing expression ["+ expression + "].");
			return undefined;
		}
	}
	else if (typeof variable.error !== 'undefined'){
		var message = variable.error.message;
		if (typeof message !== 'undefined'){
			MyOpenSpace.log("Error processing expression ["+ expression + "]. " + message);
		}
		else{
			MyOpenSpace.log("Error processing expression ["+ expression + "].");
		}
		return undefined;
	}
};

MyOpenSpace.ExpressionEvaluator.EvalData.evalProperties = function(variable, properties, expression){
	var localVariable = variable;
	for (var j = 0; j < properties.length; j++){
		if (typeof localVariable[properties[j]] === 'undefined'){
			MyOpenSpace.log("Unable to evaluate value for expression [" + expression + "]");
			return undefined;
		}
		localVariable = localVariable[properties[j]];
	}
	return localVariable;
}

MyOpenSpace.ExpressionEvaluator.getOperationFunction_= function(token){
	var operator = this.operatorPrecedence[token];
	if (!operator) {return null;}
	return {
		"f" : MyOpenSpace.ExpressionEvaluator.EvalFunctions[operator.f],
		"t" : operator.t
	};
	
};

MyOpenSpace.ExpressionEvaluator.evalExpression = function(expression, data) {
	var localExpression = expression.substr(2, expression.length-3);
	var re = /^[\s\x09]*$/gi;
	if (re.test(localExpression)) {throw "Invalid expression.";}

	//We initialize data if empty for testing
	var localData = (data)?data:{};
	if (!localData.Top) {localData.Top = {};}
	if (!localData.Context) {localData.Context = {};}
	if (!localData.Cur) {localData.Cur = {};}
	if (!localData.My) {localData.My = {};}
	
	//Remove ${ } from expression
	
	//Parse expression and return tokens in RPN
	var tokens = this.parse(localExpression);
	if (typeof tokens.error !== 'undefined'){
		throw tokens.error;
	}
	return this.evalTokens(tokens, localData);
};

MyOpenSpace.ExpressionEvaluator.evalTokens = function(tokens, localData){
	var ev = MyOpenSpace.ExpressionEvaluator.EvalData.evalValue;
	var subTokens, o1, o2, val, token;
	
	//If only one token we return the data evaluated
	if (tokens.length === 1){
		if (tokens[0].type !== "f"){
			return ev(tokens[0].value, localData);
		}
	}
	var tempArray = [];
	for (var k = 0; k < tokens.length; k+=1){
		token = tokens[k];
		if (token.type === "?"){
			o1 = tempArray.pop();
			var add = MyOpenSpace.ExpressionEvaluator.EvalFunctions.toBool(o1);
			subTokens = [];
			for (var j = k+1; j < tokens.length; j+=1){
				token = tokens[j];
				if (add){
					if (token.type === ":"){
						break;
					}
					subTokens.push(token);
				}
				else{
					if (token.type === ":"){
						add = true;
					}
				}
			}
			return this.evalTokens(subTokens, localData);
		}
		if (token.type === "("){
			subTokens = [];
			var parentesisCount = 1;
			
			do{
				k+=1;
				token = tokens[k];
				if (token.type === "("){
					parentesisCount+=1;
					subTokens.push(token);
				}
				else if (token.type === ")"){
					parentesisCount-=1;
					if (parentesisCount > 0){
						subTokens.push(token);
					}
				}
				else{
					subTokens.push(token);
				}
			}
			while (token.type !== ")" || parentesisCount > 0);
			tempArray.push(this.evalTokens(subTokens, localData));
		}
		else if(token.type === "]"){
			//TODO try to add it to operations
			o2 = tempArray.pop();
			o1 = tempArray.pop();
			val = o1[o2];
			tempArray.push(val);
		}
		else if (token.type === "u"){
			var operation = this.getOperationFunction_(token.value);
			if (operation === null){ throw "Error evaluation expression" }
			o1 = tempArray.pop();
			val = operation.f(o1);
			tempArray.push(val);
		}
		else if (token.type === "f"){
			var operation = this.getOperationFunction_(token.value);
			if (operation === null){ throw "Error evaluation expression" }
			var paramValues = [];
			for (var j = 0; j < token.params.length; j++){
				paramValues.push( MyOpenSpace.ExpressionEvaluator.evalTokens(token.params[j], localData) );
			}
			val = operation.f(paramValues);
			tempArray.push(val);
		}
		else if (token.type === "b") {
			var operation = this.getOperationFunction_(token.value);
			if (operation === null){ throw "Error evaluation expression" }
			o2 = tempArray.pop();
			o1 = tempArray.pop();
			val = operation.f(o1, o2);
			tempArray.push(val);
		}
		else{
			val = ev(token.value, localData);
			tempArray.push(val);
		}
	}
	return tempArray[0];
};

MyOpenSpace.ExpressionEvaluator.evaluate = function(expression, data){
	if (expression === null || typeof expression === 'undefined'){
		throw "Invalid expression.";
	}
	var regex = /(\${.*?})/gim;
	var dataSplited = expression.cbSplit(regex);

	var localExpression = null;
	if (dataSplited.length > 3){
		throw "Invalid expression.";
	}
	for (var i = 0; i < dataSplited.length; i+=1){
		if (dataSplited[i].replace(/\s/g, "") !==""){
			if (localExpression !== null){
				throw "Invalid expression.";
			}
			localExpression = dataSplited[i];
		}
	}
	if (localExpression === null || localExpression.indexOf("${") !== 0){
		throw "Invalid expression.";
	}
	return this.evalExpression(localExpression, data);
};

MyOpenSpace.ExpressionEvaluator.evalText = function(expression, data){
	if (!expression) {
		return "";
	}
	var regex = /(\${.*?})/gim;
	var dataSplited = expression.cbSplit(regex);
	var result = "";
	for (var i = 0; i < dataSplited.length; i+=1) {
		if (dataSplited[i].indexOf("${") !== 0){
			result += dataSplited[i];
		}
		else{
			var val = this.evalExpression(dataSplited[i], data);
			if (val === null || typeof val === 'undefined' || typeof val === "object") {
				continue;
			}
			else {
				result += val;
			}
		}
	}
	return result;
};

MyOpenSpace.ExpressionEvaluator.evalBool = function(expression, data){
	try{
		var val = this.evaluate(expression, data);
		return MyOpenSpace.ExpressionEvaluator.EvalFunctions.toBool(val);
	}
	catch(e){
		MyOpenSpace.log({
			"name": "EvaluationException",
			"message" : "The expression [" + expression + "] couldn't be evaluated to boolean. false was returned."
		});
	}
	return false;
};

MyOpenSpace.ExpressionEvaluator.evalTextValue = function(expression, data){
	if (!expression) {
		return "";
	}
	var result;
	//Find a better way to select correct evaluator other than exception
	try{
		result = this.evaluate(expression, data);
	}
	catch(e){
		result = this.evalText(expression, data);
	}
	return (result === null || typeof result === 'undefined')?"":result;
};

