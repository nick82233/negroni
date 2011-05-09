/**
 * Copyright 2007 Fox Interactive Media
 * <DISCLAIMER HERE>
 * @fileoverview Code that process OSML templates and tags and generat DOM elements to render in the page.
 * 
 * @author jreyes [_at_] myspace [_dot_] com (Jorge Reyes)
 * 
 */
 
MyOpenSpace = typeof MyOpenSpace === 'undefined'? {} :MyOpenSpace ;
MyOpenSpace.template = typeof MyOpenSpace.template === 'undefined' ? {} : MyOpenSpace.template;
MyOpenSpace.Widgets = typeof MyOpenSpace.Widgets === 'undefined' ? {} : MyOpenSpace.Widgets;
/**
 * Parse XML template data and generates DOM elements to be render in the page
 * @class
 * @constructor
 * @name MyOpenSpace.template.TemplateProcessor
 * @static
 * @internal
 */
MyOpenSpace.template.TemplateProcessor = {};
var ostp = MyOpenSpace.template.TemplateProcessor;

/**
 * Logs messages to firefox console
 * @param {Object} msg message to log.
 */
ostp.log = function(msg) {
	var console = window['console'];
	if (console && console.log) {
		console.log(msg);
	}
};

/**
 * Object that hold reference to the templates XML in string format
 * @internal
 */
ostp.templatesXml = {};

/**
 * Gets template xml or null if the template do not exist;
 * @param {Object} templateId Id of the template to return.
 */
ostp.getTemplateXml = function(templateId){
	var templateXml = this.templatesXml[templateId];
	if (typeof templateXml === 'undefined') return null;
	return templateXml;
};

/**
 * Adds template to the templatesXML collection
 * @param {Object} templateId Id of the template
 * @param {Object} xml string containing the xml of the template.
 */
ostp.registerTemplateXml = function(templateId, xml){
	this.templatesXml[templateId] = xml;
};

/**
 * This method will parse the template using the data provided on data parameters and will add
 * the elements to the result of the parsing to the container DOM element.
 * @param {Object} templateId Template Id
 * @param {Object} data Data to use
 * @param {Object} container DOM element that will hold the new elements result of the parsing and data evaluation.
 */
ostp.parseTemplate = function(templateId, data, container){
	var template = this.getTemplateXml(templateId);
	if (template === null) {
		this.log("template " + templateId + "do not exist.");
		return;
	} 
	var xmlDoc = this.loadTemplateXml(template);
	if (!xmlDoc) {
		this.log("Unable to parse XML Document.");
		return;
	}
	var templateData = {};
	templateData["Cur"] = null;
	templateData["My"] = data;
	templateData["Context"] = {"UniqueId":templateId};
	templateData["Top"] = opensocial.data.getDataContext().getData(); //
	
	this.parseTemplateContent(xmlDoc.firstChild, container, templateData, false);
};

/**
 * This will return an xml Document given a template Id or null if the template is not
 * well form.
 * @param {Object} template template id to load
 * @internal
 */
ostp.loadTemplateXml = function(template){
	var xmlDoc;
	try {
		
		if (window.DOMParser) {
			var dp = new DOMParser();
			xmlDoc = dp.parseFromString(template, "text/xml");
		}
		else {
			xmlDoc = new ActiveXObject("MSXML2.DOMDocument")
			xmlDoc.async = false;
			xmlDoc.loadXML(template);
		}
	}
	catch (e){
		this.log("Invalid template format");
		return null;
	}
	if (xmlDoc.firstChild === null || xmlDoc.firstChild.nodeName !== "Template") {
		this.log("Invalid template format");
		return null;
	}
	return xmlDoc;
};

/**
 * Returns the value of a given attribute in the element or null if the attribute do not exist.
 * @param {Object} xmlElement xml element that holds the attribute
 * @param {Object} attributeName Attribute to look form.
 */
ostp.getAttributeValue = function(xmlElement, attributeName){
	//TODO there must be a better way to do this check DOM xml documentation
	var attributes = xmlElement.attributes;
	for (var i = 0; i < attributes.length; i++) {
		if (attributes[i].nodeName === attributeName){
			return attributes[i].nodeValue;
		}
	}
	return null;
};
/**
 * Render repeaters
 * @param {Object} repeatExpression Expression to evaluate to retreive the repeater data
 * @param {Object} templateData template data
 * @param {Object} xmlElement repeater element
 * @param {Object} container domElement to add the elements result of the evaluation of the repeter.
 * @internal
 */
