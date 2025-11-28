// Validación y manejo del formulario de registro
document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('formRegistro');
    const btnRegistrar = document.getElementById('btnRegistrar');
    const btnText = document.getElementById('btnText');
    const btnSpinner = document.getElementById('btnSpinner');
    const alertMessage = document.getElementById('alertMessage');

    // Campos del formulario
    const nombre = document.getElementById('nombre');
    const apellido = document.getElementById('apellido');
    const matricula = document.getElementById('matricula');
    const carrera = document.getElementById('carrera');
    const email = document.getElementById('email');
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

    // Verificar fortaleza de contraseña
    password.addEventListener('input', function () {
        const strengthIndicator = document.getElementById('passwordStrength');
        const value = this.value;
        let strength = 0;
        let message = '';
        let color = '';

        if (value.length >= 6) strength++;
        if (value.length >= 10) strength++;
        if (/[a-z]/.test(value) && /[A-Z]/.test(value)) strength++;
        if (/\d/.test(value)) strength++;
        if (/[^a-zA-Z0-9]/.test(value)) strength++;

        if (value.length === 0) {
            message = '';
        } else if (strength <= 2) {
            message = 'Contraseña débil';
            color = 'text-danger';
        } else if (strength <= 3) {
            message = 'Contraseña media';
            color = 'text-warning';
        } else {
            message = 'Contraseña fuerte';
            color = 'text-success';
        }

        strengthIndicator.textContent = message;
        strengthIndicator.className = 'form-text ' + color;
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

    // Verificar matrícula existente (debounce)
    let matriculaTimeout;
    matricula.addEventListener('input', function () {
        clearTimeout(matriculaTimeout);
        const feedback = document.getElementById('matriculaFeedback');
        const value = this.value.trim();

        if (value.length < 3) {
            feedback.textContent = '';
            return;
        }

        matriculaTimeout = setTimeout(() => {
            fetch('/CrearCuenta/VerificarMatricula', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
                body: 'matricula=' + encodeURIComponent(value)
            })
                .then(response => response.json())
                .then(data => {
                    if (data.existe) {
                        feedback.textContent = '⚠️ Esta matrícula ya está registrada';
                        feedback.className = 'form-text text-danger';
                        matricula.setCustomValidity('Matrícula ya registrada');
                    } else {
                        feedback.textContent = '✓ Matrícula disponible';
                        feedback.className = 'form-text text-success';
                        matricula.setCustomValidity('');
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    feedback.textContent = '';
                });
        }, 500);
    });

    // Mostrar mensaje de alerta
    function showAlert(message, type) {
        alertMessage.textContent = message;
        alertMessage.className = 'alert alert-' + type;
        alertMessage.classList.remove('d-none');

        // Auto-ocultar después de 5 segundos
        setTimeout(() => {
            alertMessage.classList.add('d-none');
        }, 5000);
    }

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
            showAlert('Las contraseñas no coinciden', 'danger');
            return;
        }

        // Deshabilitar botón y mostrar spinner
        btnRegistrar.disabled = true;
        btnText.classList.add('d-none');
        btnSpinner.classList.remove('d-none');

        // Preparar datos
        const formData = new URLSearchParams();
        formData.append('nombre', nombre.value.trim());
        formData.append('apellido', apellido.value.trim());
        formData.append('matricula', matricula.value.trim());
        formData.append('carrera', carrera.value.trim());
        formData.append('email', email.value.trim());
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
                    showAlert('✓ ' + data.mensaje + ' Redirigiendo...', 'success');

                    // Redirigir al menú principal después de 2 segundos
                    setTimeout(() => {
                        window.location.href = '/Menu/Menu';
                    }, 2000);
                } else {
                    showAlert('⚠️ ' + data.mensaje, 'danger');
                    btnRegistrar.disabled = false;
                    btnText.classList.remove('d-none');
                    btnSpinner.classList.add('d-none');
                }
            })
            .catch(error => {
                console.error('Error:', error);
                showAlert('⚠️ Error al procesar el registro. Por favor, intente nuevamente.', 'danger');
                btnRegistrar.disabled = false;
                btnText.classList.remove('d-none');
                btnSpinner.classList.add('d-none');
            });
    });

    // Validación en tiempo real para todos los campos
    const inputs = [nombre, apellido, matricula, carrera, email, password, confirmPassword];
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
