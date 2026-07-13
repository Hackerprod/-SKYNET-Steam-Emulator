document.addEventListener("DOMContentLoaded", function () {
    // Variable para controlar si el formulario está en proceso de envío
    let isSubmitting = false;
    let lastSubmitter = null;

    document.body.addEventListener("submit", function (event) {
        lastSubmitter = event.submitter;
    }, true);

    // Delegar el evento para todos los formularios con OnSend o OnComplete
    document.body.addEventListener("submit", async function (event) {
        const form = event.target;

        // Solo aplicar en formularios con al menos un atributo OnSend o OnComplete
        if (form.hasAttribute("OnSend") || form.hasAttribute("OnComplete")) {
            event.preventDefault(); // Detener el envío por defecto del formulario

            // Evitar múltiples envíos simultáneos
            if (isSubmitting) {
                console.log("Formulario ya está siendo enviado");
                return;
            }

            isSubmitting = true;

            try {
                const onSendFunctionName = form.getAttribute("OnSend");
                const onCompleteFunctionName = form.getAttribute("OnComplete");

                // Verificar si las funciones están definidas globalmente
                const onSend = onSendFunctionName ? window[onSendFunctionName] : null;
                const onComplete = onCompleteFunctionName ? window[onCompleteFunctionName] : null;

                if (onSend && typeof onSend !== "function") {
                    console.error(`No se encontró la función OnSend (${onSendFunctionName})`);
                    return;
                }

                if (onComplete && typeof onComplete !== "function") {
                    console.error(`No se encontró la función OnComplete (${onCompleteFunctionName})`);
                    return;
                }

                // Si hay un OnSend, ejecutarlo primero
                let shouldContinue = true;
                if (onSend) {
                    shouldContinue = await onSend(form);
                }

                if (!shouldContinue) {
                    console.log("El envío del formulario fue cancelado.");
                    return; // Si el usuario cancela, no enviar el formulario
                }

                // Obtener el token antiforgery
                const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

                // Crear FormData con los datos del formulario
                const formData = new FormData(form, lastSubmitter);
                lastSubmitter = null;
                const action = form.getAttribute("action") || window.location.href;
                const method = form.getAttribute("method") || "POST";

                // Enviar la solicitud de forma manual usando fetch
                const response = await fetch(action, {
                    method: method,
                    body: formData,
                    headers: {
                        'RequestVerificationToken': token,
                        'X-Requested-With': 'XMLHttpRequest'
                    },
                    credentials: 'include'
                });

                const data = await response.json();

                // Llamar a la función OnComplete si está definida
                if (onComplete) {
                    onComplete(data, form);
                }
            } catch (error) {
                console.error("Error al enviar el formulario:", error);
            } finally {
                // Restaurar el estado de envío
                isSubmitting = false;
            }
        }
    });
});

document.addEventListener('DOMContentLoaded', function () {
    // Buscar elementos con el atributo auto-submit
    document.querySelectorAll('[auto-submit]').forEach(input => {
        input.addEventListener('change', function () {
            const form = this.closest('form');
            if (form) {
                const submitEvent = new Event('submit', {
                    bubbles: true,
                    cancelable: true
                });
                form.dispatchEvent(submitEvent);
            }
        });
    });
});