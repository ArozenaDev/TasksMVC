async function handdleErrorsApi(response) {
    let errorMessage = '';

    if (response.status == 400) {
        errorMessage = await response.text();
    } else if (response.status == 404) {
        errorMessage = resourceNotFound;
    } else {
        errorMessage = unexpectedError;
    }

    showErrorMessage(errorMessage);

}

function showErrorMessage(message) {
    Swal.fire({
        icon: 'error',
        title: 'Error...',
        text: message
    });
}

function confirmAction({ callbackAccept, callbackCancel, title }) {
    Swal.fire({
        title: title || '¿Realmente deseas hacer esto?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Sí',
        focusConfirm: true
    }).then((result) => {
        if (result.isConfirmed) {
            callbackAccept();
        } else if (callbackCancel) {
            callbackCancel();
        }
    })
}