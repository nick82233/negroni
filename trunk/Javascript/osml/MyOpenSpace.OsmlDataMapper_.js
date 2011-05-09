/**
 * Copyright 2007 Fox Interactive Media
 * <DISCLAIMER HERE>
 * @fileoverview Interface for container and associated classes; everything needed to query/update MySpace API data via OpenSocial interface.
 * 
 * @author mnewbould [_at_] myspace [_dot_] com (Max Newbould)
 * @author crussell [_at_] myspace [_dot_] com (Chad Russell)
 * @author jreyes [_at_] myspace [_dot_] com (Jorge Reyes)
 */
 
var MyOpenSpace = MyOpenSpace || {};
 
/**
 * Maps REST JSON responses To the format needed on OSML
 * @class
 * @constructor
 * @name MyOpenSpace.DataMapper_
 * @internal
 */
MyOpenSpace.OsmlDataMapper_ = {


	mapPeople_: function(obj) {
		var returnValue = [];
		for (var i =0; i < obj.entry; i++){
			returnValue.push(obj.entry[0].person);
		}
		return returnValue;
	},

	mapPerson_: function(obj) {
		return obj.person;
	},

	defaultMapper_:function(obj){
		return obj;
	},

	mapObject : function(obj){
		if (typeof obj.entry !== 'undefined' && obj.entry.length === 0){
			return obj.entry;
		}
		else if (typeof obj.entry !== 'undefined'){
			if (typeof obj.entry[0].person){
				return this.mapPeople_(obj);
			}
		}
		else if (typeof obj.person !== 'undefined'){
			return this.mapPerson_(obj);
		}
		else{
			return obj;
		}
	}

};