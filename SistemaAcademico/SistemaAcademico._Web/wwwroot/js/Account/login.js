// Scripts/login.js
// Sistema de Autenticación con AJAX - ASP.NET MVC 5

$(document).ready(function () {
    inicializarLogin();
});

// =============================================
// INICIALIZACIÓN
// =============================================
function inicializarLogin() {
    // Evento de submit del formulario
    $('#loginForm').on('submit', function (e) {
        e.preventDefault();
        realizarLogin();
    });

    // Toggle para mostrar/ocultar contraseña
    $('#togglePassword').on('click', function () {
        togglePasswordVisibility();
    });

    // Validación en tiempo real
    $('#UserName, #Password').on('blur', function () {
        validarCampo($(this));
    });

    // Limpiar alertas al escribir
    $('#UserName, #Password').on('input', function () {
        limpiarAlertas();
    });

    // Prevenir submit al presionar Enter en campos
    $('#UserName, #Password').on('keypress', function (e) {
        if (e.which === 13) {
            e.preventDefault();
            $('#loginForm').submit();
        }
    });

    // Auto-focus en UserName
    $('#UserName').focus();
}

// =============================================
// REALIZAR LOGIN CON AJAX
// =============================================
function realizarLogin() {
    // Limpiar mensajes previos
    limpiarAlertas();

    // Validar formulario
    if (!validarFormulario()) {
        return;
    }

    // Obtener datos del formulario
    var userName = $('#UserName').val().trim();
    var password = $('#Password').val();
    var rememberMe = $('#RememberMe').is(':checked');
    var returnUrl = $('#returnUrl').val();

    // Obtener el token antiforgery
    var token = $('input[name="__RequestVerificationToken"]').val();

    // Deshabilitar botón y mostrar loading
    var $btnLogin = $('#btnLogin');
    $btnLogin.prop('disabled', true)
        .html('<span class="spinner-border spinner-border-sm mr-2"></span>Ingresando...');

    // Petición AJAX
    $.ajax({
        url: '/Account/Login',
        type: 'POST',
        data: {
            __RequestVerificationToken: token,
            UserName: userName,
            Password: password,
            RememberMe: rememberMe,
            returnUrl: returnUrl
        },
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                // Login exitoso
                mostrarAlerta('success', response.message);

                // Esperar 1 segundo antes de redirigir
                setTimeout(function () {
                    window.location.href = response.redirectUrl;
                }, 1000);
            } else {
                // Login fallido
                if (response.isLockedOut) {
                    mostrarAlerta('danger', response.message, true);
                } else if (response.remainingAttempts !== null) {
                    var tipo = response.remainingAttempts <= 2 ? 'danger' : 'warning';
                    mostrarAlerta(tipo, response.message);
                } else {
                    mostrarAlerta('danger', response.message);
                }

                // Rehabilitar botón
                $btnLogin.prop('disabled', false)
                    .html('<i class="fas fa-sign-in-alt"></i> Ingresar al Sistema');

                // Limpiar contraseña por seguridad
                $('#Password').val('').focus();
            }
        },
        error: function (xhr, status, error) {
            console.error('Error AJAX:', error);
            mostrarAlerta('danger',
                'Error de conexión. Verifique su conexión a internet e intente nuevamente.');

            // Rehabilitar botón
            $btnLogin.prop('disabled', false)
                .html('<i class="fas fa-sign-in-alt"></i> Ingresar al Sistema');
        }
    });
}

// =============================================
// VALIDACIONES
// =============================================
function validarFormulario() {
    var esValido = true;

    // Validar UserName
    var $userName = $('#UserName');
    if (!validarCampo($userName)) {
        esValido = false;
    }

    // Validar password
    var $password = $('#Password');
    if (!validarCampo($password)) {
        esValido = false;
    }

    return esValido;
}

function validarCampo($campo) {
    var valor = $campo.val() ? $campo.val().trim() : '';
    var nombre = $campo.attr('name');
    var $mensajeValidacion = $campo.closest('.form-group').find('.text-danger');

    // Limpiar estado previo
    $campo.removeClass('is-invalid is-valid');
    $mensajeValidacion.text('');

    // Validar según el campo
    if (nombre === 'UserName') {
        if (!valor) {
            mostrarErrorCampo($campo, $mensajeValidacion, 'El Usuario es requerido');
            return false;
        }

        // Validar formato de UserName
        /*var regexUserName = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!regexEmail.test(valor)) {
            mostrarErrorCampo($campo, $mensajeValidacion, 'Formato de correo inválido');
            return false;
        }*/
    } else if (nombre === 'Password') {
        if (!valor) {
            mostrarErrorCampo($campo, $mensajeValidacion, 'La contraseña es requerida');
            return false;
        }

        if (valor.length < 8) {
            mostrarErrorCampo($campo, $mensajeValidacion, 'La contraseña debe tener al menos 8 caracteres');
            return false;
        }
    }

    // Campo válido
    $campo.addClass('is-valid');
    return true;
}

function mostrarErrorCampo($campo, $mensajeValidacion, mensaje) {
    $campo.addClass('is-invalid');
    $mensajeValidacion.text(mensaje);
}

