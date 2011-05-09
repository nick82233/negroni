// (c) Steven Levithan <stevenlevithan.com>
// MIT License
// Consistent cross-browser, ECMA-262 v3 compliant split


// avoid running twice, which would break the reference to the native `split`
if (!window.cbSplit) {

	var cbSplit = function (str, separator, limit) {
		// if `separator` is not a regex, use the native `split`
		if (Object.prototype.toString.call(separator) !== "[object RegExp]")
			return cbSplit._nativeSplit.call(str, separator, limit);

		var	output = [],
			lastLastIndex = 0,
			flags = (separator.ignoreCase ? "i" : "") +
				(separator.multiline  ? "m" : "") +
				(separator.sticky     ? "y" : ""),
			separator = RegExp(separator.source, flags + "g"), // make `global` and avoid `lastIndex` issues
			separator2, match, lastIndex, lastLength;

		str = str + ""; // type conversion
		if (!cbSplit._compliantExecNpcg)
			separator2 = RegExp("^" + separator.source + "$(?!\\s)", flags); // doesn't need /g or /y, but they don't hurt

		// behavior for `limit`: if it's...
		// - `undefined`: no limit.
		// - `NaN` or zero: return an empty array.
		// - a positive number: use `Math.floor(limit)`.
		// - a negative number: no limit.
		// - other: type-convert, then use the above rules.
		if (limit === undefined || +limit < 0) {
			limit = Infinity;
		} else {
			limit = Math.floor(+limit);
			if (!limit)
				return [];
		}

		while (match = separator.exec(str)) {
			lastIndex = match.index + match[0].length; // `separator.lastIndex` is not reliable cross-browser

			if (lastIndex > lastLastIndex) {
				output.push(str.slice(lastLastIndex, match.index));

				// fix browsers whose `exec` methods don't consistently return `undefined` for nonparticipating capturing groups
				if (!cbSplit._compliantExecNpcg && match.length > 1) {
					match[0].replace(separator2, function () {
						for (var i = 1; i < arguments.length - 2; i++) {
							if (arguments[i] === undefined)
								match[i] = undefined;
						}
					});
				}

				if (match.length > 1 && match.index < str.length)
					Array.prototype.push.apply(output, match.slice(1));

				lastLength = match[0].length;
				lastLastIndex = lastIndex;

				if (output.length >= limit)
					break;
			}

			if (separator.lastIndex === match.index)
				separator.lastIndex++; // avoid an infinite loop
		}

		if (lastLastIndex === str.length) {
			if (!separator.test("") || lastLength)
				output.push("");
		} else {
			output.push(str.slice(lastLastIndex));
		}

		return output.length > limit ? output.slice(0, limit) : output;
	};

	cbSplit._compliantExecNpcg = /()??/.exec("")[1] === undefined; // "NPCG": nonparticipating capturing group
	cbSplit._nativeSplit = String.prototype.split;

}

// for convenience...
String.prototype.cbSplit = function (separator, limit) {
	return cbSplit(this, separator, limit);
};