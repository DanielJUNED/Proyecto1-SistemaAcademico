$(document).ready(function () {

    const cfg = window.cursoConfig;

    if (!cfg) {
        console.error("❌ No se recibieron los datos desde la vista.");
        return;
    }

    const cursoCuatrimestreId = cfg.cursoCuatrimestreId;
    const puedeEditarDocentes = cfg.puedeEditarDocentes === "true";
    const registrarDocenteUrl = cfg.registrarDocenteUrl;
    const removerDocenteUrl = cfg.removerDocenteUrl;

    // --- Asignar docente ---
    if (puedeEditarDocentes) {

        $('#btnAgregarDocente').click(function () {

            const docenteId = $('#docenteSelector').val();
            if (!docenteId) {
                alert('Por favor, seleccione un docente');
                return;
            }

            const yaAsignado = $('#docentesAsignados tr').filter(function () {
                return $(this).find('td').first().text() ===
                    $('#docenteSelector option:selected').data('nombre');
            }).length > 0;

            if (yaAsignado) {
                alert('Este docente ya está asignado');
                return;
            }

            const data = {
                cursoCuatrimestreId: cursoCuatrimestreId,
                docenteId: parseInt(docenteId)
            };

            $.ajax({
                url: registrarDocenteUrl,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(data),
                success: function (response) {
                    alert(response.message);
                    if (response.success)
                        location.reload();
                },
                error: function (xhr) {
                    const error = xhr.responseJSON?.message || 'Error al asignar docente';
                    alert(error);
                }
            });
        });
    }

});

// --- remover docente global ---
function removerDocente(cursoCuatriDocenteId) {

    const cfg = window.cursoConfig;
    const puedeEditarDocentes = cfg.puedeEditarDocentes === "true";

    if (!confirm('¿Está seguro de remover este docente?')) return;

    $.ajax({
        url: cfg.removerDocenteUrl + "/" + cursoCuatriDocenteId,
        type: 'POST',
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (response) {
            alert(response.message);

            if (response.success) {
                $(`tr[data-docente-id="${cursoCuatriDocenteId}"]`).remove();
            }
        },
        error: function (xhr) {
            const error = xhr.responseJSON?.message || 'Error al remover docente';
            alert(error);
        }
    });
}
