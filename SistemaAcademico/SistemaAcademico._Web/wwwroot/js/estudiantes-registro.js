// wwwroot/js/estudiantes-registro.js
// Sistema de Registro de Estudiantes con AJAX

// Variables globales
let cursosSeleccionados = [];
let timeoutIdentificacion;
let timeoutEmail;

// Inicialización al cargar el documento
$(document).ready(function () {
    inicializarEventos();
    configurarValidaciones();
});

// ============================================
// INICIALIZACIÓN DE EVENTOS
// ============================================
function inicializarEventos() {
    // Eventos de cambio en dropdowns de ubicación
    $('#provinciaId').change(function () {
        cargarCantones($(this).val());
    });

    $('#cantonId').change(function () {
        cargarDistritos($(this).val());
    });

    // Evento de cambio en cuatrimestre
    $('#cuatrimestreId').change(function () {
        cargarCursos($(this).val());
    });

    // Validación en tiempo real de identificación
    $('#identificacion').on('input', function () {
        validarIdentificacionTiempoReal($(this).val());
    });

    // Validación en tiempo real de email
    $('#email').on('input', function () {
        validarEmailTiempoReal($(this).val());
    });

    // Submit del formulario
    $('#formRegistroEstudiante').submit(function (e) {
        e.preventDefault();
        registrarEstudiante();
    });

    // Validación de fecha de nacimiento
    $('#fechaNacimiento').change(function () {
        validarEdad($(this).val());
    });
}

// ============================================
// VALIDACIONES DEL CLIENTE
// ============================================
function configurarValidaciones() {
    // Validación: Solo números en identificación
    $('#identificacion').on('keypress', function (e) {
        if (!/^\d$/.test(String.fromCharCode(e.which))) {
            e.preventDefault();
        }
    });

    // Validación: Solo letras en nombre y apellidos
    $('#nombre, #apellidos').on('keypress', function (e) {
        const char = String.fromCharCode(e.which);
        if (!/^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]$/.test(char)) {
            e.preventDefault();
        }
    });

    // Establecer fecha máxima (15 años atrás)
    const fechaMaxima = new Date();
    fechaMaxima.setFullYear(fechaMaxima.getFullYear() - 15);
    $('#fechaNacimiento').attr('max', fechaMaxima.toISOString().split('T')[0]);
}

// Validación de identificación en tiempo real
function validarIdentificacionTiempoReal(identificacion) {
    clearTimeout(timeoutIdentificacion);

    const $campo = $('#identificacion');
    const $validacion = $('[data-valmsg-for="Identificacion"]');

    // Limpiar validación previa
    $validacion.text('');
    $campo.removeClass('is-invalid is-valid');

    if (!identificacion || identificacion.length < 9) {
        return;
    }

    // Esperar 500ms después de que el usuario deje de escribir
    timeoutIdentificacion = setTimeout(function () {
        $.ajax({
            url: '/Estudiante/VerificarIdentificacion',
            type: 'GET',
            data: { identificacion: identificacion },
            success: function (response) {
                if (response.existe) {
                    $campo.addClass('is-invalid');
                    $validacion.text('Esta identificación ya está registrada');
                } else {
                    $campo.addClass('is-valid');
                }
            },
            error: function () {
                console.error('Error al verificar identificación');
            }
        });
    }, 500);
}

// Validación de email en tiempo real
function validarEmailTiempoReal(email) {
    clearTimeout(timeoutEmail);

    const $campo = $('#email');
    const $validacion = $('[data-valmsg-for="Email"]');

    // Limpiar validación previa
    $validacion.text('');
    $campo.removeClass('is-invalid is-valid');

    // Validar formato de email
    const regexEmail = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!email || !regexEmail.test(email)) {
        return;
    }

    // Esperar 500ms después de que el usuario deje de escribir
    timeoutEmail = setTimeout(function () {
        $.ajax({
            url: '/Estudiante/VerificarEmail',
            type: 'GET',
            data: { email: email },
            success: function (response) {
                if (response.existe) {
                    $campo.addClass('is-invalid');
                    $validacion.text('Este correo electrónico ya está registrado');
                } else {
                    $campo.addClass('is-valid');
                }
            },
            error: function () {
                console.error('Error al verificar email');
            }
        });
    }, 500);
}

