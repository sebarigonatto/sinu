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

    //se agrega este metodo para la validacion discreta, recibe el valor del control y el parametro(id del control a comparar)
    jQuery.validator.addMethod('telefonocelular', function (value, element, params) {
      
        if (value == null || value == "") {
            var value2 = $("#" + params).val();
            if (value2 == null || value2=="") {
                return false;
            }
        };
        return true;
    });

    //Este método vincula el complemento de validación discreta jQuery con el método "telefonocelular", y muestra el mensaje de error del lado del cliente
    jQuery.validator.unobtrusive.adapters.add('telefonocelular', ["celtel"], function (options) {
 
        options.rules['telefonocelular'] = options.params.celtel;
        if (options.message) {
            options.messages['telefonocelular'] = options.message;
        }
    });
}(jQuery));