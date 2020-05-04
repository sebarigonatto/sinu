$(function () {
    $.validator.methods.range = function (value, element, param) {
        var globalizedValue = value.replace(",", ".");
        return this.optional(element) || (globalizedValue >= param[0] && globalizedValue <= param[1]);
    }

    $.validator.methods.number = function (value, element) {
        return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:[\s\.,]\d{3})+)(?:[\.,]\d+)?$/.test(value);
    }
    //para qu ela validacion de fecha reconosca el formato dd/MM/yyyy

    jQuery.validator.methods["date"] = function (value, element) { return true; };

    jQuery.validator.addMethod('telefonocelular', function (value, element, params) {
      
        if (value == null || value == "") {
            var value2 = $("#" + params).val();
            if (value2 == null || value2=="") {
                return false;
            }
        };
        return true;
    });

    jQuery.validator.unobtrusive.adapters.add('telefonocelular', ["celtel"], function (options) {
 
        options.rules['telefonocelular'] = options.params.celtel;
        if (options.message) {
            options.messages['telefonocelular'] = options.message;
        }
    });
}(jQuery));