// Validar edad (mínimo 15 años)
function validarEdad(fechaNacimiento) {
    const $campo = $('#fechaNacimiento');
    const $validacion = $('[data-valmsg-for="FechaNacimiento"]');

    $validacion.text('');
    $campo.removeClass('is-invalid is-valid');

    if (!fechaNacimiento) return;

    const hoy = new Date();
    const nacimiento = new Date(fechaNacimiento);
    let edad = hoy.getFullYear() - nacimiento.getFullYear();
    const mes = hoy.getMonth() - nacimiento.getMonth();

    if (mes < 0 || (mes === 0 && hoy.getDate() < nacimiento.getDate())) {
        edad--;
    }

    if (edad < 15) {
        $campo.addClass('is-invalid');
        $validacion.text('El estudiante debe tener al menos 15 años');
    } else {
        $campo.addClass('is-valid');
    }
}

// ============================================
// CARGA DINÁMICA DE DATOS
// ============================================

// Cargar cantones por provincia
function cargarCantones(provinciaId) {
    const $canton = $('#cantonId');
    const $distrito = $('#distritoId');

    // Resetear cantón y distrito
    $canton.html('<option value="">-- Cargando... --</option>').prop('disabled', true);
    $distrito.html('<option value="">-- Seleccione cantón primero --</option>').prop('disabled', true);

    if (!provinciaId) {
        $canton.html('<option value="">-- Seleccione provincia primero --</option>');
        return;
    }

    $.ajax({
        url: '/Estudiante/ObtenerCantones',
        type: 'GET',
        data: { provinciaId: provinciaId },
        success: function (cantones) {
            let options = '<option value="">-- Seleccione --</option>';
            cantones.forEach(function (canton) {
                options += `<option value="${canton.value}">${canton.text}</option>`;
            });
            $canton.html(options).prop('disabled', false);
        },
        error: function () {
            mostrarAlerta('Error al cargar los cantones', 'danger');
            $canton.html('<option value="">-- Error al cargar --</option>');
        }
    });
}

// Cargar distritos por cantón
function cargarDistritos(cantonId) {
    const $distrito = $('#distritoId');

    // Resetear distrito
    $distrito.html('<option value="">-- Cargando... --</option>').prop('disabled', true);

    if (!cantonId) {
        $distrito.html('<option value="">-- Seleccione cantón primero --</option>');
        return;
    }

    $.ajax({
        url: '/Estudiante/ObtenerDistritos',
        type: 'GET',
        data: { cantonId: cantonId },
        success: function (distritos) {
            let options = '<option value="">-- Seleccione --</option>';
            distritos.forEach(function (distrito) {
                options += `<option value="${distrito.value}">${distrito.text}</option>`;
            });
            $distrito.html(options).prop('disabled', false);
        },
        error: function () {
            mostrarAlerta('Error al cargar los distritos', 'danger');
            $distrito.html('<option value="">-- Error al cargar --</option>');
        }
    });
}

