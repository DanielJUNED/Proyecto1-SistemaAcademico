
//Crear curso - JavaScript
$('#formCurso').on('submit', function (e) {
    e.preventDefault();

    const curso = {
        Codigo: $('#codigo').val(),
        Nombre: $('#nombre').val(),
        Descripcion: $('#descripcion').val(),
        Creditos: parseInt($('#creditos').val())
    };

    $.ajax({
        url: '/curso/crear',
        type: 'POST',
        headers: {
            "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
        },
        contentType: 'application/json',
        data: JSON.stringify(curso),
        success: function (response) {
            if (!response.success) {
                alert(response.mensaje + "\n" + response.errores.join("\n"));
                return;
            }

            alert("Curso creado correctamente");
            window.location.href = "/Curso/Index";
        },
        error: function (xhr) {
            const res = xhr.responseJSON;
            alert(res.mensaje + "\n" + res.errores.join("\n"));
        }
    });
});