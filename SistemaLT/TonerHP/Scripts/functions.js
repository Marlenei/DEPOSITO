
function cargarApiUrls() {
    $.ajax({
        url: '/Toner/GetApiUrls', 
        type: "GET",
        dataType: "json",
        success: function (data) {
            appSettings.ApiUrlDev = data.ApiUrlDev;
            appSettings.ApiUrlProd = data.ApiUrlProd;
            console.log(appSettings.ApiUrlDev);
            console.log(appSettings.ApiUrlProd);

        },
        error: function (error) {
            console.error("Error al cargar las URLs de la API:", error);
        }
    });
}
function CargarRubros(urlrubros, permisos) {
    $.ajax({
        url: urlrubros,
        type: "GET",
        data: null,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $("#cbostockactual").empty();
            $("#cborubro").empty();
            var opciones = [];
            var notienetoner = permisos.includes(24) || permisos.includes(25);
            var tienetoner = permisos.includes(184) || permisos.includes(183);
            $.each(data.data, function (index, valor) {
                if (valor.Activo === true) {
                    if (notienetoner && valor.Rubro !== "Insumos Informaticos") {
                        opciones.push({
                            id: valor.IdRubro,
                            text: valor.Rubro
                        });
                    } else if (tienetoner && valor.Rubro === "Insumos Informaticos") {
                        opciones.push({
                            id: valor.IdRubro,
                            text: valor.Rubro
                        });
                    }
                }
            });
            $('#cborubro').select2({
                placeholder: "Selecciona una opción",
                data: opciones,
                allowClear: true,
                dropdownParent: $('#FormModal'),
            }).val(null).trigger('change'); 
            $("#cbotipo").empty(); 
            $("#cbodetalle").empty(); 

        },
        error: function (error) {
            console.log(error)
        },

    });
}



function CargarTipos(idRubro) {
    var isDevelopment = window.location.hostname === "localhost";
    var baseUrl = isDevelopment ? appSettings.ApiUrlDev : appSettings.ApiUrlProd;
    var urltipos = baseUrl + '/ListarTiposPorRubro?idRubro=' + idRubro;
    if (!idRubro) {
        return;
    }

    $.ajax({
        url: urltipos,
        type: "GET",
        data: null,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $("#cbotipo").empty();
            var opciones = [];
            $.each(data, function (index, valor) {
                if (valor.Activo === true) {
                    opciones.push({
                        id: valor.IdTipo,
                        text: valor.Tipo
                    });
                }
            });
            $('#cbotipo').select2({
                placeholder: "Selecciona una opción",
                data: opciones,
                allowClear: true,
                dropdownParent: $('#FormModal'),
            }).val(null).trigger('change'); 
            $("#cbodetalle").empty();

        },
        error: function (error) {
            console.log(error)
        },
    });
}



function CargarProductosporTipo(idTipo) {
    if (!idTipo) {
        return;
    }
    var isDevelopment = window.location.hostname === "localhost";
    var baseUrl = isDevelopment ? appSettings.ApiUrlDev : appSettings.ApiUrlProd;
    var urlproductos = baseUrl + '/ListarProductosPorTipo?idTipo=' + idTipo;
    $.ajax({
        url: urlproductos,
        type: "GET",
        data: null,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $("#cbodetalle").empty();
            var opciones = [];
            $.each(data, function (index, valor) {
                if (valor.Activo === true) {
                    opciones.push({
                        id: valor.IdProducto,
                        text: valor.Detalle
                    });
                }
            });
            $('#cbodetalle').select2({
                placeholder: "Selecciona una opción",
                data: opciones,
                allowClear: true,
                dropdownParent: $('#FormModal'),
            }).val(null).trigger('change'); 
        },
        error: function (error) {
            console.log(error)
        },
    });
}


function CargarCodigosID(urlcodigoid, permisos) {
    $.ajax({
        url: urlcodigoid,
        type: "GET",
        data: null,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $("#cbostockactual").empty();
            //$("#cbocodigo").empty();
            var opciones = [];
            var notienetoner = permisos.includes(24) || permisos.includes(25);
            var tienetoner = permisos.includes(184) || permisos.includes(183);
            $.each(data.data, function (index, valor) {
                if (valor.Activo === true && valor.CodigoId) {
                    if (notienetoner && valor.oRubros.Rubro !== "Insumos Informaticos") {
                        opciones.push({
                            id: valor.IdProducto,
                            text: valor.CodigoId
                        });
                    } else if (tienetoner && valor.oRubros.Rubro === "Insumos Informaticos") {
                        opciones.push({
                            id: valor.IdProducto,
                            text: valor.CodigoId
                        });
                    }
                }
            });
            $('#cbocodigo').select2({
                placeholder: "Selecciona una opción",
                data: opciones,
                allowClear: true,
                dropdownParent: $('#FormModal'),
            }).val(null).trigger('change'); 

            $("#cbodetalle").empty();
        },
        error: function (error) {
            console.log(error)
        },

    });
}


function CargarProductosporCI(selectElement) {
    var idCodigo = selectElement.options[selectElement.selectedIndex].text;
    var isDevelopment = window.location.hostname === "localhost";
    var baseUrl = isDevelopment ? appSettings.ApiUrlDev : appSettings.ApiUrlProd;
    var urlproductos = baseUrl + '/ListarProductosporCI?idCodigo=' + idCodigo;
    $.ajax({
        url: urlproductos,
        type: "GET",
        data: null,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $("#cbodetalle").empty();
            let opciones = [];
            $.each(data, function (index, valor) {
                if (valor.Activo === true) {
                    opciones.push({
                        id: valor.IdProducto,
                        text: valor.Detalle
                    });
                }
            });
            $('#cbodetalle').select2({
                placeholder: "Selecciona una opción",
                data: opciones,
                allowClear: true,
                dropdownParent: $('#FormModal'),
            }).val(null).trigger('change'); 
        },
        error: function (error) {
            console.log(error)
        },
    });
}

function selects2() {
    $('#cborubro').select2({
        placeholder: "Seleccionar una opción",
        allowClear: true,
        dropdownParent: $('#FormModal'),
    });
    $('#cbotipo').select2({
        placeholder: "Seleccionar un rubro",
        allowClear: true,
        dropdownParent: $('#FormModal'),
    });
    $('#cbocodigo').select2({
        placeholder: "Seleccionar una opción",
        allowClear: true,
        dropdownParent: $('#FormModal'),
    });
    $('#cbodetalle').select2({
        placeholder: "Seleccionar una opción",
        allowClear: true,
        dropdownParent: $('#FormModal'),
    });
}
function LimpiarCampos() {
    $('.modal').on('hidden.bs.modal', function (e) {
        $(this).find('input, select, textarea').val(''); 
        $(this).find('select').val('').trigger('change');
        $(this).removeData();
    });
}