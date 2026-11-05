// matricula.js
// Maneja las solicitudes asíncronas (AJAX vía fetch) para matricular
// y cancelar cursos sin recargar la página, tanto en la vista
// "Cursos Disponibles" como en "Mi Portal" (PortalEstudiante).

(function () {
    function getToken() {
        var input = document.querySelector('input[name="__RequestVerificationToken"]');
        return input ? input.value : "";
    }

    function mostrarAlerta(contenedorId, mensaje, tipo) {
        var contenedor = document.getElementById(contenedorId);
        if (!contenedor) return;
        contenedor.innerHTML =
            '<div class="alert alert-' + tipo + ' alert-dismissible fade show" role="alert">' +
            mensaje +
            '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
            '</div>';
    }

    async function postForm(url, data) {
        // El token se envía como campo del formulario (__RequestVerificationToken),
        // que es lo que [ValidateAntiForgeryToken] espera por defecto en ASP.NET Core.
        data.__RequestVerificationToken = getToken();
        var body = new URLSearchParams(data);
        var response = await fetch(url, {
            method: "POST",
            headers: {
                "Content-Type": "application/x-www-form-urlencoded",
                "X-Requested-With": "XMLHttpRequest"
            },
            body: body.toString()
        });
        return response.json();
    }

    // --- Matricularme (vista Disponibles) ---
    document.addEventListener("click", async function (e) {
        var boton = e.target.closest(".btn-matricular");
        if (!boton) return;

        var cursoId = boton.getAttribute("data-curso-id");
        boton.disabled = true;
        var textoOriginal = boton.innerHTML;
        boton.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span>Matriculando...';

        try {
            var resultado = await postForm("/Matricula/Matricular", { cursoId: cursoId });

            if (resultado.exito) {
                mostrarAlerta("alertaMatricula", '<i class="bi bi-check-circle-fill me-2"></i>' + resultado.mensaje, "success");
                var fila = document.getElementById("fila-curso-" + cursoId);
                if (fila) {
                    fila.style.transition = "opacity 0.4s";
                    fila.style.opacity = "0";
                    setTimeout(function () { fila.remove(); }, 400);
                }
            } else {
                mostrarAlerta("alertaMatricula", '<i class="bi bi-exclamation-triangle-fill me-2"></i>' + resultado.mensaje, "danger");
                boton.disabled = false;
                boton.innerHTML = textoOriginal;
            }
        } catch (err) {
            mostrarAlerta("alertaMatricula", "Ocurrió un error de conexión. Intente de nuevo.", "danger");
            boton.disabled = false;
            boton.innerHTML = textoOriginal;
        }
    });

    // --- Cancelar matrícula (vista PortalEstudiante) ---
    document.addEventListener("click", async function (e) {
        var boton = e.target.closest(".btn-cancelar-matricula");
        if (!boton) return;

        if (!confirm("¿Está seguro de que desea cancelar esta matrícula?")) return;

        var matriculaId = boton.getAttribute("data-matricula-id");
        boton.disabled = true;
        var textoOriginal = boton.innerHTML;
        boton.innerHTML = '<span class="spinner-border spinner-border-sm"></span>';

        try {
            var resultado = await postForm("/Matricula/Cancelar", { matriculaId: matriculaId });

            if (resultado.exito) {
                mostrarAlerta("alertaPortal", '<i class="bi bi-check-circle-fill me-2"></i>' + resultado.mensaje, "success");
                var fila = document.getElementById("fila-matricula-" + matriculaId);
                if (fila) {
                    fila.style.transition = "opacity 0.4s";
                    fila.style.opacity = "0";
                    setTimeout(function () { fila.remove(); }, 400);
                }
            } else {
                mostrarAlerta("alertaPortal", '<i class="bi bi-exclamation-triangle-fill me-2"></i>' + resultado.mensaje, "danger");
                boton.disabled = false;
                boton.innerHTML = textoOriginal;
            }
        } catch (err) {
            mostrarAlerta("alertaPortal", "Ocurrió un error de conexión. Intente de nuevo.", "danger");
            boton.disabled = false;
            boton.innerHTML = textoOriginal;
        }
    });
})();
