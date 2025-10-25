// Scripts/evaluaciones.js
// Sistema de Evaluación de Estudiantes con AJAX

// Variables globales
let estudianteSeleccionado = null;
let cursoSeleccionado = null;
let timeoutBusqueda;

$(document).ready(function () {
    inicializarEvaluaciones();
    console.log(formatearFecha('/Date(1761373407563)/'));
});

// =============================================
// INICIALIZACIÓN
// =============================================
function inicializarEvaluaciones() {
    // Búsqueda dinámica
    $('#criterioBusqueda').on('input', function () {
        clearTimeout(timeoutBusqueda);
        const criterio = $(this).val().trim();

        if (criterio.length >= 2) {
            timeoutBusqueda = setTimeout(function () {
                buscarEstudiantes(criterio);
            }, 500);
        } else if (criterio.length === 0) {
            $('#resultadosBusqueda').hide();
            $('#listaEstudiantes').empty();
        }
    });

    // Botón de búsqueda
    $('#btnBuscar').on('click', function () {
        const criterio = $('#criterioBusqueda').val().trim();
        if (criterio.length >= 2) {
            buscarEstudiantes(criterio);
        }
    });

    // Enter en búsqueda
    $('#criterioBusqueda').on('keypress', function (e) {
        if (e.which === 13) {
            e.preventDefault();
            $('#btnBuscar').click();
        }
    });

    // Limpiar búsqueda
    $('#btnLimpiarBusqueda').on('click', limpiarBusqueda);

    // Submit del formulario
    $('#formEvaluacion').on('submit', function (e) {
        e.preventDefault();
        guardarEvaluacion();
    });

    // Cancelar evaluación
    $('#btnCancelar').on('click', cancelarEvaluacion);

    // Nueva evaluación desde modal
    $('#btnNuevaEvaluacion').on('click', function () {
        $('#modalConfirmacion').modal('hide');
        limpiarBusqueda();
    });

    // Cambio en nota - calcular estado automático
    $('#nota').on('input', function () {
        calcularEstadoAutomatico();
    });

    // Contador de caracteres
    $('#observaciones').on('input', function () {
        const count = $(this).val().length;
        $('#contadorCaracteres').text(count);
    });

    // Validaciones en tiempo real
    $('#nota').on('blur', function () {
        validarNota();
    });
}

// =============================================
// BÚSQUEDA DE ESTUDIANTES
// =============================================
function buscarEstudiantes(criterio) {
    $.ajax({
        url: '/Evaluacion/BuscarEstudiantes',
        type: 'GET',
        data: { criterio: criterio },
        beforeSend: function () {
            $('#listaEstudiantes').html('<div class="text-center p-3"><div class="spinner-border text-primary"></div><p class="mt-2">Buscando...</p></div>');
            $('#resultadosBusqueda').show();
        },
        success: function (response) {
            if (response.success) {
                mostrarResultadosBusqueda(response.data);
                $('#contadorResultados').text(response.count);
            } else {
                mostrarAlerta('danger', response.message);
                $('#resultadosBusqueda').hide();
            }
        },
        error: function () {
            mostrarAlerta('danger', 'Error al buscar estudiantes. Intente nuevamente.');
            $('#resultadosBusqueda').hide();
        }
    });
}

function mostrarResultadosBusqueda(estudiantes) {
    const $lista = $('#listaEstudiantes');
    $lista.empty();

    if (estudiantes.length === 0) {
        $lista.html(`
            <div class="alert alert-warning">
                <i class="fas fa-exclamation-triangle"></i>
                No se encontraron estudiantes con ese criterio.
            </div>
        `);
        return;
    }

    estudiantes.forEach(function (estudiante) {
        const html = `
            <div class="estudiante-item" data-estudiante-id="${estudiante.EstudianteID}">
                <div class="row align-items-center">
                    <div class="col-md-8">
                        <h6 class="mb-1">
                            <i class="fas fa-user text-primary"></i>
                            <strong>${estudiante.NombreCompleto}</strong>
                        </h6>
                        <p class="mb-0 text-muted small">
                            <i class="fas fa-id-card"></i> ${estudiante.Identificacion} | 
                            <i class="fas fa-envelope"></i> ${estudiante.Email}
                        </p>
                    </div>
                    <div class="col-md-4 text-right">
                        <span class="badge badge-info">
                            ${estudiante.CursosMatriculados ? estudiante.CursosMatriculados.length : 0} cursos
                        </span>
                        <span class="badge badge-secondary">
                            ${estudiante.Edad} años
                        </span>
                    </div>
                </div>
            </div>
        `;
        $lista.append(html);
    });

    // Evento click en estudiante
    $('.estudiante-item').on('click', function () {
        const estudianteId = $(this).data('estudiante-id');
        seleccionarEstudiante(estudianteId);

        // Marcar como seleccionado
        $('.estudiante-item').removeClass('selected');
        $(this).addClass('selected');
    });
}

