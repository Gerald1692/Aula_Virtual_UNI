document.addEventListener('DOMContentLoaded', function () {
    const form = document.querySelector('.project-form');
    const list = document.getElementById('projectList');
    const submitLabel = form?.querySelector('.btn-create span.label-text');
    const template = document.getElementById('project-card-template');
    let editingCard = null;

    const STATUS_LABELS = {
        pendiente: 'Pendiente',
        terminada: 'Terminada',
        en_progreso: 'En Progreso'
    };

    if (!form || !list || !template) {
        return;
    }

    const inputs = {
        nombre: form.elements['nombre'],
        curso: form.elements['curso'],
        descripcion: form.elements['descripcion'],
        fecha: form.elements['fecha']
    };

    // ========== UTILIDADES ==========
    function formatDate(value) {
        const date = new Date(value + 'T00:00:00');
        if (isNaN(date)) {
            return value;
        }
        return date.toLocaleDateString('es-ES', { day: '2-digit', month: 'short', year: 'numeric' });
    }

    function normalizeStatus(value) {
        if (value === 'terminada') return 'terminada';
        if (value === 'en_progreso') return 'en_progreso';
        return 'pendiente';
    }

    function updateStatusBadge(badge, estado) {
        const normalized = normalizeStatus(estado);
        badge.textContent = STATUS_LABELS[normalized] || '';
        badge.classList.remove('is-done', 'is-pending', 'is-progress');
        if (normalized === 'terminada') {
            badge.classList.add('is-done');
        } else if (normalized === 'en_progreso') {
            badge.classList.add('is-progress');
        } else {
            badge.classList.add('is-pending');
        }
        return normalized;
    }

    function updateStatusButton(button, estado) {
        const normalized = normalizeStatus(estado);
        button.innerHTML = `<i class="fa-solid fa-flag"></i> Estado: ${STATUS_LABELS[normalized]}`;
        button.classList.toggle('is-reset', normalized === 'terminada');
    }

    function applyStatus(card, estado) {
        const normalized = normalizeStatus(estado);
        card.dataset.estado = normalized;

        const badge = card.querySelector('.task-status');
        if (badge) {
            updateStatusBadge(badge, normalized);
        }

        const toggleButton = card.querySelector('.btn-status');
        if (toggleButton) {
            updateStatusButton(toggleButton, normalized);
        }
    }

    function ensureEmptyState() {
        const emptyState = document.getElementById('emptyState');
        const hasCards = list.querySelector('.project-card');
        
        if (!hasCards && !emptyState) {
            const message = document.createElement('p');
            message.className = 'empty-state';
            message.id = 'emptyState';
            message.textContent = 'Aún no hay proyectos creados.';
            list.appendChild(message);
        } else if (hasCards && emptyState) {
            emptyState.remove();
        }
    }

    function clearForm() {
        form.reset();
        editingCard = null;
        form.removeAttribute('data-editing');
        if (submitLabel) {
            submitLabel.textContent = 'Crear';
        }
    }

    function populateForm(card) {
        inputs.nombre.value = card.dataset.nombre || '';
        inputs.curso.value = card.dataset.curso || '';
        inputs.descripcion.value = card.dataset.descripcion || '';
        inputs.fecha.value = card.dataset.fecha || '';
        editingCard = card;
        form.dataset.editing = 'true';
        if (submitLabel) {
            submitLabel.textContent = 'Actualizar';
        }
    }

    // ========== CREAR TARJETA DESDE TEMPLATE ==========
    function createCardFromTemplate(data) {
        const normalizedEstado = normalizeStatus(data.estado || 'pendiente');
        
        // Clonar el template
        const clone = template.content.cloneNode(true);
        const card = clone.querySelector('.project-card');

        // Establecer data attributes
        card.dataset.nombre = data.nombre;
        card.dataset.curso = data.curso;
        card.dataset.descripcion = data.descripcion;
        card.dataset.fecha = data.fecha;
        card.dataset.estado = normalizedEstado;

        // Llenar contenido
        card.querySelector('.project-title').textContent = data.nombre;
        card.querySelector('.project-date').textContent = formatDate(data.fecha);
        card.querySelector('.course-text').textContent = data.curso;
        card.querySelector('.project-description').textContent = data.descripcion;

        // Aplicar estado
        applyStatus(card, normalizedEstado);

        // Agregar event listeners
        attachCardEvents(card);

        return card;
    }

    function updateCard(card, data) {
        card.dataset.nombre = data.nombre;
        card.dataset.curso = data.curso;
        card.dataset.descripcion = data.descripcion;
        card.dataset.fecha = data.fecha;

        card.querySelector('.project-title').textContent = data.nombre;
        card.querySelector('.project-date').textContent = formatDate(data.fecha);
        card.querySelector('.course-text').textContent = data.curso;
        card.querySelector('.project-description').textContent = data.descripcion;

        applyStatus(card, card.dataset.estado || 'pendiente');
    }

    // ========== EVENT LISTENERS PARA TARJETAS ==========
    function attachCardEvents(card) {
        // Botón Editar
        const editBtn = card.querySelector('.btn-edit');
        editBtn.addEventListener('click', function () {
            populateForm(card);
        });

        // Botón Estado
        const statusBtn = card.querySelector('.btn-status');
        statusBtn.addEventListener('click', function () {
            let nextState;
            switch (card.dataset.estado) {
                case 'pendiente':
                    nextState = 'en_progreso';
                    break;
                case 'en_progreso':
                    nextState = 'terminada';
                    break;
                default:
                    nextState = 'pendiente';
            }
            applyStatus(card, nextState);
            if (editingCard === card) {
                editingCard.dataset.estado = nextState;
            }
        });

        // Botón Eliminar
        const deleteBtn = card.querySelector('.btn-delete');
        deleteBtn.addEventListener('click', function () {
            if (editingCard === card) {
                clearForm();
            }
            card.remove();
            ensureEmptyState();
        });
    }

    // ========== INICIALIZAR TARJETAS EXISTENTES ==========
    function initializeExistingCards() {
        const existingCards = list.querySelectorAll('.project-card');
        existingCards.forEach(card => {
            attachCardEvents(card);
            applyStatus(card, card.dataset.estado || 'pendiente');
        });
    }

    // ========== SUBMIT FORM ==========
    form.addEventListener('submit', function (event) {
        event.preventDefault();

        const data = {
            nombre: inputs.nombre.value.trim(),
            curso: inputs.curso.value.trim(),
            descripcion: inputs.descripcion.value.trim(),
            fecha: inputs.fecha.value,
            estado: editingCard ? editingCard.dataset.estado : 'pendiente'
        };

        if (!data.nombre || !data.curso || !data.descripcion || !data.fecha) {
            return;
        }

        if (editingCard) {
            // Actualizar tarjeta existente
            updateCard(editingCard, data);
        } else {
            // Crear nueva tarjeta
            fetch('../Proyectos/InsertarProyecto', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(data)
            })
            .then(response => response.json())
            .then(resultado => {
                if (resultado.ok) {
                    Swal.fire({
                        title: "Éxito!",
                        text: `${resultado.mensaje}`,
                        icon: "success",
                        confirmButtonText: 'Entendido',
                        confirmButtonColor: '#297ea6'
                    });
                } else {
                    Swal.fire({
                        title: "Advertencia",
                        text: `${resultado.mensaje}`,
                        icon: "warning",
                        confirmButtonText: 'Entendido',
                        confirmButtonColor: '#297ea6'
                    });
                }
            });

            const card = createCardFromTemplate(data);
            list.appendChild(card);
        }

        clearForm();
        ensureEmptyState();
    });

    // ========== INICIALIZACIÓN ==========
    initializeExistingCards();
    ensureEmptyState();
});
