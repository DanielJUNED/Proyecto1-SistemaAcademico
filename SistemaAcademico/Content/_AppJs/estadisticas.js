// Scripts/estadisticas.js
// Panel de Estadísticas con Chart.js

// Variables globales
let chartEstados = null;
let chartParticipacion = null;
let chartNotas = null;
let datosActuales = null;

$(document).ready(function () {
    inicializarEstadisticas();
});

// =============================================
// INICIALIZACIÓN
// =============================================
function inicializarEstadisticas() {
    // Cambio en cuatrimestre
    $('#filtroCuatrimestre').on('change', function () {
        const cuatrimestreId = $(this).val();
        if (cuatrimestreId) {
            cargarCursos(cuatrimestreId);
        } else {
            $('#filtroCurso').prop('disabled', true).html('<option value="">-- Todos los cursos --</option>');
        }
    });

    // Botón consultar
    $('#btnConsultar').on('click', consultarEstadisticas);

    // Enter en filtros
    $('#filtroCuatrimestre, #filtroCurso').on('keypress', function (e) {
        if (e.which === 13) {
            e.preventDefault();
            consultarEstadisticas();
        }
    });

    // Botón exportar
    $('#btnExportar').on('click', exportarEstadisticas);

    // Botón comparativa
    $('#btnComparativa').on('click', mostrarComparativa);
}

// =============================================
// CARGAR CURSOS
// =============================================
function cargarCursos(cuatrimestreId) {
    $.ajax({
        url: '/Estadistica/ObtenerCursos',
        type: 'GET',
        data: { cuatrimestreId: cuatrimestreId },
        beforeSend: function () {
            $('#filtroCurso').prop('disabled', true)
                .html('<option value="">Cargando...</option>');
        },
        success: function (response) {
            if (response.success) {
                let options = '<option value="">-- Todos los cursos --</option>';
                response.data.forEach(function (curso) {
                    options += `<option value="${curso.CursoID}">
                        ${curso.Codigo} - ${curso.Nombre} 
                        (${curso.TotalEstudiantes} estudiantes, ${curso.TotalEvaluaciones} evaluaciones)
                    </option>`;
                });
                $('#filtroCurso').html(options).prop('disabled', false);
            } else {
                mostrarAlerta('danger', response.message);
            }
        },
        error: function () {
            mostrarAlerta('danger', 'Error al cargar cursos');
            $('#filtroCurso').html('<option value="">-- Error al cargar --</option>');
        }
    });
}

// =============================================
// CONSULTAR ESTADÍSTICAS
// =============================================
function consultarEstadisticas() {
    const cuatrimestreId = $('#filtroCuatrimestre').val();
    const cursoId = $('#filtroCurso').val() || null;

    if (!cuatrimestreId) {
        mostrarAlerta('warning', 'Debe seleccionar un cuatrimestre');
        return;
    }

    $.ajax({
        url: '/Estadistica/ObtenerEstadisticas',
        type: 'GET',
        data: {
            cuatrimestreId: cuatrimestreId,
            cursoId: cursoId
        },
        beforeSend: function () {
            mostrarCargando();
        },
        success: function (response) {
            if (response.success) {
                datosActuales = response.data;
                mostrarEstadisticas(response.data);
            } else {
                mostrarAlerta('danger', response.message);
                ocultarEstadisticas();
            }
        },
        error: function () {
            mostrarAlerta('danger', 'Error al obtener estadísticas');
            ocultarEstadisticas();
        }
    });
}

// =============================================
// MOSTRAR ESTADÍSTICAS
// =============================================
function mostrarEstadisticas(data) {
    // Ocultar estado inicial
    $('#estadoInicial').hide();
    $('#seccionEstadisticas').fadeIn();

    // Actualizar títulos
    let titulo = data.NombreCuatrimestre;
    let subtitulo = data.NombreCurso
        ? `${data.CodigoCurso} - ${data.NombreCurso}`
        : 'Todos los cursos';

    $('#tituloEstadisticas').text(titulo);
    $('#subtituloEstadisticas').text(subtitulo);

    // Actualizar indicadores
    actualizarIndicadores(data.Generales);

    // Actualizar gráficos
    actualizarGraficos(data.Graficos);

    // Actualizar tabla de estudiantes
    actualizarTablaEstudiantes(data.Estudiantes);

    // Scroll suave
    $('html, body').animate({
        scrollTop: $('#seccionEstadisticas').offset().top - 20
    }, 500);
}