// =============================================
// SELECCIONAR ESTUDIANTE
// =============================================
function seleccionarEstudiante(estudianteId) {
    $.ajax({
        url: '/Evaluacion/ObtenerEstudiante',
        type: 'GET',
        data: { estudianteId: estudianteId },
        beforeSend: function () {
            mostrarCargando();
        },
        success: function (response) {
            if (response.success) {
                estudianteSeleccionado = response.data;
                mostrarInformacionEstudiante(response.data);
                mostrarCursosMatriculados(response.data.CursosMatriculados);

                // Scroll a la información del estudiante
                $('html, body').animate({
                    scrollTop: $('#cardEstudiante').offset().top - 20
                }, 500);
            } else {
                mostrarAlerta('danger', response.message);
            }
        },
        error: function () {
            mostrarAlerta('danger', 'Error al obtener información del estudiante.');
        }
    });
}

function mostrarInformacionEstudiante(estudiante) {
    $('#infoIdentificacion').text(estudiante.Identificacion);
    $('#infoNombre').text(estudiante.NombreCompleto);
    $('#infoEmail').text(estudiante.Email);
    $('#infoEdad').text(`${estudiante.Edad} años`);
    $('#infoDireccion').text(estudiante.DireccionCompleta);

    $('#cardEstudiante').fadeIn();
}

function mostrarCursosMatriculados(cursos) {
    const $lista = $('#listaCursos');
    $lista.empty();

    if (!cursos || cursos.length === 0) {
        $lista.html(`
            <div class="alert alert-warning">
                <i class="fas fa-exclamation-triangle"></i>
                Este estudiante no tiene cursos matriculados.
            </div>
        `);
        $('#cardCursos').fadeIn();
        return;
    }

    cursos.forEach(function (curso) {
        const tieneEvaluacion = curso.TieneEvaluacion;
        const cardClass = tieneEvaluacion ? 'curso-card evaluado' : 'curso-card';
        const badge = tieneEvaluacion
            ? `<span class="badge badge-success">✓ Evaluado: ${curso.NotaActual}</span>`
            : `<span class="badge badge-warning">Pendiente</span>`;
        const botonTexto = tieneEvaluacion ? 'Editar Evaluación' : 'Evaluar';
        const botonClass = tieneEvaluacion ? 'btn-warning' : 'btn-primary';

        const html = `
            <div class="${cardClass}">
                <div class="row align-items-center">
                    <div class="col-md-8">
                        <h6 class="mb-1">
                            <strong>${curso.CodigoCurso}</strong> - ${curso.NombreCurso}
                        </h6>
                        <p class="mb-0 text-muted small">
                            <i class="fas fa-calendar"></i> ${curso.NombreCuatrimestre} | 
                            <i class="fas fa-award"></i> ${curso.Creditos} créditos
                        </p>
                        ${tieneEvaluacion ? `
                            <p class="mb-0 mt-1 small">
                                <strong>Estado:</strong> 
                                <span class="badge badge-${obtenerColorEstado(curso.EstadoActual)}">
                                    ${curso.EstadoActual}
                                </span>
                            </p>
                        ` : ''}
                    </div>
                    <div class="col-md-4 text-right">
                        ${badge}
                        <button class="btn ${botonClass} btn-sm d-block w-100 mt-2" 
                                onclick="abrirFormularioEvaluacion(${curso.EstudianteCursoID}, ${tieneEvaluacion},${curso.EvaluacionId})">
                            <i class="fas fa-${tieneEvaluacion ? 'edit' : 'plus'}"></i> ${botonTexto}
                        </button>
                    </div>
                </div>
            </div>
        `;
        $lista.append(html);
    });

    $('#cardCursos').fadeIn();
}

