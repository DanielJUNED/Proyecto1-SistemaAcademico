// Scripts/manage.js
// Sistema de Gestión de Perfil y Contraseña con AJAX

$(document).ready(function () {
    inicializarManage();
});

// =============================================
// INICIALIZACIÓN
// =============================================
function inicializarManage() {
    // Eventos para actualizar perfil
    $('#formUpdateProfile').on('submit', function (e) {
        e.preventDefault();
        actualizarPerfil();
    }); 

    // Validación en tiempo real para nombres
    $('#Nombre, #Apellidos').on('input', function () {
        validarSoloLetras($(this));
    }); 
}

// =============================================
// ACTUALIZAR PERFIL
// =============================================
function actualizarPerfil() {
    // Limpiar alertas
    $('#alertContainer').empty();

    // Validar formulario
    if (!validarFormularioPerfil()) {
        return;
    }

    var $btn = $('#btnUpdateProfile');
    var formData = $('#formUpdateProfile').serialize();

    // Deshabilitar botón
    $btn.prop('disabled', true)
        .html('<span class="spinner-border spinner-border-sm mr-2"></span>Guardando...');

    $.ajax({
        url: '/Manage/UpdateProfile',
        type: 'POST',
        data: formData,
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                mostrarAlerta('alertContainer', 'success', response.message);

                // Actualizar nombre en el menú de usuario
                $('#nombreUsuario').text(response.nombreCompleto);

                // Scroll a la alerta
                $('html, body').animate({
                    scrollTop: $('#alertContainer').offset().top - 20
                }, 300);
            } else {
                var mensajeError = response.message;
                if (response.errors && response.errors.length > 0) {
                    mensajeError += '<ul class="mb-0 mt-2">';
                    response.errors.forEach(function (error) {
                        mensajeError += '<li>' + error + '</li>';
                    });
                    mensajeError += '</ul>';
                }
                mostrarAlerta('alertContainer', 'danger', mensajeError);
            }
        },
        error: function () {
            mostrarAlerta('alertContainer', 'danger',
                'Error de conexión. Por favor, intente nuevamente.');
        },
        complete: function () {
            $btn.prop('disabled', false)
                .html('<i class="fas fa-save"></i> Guardar Cambios');
        }
    });
} 

// =============================================
// VALIDACIONES
// =============================================
function validarFormularioPerfil() {
    var esValido = true;

    // Validar nombre
    var nombre = $('#Nombre').val().trim();
    if (!nombre) {
        mostrarErrorCampo('Nombre', 'El nombre es requerido');
        esValido = false;
    } else if (nombre.length < 2) {
        mostrarErrorCampo('Nombre', 'El nombre debe tener al menos 2 caracteres');
        esValido = false;
    } else if (!/^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$/.test(nombre)) {
        mostrarErrorCampo('Nombre', 'El nombre solo debe contener letras');
        esValido = false;
    }

    // Validar apellidos
    var apellidos = $('#Apellidos').val().trim();
    if (!apellidos) {
        mostrarErrorCampo('Apellidos', 'Los apellidos son requeridos');
        esValido = false;
    } else if (apellidos.length < 2) {
        mostrarErrorCampo('Apellidos', 'Los apellidos deben tener al menos 2 caracteres');
        esValido = false;
    } else if (!/^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$/.test(apellidos)) {
        mostrarErrorCampo('Apellidos', 'Los apellidos solo deben contener letras');
        esValido = false;
    }

    return esValido;
} 

function validarSoloLetras($campo) {
    var valor = $campo.val();
    var valorLimpio = valor.replace(/[^a-zA-ZáéíóúÁÉÍÓÚñÑ\s]/g, '');

    if (valor !== valorLimpio) {
        $campo.val(valorLimpio);
    }
} 

function mostrarErrorCampo(nombreCampo, mensaje) {
    var $campo = $('#' + nombreCampo);
    var $mensaje = $('[data-valmsg-for="' + nombreCampo + '"]');

    $campo.addClass('is-invalid');
    $mensaje.text(mensaje);
} 

// =============================================
// ALERTAS
// =============================================
function mostrarAlerta(contenedor, tipo, mensaje) {
    var iconos = {
        success: 'fa-check-circle',
        danger: 'fa-times-circle',
        warning: 'fa-exclamation-triangle',
        info: 'fa-info-circle'
    };

    var html = `
        <div class="alert alert-${tipo} alert-dismissible fade show" role="alert">
            <i class="fas ${iconos[tipo]} mr-2"></i>
            ${mensaje}
            <button type="button" class="close" data-dismiss="alert">
                <span>&times;</span>
            </button>
        </div>
    `;

    $('#' + contenedor).html(html);

    // Auto-ocultar después de 5 segundos para success
    if (tipo === 'success') {
        setTimeout(function () {
            $('#' + contenedor + ' .alert').fadeOut('slow', function () {
                $(this).remove();
            });
        }, 5000);
    }
}