// =============================================
// ACTUALIZAR INDICADORES
// =============================================
function actualizarIndicadores(generales) {
    // Total estudiantes
    $('#statTotalEstudiantes').text(generales.TotalEstudiantes);
    $('#statMatriculados').text(`${generales.TotalMatriculados} matrículas`);

    // Aprobación
    $('#statAprobacion').text(generales.PorcentajeAprobacion.toFixed(1) + '%');
    $('#statAprobados').text(`${generales.EstudiantesAprobados} aprobados`);

    // Reprobación
    $('#statReprobacion').text(generales.PorcentajeReprobacion.toFixed(1) + '%');
    $('#statReprobados').text(`${generales.EstudiantesReprobados} reprobados`);

    // Promedio
    $('#statPromedio').text(generales.PromedioGeneral.toFixed(1));
    $('#statParticipacion').text(`${generales.PorcentajeParticipacion.toFixed(1)}% participación`);

    // Animación de números
    animarNumeros();
}

// =============================================
// ACTUALIZAR GRÁFICOS
// =============================================
function actualizarGraficos(graficos) {
    // Gráfico de Estados (Pie)
    actualizarGraficoEstados(graficos);

    // Gráfico de Participación (Doughnut)
    actualizarGraficoParticipacion(graficos);

    // Gráfico de Distribución de Notas (Bar)
    actualizarGraficoNotas(graficos);
}

function actualizarGraficoEstados(graficos) {
    const ctx = document.getElementById('chartEstados').getContext('2d');

    // Destruir gráfico anterior si existe
    if (chartEstados) {
        chartEstados.destroy();
    }

    chartEstados = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: graficos.EstadosLabels,
            datasets: [{
                data: graficos.EstadosData,
                backgroundColor: graficos.EstadosColors,
                borderWidth: 2,
                borderColor: '#fff'
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: {
                        padding: 15,
                        font: {
                            size: 12
                        }
                    }
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            const label = context.label || '';
                            const value = context.parsed || 0;
                            const total = context.dataset.data.reduce((a, b) => a + b, 0);
                            const percentage = total > 0 ? ((value / total) * 100).toFixed(1) : 0;
                            return `${label}: ${value} (${percentage}%)`;
                        }
                    }
                }
            }
        }
    });
}

function actualizarGraficoParticipacion(graficos) {
    const ctx = document.getElementById('chartParticipacion').getContext('2d');

    if (chartParticipacion) {
        chartParticipacion.destroy();
    }

    chartParticipacion = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: graficos.ParticipacionLabels,
            datasets: [{
                data: graficos.ParticipacionData,
                backgroundColor: graficos.ParticipacionColors,
                borderWidth: 2,
                borderColor: '#fff'
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: {
                        padding: 15,
                        font: {
                            size: 12
                        }
                    }
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            const label = context.label || '';
                            const value = context.parsed || 0;
                            return `${label}: ${value} estudiantes`;
                        }
                    }
                }
            }
        }
    });
}

function actualizarGraficoNotas(graficos) {
    const ctx = document.getElementById('chartNotas').getContext('2d');

    if (chartNotas) {
        chartNotas.destroy();
    }

    chartNotas = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: graficos.NotasRangos,
            datasets: [{
                label: 'Cantidad de Estudiantes',
                data: graficos.NotasDistribucion,
                backgroundColor: [
                    'rgba(220, 53, 69, 0.8)',   // 0-59 Rojo
                    'rgba(255, 193, 7, 0.8)',   // 60-69 Amarillo
                    'rgba(23, 162, 184, 0.8)',  // 70-79 Cian
                    'rgba(40, 167, 69, 0.8)',   // 80-89 Verde
                    'rgba(0, 123, 255, 0.8)'    // 90-100 Azul
                ],
                borderColor: [
                    'rgb(220, 53, 69)',
                    'rgb(255, 193, 7)',
                    'rgb(23, 162, 184)',
                    'rgb(40, 167, 69)',
                    'rgb(0, 123, 255)'
                ],
                borderWidth: 2
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        stepSize: 1
                    }
                }
            },
            plugins: {
                legend: {
                    display: false
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            return `${context.parsed.y} estudiantes`;
                        }
                    }
                }
            }
        }
    });
}

