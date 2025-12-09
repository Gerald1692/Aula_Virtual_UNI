// Validación y manejo del formulario de registro
document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('formRegistro');
    const btnRegistrar = document.getElementById('btnRegistrar');
    const btnText = document.getElementById('btnText');
    const btnSpinner = document.getElementById('btnSpinner');
    const alertMessage = document.getElementById('alertMessage');

    // Campos del formulario
    const nombre = document.getElementById('nombre');
    const apellido1 = document.getElementById('apellido1');
    const apellido2 = document.getElementById('apellido2');
    const cedula = document.getElementById('cedula');
    const telefono = document.getElementById('telefono');
    const correo = document.getElementById('correo');
    const sede = document.getElementById('sede');
    const password = document.getElementById('password');
    const confirmPassword = document.getElementById('confirmPassword');

    // Toggle password visibility
    const togglePassword = document.getElementById('togglePassword');
    const toggleConfirmPassword = document.getElementById('toggleConfirmPassword');

    togglePassword.addEventListener('click', function () {
        const type = password.getAttribute('type') === 'password' ? 'text' : 'password';
        password.setAttribute('type', type);
        this.querySelector('i').classList.toggle('fa-eye');
        this.querySelector('i').classList.toggle('fa-eye-slash');
    });

    toggleConfirmPassword.addEventListener('click', function () {
        const type = confirmPassword.getAttribute('type') === 'password' ? 'text' : 'password';
        confirmPassword.setAttribute('type', type);
        this.querySelector('i').classList.toggle('fa-eye');
        this.querySelector('i').classList.toggle('fa-eye-slash');
    });


    // Verificar que las contraseñas coincidan
    confirmPassword.addEventListener('input', function () {
        if (password.value !== confirmPassword.value) {
            confirmPassword.setCustomValidity('Las contraseñas no coinciden');
            confirmPassword.classList.add('is-invalid');
        } else {
            confirmPassword.setCustomValidity('');
            confirmPassword.classList.remove('is-invalid');
            confirmPassword.classList.add('is-valid');
        }
    });

    password.addEventListener('input', function () {
        if (confirmPassword.value !== '') {
            if (password.value !== confirmPassword.value) {
                confirmPassword.setCustomValidity('Las contraseñas no coinciden');
                confirmPassword.classList.add('is-invalid');
                confirmPassword.classList.remove('is-valid');
            } else {
                confirmPassword.setCustomValidity('');
                confirmPassword.classList.remove('is-invalid');
                confirmPassword.classList.add('is-valid');
            }
        }
    });

    // Verificar cédula existente (debounce)
    let cedulaTimeout;
    cedula.addEventListener('input', function () {
        clearTimeout(cedulaTimeout);
        const feedback = document.getElementById('cedulaFeedback');
        const value = this.value.trim();

        if (value.length < 5) {
            feedback.textContent = '';
            return;
        }

        cedulaTimeout = setTimeout(() => {
            fetch('/CrearCuenta/VerificarCedula', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
                body: 'cedula=' + encodeURIComponent(value)
            })
                .then(response => response.json())
                .then(data => {
                    if (data.existe) {
                        feedback.textContent = '⚠️ Esta cédula ya está registrada';
                        feedback.className = 'form-text text-danger';
                        cedula.setCustomValidity('Cédula ya registrada');
                    } else {
                        feedback.textContent = '✓ Cédula disponible';
                        feedback.className = 'form-text text-success';
                        cedula.setCustomValidity('');
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    feedback.textContent = '';
                });
        }, 500);
    });

    // Manejar envío del formulario
    form.addEventListener('submit', function (e) {
        e.preventDefault();

        // Validar formulario
        if (!form.checkValidity()) {
            e.stopPropagation();
            form.classList.add('was-validated');
            return;
        }

        // Verificar que las contraseñas coincidan
        if (password.value !== confirmPassword.value) {
            Swal.fire({
                title: "Error",
                text: "Las contraseñas no coinciden",
                icon: "error"
            });
            return;
        }

        // Deshabilitar botón y mostrar spinner
        btnRegistrar.disabled = true;
        btnText.classList.add('d-none');
        btnSpinner.classList.remove('d-none');

        // Preparar datos
        const formData = new URLSearchParams();
        formData.append('nombre', nombre.value.trim());
        formData.append('apellido1', apellido1.value.trim());
        formData.append('apellido2', apellido2.value.trim());
        formData.append('cedula', cedula.value.trim());
        formData.append('telefono', telefono.value.trim());
        formData.append('correo', correo.value.trim());
        formData.append('sede', sede.value.trim());
        formData.append('password', password.value);

        // Enviar datos
        fetch('/CrearCuenta/RegistrarUsuario', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            },
            body: formData.toString()
        })
            .then(response => response.json())
            .then(data => {
                if (data.ok) {
                    Swal.fire({
                        title: "¡Registro Exitoso!",
                        text: data.mensaje || "Te has registrado correctamente",
                        icon: "success",
                        confirmButtonText: 'Ir al Login',
                        confirmButtonColor: '#297ea6'
                    }).then(() => {
                        // Redirigir al login
                        window.location.href = '/Login/Login';
                    });
                } else {
                    Swal.fire({
                        title: "Error al Registrar",
                        text: data.mensaje || "No se pudo completar el registro",
                        icon: "error",
                        confirmButtonText: 'Entendido',
                        confirmButtonColor: '#297ea6'
                    });
                    btnRegistrar.disabled = false;
                    btnText.classList.remove('d-none');
                    btnSpinner.classList.add('d-none');
                }
            })
            .catch(error => {
                console.error('Error:', error);
                Swal.fire({
                    title: "Error",
                    text: "Error al procesar el registro. Por favor, intente nuevamente.",
                    icon: "error",
                    confirmButtonText: 'Entendido',
                    confirmButtonColor: '#297ea6'
                });
                btnRegistrar.disabled = false;
                btnText.classList.remove('d-none');
                btnSpinner.classList.add('d-none');
            });
    });

    // Validación en tiempo real para todos los campos
    const inputs = [nombre, apellido1, apellido2, cedula, telefono, correo, sede, password, confirmPassword];
    inputs.forEach(input => {
        input.addEventListener('blur', function () {
            if (this.checkValidity()) {
                this.classList.remove('is-invalid');
                this.classList.add('is-valid');
            } else {
                this.classList.remove('is-valid');
                this.classList.add('is-invalid');
            }
        });
    });
});