ostp.evaluateRepeat = function(repeatExpression, templateData, xmlElement, container) {
	var repeatData = MyOpenSpace.ExpressionEvaluator.evaluate(repeatExpression, templateData);
	if (repeatData.constructor.toString().indexOf("Array") === -1) {
		repeatData = [repeatData];
	}

	var elementData = {};
	elementData["My"] = templateData["My"];
	elementData["Top"] = templateData["Top"];
	elementData["Context"] = templateData["Context"];
	if (typeof elementData["Context"] === 'undefined') {
		elementData["Context"] = {};
	}
	var valAttribute = this.getAttributeValue(xmlElement, "var");
	var indexAttribute = this.getAttributeValue(xmlElement, "index");
	for (var j = 0; j < repeatData.length; j++) {
		elementData["Context"]["Index"] = j;
		elementData["Context"]["Count"] = repeatData.length;

		elementData["Cur"] = repeatData[j];
		if (valAttribute != null) {
			elementData["Context"][valAttribute] = repeatData[j];
		}
		if (indexAttribute != null) {
			elementData["Context"][indexAttribute] = j;
		}
		if (xmlElement.tagName === "os:Repeat") {
			var nodes = xmlElement.childNodes;
			for (var n = 0; n < nodes.length; n++) {
				this.parseTemplateContent(nodes[n], container, elementData, false);
			}
		}
		else {
			this.parseTemplateContent(xmlElement, container, elementData, true);
		}
	}
};
/**
 * Checks the node for special attributes and creates DOM elements and inserts them into the container 
 * @param {Object} xmlElement xml element to evaluate
 * @param {Object} container DOM element that will contain the newly created DOM Element 
 * @param {Object} templateData Data to be used in the creation of the element
 * @param {bool} repeteElement If true and the repeat element is present this node will be treated as a normal node
 */
ostp.evaluateNode = function(xmlElement, container, templateData, repeteElement){
	var elementName = xmlElement.tagName;
	var attributes = xmlElement.attributes;
	var elementAttributes = [];
	var ifExpression = null;
	for (var i = 0; i < attributes.length; i++) {
		var attribute = attributes[i];
		switch (attribute.nodeName) {
			case "if":
				ifExpression = attribute.nodeValue;
				break;
			case "repeat":
				if (repeteElement === true) continue;
				this.evaluateRepeat(attribute.nodeValue, templateData, xmlElement, container);
				return;
			case "var":
			case "index":
				// We remove this custum attributes
				break;
			default:
				elementAttributes.push({"name":attribute.nodeName, "value":attribute.nodeValue});
		}
	}

	//We are assuming that the check in the case of a repeat is while iterating.
	if (ifExpression !== null ){
		var display = MyOpenSpace.ExpressionEvaluator.evalBool(ifExpression, templateData);
		if (!display) return;
	}
	
	var element;
	//IE7 needs a different way to create input elements it needs to specify
	//the type at creation time if not radio buttons and other elements will fail
	if (elementName.toLowerCase() === "input"){
		var type = "";
		for (k = 0; k < elementAttributes.length; k++) {
			if (elementAttributes[k].name.toLowerCase() === 'type') {
				type = elementAttributes[k].value;
			}
		}
		if (type != ""){
			try {
				element = document.createElement("<input type=\"" + type + "\">");
			}
			catch (e){
				element = document.createElement(elementName);
				element.setAttribute("type", type);
			}
		}
		else{
			element = document.createElement(elementName);
		}
	}
	else{
		element = document.createElement(elementName);
	}
	
	for (var k=0; k < elementAttributes.length; k++){
		if (elementName.toLowerCase() === "input" && elementAttributes[k].name.toLowerCase() == 'type'){
			continue;
		}
		element.setAttribute(elementAttributes[k].name, MyOpenSpace.ExpressionEvaluator.evalText(elementAttributes[k].value, templateData));
	}
	container.appendChild(element);
	var nodes = xmlElement.childNodes;
	for (var j = 0; j < nodes.length; j++){
		this.parseTemplateContent(nodes[j], element, templateData, false);
	}
};

