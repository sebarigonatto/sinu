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



    ///AVALIDADION DE LADO DEL CLIENTE PARA LA VALIDACION DE LOS CAMPOR TELEFONO Y CELULAR
    //se agrega este metodo para la validacion discreta, recibe el valor del control y el parametro(id del control a comparar)
    jQuery.validator.addMethod('telefonocelular', function (value, element, params) {

        if (value == null || value == "") {
            var value2 = $("#" + params).val();
            if (value2 == null || value2 == "") {
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


    //VALIDACION DEL LADO DE CLIENTE DONDE SE VALIDA QUE LA FECHA DE FIN SEA MAYOR QUE LA FECHA DE INICIO
    //se agrega este metodo para la validacion discreta, recibe el valor del control y el parametro(id del control a comparar)
    jQuery.validator.addMethod('fimenorff', function (value, element, params) {
        var fechaIni = $("#" + params).val().split('/');
        var fechaFin = value.split('/');
        var fechainicio = new Date(fechaIni[2], fechaIni[1], fechaIni[0]);
        var fechafinal = new Date(fechaFin[2], fechaFin[1], fechaFin[0]);
        if (fechafinal <= fechainicio) {
            return false;
        };
        return true;
    });

    //Este método vincula el complemento de validación discreta jQuery con el método "telefonocelular", y muestra el mensaje de error del lado del cliente
    jQuery.validator.unobtrusive.adapters.add('fimenorff', ["fechainicio"], function (options) {

        options.rules['fimenorff'] = options.params.fechainicio;
        options.messages['fimenorff'] = options.message;
    });



    //VALIDACION DE LADO DEL CLIENTE PARA LA VALIDACION DE LOS CAMPOR TELEFONO Y CELULAR
    //se agrega este metodo para la validacion discreta, recibe el valor del control y el parametro(id del control a comparar)

    jQuery.validator.addMethod('requiredif', function (value, element, params) {
    //    alert($("select[name$='" + params.nombrecampo + "']").val());
        var valueCampoComparar = $("select[name$='" + params.nombrecampo + "']").val();
        if ((valueCampoComparar && params.valuecampo) || (!valueCampoComparar && !params.valuecampo) )
        {
            if (value == null || value == "")
            {
                return false;
            };

        };
        return true;
    });

    //Este método vincula el complemento de validación discreta jQuery con el método "telefonocelular", y muestra el mensaje de error del lado del cliente
    jQuery.validator.unobtrusive.adapters.add('requiredif', ["nombrecampo", "valuecampo"], function (options) {

        options.rules['requiredif'] = {
            nombrecampo: options.params.nombrecampo,
            valuecampo: options.params.valuecampo
        };
        options.messages['requiredif'] = options.message;
    });


}(jQuery));