// =============================================
// ACTUALIZAR TABLA DE ESTUDIANTES
// =============================================
function actualizarTablaEstudiantes(estudiantes) {
    const $tbody = $('#bodyEstudiantes');
    $tbody.empty();

    if (!estudiantes || estudiantes.length === 0) {
        $tbody.html(`
            <tr>
                <td colspan="7" class="text-center py-4">
                    <i class="fas fa-info-circle text-muted"></i>
                    No hay estudiantes para mostrar
                </td>
            </tr>
        `);
        return;
    }

    estudiantes.forEach(function (estudiante) {
        const tieneEvaluacion = estudiante.TieneEvaluacion;
        const nota = tieneEvaluacion ? estudiante.Nota.toFixed(2) : '-';
        const estado = tieneEvaluacion ? estudiante.Estado : 'Sin evaluar';
        const participacion = tieneEvaluacion ? estudiante.TipoParticipacion : '-';
        const fecha = tieneEvaluacion && estudiante.FechaEvaluacion
            ? formatearFecha(estudiante.FechaEvaluacion)
            : '-';

        const colorEstado = obtenerColorEstado(estado);
        const colorNota = obtenerColorNota(estudiante.Nota);

        const row = `
            <tr class="${!tieneEvaluacion ? 'table-warning' : ''}">
                <td>${estudiante.Identificacion}</td>
                <td>${estudiante.NombreCompleto}</td>
                <td><small>${estudiante.Email}</small></td>
                <td class="text-center">
                    <span class="badge badge-xl badge-${colorNota}">${nota}</span>
                </td>
                <td class="text-center">
                    <span class="badge badge-xl badge-${colorEstado}">${estado}</span>
                </td>
                <td class="text-center">${participacion}</td>
                <td class="text-center"><small>${fecha}</small></td>
            </tr>
        `;
        $tbody.append(row);
    });
}

// =============================================
// COMPARATIVA DE CURSOS
// =============================================
function mostrarComparativa() {
    const cuatrimestreId = $('#filtroCuatrimestre').val();

    if (!cuatrimestreId) {
        mostrarAlerta('warning', 'Debe seleccionar un cuatrimestre');
        return;
    }

    $.ajax({
        url: '/Estadistica/ObtenerComparativa',
        type: 'GET',
        data: { cuatrimestreId: cuatrimestreId },
        beforeSend: function () {
            $('#modalBodyComparativa').html(`
                <div class="text-center py-5">
                    <div class="spinner-border text-primary" role="status"></div>
                    <p class="mt-3">Cargando comparativa...</p>
                </div>
            `);
            $('#modalComparativa').modal('show');
        },
        success: function (response) {
            if (response.success) {
                renderizarComparativa(response.data);
            } else {
                $('#modalBodyComparativa').html(`
                    <div class="alert alert-danger">
                        <i class="fas fa-exclamation-triangle"></i> ${response.message}
                    </div>
                `);
            }
        },
        error: function () {
            $('#modalBodyComparativa').html(`
                <div class="alert alert-danger">
                    <i class="fas fa-times-circle"></i> Error al cargar comparativa
                </div>
            `);
        }
    });
}

function renderizarComparativa(data) {
    if (!data.Cursos || data.Cursos.length === 0) {
        $('#modalBodyComparativa').html(`
            <div class="alert alert-warning">
                <i class="fas fa-info-circle"></i> No hay cursos para comparar
            </div>
        `);
        return;
    }

    let html = '<div class="table-responsive"><table class="table table-hover">';
    html += `
        <thead class="thead-dark">
            <tr>
                <th>Código</th>
                <th>Curso</th>
                <th class="text-center">Estudiantes</th>
                <th class="text-center">Promedio</th>
                <th class="text-center">% Aprobación</th>
                <th class="text-center">Aprobados</th>
                <th class="text-center">Reprobados</th>
            </tr>
        </thead>
        <tbody>
    `;

    data.Cursos.forEach(function (curso) {
        const colorAprobacion = curso.PorcentajeAprobacion >= 70 ? 'success' :
            curso.PorcentajeAprobacion >= 50 ? 'warning' : 'danger';

        html += `
            <tr>
                <td><strong>${curso.CodigoCurso}</strong></td>
                <td>${curso.NombreCurso}</td>
                <td class="text-center">${curso.TotalEstudiantes}</td>
                <td class="text-center">
                    <span class="badge badge-info">${curso.PromedioNota.toFixed(2)}</span>
                </td>
                <td class="text-center">
                    <span class="badge badge-${colorAprobacion}">
                        ${curso.PorcentajeAprobacion.toFixed(1)}%
                    </span>
                </td>
                <td class="text-center">
                    <span class="badge badge-success">${curso.Aprobados}</span>
                </td>
                <td class="text-center">
                    <span class="badge badge-danger">${curso.Reprobados}</span>
                </td>
            </tr>
        `;
    });

    html += '</tbody></table></div>';
    $('#modalBodyComparativa').html(html);
}