// =============================================
// FORMULARIO DE EVALUACIÓN
// =============================================
function abrirFormularioEvaluacion(estudianteCursoId, esEdicion,evaluacionId) {
    // Buscar el curso seleccionado
    cursoSeleccionado = estudianteSeleccionado.CursosMatriculados.find(
        c => c.EstudianteCursoID === estudianteCursoId
    );

    if (!cursoSeleccionado) {
        mostrarAlerta('danger', 'Error al cargar el curso');
        return;
    }

    // Configurar formulario
    $('#estudianteCursoId').val(estudianteCursoId);
    $('#modoEdicion').val(esEdicion ? 'editar' : 'crear');

    // Mostrar información
    $('#evalEstudiante').text(estudianteSeleccionado.NombreCompleto);
    $('#evalCurso').text(`${cursoSeleccionado.CodigoCurso} - ${cursoSeleccionado.NombreCurso}`);
    $('#evalCuatrimestre').text(cursoSeleccionado.NombreCuatrimestre);

    if (esEdicion) {
        // Cargar datos existentes
        cargarEvaluacionExistente(estudianteCursoId);
        $('#tituloFormulario').text('Actualizar Evaluación');
        $('#textoBoton').text('Actualizar Evaluación');
    } else {
        // Limpiar formulario
        limpiarFormularioEvaluacion();
        $('#tituloFormulario').text('Registrar Evaluación');
        $('#textoBoton').text('Guardar Evaluación');
    }

    // Mostrar y scroll
    $('#cardEvaluacion').fadeIn();
    $('html, body').animate({
        scrollTop: $('#cardEvaluacion').offset().top - 20
    }, 500);
}

function cargarEvaluacionExistente(estudianteCursoId) {
    // Obtener la evaluación actual del array
    const curso = cursoSeleccionado;

    $('#nota').val(curso.NotaActual);

    $('#evaluacionId').val(curso.EvaluacionId);
    $('#observaciones').val(curso.Observacion);
    $('#tipoParticipacion').val(curso.TipoParticipacion); // Se cargará con AJAX completo
    $('#estado').val(curso.EstadoActual);

    // Aquí podrías hacer una llamada AJAX para obtener todos los detalles
    // Por ahora usamos los datos básicos que ya tenemos
}

function limpiarFormularioEvaluacion() {
    $('#formEvaluacion')[0].reset();
    $('#evaluacionId').val('');
    $('#contadorCaracteres').text('0');
    $('.is-invalid, .is-valid').removeClass('is-invalid is-valid');
    $('.field-validation').text('');
}

function cancelarEvaluacion() {
    if (confirm('¿Está seguro que desea cancelar? Los cambios no guardados se perderán.')) {
        $('#cardEvaluacion').fadeOut();
        limpiarFormularioEvaluacion();
    }
}

// =============================================
// GUARDAR EVALUACIÓN
// =============================================
function guardarEvaluacion() {
    // Limpiar alertas
    $('#alertContainer').empty();

    // Validar formulario
    if (!validarFormularioEvaluacion()) {
        return;
    }

    const $btn = $('#btnGuardarEvaluacion');
    const token = $('input[name="__RequestVerificationToken"]').val();
    const modoEdicion = $('#modoEdicion').val() === 'editar';
    const url = modoEdicion ? '/Evaluacion/ActualizarEvaluacion' : '/Evaluacion/RegistrarEvaluacion';

    // Preparar datos
    const datos = {
        __RequestVerificationToken: token,
        EstudianteCursoID: $('#estudianteCursoId').val(),
        Nota: $('#nota').val(),
        Observaciones: $('#observaciones').val(),
        TipoParticipacion: $('#tipoParticipacion').val(),
        Estado: $('#estado').val()
    };

    if (modoEdicion) {
        datos.EvaluacionID = $('#evaluacionId').val();
    }

    // Deshabilitar botón
    $btn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm mr-2"></span>Guardando...');

    $.ajax({
        url: url,
        type: 'POST',
        data: datos,
        success: function (response) {
            if (response.Success) {
                // Mostrar modal de confirmación
                mostrarModalConfirmacion(response.Evaluacion);

                // Ocultar formulario
                $('#cardEvaluacion').fadeOut();

                // Actualizar lista de cursos
                if (estudianteSeleccionado) {
                    seleccionarEstudiante(estudianteSeleccionado.EstudianteID);
                }
            } else {
                var mensajeError = response.Message;
                if (response.Errors && response.Errors.length > 0) {
                    mensajeError += '<ul class="mb-0 mt-2">';
                    response.Errors.forEach(function (error) {
                        mensajeError += '<li>' + error + '</li>';
                    });
                    mensajeError += '</ul>';
                }
                mostrarAlerta('danger', mensajeError);

                // Scroll a la alerta
                $('html, body').animate({
                    scrollTop: $('#alertContainer').offset().top - 20
                }, 300);
            }
        },
        error: function () {
            mostrarAlerta('danger', 'Error de conexión. Por favor, intente nuevamente.');
        },
        complete: function () {
            $btn.prop('disabled', false).html('<i class="fas fa-save"></i> <span id="textoBoton">Guardar Evaluación</span>');
        }
    });
}

