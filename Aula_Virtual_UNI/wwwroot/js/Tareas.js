document.addEventListener('DOMContentLoaded', () => {
    const form = document.querySelector('.project-form');
    const list = document.getElementById('projectList');
    const emptyState = () => document.getElementById('emptyState');
    const submitLabel = form?.querySelector('.btn-create .label-text');
    const STATUS_LABELS = {
        pendiente: 'Pendiente',
        terminada: 'Terminada'
    };
    let editingCard = null;

    if (!form || !list) {
        return;
    }

    const inputs = {
        nombre: form.elements['nombre'],
        curso: form.elements['curso'],
        descripcion: form.elements['descripcion'],
        fecha: form.elements['fecha']
    };

    function formatDate(value) {
        const date = new Date(value + 'T00:00:00');
        if (isNaN(date)) {
            return value;
        }
        return date.toLocaleDateString('es-ES', { day: '2-digit', month: 'short', year: 'numeric' });
    }

    function normalizeStatus(value) {
        return value === 'terminada' ? 'terminada' : 'pendiente';
    }

    function updateStatusBadge(badge, estado) {
        const normalized = normalizeStatus(estado);
        badge.textContent = STATUS_LABELS[normalized];
        badge.classList.remove('is-done', 'is-pending');
        badge.classList.add(normalized === 'terminada' ? 'is-done' : 'is-pending');
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
        if (list.children.length === 0 && !emptyState()) {
            const message = document.createElement('p');
            message.className = 'empty-state';
            message.id = 'emptyState';
            message.textContent = 'Aún no hay tareas creadas.';
            list.appendChild(message);
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

    function createCardElements(data) {
        const normalizedEstado = normalizeStatus(data.estado);
        const card = document.createElement('article');
        card.className = 'project-card task-card';
        card.dataset.nombre = data.nombre;
        card.dataset.curso = data.curso;
        card.dataset.descripcion = data.descripcion;
        card.dataset.fecha = data.fecha;
        card.dataset.estado = normalizedEstado;

        const header = document.createElement('div');
        header.className = 'project-card-header task-card-header';

        const title = document.createElement('h3');
        title.className = 'project-title task-title';
        title.textContent = data.nombre;

        const meta = document.createElement('div');
        meta.className = 'task-meta';

        const statusBadge = document.createElement('span');
        statusBadge.className = 'task-status';

        const date = document.createElement('span');
        date.className = 'project-date task-date';
        date.textContent = formatDate(data.fecha);

        meta.appendChild(statusBadge);
        meta.appendChild(date);

        header.appendChild(title);
        header.appendChild(meta);

        const course = document.createElement('p');
        course.className = 'project-course task-course';
        const courseIcon = document.createElement('i');
        courseIcon.className = 'fa-solid fa-graduation-cap';
        const courseText = document.createElement('span');
        courseText.className = 'course-text';
        courseText.textContent = data.curso;
        course.appendChild(courseIcon);
        course.appendChild(courseText);

        const description = document.createElement('p');
        description.className = 'project-description task-description';
        description.textContent = data.descripcion;

        const actions = document.createElement('div');
        actions.className = 'project-card-actions task-card-actions';

        const editButton = document.createElement('button');
        editButton.type = 'button';
        editButton.className = 'btn-action btn-edit';
        editButton.innerHTML = '<i class="fa-solid fa-pen-to-square"></i> Editar';
        editButton.addEventListener('click', () => {
            populateForm(card);
        });

        const statusButton = document.createElement('button');
        statusButton.type = 'button';
        statusButton.className = 'btn-action btn-status';
        statusButton.addEventListener('click', () => {
            const nextState = card.dataset.estado === 'terminada' ? 'pendiente' : 'terminada';
            applyStatus(card, nextState);
            if (editingCard === card) {
                editingCard.dataset.estado = nextState;
            }
        });

        const deleteButton = document.createElement('button');
        deleteButton.type = 'button';
        deleteButton.className = 'btn-action btn-delete';
        deleteButton.innerHTML = '<i class="fa-solid fa-trash"></i> Eliminar';
        deleteButton.addEventListener('click', () => {
            if (editingCard === card) {
                clearForm();
            }
            card.remove();
            ensureEmptyState();
        });

        actions.appendChild(editButton);
        actions.appendChild(statusButton);
        actions.appendChild(deleteButton);

        card.appendChild(header);
        card.appendChild(course);
        card.appendChild(description);
        card.appendChild(actions);

        applyStatus(card, normalizedEstado);
        return card;
    }

    function updateCard(card, data) {
        card.dataset.nombre = data.nombre;
        card.dataset.curso = data.curso;
        card.dataset.descripcion = data.descripcion;
        card.dataset.fecha = data.fecha;
        card.querySelector('.task-title').textContent = data.nombre;
        card.querySelector('.task-date').textContent = formatDate(data.fecha);
        const courseText = card.querySelector('.course-text');
        if (courseText) {
            courseText.textContent = data.curso;
        }
        card.querySelector('.task-description').textContent = data.descripcion;
        applyStatus(card, card.dataset.estado);
    }

    form.addEventListener('submit', (event) => {
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
            updateCard(editingCard, data);
        } else {
            const emptyEl = emptyState();
            if (emptyEl) {
                emptyEl.remove();
            }
            const card = createCardElements(data);
            list.appendChild(card);
        }

        clearForm();
        ensureEmptyState();
    });

    ensureEmptyState();
});