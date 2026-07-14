document.addEventListener("DOMContentLoaded", function () {
    // Variable to control whether the form is being submitted
    let isSubmitting = false;
    let lastSubmitter = null;

    document.body.addEventListener("submit", function (event) {
        lastSubmitter = event.submitter;
    }, true);

    // Delegate the event for all forms with OnSend or OnComplete
    document.body.addEventListener("submit", async function (event) {
        const form = event.target;

        // Only apply to forms with at least one OnSend or OnComplete attribute
        if (form.hasAttribute("OnSend") || form.hasAttribute("OnComplete")) {
            event.preventDefault(); // Stop the default form submission

            // Prevent multiple simultaneous submissions
            if (isSubmitting) {
                console.log("Form is already being submitted");
                return;
            }

            isSubmitting = true;

            try {
                const onSendFunctionName = form.getAttribute("OnSend");
                const onCompleteFunctionName = form.getAttribute("OnComplete");

                // Check if the functions are defined globally
                const onSend = onSendFunctionName ? window[onSendFunctionName] : null;
                const onComplete = onCompleteFunctionName ? window[onCompleteFunctionName] : null;

                if (onSend && typeof onSend !== "function") {
                    console.error(`OnSend function not found (${onSendFunctionName})`);
                    return;
                }

                if (onComplete && typeof onComplete !== "function") {
                    console.error(`OnComplete function not found (${onCompleteFunctionName})`);
                    return;
                }

                // If there is an OnSend, execute it first
                let shouldContinue = true;
                if (onSend) {
                    shouldContinue = await onSend(form);
                }

                if (!shouldContinue) {
                    console.log("Form submission was cancelled.");
                    return; // If the user cancels, do not submit the form
                }

                // Get the antiforgery token
                const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

                // Create FormData with the form data
                const formData = new FormData(form, lastSubmitter);
                lastSubmitter = null;
                const action = form.getAttribute("action") || window.location.href;
                const method = form.getAttribute("method") || "POST";

                // Send the request manually using fetch
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

                // Call the OnComplete function if defined
                if (onComplete) {
                    onComplete(data, form);
                }
            } catch (error) {
                console.error("Error sending form:", error);
            } finally {
                // Restore the submission state
                isSubmitting = false;
            }
        }
    });
});

document.addEventListener('DOMContentLoaded', function () {
    // Look for elements with the auto-submit attribute
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