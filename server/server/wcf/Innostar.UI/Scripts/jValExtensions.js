(function ($) {
	$.validator.setDefaults({
		onfocusout: function (element, event) {
			$.showValidationErrorMessage(element, $(element).valid());
		},
	});
	
	var data = $("meta[name='accept-language']").attr("content");
	Globalize.culture(data);
	
	$.validator.methods.number = function(value, element) {
		return this.optional(element) ||
			!isNaN(Globalize.parseFloat(value));
	};

	$.validator.methods.date = function(value, element) {
		return this.optional(element) || Globalize.parseDate(value) != null;
	};
	
})(jQuery);