// JavaScript para la pantalla de recuperación de contraseña

document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('formRecuperarContrasena');
    const inputUsuario = document.getElementById('inputUsuario');
    const inputContrasena = document.getElementById('inputContrasena');
    const contenedorContrasena = document.getElementById('contenedorContrasena');
    const btnMostrarContrasena = document.getElementById('btnMostrarContrasena');

    // Manejar el envío del formulario
    if (form && btnMostrarContrasena) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();

            const usuario = inputUsuario ? inputUsuario.value.trim() : '';

            // Validar que se haya ingresado un usuario
            if (!usuario) {
                Swal.fire({
                    title: "Advertencia",
                    text: "Por favor, ingrese su usuario o correo electrónico",
                    icon: "warning",
                    confirmButtonText: 'Entendido',
                    confirmButtonColor: '#297ea6'
                });
                return;
            }

            // Deshabilitar el botón mientras se procesa
            btnMostrarContrasena.disabled = true;
            btnMostrarContrasena.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Buscando...';

            // Realizar la solicitud AJAX
            $.ajax({
                url: '/Olvidar_Contrasena_/RecuperarContrasena',
                type: 'POST',
                data: { Usuario: usuario },
                success: function (result) {
                    console.log(result); // Para debugging
                    if (result.ok && result.valorRetorno) {
                        // Mostrar el campo de contraseña con el valor
                        if (inputContrasena) {
                            inputContrasena.value = result.valorRetorno;
                        }

                        Swal.fire({
                            title: "¡Contraseña Encontrada!",
                            text: result.mensaje || "Su contraseña se ha mostrado a continuación",
                            icon: "success",
                            confirmButtonText: 'Entendido',
                            confirmButtonColor: '#297ea6'
                        });

                        // Cambiar el texto del botón
                        btnMostrarContrasena.innerHTML = 'Buscar Otra Vez';
                    } else {
                        // Limpiar el campo de contraseña si no se encontró
                        if (inputContrasena) {
                            inputContrasena.value = '';
                        }

                        Swal.fire({
                            title: "Error",
                            text: result.mensaje || "No se pudo recuperar la contraseña",
                            icon: "error",
                            confirmButtonText: 'Entendido',
                            confirmButtonColor: '#297ea6'
                        });
                    }
                },
                error: function (xhr, status, error) {
                    Swal.fire({
                        title: "Error",
                        text: "Ocurrió un error al procesar la solicitud. Por favor, intente nuevamente.",
                        icon: "error",
                        confirmButtonText: 'Entendido',
                        confirmButtonColor: '#297ea6'
                    });
                },
                complete: function () {
                    // Rehabilitar el botón
                    btnMostrarContrasena.disabled = false;
                    if (btnMostrarContrasena.innerHTML.includes('Buscando')) {
                        btnMostrarContrasena.innerHTML = 'Mostrar Contraseña';
                    }
                }
            });
        });
    }
});