// Cargar cursos por cuatrimestre
function cargarCursos(cuatrimestreId) {
    const $container = $('#cursosContainer');

    // Limpiar selección previa
    cursosSeleccionados = [];

    if (!cuatrimestreId) {
        $container.html(`
            <p class="text-muted text-center">
                <i class="fas fa-info-circle"></i> 
                Seleccione un cuatrimestre para ver los cursos disponibles
            </p>
        `);
        return;
    }

    // Mostrar loading
    $container.html(`
        <div class="text-center">
            <div class="spinner-border text-primary" role="status">
                <span class="sr-only">Cargando...</span>
            </div>
            <p class="mt-2">Cargando cursos disponibles...</p>
        </div>
    `);

    $.ajax({
        url: '/Estudiante/ObtenerCursos',
        type: 'GET',
        data: { cuatrimestreId: cuatrimestreId },
        success: function (cursos) {
            if (cursos.length === 0) {
                $container.html(`
                    <p class="text-warning text-center">
                        <i class="fas fa-exclamation-triangle"></i> 
                        No hay cursos disponibles para este cuatrimestre
                    </p>
                `);
                return;
            }

            let html = '<div class="row">';
            cursos.forEach(function (curso) {
                html += `
                    <div class="col-md-6">
                        <div class="curso-checkbox">
                            <div class="custom-control custom-checkbox">
                                <input type="checkbox" 
                                       class="custom-control-input curso-check" 
                                       id="curso_${curso.value}" 
                                       value="${curso.value}"
                                       data-codigo="${curso.codigo}"
                                       data-nombre="${curso.nombre}"
                                       onchange="actualizarCursosSeleccionados()">
                                <label class="custom-control-label" for="curso_${curso.value}">
                                    <strong>${curso.codigo}</strong> - ${curso.nombre}
                                    <br>
                                    <small class="text-muted">Créditos: ${curso.creditos}</small>
                                </label>
                            </div>
                        </div>
                    </div>
                `;
            });
            html += '</div>';

            $container.html(html);
        },
        error: function () {
            mostrarAlerta('Error al cargar los cursos', 'danger');
            $container.html(`
                <p class="text-danger text-center">
                    <i class="fas fa-times-circle"></i> 
                    Error al cargar los cursos. Intente nuevamente.
                </p>
            `);
        }
    });
}

// Actualizar array de cursos seleccionados
function actualizarCursosSeleccionados() {
    cursosSeleccionados = [];
    $('.curso-check:checked').each(function () {
        cursosSeleccionados.push(parseInt($(this).val()));
    });

    // Limpiar validación si hay cursos seleccionados
    if (cursosSeleccionados.length > 0) {
        $('[data-valmsg-for="CursosSeleccionados"]').text('');
    }
}

// ============================================
// REGISTRO DE ESTUDIANTE
// ============================================
function registrarEstudiante() {
    // Validar formulario antes de enviar
    if (!validarFormulario()) {
        return;
    }

    // Deshabilitar botón y mostrar loading
    const $btnRegistrar = $('#btnRegistrar');
    $btnRegistrar.prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Registrando...');

    // Obtener el token anti-forgery
    const token = $('input[name="__RequestVerificationToken"]').val();

    // Preparar datos
    const datos = {
        __RequestVerificationToken: token,
        identificacion: $('#identificacion').val().trim(),
        nombre: $('#nombre').val().trim(),
        apellidos: $('#apellidos').val().trim(),
        fechaNacimiento: $('#fechaNacimiento').val(),
        email: $('#email').val().trim().toLowerCase(),
        provinciaID: parseInt($('#provinciaId').val()),
        cantonID: parseInt($('#cantonId').val()),
        distritoID: parseInt($('#distritoId').val()),
        cuatrimestreID: parseInt($('#cuatrimestreId').val()),
        cursosSeleccionados: cursosSeleccionados
    };

    // Enviar por AJAX
    $.ajax({
        url: '/Estudiante/Registrar',
        type: 'POST',
        //contentType: 'application/json',
        //headers: {
        //    'RequestVerificationToken': token
        //},
        data: datos,//JSON.stringify(datos),
        success: function (response) {
            if (response.exitoso) {
                // Mostrar modal de éxito
                $('#mensajeExito').html(`
                    <p><strong>¡Estudiante registrado exitosamente!</strong></p>
                    <p>Identificación: <strong>${datos.identificacion}</strong></p>
                    <p>Nombre: <strong>${datos.nombre} ${datos.apellidos}</strong></p>
                    <p>Cursos matriculados: <strong>${cursosSeleccionados.length}</strong></p>
                `);
                $('#modalExito').modal('show');
            } else {
                // Mostrar errores
                let mensajeError = response.mensaje;
                if (response.errores && response.errores.length > 0) {
                    mensajeError += '<ul class="mt-2">';
                    response.Errores.forEach(function (error) {
                        mensajeError += `<li>${error}</li>`;
                    });
                    mensajeError += '</ul>';
                }
                mostrarAlerta(mensajeError, 'danger');
            }
        },
        error: function (xhr, status, error) {
            console.error('Error en la petición:', error);
            mostrarAlerta('Error de conexión. Por favor, intente nuevamente.', 'danger');
        },
        complete: function () {
            // Rehabilitar botón
            $btnRegistrar.prop('disabled', false).html('<i class="fas fa-save"></i> Registrar Estudiante');
        }
    });
}

