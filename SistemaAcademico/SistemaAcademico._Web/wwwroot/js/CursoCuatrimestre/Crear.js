let docentesSeleccionados = [];

$(document).ready(function () {
    // Mostrar información del curso seleccionado
    $('#cursoId').change(function () {
        const selected = $(this).find('option:selected');
        if (selected.val()) {
            $('#cursoCodigo').text(selected.data('codigo'));
            $('#cursoNombre').text(selected.data('nombre'));
            $('#cursoDescripcion').text(selected.data('descripcion') || 'N/A');
            $('#cursoCreditos').text(selected.data('creditos'));
            $('#infoCurso').show();
        } else {
            $('#infoCurso').hide();
        }
    });

    // Agregar docente
    $('#btnAgregarDocente').click(function () {
        const docenteId = $('#docenteSelector').val();
        if (!docenteId) {
            alert('Por favor, seleccione un docente');
            return;
        }

        // Verificar si ya está agregado
        if (docentesSeleccionados.includes(parseInt(docenteId))) {
            alert('Este docente ya está en la lista');
            return;
        }

        const selected = $('#docenteSelector option:selected');
        const nombre = selected.data('nombre');
        const email = selected.data('email');

        agregarDocenteTabla(docenteId, nombre, email);
        docentesSeleccionados.push(parseInt(docenteId));

        // Limpiar selector
        $('#docenteSelector').val('');
    });

    // Submit del formulario
    $('#formCursoCuatrimestre').submit(function (e) {
        e.preventDefault();

        const cursoId = $('#cursoId').val();
        const cuatrimestreId = $('#cuatrimestreId').val();

        if (!cursoId) {
            alert('Por favor, seleccione un curso');
            return;
        }

        const data = {
            cursoId: parseInt(cursoId),
            cuatrimestreId: parseInt(cuatrimestreId),
            docenteIds: docentesSeleccionados
        };

        $.ajax({
            url: '@Url.Action("Crear")',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    window.location.href = '@Url.Action("Index")?cuatrimestreId=' + cuatrimestreId;
                } else {
                    alert(response.message);
                }
            },
            error: function (xhr) {
                const error = xhr.responseJSON?.message || 'Error al crear el registro';
                alert(error);
            }
        });
    });
});

function agregarDocenteTabla(id, nombre, email) {
    $('#noDocentesRow').hide();

    const row = `
                <tr data-docente-id="${id}">
                    <td>${nombre}</td>
                    <td>${email}</td>
                    <td class="text-center">
                        <button type="button" class="btn btn-sm btn-danger"
                                onclick="removerDocente(${id})">
                            <i class="fas fa-trash"></i>
                        </button>
                    </td>
                </tr>
            `;

    $('#docentesSeleccionados').append(row);
}

function removerDocente(id) {
    $(`tr[data-docente-id="${id}"]`).remove();
    docentesSeleccionados = docentesSeleccionados.filter(d => d !== id);

    if (docentesSeleccionados.length === 0) {
        $('#noDocentesRow').show();
    }
}