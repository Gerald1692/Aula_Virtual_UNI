// Write your JavaScript code.

/// <summary>
/// JavaScript para la pantalla de login
/// </summary>
/// <createdate>10-10-2024</createdate>
/// <author>Gerald Arias</author>
/// <lastmodificationdate></lastmodificationdate>
/// <lastmodificationdescription></lastmodificationdescription>
/// <lastmodifierauthor></lastmodifierauthor>

jslogin = {

    objetos: {


    },
    controles: {

        inputUsuario: '#username',
        inputContrasena: '#password'

    },

    botones: {

        btnIniciarSesion: '#btningresar'

    },

    variables: {




    },
    metodos: {

        IniciarSesion: function () {

            event.preventDefault();

            let Usuario = $(jslogin.controles.inputUsuario).val();
            let Contrasena = $(jslogin.controles.inputContrasena).val();




            // Realizar la solicitud AJAX
            $.ajax({
                url: '../Login/IniciarSesion',
                type: 'POST',
                data: { Usuario: Usuario, Contrasena: Contrasena },
                success: function (result) {
                    console.log(result)
                    if (result.ok) {
                        Swal.fire({
                            title: "Éxito",
                            text: `${result.mensaje}`,
                            icon: "success"
                        }); 

                        // Redirigir a la página principal después de 2 segundos
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
                error: function () {
                    // Manejo de errores si es necesario

                    Swal.fire({
                        title: "Error",
                        text: `${result.mensaje}`,
                        icon: "error"
                    });

                }
            });


        }


    },
    eventos:
        function () {

            $(jslogin.botones.btnIniciarSesion).on('click', function () {

                jslogin.metodos.IniciarSesion();

            });


        }

}

$(function () {
    jslogin.eventos();
});