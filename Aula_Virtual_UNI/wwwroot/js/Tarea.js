document.addEventListener('DOMContentLoaded', function () {
    const form = document.querySelector('.task-form');
    const list = document.getElementById('taskList');
    const submitLabel = form?.querySelector('.btn-create span.label-text');
    let editingCard = null;

    const STATUS_LABELS = {
        pendiente: 'Pendiente',
        terminada: 'Terminada',
        en_progreso: 'En Progreso'
    };

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
        if (list.children.length === 0 && !document.getElementById('emptyState')) {
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
        const normalizedEstado = normalizeStatus(data.estado || 'pendiente');

        const card = document.createElement('article');
        card.className = 'task-card';
        card.dataset.nombre = data.nombre;
        card.dataset.curso = data.curso;
        card.dataset.descripcion = data.descripcion;
        card.dataset.fecha = data.fecha;
        card.dataset.estado = normalizedEstado;

        const header = document.createElement('div');
        header.className = 'task-card-header';

        const title = document.createElement('h3');
        title.className = 'task-title';
        title.textContent = data.nombre;

        const meta = document.createElement('div');
        meta.className = 'task-meta';

        const statusBadge = document.createElement('span');
        statusBadge.className = 'task-status';

        const date = document.createElement('span');
        date.className = 'task-date';
        date.textContent = formatDate(data.fecha);

        meta.appendChild(statusBadge);
        meta.appendChild(date);

        header.appendChild(title);
        header.appendChild(meta);

        const course = document.createElement('p');
        course.className = 'task-course';
        const courseIcon = document.createElement('i');
        courseIcon.className = 'fa-solid fa-graduation-cap';
        course.appendChild(courseIcon);
        const courseText = document.createElement('span');
        courseText.className = 'course-text';
        courseText.textContent = data.curso;
        course.appendChild(courseText);

        const description = document.createElement('p');
        description.className = 'task-description';
        description.textContent = data.descripcion;

        const actions = document.createElement('div');
        actions.className = 'task-card-actions';

        // Create dropdown container
        const dropdownContainer = document.createElement('div');
        dropdownContainer.className = 'container';

        const dropdown = document.createElement('div');
        dropdown.className = 'dropdown';

        const dropdownButton = document.createElement('button');
        dropdownButton.className = 'btn btn-primary dropdown-toggle';
        dropdownButton.type = 'button';
        dropdownButton.id = 'dropdown' + Date.now();
        dropdownButton.setAttribute('data-toggle', 'dropdown');
        dropdownButton.textContent = 'Asignar Alumnos';

        const dropdownMenu = document.createElement('div');
        dropdownMenu.className = 'dropdown-menu';

        // Add sample dropdown items (you can modify this to load actual students)
        const item1 = document.createElement('a');
        item1.className = 'dropdown-item';
        item1.href = 'http://www.google.com';
        item1.textContent = 'Google';

        const item2 = document.createElement('a');
        item2.className = 'dropdown-item';
        item2.href = 'http://www.bing.com';
        item2.textContent = 'Bing';

        const item3 = document.createElement('a');
        item3.className = 'dropdown-item';
        item3.href = 'http://www.yahoo.com';
        item3.textContent = 'Yahoo';

        dropdownMenu.appendChild(item1);
        dropdownMenu.appendChild(item2);
        dropdownMenu.appendChild(item3);

        dropdown.appendChild(dropdownButton);
        dropdown.appendChild(dropdownMenu);
        dropdownContainer.appendChild(dropdown);

        const editButton = document.createElement('button');
        editButton.type = 'button';
        editButton.className = 'btn-action btn-edit';
        editButton.innerHTML = '<i class="fa-solid fa-pen-to-square"></i> Editar';
        editButton.addEventListener('click', function () {
            populateForm(card);
        });

        const statusButton = document.createElement('button');
        statusButton.type = 'button';
        statusButton.className = 'btn-action btn-status';
        statusButton.addEventListener('click', function () {
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

        const deleteButton = document.createElement('button');
        deleteButton.type = 'button';
        deleteButton.className = 'btn-action btn-delete';
        deleteButton.innerHTML = '<i class="fa-solid fa-trash"></i> Eliminar';
        deleteButton.addEventListener('click', function () {
            if (editingCard === card) {
                clearForm();
            }
            card.remove();
            ensureEmptyState();
        });

        actions.appendChild(dropdownContainer);
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

        applyStatus(card, card.dataset.estado || 'pendiente');
    }

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
            updateCard(editingCard, data);
        } else {
            const emptyEl = document.getElementById('emptyState');
            if (emptyEl) {
                emptyEl.remove();
            }

            console.log(data)

            fetch('../Tareas/InsertarTarea', {
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





                })

            const card = createCardElements(data);
            list.appendChild(card);
        }

        clearForm();
        ensureEmptyState();
    });

    ensureEmptyState();
});
