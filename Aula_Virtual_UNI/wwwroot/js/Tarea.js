document.addEventListener('DOMContentLoaded', function () {
    const form = document.querySelector('.task-form');
    const taskList = document.getElementById('taskList');
    const emptyState = document.getElementById('emptyState');
    const template = document.getElementById('task-card-template');
    let editingCard = null;
    let submitLabel = null;

    // Referencias a los inputs del formulario
    const inputs = {
        titulo: form.elements['titulo'],
        proyecto: form.elements['proyecto'],
        estudiante: form.elements['estudiante'],
        descripcion: form.elements['descripcion'],
        fechaLimite: form.elements['fechaLimite']
    };

    // Buscar el botón de submit para cambiar su texto
    const submitBtn = form.querySelector('button[type="submit"]');
    if (submitBtn) {
        submitLabel = submitBtn.querySelector('.label-text');
    }

    // Inicializar eventos para las tarjetas existentes
    document.querySelectorAll('.task-card').forEach(card => {
        attachCardEvents(card);
    });

    // Manejar el envío del formulario (Crear / Editar)
    form.addEventListener('submit', function (event) {
        event.preventDefault();

        const proyectoSelect = inputs.proyecto;
        const selectedOption = proyectoSelect.options[proyectoSelect.selectedIndex];
        const proyectoNombre = selectedOption.getAttribute('data-nombre') || selectedOption.text;

        const estudianteSelect = inputs.estudiante;
        const selectedEstudianteOption = estudianteSelect.options[estudianteSelect.selectedIndex];
        const estudianteNombre = selectedEstudianteOption.text;

        // Datos para enviar al servidor (solo propiedades de la entidad)
        const data = {
            titulo: inputs.titulo.value.trim(),
            proyectoId: parseInt(inputs.proyecto.value),
            estudianteAsignadoId: parseInt(inputs.estudiante.value),
            descripcion: inputs.descripcion.value.trim(),
            fechaLimite: inputs.fechaLimite.value,
            estado: editingCard ? editingCard.dataset.estado : 'pendiente'
        };

        // Datos adicionales para la UI
        const uiData = {
            ...data,
            proyectoNombre: proyectoNombre,
            nombreAsignado: estudianteNombre
        };

        if (form.dataset.editing === 'true' && editingCard) {
            // Modo Edición: Actualizar tarjeta existente

            // Agregar ID de la tarea al objeto data
            const updateData = {
                ...data,
                id: parseInt(editingCard.dataset.id)
            };

            // Llamada AJAX para actualizar en BD
            fetch('../Tareas/ActualizarTarea', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(updateData)
            })
                .then(response => response.json())
                .then(resultado => {
                    if (resultado.ok) {
                        updateCard(editingCard, uiData);
                        clearForm();
                        Swal.fire({
                            title: "¡Actualizado!",
                            text: resultado.mensaje,
                            icon: "success"
                        });
                    } else {
                        Swal.fire({
                            title: "Error",
                            text: resultado.mensaje,
                            icon: "error"
                        });
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    Swal.fire({
                        title: "Error",
                        text: "Ocurrió un error al actualizar la tarea",
                        icon: "error"
                    });
                });

        } else {
            // Modo Creación: Crear nueva tarjeta

            // Enviar al servidor
            fetch('../Tareas/InsertarTarea', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            })
                .then(response => response.json())
                .then(resultado => {
                    if (resultado.ok) {
                        // Actualizar el ID en los datos de la UI con el ID devuelto por el servidor
                        if (resultado.valorRetorno && resultado.valorRetorno.id) {
                            uiData.id = resultado.valorRetorno.id;
                        }

                        createCardFromTemplate(uiData);
                        ensureEmptyState();
                        clearForm();

                        Swal.fire({
                            title: "¡Éxito!",
                            text: resultado.mensaje,
                            icon: "success"
                        });
                    } else {
                        Swal.fire({
                            title: "Error",
                            text: resultado.mensaje,
                            icon: "error"
                        });
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    Swal.fire({
                        title: "Error",
                        text: "Ocurrió un error al procesar la solicitud",
                        icon: "error"
                    });
                });
        }
    });

    // Función para crear una tarjeta usando el template HTML
    function createCardFromTemplate(data) {
        const clone = template.content.cloneNode(true);
        const card = clone.querySelector('.task-card');

        // Llenar datos visuales
        fillCardData(card, data);

        // Configurar atributos de datos para persistencia en DOM
        setCardAttributes(card, data);

        // Adjuntar eventos
        attachCardEvents(card);

        // Insertar en la lista
        taskList.appendChild(card);
    }

    // Función para actualizar una tarjeta existente
    function updateCard(card, data) {
        fillCardData(card, data);
        setCardAttributes(card, data);
    }

    // Función auxiliar para llenar los elementos visuales de la tarjeta
    function fillCardData(card, data) {
        card.querySelector('.task-title').textContent = data.titulo;
        card.querySelector('.task-status').textContent = data.estado;

        // Formatear fecha para mostrar
        const fechaObj = new Date(data.fechaLimite);
        // Ajuste de zona horaria simple para visualización
        const fechaUser = new Date(fechaObj.getTime() + fechaObj.getTimezoneOffset() * 60000);

        const opciones = { day: 'numeric', month: 'short', year: 'numeric' };
        card.querySelector('.task-date').textContent = fechaUser.toLocaleDateString('es-ES', opciones);

        card.querySelector('.course-text').textContent = data.proyectoNombre;
        card.querySelector('.task-description').textContent = data.descripcion;

        // Mostrar estudiante asignado si existe
        const assignedText = card.querySelector('.assigned-text');
        if (assignedText) {
            assignedText.textContent = data.nombreAsignado;
            // Mostrar/ocultar contenedor si es necesario
            const assignedContainer = card.querySelector('.task-assigned');
            if (assignedContainer) {
                assignedContainer.style.display = data.nombreAsignado ? 'block' : 'none';
            }
        }

        applyStatus(card, data.estado);
    }

    // Función auxiliar para establecer atributos data-*
    function setCardAttributes(card, data) {
        card.dataset.titulo = data.titulo;
        card.dataset.proyectoid = data.proyectoId;
        card.dataset.proyecto = data.proyectoNombre;
        card.dataset.estudianteid = data.estudianteAsignadoId;
        card.dataset.nombreasignado = data.nombreAsignado;
        card.dataset.descripcion = data.descripcion;
        card.dataset.fechalimite = data.fechaLimite;
        card.dataset.estado = data.estado;
        // No sobrescribimos el ID si ya existe
        if (data.id) card.dataset.id = data.id;
    }

    // Función para adjuntar eventos a los botones de una tarjeta
    function attachCardEvents(card) {
        const btnEdit = card.querySelector('.btn-edit');
        const btnStatus = card.querySelector('.btn-status');
        const btnDelete = card.querySelector('.btn-delete');

        // Editar
        btnEdit.addEventListener('click', () => {
            populateForm(card);
            window.scrollTo({ top: 0, behavior: 'smooth' });
        });

        // Cambiar Estado
        btnStatus.addEventListener('click', () => {
            const currentStatus = card.dataset.estado;
            let newStatus;

            if (currentStatus === 'pendiente') newStatus = 'en progreso';
            else if (currentStatus === 'en progreso') newStatus = 'completada';
            else newStatus = 'pendiente';

            // Llamada AJAX para actualizar estado
            const taskId = parseInt(card.dataset.id);

            fetch('../Tareas/ActualizarEstadoTarea', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ id: taskId, estado: newStatus })
            })
                .then(response => response.json())
                .then(resultado => {
                    if (resultado.ok) {
                        card.dataset.estado = newStatus;
                        card.querySelector('.task-status').textContent = newStatus;
                        applyStatus(card, newStatus);

                        const Toast = Swal.mixin({
                            toast: true,
                            position: "top-end",
                            showConfirmButton: false,
                            timer: 3000,
                            timerProgressBar: true,
                        });
                        Toast.fire({
                            icon: "success",
                            title: "Estado actualizado"
                        });
                    } else {
                        Swal.fire({
                            title: "Error",
                            text: "No se pudo actualizar el estado",
                            icon: "error"
                        });
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                });
        });

        // Eliminar
        btnDelete.addEventListener('click', () => {
            const taskId = parseInt(card.dataset.id);
            
            if (!taskId) {
                // Si no hay ID, solo eliminar del DOM (tarea nueva no guardada)
                if (editingCard === card) {
                    clearForm();
                }
                card.remove();
                ensureEmptyState();
                return;
            }

            // Confirmar eliminación
            Swal.fire({
                title: '¿Estás seguro?',
                text: 'Esta acción no se puede deshacer',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6',
                confirmButtonText: 'Sí, eliminar',
                cancelButtonText: 'Cancelar'
            }).then((result) => {
                if (result.isConfirmed) {
                    // Llamar al servidor para eliminar
                    fetch('../Tareas/EliminarTarea', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify(taskId)
                    })
                    .then(response => {
                        if (!response.ok) {
                            throw new Error(`HTTP error! status: ${response.status}`);
                        }
                        return response.json();
                    })
                    .then(resultado => {
                        if (resultado.ok) {
                            if (editingCard === card) {
                                clearForm();
                            }
                            card.remove();
                            ensureEmptyState();
                            
                            Swal.fire({
                                title: '¡Eliminado!',
                                text: resultado.mensaje || 'La tarea ha sido eliminada.',
                                icon: 'success',
                                confirmButtonColor: '#297ea6'
                            });
                        } else {
                            Swal.fire({
                                title: 'Error',
                                text: resultado.mensaje || 'No se pudo eliminar la tarea.',
                                icon: 'error',
                                confirmButtonColor: '#297ea6'
                            });
                        }
                    })
                    .catch(error => {
                        console.error('Error:', error);
                        Swal.fire({
                            title: 'Error',
                            text: 'Ocurrió un error al eliminar la tarea: ' + error.message,
                            icon: 'error',
                            confirmButtonColor: '#297ea6'
                        });
                    });
                }
            });
        });
    }

    // Función para llenar el formulario con datos de una tarjeta (Modo Edición)
    function populateForm(card) {
        inputs.titulo.value = card.dataset.titulo || '';
        inputs.proyecto.value = card.dataset.proyectoid || '';
        inputs.estudiante.value = card.dataset.estudianteid || '';
        inputs.descripcion.value = card.dataset.descripcion || '';
        inputs.fechaLimite.value = card.dataset.fechalimite || '';

        editingCard = card;
        form.dataset.editing = 'true';
        if (submitLabel) {
            submitLabel.textContent = 'Actualizar';
        }
    }

    // Función para aplicar estilos según el estado
    function applyStatus(card, status) {
        const statusBadge = card.querySelector('.task-status');
        const btnStatus = card.querySelector('.btn-status');

        // Resetear clases
        statusBadge.className = 'task-status';

        // Aplicar clases según estado
        if (status === 'pendiente') {
            statusBadge.classList.add('is-pending');
            btnStatus.innerHTML = '<i class="fa-solid fa-flag"></i> Estado: Pendiente';
        } else if (status === 'en progreso') {
            statusBadge.classList.add('is-progress');
            btnStatus.innerHTML = '<i class="fa-solid fa-spinner"></i> Estado: En Progreso';
        } else if (status === 'completada') {
            statusBadge.classList.add('is-completed');
            btnStatus.innerHTML = '<i class="fa-solid fa-check"></i> Estado: Completada';
        }
    }

    // Función para manejar el estado vacío de la lista
    function ensureEmptyState() {
        const hasCards = taskList.querySelectorAll('.task-card').length > 0;
        if (hasCards) {
            emptyState.style.display = 'none';
        } else {
            emptyState.style.display = 'block';
        }
    }

    // Función para limpiar el formulario
    function clearForm() {
        form.reset();
        delete form.dataset.editing;
        editingCard = null;
        if (submitLabel) {
            submitLabel.textContent = 'Crear';
        }
    }

    // Verificar estado inicial
    ensureEmptyState();
});