// =============================================
// MODAL DE CONFIRMACIÓN
// =============================================
function mostrarModalConfirmacion(evaluacion) {
    const colorEstado = obtenerColorEstado(evaluacion.Estado);
    const iconoEstado = evaluacion.Estado === 'Aprobado' ? 'check-circle' :
        evaluacion.Estado === 'Reprobado' ? 'times-circle' : 'clock';

    const html = `
        <div class="card border-0">
            <div class="card-body">
                <h5 class="text-center mb-4">
                    <i class="fas fa-check-circle text-success" style="font-size: 3rem;"></i>
                </h5>
                
                <h6 class="text-center mb-4">
                    La evaluación ha sido registrada exitosamente
                </h6>

                <div class="row">
                    <div class="col-md-6">
                        <p class="mb-2">
                            <strong><i class="fas fa-user text-primary"></i> Estudiante:</strong><br>
                            ${evaluacion.NombreEstudiante}
                        </p>
                        <p class="mb-2">
                            <strong><i class="fas fa-id-card text-primary"></i> Identificación:</strong><br>
                            ${evaluacion.IdentificacionEstudiante}
                        </p>
                        <p class="mb-2">
                            <strong><i class="fas fa-book text-primary"></i> Curso:</strong><br>
                            ${evaluacion.CodigoCurso} - ${evaluacion.NombreCurso}
                        </p>
                    </div>
                    <div class="col-md-6">
                        <p class="mb-2">
                            <strong><i class="fas fa-calendar text-primary"></i> Cuatrimestre:</strong><br>
                            ${evaluacion.NombreCuatrimestre}
                        </p>
                        <p class="mb-2">
                            <strong><i class="fas fa-star text-primary"></i> Nota:</strong><br>
                            <span class="badge badge-xl badge-primary">${evaluacion.Nota} / 100</span>
                        </p>
                        <p class="mb-2">
                            <strong><i class="fas fa-${iconoEstado} text-primary"></i> Estado:</strong><br>
                            <span class="badge badge-xl badge-${colorEstado}">${evaluacion.Estado}</span>
                        </p>
                    </div>
                </div>

                ${evaluacion.Observaciones ? `
                    <hr>
                    <p class="mb-0">
                        <strong><i class="fas fa-comment text-primary"></i> Observaciones:</strong><br>
                        ${evaluacion.Observaciones}
                    </p>
                ` : ''}

                <hr>
                <p class="mb-0 small text-muted text-center">
                    <i class="fas fa-clock"></i> 
                    Evaluado por ${evaluacion.NombreDocente} el ${formatearFecha(evaluacion.FechaEvaluacion)}
                </p>
            </div>
        </div>
    `;

    $('#modalBody').html(html);
    $('#modalConfirmacion').modal('show');
}

