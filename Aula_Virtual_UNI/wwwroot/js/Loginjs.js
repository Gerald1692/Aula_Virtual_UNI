jslogin = {

    controles: {
        inputUsuario: '#username',
        inputContrasena: '#password'
    },

    botones: {
        btnIniciarSesion: '#btningresar'
    },

    metodos: {

        IniciarSesion: function (event) {

            event.preventDefault();

            let Usuario = $(jslogin.controles.inputUsuario).val();
            let Contrasena = $(jslogin.controles.inputContrasena).val();

            // Validar campos vacíos
            if (Usuario.trim() === "" || Contrasena.trim() === "") {
               
                Swal.fire({
                    title: "Campos vacíos",
                    text: "Debe ingresar usuario y contraseña.",
                    icon: "warning"
                });
                return;
            }

            // Loading
            Swal.fire({
                title: "Validando...",
                allowOutsideClick: false,
                didOpen: () => Swal.showLoading()
            });

            $.ajax({
                url: '../Login/IniciarSesion',
                type: 'POST',
                data: { Usuario: Usuario, Contrasena: Contrasena },

                success: function (result) {
                   
                    if (result.ok) {

                        // Loading
                        Swal.fire({
                            title: "Validando...",
                            allowOutsideClick: false,
                            didOpen: () => Swal.showLoading()
                        });

                        Swal.fire({
                            title: "Has ingresado con Éxito",
                            text: `${result.mensaje}`,
                            icon: "success"
                        });

                        setTimeout(function () {
                            window.location.href = '/Menu/V_Menu';
                        }, 2100);

                    } else {
                        Swal.fire({
                            title: "Advertencia",
                            text: `${result.mensaje}`,
                            icon: "warning"
                        });
                    }
                },

                error: function (xhr, status, error) {
                    Swal.fire({
                        title: "Error",
                        text: "Ocurrió un error en la solicitud.",
                        icon: "error"
                    });
                }
            });

        }

    },

    eventos: function () {
        $(jslogin.botones.btnIniciarSesion).on('click', function (e) {
            jslogin.metodos.IniciarSesion(e);
        });
    }

}

$(function () {
    jslogin.eventos();
});