// =============================================
// MOSTRAR/OCULTAR CONTRASEÑA
// =============================================
function togglePasswordVisibility() {
    var $password = $('#Password');
    var $icon = $('#toggleIcon');

    if ($password.attr('type') === 'password') {
        $password.attr('type', 'text');
        $icon.removeClass('fa-eye').addClass('fa-eye-slash');
    } else {
        $password.attr('type', 'password');
        $icon.removeClass('fa-eye-slash').addClass('fa-eye');
    }
}

// =============================================
// ALERTAS
// =============================================
function mostrarAlerta(tipo, mensaje, permanente) {
    var iconos = {
        success: 'fa-check-circle',
        danger: 'fa-times-circle',
        warning: 'fa-exclamation-triangle',
        info: 'fa-info-circle'
    };

    var html = `
        <div class="alert alert-${tipo} alert-dismissible fade show" role="alert">
            <i class="fas ${iconos[tipo]} mr-2"></i>
            <strong>${mensaje}</strong>
            <button type="button" class="close" data-dismiss="alert">
                <span>&times;</span>
            </button>
        </div>
    `;

    $('#alertContainer').html(html);

    // Scroll a la alerta
    $('html, body').animate({
        scrollTop: $('#alertContainer').offset().top - 20
    }, 300);

    // Auto-ocultar después de 5 segundos (solo para success e info)
    if (!permanente && (tipo === 'success' || tipo === 'info')) {
        setTimeout(function () {
            $('.alert').fadeOut('slow', function () {
                $(this).remove();
            });
        }, 5000);
    }
}

function limpiarAlertas() {
    $('#alertContainer').empty();
    $('.is-invalid').removeClass('is-invalid');
    $('.text-danger').text('');
}

// =============================================
// PROTECCIÓN CONTRA ATAQUES
// =============================================

// Prevenir ataques de fuerza bruta con delay progresivo
var intentosFallidos = 0;
var ultimoIntento = 0;

function aplicarDelaySeguridad() {
    var ahora = Date.now();
    var tiempoTranscurrido = ahora - ultimoIntento;

    // Aplicar delay progresivo: 2^intentos segundos
    var delayRequerido = Math.pow(2, intentosFallidos) * 1000;

    if (tiempoTranscurrido < delayRequerido) {
        var segundosRestantes = Math.ceil((delayRequerido - tiempoTranscurrido) / 1000);
        mostrarAlerta('warning',
            `Por favor espere ${segundosRestantes} segundos antes de intentar nuevamente.`);
        return false;
    }

    ultimoIntento = ahora;
    return true;
}

// Incrementar contador de intentos fallidos
$(document).ajaxSuccess(function (event, xhr, settings) {
    if (settings.url.indexOf('/Account/Login') !== -1) {
        var response = JSON.parse(xhr.responseText);
        if (!response.Success) {
            intentosFallidos++;
        } else {
            intentosFallidos = 0;
        }
    }
});

// Prevenir copiar/pegar en campo de contraseña (opcional, para mayor seguridad)
$('#Password').on('paste', function (e) {
    // Descomentar la siguiente línea para deshabilitar paste
    // e.preventDefault();
    // mostrarAlerta('info', 'Por seguridad, no puede pegar la contraseña.');
});

// =============================================
// DETECCIÓN DE CAPS LOCK
// =============================================
$('#Password').on('keypress', function (e) {
    var charCode = e.which || e.keyCode;
    var shiftKey = e.shiftKey || false;

    // Detectar si Caps Lock está activado
    if ((charCode >= 65 && charCode <= 90 && !shiftKey) ||
        (charCode >= 97 && charCode <= 122 && shiftKey)) {

        if (!$('#capsLockWarning').length) {
            $(this).closest('.form-group').append(
                '<small id="capsLockWarning" class="text-warning">' +
                '<i class="fas fa-exclamation-triangle"></i> ' +
                'Advertencia: Bloqueo de mayúsculas activado' +
                '</small>'
            );
        }
    } else {
        $('#capsLockWarning').remove();
    }
});

// =============================================
// PROTECCIÓN XSS
// =============================================
function sanitizeInput(input) {
    var div = document.createElement('div');
    div.textContent = input;
    return div.innerHTML;
}

// Sanitizar inputs antes de enviar
$('#loginForm').on('submit', function () {
    $('#UserName').val(sanitizeInput($('#UserName').val()));
});

// =============================================
// REGISTRO DE EVENTOS PARA AUDITORÍA
// =============================================
function registrarEvento(tipo, descripcion) {
    // Aquí podrías enviar eventos al servidor para auditoría
    console.log(`[${new Date().toISOString()}] ${tipo}: ${descripcion}`);
}

// Registrar intento de login
$('#loginForm').on('submit', function () {
    registrarEvento('LOGIN_ATTEMPT', 'Usuario intentando iniciar sesión');
});

// =============================================
// ACCESIBILIDAD
// =============================================

// Mejorar navegación por teclado
$(document).on('keydown', function (e) {
    // ESC para limpiar formulario
    if (e.key === 'Escape') {
        limpiarFormulario();
    }
});

function limpiarFormulario() {
    $('#loginForm')[0].reset();
    limpiarAlertas();
    $('#UserName').focus();
}