ostp.evaluateTag = function(tag, xmlElement, container, templateData){
	var xmlDoc = this.loadTemplateXml(tag);
	if (!xmlDoc) return;
	
	var My = {};
	var attNames = {};
	var attributes = xmlElement.attributes;
	for (var att = 0; att < attributes.length; att++){
		var value = MyOpenSpace.ExpressionEvaluator.evalTextValue(attributes[att].nodeValue, templateData);
		My[attributes[att].nodeName] = value;
		attNames[attributes[att].nodeName] = true;
	}
	
	var nodes = xmlElement.childNodes;
	for (var n = 0; n < nodes.length; n++){
		
		var node = nodes[n];
		if (node.nodeType !== 1) continue;//Just elements are added

		var baseName;
		if (node.baseName){
			baseName = node.baseName;
		} 
		else{
			baseName = node.localName;
		}
		
		//Attributes has precedence over elements 
		if (attNames[baseName])continue;
		
		My[baseName] = {};
		My[baseName]["nodeElement_"] = node;
		var nodeAttributes = node.attributes;
		for (var na = 0; na < nodeAttributes.length; na++){
			var attVal = MyOpenSpace.ExpressionEvaluator.evalText(nodeAttributes[na].nodeValue, templateData);
			My[baseName][nodeAttributes[na].nodeName] = attVal;
		}
	}
	var elementData = {};
	elementData["Top"] = templateData["Top"];
	elementData["My"] = My;
	elementData["Cur"] = templateData["Cur"];
	elementData["Context"] = templateData["Context"];
	this.parseTemplateContent(xmlDoc.firstChild, container, elementData, false);
};

ostp.parseElementVariables = function(My, element){
	var elementName = element.tagName;
	if (typeof My[elementName] === 'undefined'){
		My[elementName] = {};
	}
	
	var elementData = My[elementName];
}

ostp.parseTemplateContent = function(xmlElement, container, templateData, repeteElement){

	switch (xmlElement.nodeType){
		case 1:
			var elementName = xmlElement.tagName;
			switch(elementName){
				case "Template":
					var nodes = xmlElement.childNodes;
					for (var i = 0; i < nodes.length; i++){
						this.parseTemplateContent(nodes[i], container, templateData, false);
					}
					break; 
				case "os:Repeat":
					var repeatExpression = this.getAttributeValue(xmlElement, "expression");
					if (repeatExpression){
						this.evaluateRepeat(repeatExpression, templateData, xmlElement, container);
					}
					break;
				case "os:If":
					var displayCondition = this.getAttributeValue(xmlElement, "condition");
					if (!displayCondition){
						break;
					}
					var display = MyOpenSpace.ExpressionEvaluator.eval(displayCondition, templateData);
					if (!display) break;
					var nodes = xmlElement.childNodes;
					for (var i = 0; i < nodes.length; i++){
						this.parseTemplateContent(nodes[i], container, templateData, false);
					}
					break;
				case "os:Render":
					var contentLabel = this.getAttributeValue(xmlElement, "content");
					if (contentLabel){
						var contentVar = templateData["My"][contentLabel]
						if (!contentVar)break;
						var contentElement = contentVar["nodeElement_"];
						if (!contentElement) break;
						var nodes = contentElement.childNodes;
						for (var i = 0; i < nodes.length; i++){
							this.parseTemplateContent(nodes[i], container, templateData, false);
						}
					}
					break;
				default:
					if (xmlElement.prefix === "os"){
						break;
					}
					var tag = this.getTemplateXml(elementName);
					if (tag) {
						this.evaluateTag(tag, xmlElement, container, templateData)
					}
					else {
						this.evaluateNode(xmlElement, container, templateData, repeteElement);
					}
					break;
			}
			break;
		case 3:
			container.innerHTML += MyOpenSpace.ExpressionEvaluator.evalText(xmlElement.nodeValue, templateData);
			break;
	}
};

MyOpenSpace.Widgets.PeopleSelector = function(textContainer, itemContainer, maxSelection, clientHandler, formElement, dataContext) {
    var self = this;
    this._textContainer = document.getElementById(textContainer);
    this._itemContainer = document.getElementById(itemContainer);
    this._maxSelection = maxSelection;
    this._clientHandler = clientHandler;
    this._formElement = document.getElementById(formElement);
    this._dataContext = dataContext;

    if (this._textContainer && this._itemContainer) {
        var instance = this;

        var toggleHandler = function(e) {
            instance.ToggleDisplay(e);
        }
        MyOpenSpace.Util.safeAttachEvent(this._textContainer, "click", toggleHandler)
        MyOpenSpace.Util.safeAttachEvent(this._itemContainer, "click", MyOpenSpace.Util.stopEventBubbling)

        var checkBoxList = this._itemContainer.getElementsByTagName('input');


        for (var i = 0; i < checkBoxList.length; i++) {

            var checkHandler = function() {
                var checkbox = checkBoxList[i];
                return function(e) { instance.SelectItem(e, checkbox); }

            } ();

            MyOpenSpace.Util.safeAttachEvent(checkBoxList[i], "click", checkHandler)
        }

        if (checkBoxList.length > 5)
            this._itemContainer.style.height = "100px";
    }
};