// =============================================
// VALIDACIONES
// =============================================
function validarFormularioEvaluacion() {
    let esValido = true;
    limpiarValidaciones();

    // Validar nota
    const nota = parseFloat($('#nota').val());
    if (!nota && nota !== 0) {
        mostrarErrorCampo('Nota', 'La nota es requerida');
        esValido = false;
    } else if (nota < 0 || nota > 100) {
        mostrarErrorCampo('Nota', 'La nota debe estar entre 0 y 100');
        esValido = false;
    }

    // Validar tipo de participación
    if (!$('#tipoParticipacion').val()) {
        mostrarErrorCampo('TipoParticipacion', 'El tipo de participación es requerido');
        esValido = false;
    }

    // Validar estado
    if (!$('#estado').val()) {
        mostrarErrorCampo('Estado', 'El estado es requerido');
        esValido = false;
    }

    if (!esValido) {
        mostrarAlerta('warning', 'Por favor, complete todos los campos requeridos');
    }

    return esValido;
}

function validarNota() {
    const nota = parseFloat($('#nota').val());
    const $campo = $('#nota');
    const $mensaje = $('[data-valmsg-for="Nota"]');

    $campo.removeClass('is-invalid is-valid');
    $mensaje.text('');

    if (!nota && nota !== 0) {
        return;
    }

    if (nota < 0 || nota > 100) {
        $campo.addClass('is-invalid');
        $mensaje.text('La nota debe estar entre 0 y 100');
    } else {
        $campo.addClass('is-valid');
    }
}

function calcularEstadoAutomatico() {
    const nota = parseFloat($('#nota').val());

    if (!nota && nota !== 0) return;

    let estado = '';
    if (nota >= 70) {
        estado = 'Aprobado';
    } else if (nota >= 60) {
        estado = 'En Proceso';
    } else {
        estado = 'Reprobado';
    }

    $('#estado').val(estado);
}

function mostrarErrorCampo(campo, mensaje) {
    const $campo = $('#' + campo.toLowerCase());
    const $mensaje = $('[data-valmsg-for="' + campo + '"]');

    $campo.addClass('is-invalid');
    $mensaje.text(mensaje);
}

function limpiarValidaciones() {
    $('.is-invalid, .is-valid').removeClass('is-invalid is-valid');
    $('.field-validation').text('');
}

// =============================================
// UTILIDADES
// =============================================
function limpiarBusqueda() {
    $('#criterioBusqueda').val('');
    $('#resultadosBusqueda').hide();
    $('#listaEstudiantes').empty();
    $('#cardEstudiante').hide();
    $('#cardCursos').hide();
    $('#cardEvaluacion').hide();
    $('#alertContainer').empty();

    estudianteSeleccionado = null;
    cursoSeleccionado = null;

    $('#criterioBusqueda').focus();
}

function mostrarCargando() {
    $('#cardEstudiante').hide();
    $('#cardCursos').hide();
    $('#cardEvaluacion').hide();
}

function mostrarAlerta(tipo, mensaje) {
    const iconos = {
        success: 'fa-check-circle',
        danger: 'fa-times-circle',
        warning: 'fa-exclamation-triangle',
        info: 'fa-info-circle'
    };

    const html = `
        <div class="alert alert-${tipo} alert-dismissible fade show" role="alert">
            <i class="fas ${iconos[tipo]} mr-2"></i>
            ${mensaje}
            <button type="button" class="close" data-dismiss="alert">
                <span>&times;</span>
            </button>
        </div>
    `;

    $('#alertContainer').html(html);

    // Scroll a la alerta
    $('html, body').animate({
        scrollTop: 0
    }, 300);

    // Auto-ocultar después de 5 segundos
    if (tipo === 'success' || tipo === 'info') {
        setTimeout(function () {
            $('#alertContainer .alert').fadeOut('slow', function () {
                $(this).remove();
            });
        }, 5000);
    }
}

function obtenerColorEstado(estado) {
    switch (estado) {
        case 'Aprobado': return 'success';
        case 'Reprobado': return 'danger';
        case 'En Proceso': return 'warning';
        default: return 'secondary';
    }
}

function formatearFecha(fecha) {

    // Extraer el número entre paréntesis
    const timestamp = parseInt(fecha.replace(/\/Date\((\d+)\)\//, '$1'));
    const date = new Date(timestamp);
    // Opciones de formato
    const opciones = {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    };
    return date.toLocaleDateString('es-ES', opciones);
}

// Hacer funciones globales accesibles
window.abrirFormularioEvaluacion = abrirFormularioEvaluacion;