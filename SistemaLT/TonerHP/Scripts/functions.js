function CargarRubros(urlrubros) {
    $.ajax({
        url: urlrubros,
        type: "GET",
        data: null,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $("#cborubro").empty();
            $("#cbostockactual").empty();
            $("<option>").attr({ "value": "" }).text("Seleccione un Rubro").appendTo("#cborubro"); 
            $.each(data.data, function (index, valor) {
                if (valor.Activo === true) {
                    $("<option>").attr({ "value": valor.IdRubro }).text(valor.Rubro).appendTo("#cborubro");
                }
            });
            $("#cbotipo").empty(); 


        },
        error: function (error) {
            console.log(error)
        },

    });
}

 
function CargarTipos(idRubro) {

    var baseUrl;

    if (isDevelopment) {
        baseUrl = 'https://localhost:44347/Toner'; // URL de desarrollo
    } else {
        baseUrl = 'http://10.4.50.13/SistemaLT/TonerHP/Toner'; // URL de producción
    }

    var urltipos = baseUrl + '/ListarTiposPorRubro?idRubro=' + idRubro;



    //var port = window.location.port;
    //var urltipos = window.location.protocol + "//" + window.location.hostname + (port ? ":" + port : "") + '/Toner/ListarTiposPorRubro?idRubro=' + idRubro;
    //var urltipos = 'https://10.4.50.13/SistemaLT/TonerHP/Toner/ListarTiposPorRubro?idRubro=' + idRubro;
    $.ajax({
        url: urltipos,
        type: "GET",
        data: null,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $("#cbotipo").empty();
            $("<option>").attr({ "value": 0 }).text("Seleccionar Tipo").appendTo("#cbotipo");
            $.each(data, function (index, valor) {
                if (valor.Activo === true) {
                    $("<option>").attr({ "value": valor.IdTipo }).text(valor.Tipo).appendTo("#cbotipo");
                }
            });
            $("#cbodetalle").empty(); 
            $("<option>").attr({ "value": 0}).text("Seleccionar producto").appendTo("#cbodetalle");
        },
        error: function (error) {
            console.log(error)
        },
    });
}



function CargarProductosporTipo(idTipo) {
    var port = window.location.port;
    var urlproductos = window.location.protocol + "//" + window.location.hostname + (port ? ":" + port : "") + '/SistemaLT/TonerHP/Toner/ListarProductosPorTipo?idTipo=' + idTipo;
    $.ajax({
        url: urlproductos,
        type: "GET",
        data: null,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $("#cbodetalle").empty();
            $("<option>").attr({ "value": 0 }).text("Seleccionar producto").appendTo("#cbodetalle");
            $.each(data, function (index, valor) {
                if (valor.Activo === true) {
                    productos[valor.IdProducto] = valor.StockActual;
                    $("<option>").attr({ "value": valor.IdProducto }).text(valor.Detalle).appendTo("#cbodetalle");
                }
            })
        },
        error: function (error) {
            console.log(error)
        },
    });
}


function CargarCodigosID(urlcodigoid) {
    $.ajax({
        url: urlcodigoid,
        type: "GET",
        data: null,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $("#cbocodigo").empty();
            $("#cbostockactual").empty();
            $("<option>").attr({ "value": "" }).text("Seleccione un Codigo ID").appendTo("#cbocodigo"); 

            $.each(data.data, function (index, valor) {
                if (valor.Activo === true && valor.CodigoId) {
                    $("<option>").attr({ "value": valor.IdProducto }).text(valor.CodigoId).appendTo("#cbocodigo");
                }
            });
            $("#cbodetalle").empty();
        },
        error: function (error) {
            console.log(error)
        },

    });
}


function CargarProductosporCI(selectElement) {
    var idCodigo = selectElement.options[selectElement.selectedIndex].text;
    var port = window.location.port;
    var urlproductos = window.location.protocol + "//" + window.location.hostname + (port ? ":" + port : "") + '/SistemaLT/TonerHP/Toner/ListarProductosporCI?idCodigo=' + idCodigo;

    $.ajax({
        url: urlproductos,
        type: "GET",
        data: null,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $("#cbodetalle").empty();
            //$("<option>").attr({ "value": 0 }).text("Seleccionar producto").appendTo("#cbodetalle");

            $.each(data, function (index, valor) {
                if (valor.Activo === true) {
                    productos[valor.IdProducto] = valor.StockActual;
                    $("<option>").attr({ "value": valor.IdProducto }).text(valor.Detalle).appendTo("#cbodetalle");
                }
            })
        },
        error: function (error) {
            console.log(error)
        },
    });
}

function selects2() {
    $('#cborubro').select2({
        placeholder: "Selecciona una opción",
        allowClear: true,
        dropdownParent: $('#FormModal'),
    });
    $('#cbotipo').select2({
        placeholder: "Selecciona una opción",
        allowClear: true,
        dropdownParent: $('#FormModal'),
    });
    $('#cbocodigo').select2({
        placeholder: "Selecciona una opción",
        allowClear: true,
        dropdownParent: $('#FormModal'),
    });
    $('#cbodetalle').select2({
        placeholder: "Selecciona una opción",
        allowClear: true,
        dropdownParent: $('#FormModal'),
    });

}

function LimpiarCampos() {
    $('.modal').on('hidden.bs.modal', function (e) {
        $(this).removeData();
    });
}

function LimpiarColapse() {
    let productoSeleccionado = null;
    $('#collapseExample').on('hide.bs.collapse', function () {
        productoSeleccionado = $('#cbodetalle').val();
    });
    $('#collapseExample').on('hidden.bs.collapse', function () {
        if (productoSeleccionado) {
            $('#cbodetalle').val(productoSeleccionado);
        }
        CargarProductos();
    });
}