MyOpenSpace.Widgets.PeopleSelector.prototype = {
    ToggleDisplay: function(e) {
        MyOpenSpace.Util.stopEventBubbling(e);
        var show = this._itemContainer.getAttribute('show');
        
        if (show == 'true')
            this.Hide(e);
        else
            this.Show(e);
    },
    Show: function(e) {

        var instance = this;
        var hideHandler = function(e) {
            instance.Hide(e);
        };

        this._itemContainer.style.display = "";
        this._itemContainer.style.zIndex = "5000";
        this._itemContainer.style.left = this._textContainer.offsetLeft + this._textContainer.offsetParent.offsetLeft + "px";
        this._itemContainer.style.top = this._textContainer.offsetTop + this._textContainer.offsetParent.offsetTop + this._textContainer.clientHeight + 2 + "px";
        this._itemContainer.style.width = this._textContainer.clientWidth - 1 + "px";
        this._itemContainer.setAttribute('show', 'true');
        MyOpenSpace.Util.safeAttachEvent(document, "click", hideHandler);
    },
    Hide: function(e) {
        var self = this;
        var hideHandler = function(e) {
            self.Hide(e);
        };

        this._itemContainer.setAttribute('show', 'false');
        this._itemContainer.style.display = 'none';
        this._itemContainer.style.zIndex = "0";
        MyOpenSpace.Util.safeDetachEvent(document, "click", hideHandler);
    },
    SelectItem: function(e, checkbox) {
        var selectedObjects = new Array();
        if (checkbox.checked) {
            var checkboxList = this._itemContainer.getElementsByTagName('input');
            var values = "", checkedCount = 0;
            for (var i = 0; i < checkboxList.length; i++) {
                if (checkboxList[i].checked) {
                    selectedObjects[selectedObjects.length] = this.GetSelectedData(i, checkboxList[i].value);
                    if (this._maxSelection == 0 || checkedCount < this._maxSelection) {
                        checkedCount++;
                        if (values.length <= 0)
                            values = checkboxList[i].title;
                        else
                            values = values + "," + checkboxList[i].title;
                    }
                }
            }

            if (this._maxSelection > 0 && checkedCount >= this._maxSelection) {
                for (var i = 0; i < checkboxList.length; i++) {
                    if (!checkboxList[i].checked) {
                        checkboxList[i].disabled = true;
                    }
                }
            }
            this._textContainer.innerHTML = values;

            if (this._formElement) {
                if (this._formElement.value !== undefined)
                    this._formElement.value = values;
                else if (this._formElement.innerHTML !== undefined)
                    this._formElement.innerHTML = values;
            }
        }
        else {
            var checkboxList = this._itemContainer.getElementsByTagName('input');
            for (var i = 0; i < checkboxList.length; i++) {
                checkboxList[i].disabled = false;
                if (checkboxList[i].checked)
                    selectedObjects[selectedObjects.length] = this.GetSelectedData(i, checkboxList[i].value);
            }

            var ids = this._textContainer.innerHTML.split(',');
            for (var i = 0; i < ids.length; i++) {
                if (ids[i] == checkbox.title) {
                    ids.splice(i, 1);
                    break;
                }
            }
            this._textContainer.innerHTML = ids.join(',');

            if (this._formElement) {
                if (this._formElement.value !== undefined)
                    this._formElement.value = ids.join(',');
                else if (this._formElement.innerHTML !== undefined)
                    this._formElement.innerHTML = ids.join(',');
            }
        }
        if (this._clientHandler && typeof this._clientHandler == "function")
            this._clientHandler(selectedObjects);
    },
    GetSelectedData: function(index, currentVale, dataContext) {
        var localDataContext;
        dataContext == null ?  localDataContext = this._dataContext : localDataContext = dataContext;
        var objArray = opensocial.data.getDataContext().getDataSet(localDataContext);
        if (objArray && objArray.entry && objArray.entry[index]) {
            if (objArray.entry[index].person.id == currentVale)
                return objArray.entry[index].person;
        }
        return null;
    }
};

MyOpenSpace.Widgets.PeopleSelector.OnSingleSelectSelectionChange = function(dropDown, textContainer, clientHandler, dataContext) {
    var selectedObjects;
    if (textContainer) {
        if (textContainer.value !== undefined)
            textContainer.value = dropDown.options[dropDown.selectedIndex].value;
        else if (textContainer.innerHTML !== undefined)
            textContainer.innerHTML = dropDown.options[dropDown.selectedIndex].value;
    }
    if (clientHandler && typeof clientHandler == "function") {
        selectedObjects = MyOpenSpace.Widgets.PeopleSelector.prototype.GetSelectedData(dropDown.selectedIndex, dropDown.options[dropDown.selectedIndex].value, dataContext);
        clientHandler(selectedObjects);
    }
};