// ============================================
// VALIDACIÓN DEL FORMULARIO
// ============================================
function validarFormulario() {
    let esValido = true;
    limpiarValidaciones();

    // Validar identificación
    const identificacion = $('#identificacion').val().trim();
    if (!identificacion) {
        mostrarError('Identificacion', 'La identificación es requerida');
        esValido = false;
    } else if (!/^\d+$/.test(identificacion)) {
        mostrarError('Identificacion', 'La identificación solo debe contener números');
        esValido = false;
    } else if (identificacion.length < 9) {
        mostrarError('Identificacion', 'La identificación debe tener al menos 9 dígitos');
        esValido = false;
    }

    // Validar nombre
    const nombre = $('#nombre').val().trim();
    if (!nombre) {
        mostrarError('Nombre', 'El nombre es requerido');
        esValido = false;
    } else if (nombre.length < 2) {
        mostrarError('Nombre', 'El nombre debe tener al menos 2 caracteres');
        esValido = false;
    } else if (!/^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$/.test(nombre)) {
        mostrarError('Nombre', 'El nombre solo debe contener letras');
        esValido = false;
    }

    // Validar apellidos
    const apellidos = $('#apellidos').val().trim();
    if (!apellidos) {
        mostrarError('Apellidos', 'Los apellidos son requeridos');
        esValido = false;
    } else if (apellidos.length < 2) {
        mostrarError('Apellidos', 'Los apellidos deben tener al menos 2 caracteres');
        esValido = false;
    } else if (!/^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$/.test(apellidos)) {
        mostrarError('Apellidos', 'Los apellidos solo deben contener letras');
        esValido = false;
    }

    // Validar fecha de nacimiento
    const fechaNacimiento = $('#fechaNacimiento').val();
    if (!fechaNacimiento) {
        mostrarError('FechaNacimiento', 'La fecha de nacimiento es requerida');
        esValido = false;
    } else {
        const hoy = new Date();
        const nacimiento = new Date(fechaNacimiento);
        let edad = hoy.getFullYear() - nacimiento.getFullYear();
        const mes = hoy.getMonth() - nacimiento.getMonth();

        if (mes < 0 || (mes === 0 && hoy.getDate() < nacimiento.getDate())) {
            edad--;
        }

        if (edad < 15) {
            mostrarError('FechaNacimiento', 'El estudiante debe tener al menos 15 años');
            esValido = false;
        }
    }

    // Validar email
    const email = $('#email').val().trim();
    const regexEmail = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!email) {
        mostrarError('Email', 'El correo electrónico es requerido');
        esValido = false;
    } else if (!regexEmail.test(email)) {
        mostrarError('Email', 'El formato del correo electrónico es inválido');
        esValido = false;
    }

    // Validar provincia
    const provinciaId = $('#provinciaId').val();
    if (!provinciaId) {
        mostrarError('ProvinciaID', 'Debe seleccionar una provincia');
        esValido = false;
    }

    // Validar cantón
    const cantonId = $('#cantonId').val();
    if (!cantonId) {
        mostrarError('CantonID', 'Debe seleccionar un cantón');
        esValido = false;
    }

    // Validar distrito
    const distritoId = $('#distritoId').val();
    if (!distritoId) {
        mostrarError('DistritoID', 'Debe seleccionar un distrito');
        esValido = false;
    }

    // Validar cuatrimestre
    const cuatrimestreId = $('#cuatrimestreId').val();
    if (!cuatrimestreId) {
        mostrarError('CuatrimestreID', 'Debe seleccionar un cuatrimestre');
        esValido = false;
    } 

    // Validar cursos seleccionados
    if (cursosSeleccionados.length === 0) {
        mostrarError('CursosSeleccionados', 'Debe seleccionar al menos un curso');
        esValido = false;
    }


    if (!esValido) {
        mostrarAlerta('Por favor, corrija los errores en el formulario', 'warning');
        // Scroll al primer error

        const primerError = $('.is-invalid:first');
        if (primerError.length > 0) {
            $('html, body').animate({
                scrollTop: primerError.offset().top - 100
            }, 500);
        }

    }


    return esValido;
}

