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

    // Inicializar Select2 para el dropdown de estudiantes (múltiple)
    function inicializarSelect2Estudiantes() {
        if (inputs.estudiante && typeof jQuery !== 'undefined' && jQuery.fn.select2) {
            // Verificar si ya está inicializado
            if (!jQuery('#estudianteSelect').hasClass('select2-hidden-accessible')) {
                jQuery('#estudianteSelect').select2({
                    placeholder: 'Seleccione un proyecto primero',
                    allowClear: true,
                    width: '100%',
                    language: {
                        noResults: function() {
                            return "No hay estudiantes asignados a este proyecto";
                        }
                    }
                });
            }
        }
    }

    // Función para cargar estudiantes de un proyecto
    function cargarEstudiantesDelProyecto(proyectoId, estudiantesSeleccionadosIds = null) {
        if (!inputs.estudiante) {
            console.error('El select de estudiante no existe');
            return;
        }
        
        const estudianteSelect = inputs.estudiante;
        
        // Limpiar el dropdown
        estudianteSelect.innerHTML = '';
        
        if (proyectoId && proyectoId !== '') {
            console.log('Cargando estudiantes para proyecto:', proyectoId);
            fetch(`/Tareas/ObtenerEstudiantesPorProyecto?proyectoId=${proyectoId}`)
                .then(response => {
                    console.log('Response status:', response.status);
                    if (!response.ok) {
                        throw new Error(`HTTP error! status: ${response.status}`);
                    }
                    return response.json();
                })
                .then(resultado => {
                    console.log('Resultado completo:', resultado);
                    if (resultado.ok && resultado.valorRetorno && resultado.valorRetorno.length > 0) {
                        // Agregar estudiantes al dropdown
                        // Manejar tanto camelCase (id) como PascalCase (Id)
                        resultado.valorRetorno.forEach(estudiante => {
                            const option = document.createElement('option');
                            const estudianteId = estudiante.id || estudiante.Id;
                            const estudianteNombre = estudiante.nombreCompleto || estudiante.NombreCompleto;
                            option.value = estudianteId;
                            option.textContent = estudianteNombre;
                            
                            // Si hay estudiantes seleccionados previamente, marcarlos
                            if (estudiantesSeleccionadosIds && Array.isArray(estudiantesSeleccionadosIds)) {
                                // Comparar como números para evitar problemas de tipo
                                const estudianteIdNum = parseInt(estudianteId);
                                if (estudiantesSeleccionadosIds.some(id => parseInt(id) === estudianteIdNum)) {
                                    option.selected = true;
                                }
                            } else if (estudiantesSeleccionadosIds && parseInt(estudiantesSeleccionadosIds) === parseInt(estudianteId)) {
                                option.selected = true;
                            }
                            
                            estudianteSelect.appendChild(option);
                        });
                        
                        // Actualizar Select2 después de agregar las opciones
                        if (typeof jQuery !== 'undefined' && jQuery.fn.select2) {
                            try {
                                // Establecer valores seleccionados si hay
                                if (estudiantesSeleccionadosIds && estudiantesSeleccionadosIds.length > 0) {
                                    const idsNumericos = estudiantesSeleccionadosIds.map(id => parseInt(id));
                                    jQuery('#estudianteSelect').val(idsNumericos).trigger('change');
                                } else {
                                    jQuery('#estudianteSelect').val(null).trigger('change');
                                }
                            } catch (e) {
                                console.warn('Error al actualizar Select2:', e);
                            }
                        }
                        
                        console.log(`Se cargaron ${resultado.valorRetorno.length} estudiantes`);
                    } else {
                        const option = document.createElement('option');
                        option.value = '';
                        option.textContent = 'No hay estudiantes asignados a este proyecto';
                        option.disabled = true;
                        estudianteSelect.appendChild(option);
                        console.log('No hay estudiantes asignados al proyecto. Resultado:', resultado);
                    }
                })
                .catch(error => {
                    console.error('Error al obtener estudiantes:', error);
                    const option = document.createElement('option');
                    option.value = '';
                    option.textContent = 'Error al cargar estudiantes';
                    option.disabled = true;
                    estudianteSelect.appendChild(option);
                });
        } else {
            // Si no hay proyecto seleccionado, mostrar mensaje
            const option = document.createElement('option');
            option.value = '';
            option.textContent = 'Seleccione un proyecto primero';
            option.disabled = true;
            estudianteSelect.appendChild(option);
        }
    }

    // Inicializar Select2 para estudiantes (después de que el DOM esté listo)
    // Usar setTimeout para asegurar que jQuery y Select2 estén cargados
    setTimeout(function() {
        inicializarSelect2Estudiantes();
    }, 100);

    // Inicializar eventos para las tarjetas existentes
    document.querySelectorAll('.task-card').forEach(card => {
        attachCardEvents(card);
    });

    // Actualizar dropdown de estudiantes cuando se selecciona un proyecto
    if (inputs.proyecto && inputs.estudiante) {
        inputs.proyecto.addEventListener('change', function() {
            const proyectoId = this.value;
            console.log('Proyecto seleccionado:', proyectoId);
            // Limpiar la selección del estudiante al cambiar de proyecto
            cargarEstudiantesDelProyecto(proyectoId);
        });
    } else {
        console.error('No se encontraron los elementos proyecto o estudiante en el formulario');
    }

    // Manejar el envío del formulario (Crear / Editar)
    form.addEventListener('submit', function (event) {
        event.preventDefault();

        const proyectoSelect = inputs.proyecto;
        const selectedOption = proyectoSelect.options[proyectoSelect.selectedIndex];
        const proyectoNombre = selectedOption.getAttribute('data-nombre') || selectedOption.text;

        // Obtener estudiantes seleccionados (múltiples)
        let estudiantesSeleccionados = [];
        let estudianteNombre = "";
        if (inputs.estudiante) {
            // Obtener valores seleccionados (puede ser array o string)
            const estudiantesVal = inputs.estudiante.value || (typeof jQuery !== 'undefined' ? jQuery('#estudianteSelect').val() : null);
            if (estudiantesVal) {
                estudiantesSeleccionados = Array.isArray(estudiantesVal) ? estudiantesVal : [estudiantesVal];
                
                // Obtener nombres de los estudiantes seleccionados
                const nombres = [];
                estudiantesSeleccionados.forEach(id => {
                    const option = inputs.estudiante.querySelector(`option[value="${id}"]`);
                    if (option) {
                        nombres.push(option.textContent);
                    }
                });
                estudianteNombre = nombres.join(', ');
            }
        }

        // Datos para enviar al servidor (solo propiedades de la entidad)
        const data = {
            titulo: inputs.titulo.value.trim(),
            proyectoId: parseInt(inputs.proyecto.value),
            descripcion: inputs.descripcion.value.trim(),
            fechaLimite: inputs.fechaLimite.value,
            estado: editingCard ? editingCard.dataset.estado : 'pendiente'
        };

        // Agregar estudiantes asignados (puede ser uno o varios)
        if (estudiantesSeleccionados.length > 0) {
            // Si solo hay uno, mantener compatibilidad con el campo existente
            if (estudiantesSeleccionados.length === 1) {
                data.estudianteAsignadoId = parseInt(estudiantesSeleccionados[0]);
            } else {
                // Si hay múltiples, usar el primero como principal y agregar lista
                data.estudianteAsignadoId = parseInt(estudiantesSeleccionados[0]);
                data.estudiantesAsignadosIds = estudiantesSeleccionados.map(id => parseInt(id));
            }
        }

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
        const proyectoId = card.dataset.proyectoid || '';
        const tareaId = card.dataset.id || '';
        inputs.descripcion.value = card.dataset.descripcion || '';
        inputs.fechaLimite.value = card.dataset.fechalimite || '';

        // Establecer el proyecto primero
        inputs.proyecto.value = proyectoId;
        
        // Obtener todos los estudiantes asignados a la tarea desde el servidor
        if (tareaId && proyectoId) {
            fetch(`/Tareas/ObtenerEstudiantesTarea?tareaId=${tareaId}`)
                .then(response => response.json())
                .then(resultado => {
                    if (resultado.ok && resultado.valorRetorno && resultado.valorRetorno.length > 0) {
                        // Extraer IDs de los estudiantes
                        const estudiantesIds = resultado.valorRetorno.map(e => e.id || e.Id);
                        // Cargar estudiantes del proyecto y seleccionar los asignados
                        cargarEstudiantesDelProyecto(proyectoId, estudiantesIds);
                    } else {
                        // Si no hay estudiantes asignados, solo cargar la lista del proyecto
                        cargarEstudiantesDelProyecto(proyectoId);
                    }
                })
                .catch(error => {
                    console.error('Error al obtener estudiantes de la tarea:', error);
                    // En caso de error, cargar la lista del proyecto sin selección
                    cargarEstudiantesDelProyecto(proyectoId);
                });
        } else if (proyectoId) {
            // Si no hay tareaId, solo cargar la lista del proyecto
            cargarEstudiantesDelProyecto(proyectoId);
        }

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

    // Función para limpiar el formulario
    function clearForm() {
        inputs.titulo.value = '';
        inputs.proyecto.value = '';
        inputs.descripcion.value = '';
        inputs.fechaLimite.value = '';
        
        // Limpiar Select2 de estudiantes
        if (inputs.estudiante && typeof jQuery !== 'undefined' && jQuery.fn.select2) {
            jQuery('#estudianteSelect').val(null).trigger('change');
            inputs.estudiante.innerHTML = '<option value="">Seleccione un proyecto primero</option>';
        } else if (inputs.estudiante) {
            inputs.estudiante.innerHTML = '<option value="">Seleccione un proyecto primero</option>';
        }
        
        editingCard = null;
        form.dataset.editing = 'false';
        if (submitLabel) {
            submitLabel.textContent = 'Crear';
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
        
        // Limpiar Select2 de estudiantes
        if (inputs.estudiante && typeof jQuery !== 'undefined' && jQuery.fn.select2) {
            jQuery('#estudianteSelect').val(null).trigger('change');
            // Limpiar opciones y dejar solo el placeholder
            inputs.estudiante.innerHTML = '<option value="">Seleccione un proyecto primero</option>';
        } else if (inputs.estudiante) {
            inputs.estudiante.innerHTML = '<option value="">Seleccione un proyecto primero</option>';
        }
        
        delete form.dataset.editing;
        editingCard = null;
        if (submitLabel) {
            submitLabel.textContent = 'Crear';
        }
    }

    // Verificar estado inicial
    ensureEmptyState();
});