// =============================================
// EXPORTAR ESTADÍSTICAS
// =============================================
function exportarEstadisticas() {
    if (!datosActuales) {
        mostrarAlerta('warning', 'No hay datos para exportar');
        return;
    }

    // Crear CSV
    let csv = 'Estadísticas Académicas\n\n';
    csv += `Cuatrimestre: ${datosActuales.NombreCuatrimestre}\n`;
    csv += `Curso: ${datosActuales.NombreCurso || 'Todos'}\n\n`;

    csv += 'INDICADORES GENERALES\n';
    csv += `Total Estudiantes,${datosActuales.Generales.TotalEstudiantes}\n`;
    csv += `Promedio General,${datosActuales.Generales.PromedioGeneral.toFixed(2)}\n`;
    csv += `% Aprobación,${datosActuales.Generales.PorcentajeAprobacion.toFixed(2)}%\n`;
    csv += `% Reprobación,${datosActuales.Generales.PorcentajeReprobacion.toFixed(2)}%\n\n`;

    csv += 'DETALLE DE ESTUDIANTES\n';
    csv += 'Identificación,Nombre,Email,Nota,Estado,Participación\n';

    datosActuales.Estudiantes.forEach(function (est) {
        csv += `${est.Identificacion},"${est.NombreCompleto}",${est.Email},`;
        csv += `${est.Nota || 'N/A'},${est.Estado || 'Sin evaluar'},${est.TipoParticipacion || 'N/A'}\n`;
    });

    // Descargar
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);

    link.setAttribute('href', url);
    link.setAttribute('download', `estadisticas_${Date.now()}.csv`);
    link.style.visibility = 'hidden';

    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);

    mostrarAlerta('success', 'Estadísticas exportadas correctamente');
}

// =============================================
// UTILIDADES
// =============================================
function mostrarCargando() {
    $('#estadoInicial').html(`
        <div class="text-center py-5">
            <div class="spinner-border text-primary mb-3" style="width: 3rem; height: 3rem;" role="status"></div>
            <h5 class="text-muted">Cargando estadísticas...</h5>
        </div>
    `).show();
    $('#seccionEstadisticas').hide();
}

function ocultarEstadisticas() {
    $('#seccionEstadisticas').hide();
    $('#estadoInicial').html(`
        <i class="fas fa-chart-bar fa-5x text-muted mb-4"></i>
        <h4 class="text-muted">Seleccione un cuatrimestre para ver las estadísticas</h4>
        <p class="text-muted">Use los filtros superiores para consultar el rendimiento académico</p>
    `).show();
}

function animarNumeros() {
    $('.display-4').each(function () {
        const $this = $(this);
        const text = $this.text();

        if (text && text !== '0' && text !== '0%') {
            $this.css('opacity', '0').animate({ opacity: 1 }, 800);
        }
    });
}

function obtenerColorEstado(estado) {
    switch (estado) {
        case 'Aprobado': return 'success';
        case 'Reprobado': return 'danger';
        case 'En Proceso': return 'warning';
        default: return 'secondary';
    }
}

function obtenerColorNota(nota) {
    if (!nota) return 'secondary';
    if (nota >= 90) return 'primary';
    if (nota >= 80) return 'success';
    if (nota >= 70) return 'info';
    if (nota >= 60) return 'warning';
    return 'danger';
}

function formatearFecha(fecha) {
    if (!fecha) return '-';
    // Extraer el número entre paréntesis
    const timestamp = parseInt(fecha.replace(/\/Date\((\d+)\)\//, '$1'));

    const date = new Date(timestamp);
    return date.toLocaleDateString('es-ES', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit'
    });
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

    // Auto-ocultar
    if (tipo === 'success' || tipo === 'info') {
        setTimeout(function () {
            $('#alertContainer .alert').fadeOut('slow', function () {
                $(this).remove();
            });
        }, 5000);
    }
}