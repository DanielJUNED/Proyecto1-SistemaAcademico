// Scripts/manage.js
// Sistema de Gestión de Perfil y Contraseña con AJAX

$(document).ready(function () {
    inicializarManage();
});

// =============================================
// INICIALIZACIÓN
// =============================================
function inicializarManage() {
    // Eventos para cambiar contraseña
    $('#formChangePassword').on('submit', function (e) {
        e.preventDefault();
        cambiarContrasena();
    });

    // Toggle para mostrar/ocultar contraseñas
    $('#toggleCurrentPassword').on('click', function () {
        togglePassword('CurrentPassword', 'toggleIconCurrent');
    });

    $('#toggleNewPassword').on('click', function () {
        togglePassword('NewPassword', 'toggleIconNew');
    });

    $('#toggleConfirmPassword').on('click', function () {
        togglePassword('ConfirmPassword', 'toggleIconConfirm');
    });

    // Validación de fortaleza de contraseña
    $('#NewPassword').on('input', function () {
        validarFortalezaContrasena($(this).val());
    });

    // Validación de coincidencia de contraseñas
    $('#ConfirmPassword').on('input', function () {
        validarCoincidenciaContrasenas();
    });
}

// =============================================
// CAMBIAR CONTRASEÑA
// =============================================
function cambiarContrasena() {
    // Limpiar alertas
    $('#alertContainerPassword').empty();

    // Validar formulario
    if (!validarFormularioContrasena()) {
        return;
    }

    var $btn = $('#btnChangePassword');
    var formData = $('#formChangePassword').serialize();

    // Deshabilitar botón
    $btn.prop('disabled', true)
        .html('<span class="spinner-border spinner-border-sm mr-2"></span>Cambiando...');

    $.ajax({
        url: '/AccountManage/ChangePassword',
        type: 'POST',
        data: formData,
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                mostrarAlerta('alertContainerPassword', 'success', response.message);

                // Limpiar formulario
                $('#formChangePassword')[0].reset();
                $('.is-valid, .is-invalid').removeClass('is-valid is-invalid');
                $('.field-validation').text('');

                // Scroll a la alerta
                $('html, body').animate({
                    scrollTop: $('#alertContainerPassword').offset().top - 20
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
                mostrarAlerta('alertContainerPassword', 'danger', mensajeError);
            }
        },
        error: function () {
            mostrarAlerta('alertContainerPassword', 'danger',
                'Error de conexión. Por favor, intente nuevamente.');
        },
        complete: function () {
            $btn.prop('disabled', false)
                .html('<i class="fas fa-key"></i> Cambiar Contraseña');
        }
    });
}

// =============================================
// VALIDACIONES
// =============================================
function validarFormularioContrasena() {
    var esValido = true;

    // Validar contraseña actual
    var currentPassword = $('#CurrentPassword').val();
    if (!currentPassword) {
        mostrarErrorCampo('CurrentPassword', 'La contraseña actual es requerida');
        esValido = false;
    }

    // Validar nueva contraseña
    var newPassword = $('#NewPassword').val();
    if (!newPassword) {
        mostrarErrorCampo('NewPassword', 'La nueva contraseña es requerida');
        esValido = false;
    } else if (newPassword.length < 8) {
        mostrarErrorCampo('NewPassword', 'La contraseña debe tener al menos 8 caracteres');
        esValido = false;
    } else if (!validarFortalezaContrasena(newPassword)) {
        esValido = false;
    }

    // Validar confirmación de contraseña
    var confirmPassword = $('#ConfirmPassword').val();
    if (!confirmPassword) {
        mostrarErrorCampo('ConfirmPassword', 'Debe confirmar la nueva contraseña');
        esValido = false;
    } else if (newPassword !== confirmPassword) {
        mostrarErrorCampo('ConfirmPassword', 'Las contraseñas no coinciden');
        esValido = false;
    }

    return esValido;
}

function validarFortalezaContrasena(password) {
    var $campo = $('#NewPassword');
    var $mensaje = $('[data-valmsg-for="NewPassword"]');

    // Limpiar estado previo
    $campo.removeClass('is-invalid is-valid');
    $mensaje.text('');

    if (!password) return true; // No validar si está vacío (ya se valida en requerido)

    var tieneMayuscula = /[A-Z]/.test(password);
    var tieneMinuscula = /[a-z]/.test(password);
    var tieneNumero = /\d/.test(password);
    var tieneEspecial = /[!@#$%^&*(),.?":{}|<>]/.test(password);
    var longitudValida = password.length >= 8;

    if (!longitudValida) {
        $campo.addClass('is-invalid');
        $mensaje.text('La contraseña debe tener al menos 8 caracteres');
        return false;
    }

    if (!tieneMayuscula) {
        $campo.addClass('is-invalid');
        $mensaje.text('La contraseña debe contener al menos una letra mayúscula');
        return false;
    }

    if (!tieneMinuscula) {
        $campo.addClass('is-invalid');
        $mensaje.text('La contraseña debe contener al menos una letra minúscula');
        return false;
    }

    if (!tieneNumero) {
        $campo.addClass('is-invalid');
        $mensaje.text('La contraseña debe contener al menos un número');
        return false;
    }

    if (!tieneEspecial) {
        $campo.addClass('is-invalid');
        $mensaje.text('La contraseña debe contener al menos un carácter especial');
        return false;
    }

    $campo.addClass('is-valid');
    return true;
}

function validarCoincidenciaContrasenas() {
    var newPassword = $('#NewPassword').val();
    var confirmPassword = $('#ConfirmPassword').val();
    var $campo = $('#ConfirmPassword');
    var $mensaje = $('[data-valmsg-for="ConfirmPassword"]');

    $campo.removeClass('is-invalid is-valid');
    $mensaje.text('');

    if (!confirmPassword) return;

    if (newPassword !== confirmPassword) {
        $campo.addClass('is-invalid');
        $mensaje.text('Las contraseñas no coinciden');
    } else {
        $campo.addClass('is-valid');
    }
}

function mostrarErrorCampo(nombreCampo, mensaje) {
    var $campo = $('#' + nombreCampo);
    var $mensaje = $('[data-valmsg-for="' + nombreCampo + '"]');

    $campo.addClass('is-invalid');
    $mensaje.text(mensaje);
}

// =============================================
// TOGGLE PASSWORD
// =============================================
function togglePassword(campoId, iconoId) {
    var $campo = $('#' + campoId);
    var $icono = $('#' + iconoId);

    if ($campo.attr('type') === 'password') {
        $campo.attr('type', 'text');
        $icono.removeClass('fa-eye').addClass('fa-eye-slash');
    } else {
        $campo.attr('type', 'password');
        $icono.removeClass('fa-eye-slash').addClass('fa-eye');
    }
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