// Mostrar error en campo específico
function mostrarError(campo, mensaje) {
    const $campo = $(`#${campo.charAt(0).toLowerCase() + campo.slice(1)}`);
    const $validacion = $(`[data-valmsg-for="${campo}"]`);

    $campo.addClass('is-invalid');
    $validacion.text(mensaje);
}

// Limpiar todas las validaciones
function limpiarValidaciones() {
    $('.is-invalid').removeClass('is-invalid');
    $('.is-valid').removeClass('is-valid');
    $('.field-validation').text('');
    $('#alertContainer').empty();
}

// ============================================
// UTILIDADES
// ============================================

// Mostrar alerta
function mostrarAlerta(mensaje, tipo = 'info') {
    const iconos = {
        success: 'fa-check-circle',
        danger: 'fa-times-circle',
        warning: 'fa-exclamation-triangle',
        info: 'fa-info-circle'
    };

    const html = `
        <div class="alert alert-${tipo} alert-dismissible fade show" role="alert">
            <i class="fas ${iconos[tipo]}"></i> ${mensaje}
            <button type="button" class="close" data-dismiss="alert" onclick="ocultarAlerta()">
                <span>&times;</span>
            </button>
        </div>
    `;

    $('#alertContainer').html(html);

    $('#alertContainer').show();
    // Scroll a la alerta
    /*$('html, body').animate({
        scrollTop: $('#alertContainer').offset().top - 100
    }, 500);*/

    // Auto-ocultar después de 5 segundos (solo para success e info)
    if (tipo === 'success' || tipo === 'info') {
        setTimeout(function () {
            $('.alert').fadeOut('slow', function () {
                $(this).remove();
            });
        }, 5000);
    }
}
function ocultarAlerta() {
    const $alerta = $('#alertContainer');
    if ($alerta.length) {
        $alerta.fadeOut('slow', function () {
            $(this).hide();
        });
    }
}
// Limpiar formulario
function limpiarFormulario() {
    // Resetear formulario
    $('#formRegistroEstudiante')[0].reset();

    // Resetear estados
    cursosSeleccionados = [];
    limpiarValidaciones();

    // Resetear dropdowns dinámicos
    $('#cantonId').html('<option value="">-- Seleccione provincia primero --</option>').prop('disabled', true);
    $('#distritoId').html('<option value="">-- Seleccione cantón primero --</option>').prop('disabled', true);
    $('#cursosContainer').html(`
        <p class="text-muted text-center">
            <i class="fas fa-info-circle"></i> 
            Seleccione un cuatrimestre para ver los cursos disponibles
        </p>
    `);

    // Limpiar alertas
    $('#alertContainer').empty();

    // Scroll al inicio
    $('html, body').animate({ scrollTop: 0 }, 500);

    $('#modalExito').modal('hide');
}

// ============================================
// FUNCIONES GLOBALES
// ============================================

// Hacer funciones accesibles globalmente
window.limpiarFormulario = limpiarFormulario;
window.actualizarCursosSeleccionados = actualizarCursosSeleccionados;