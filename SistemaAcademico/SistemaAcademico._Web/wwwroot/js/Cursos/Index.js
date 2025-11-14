//Gestion de cursos - JavaScript  

// Eliminar curso
$('.btnEliminar').click(function () {
    const cursoId = $(this).data('id');
    const cursoNombre = $(this).data('nombre'); 

    if (confirm(`¿Está seguro que desea eliminar el curso "${cursoNombre}"?`)) {
        $.ajax({
            url: eliminarUrlBase + '/' + cursoId,
            type: 'DELETE',
            success: function (result) {

                if (result.success) {
                    alert(result.mensaje);
                    location.reload();
                } else {
                    alert("Error: " + (result.mensaje || "No se pudo eliminar"));
                }
            },

            error: function (xhr) {

                if (xhr.responseJSON) {
                    alert(xhr.responseJSON.mensaje + "\n" + (xhr.responseJSON.errores?.join("\n") ?? ""));
                } else {
                    alert("Error inesperado al eliminar el curso");
                }
            }
        });
    }
});

