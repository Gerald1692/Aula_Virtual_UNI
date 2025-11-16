document.addEventListener('DOMContentLoaded', function () {
    const form = document.querySelector('.project-form');
    const list = document.getElementById('projectList');
    const emptyState = document.getElementById('emptyState');
    const submitLabel = form?.querySelector('.btn-create span.label-text');
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

    function ensureEmptyState() {
        if (list.children.length === 0 && !document.getElementById('emptyState')) {
            const message = document.createElement('p');
            message.className = 'empty-state';
            message.id = 'emptyState';
            message.textContent = 'Aún no hay proyectos creados.';
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
        const card = document.createElement('article');
        card.className = 'project-card';
        card.dataset.nombre = data.nombre;
        card.dataset.curso = data.curso;
        card.dataset.descripcion = data.descripcion;
        card.dataset.fecha = data.fecha;

        const header = document.createElement('div');
        header.className = 'project-card-header';

        const title = document.createElement('h3');
        title.className = 'project-title';
        title.textContent = data.nombre;

        const date = document.createElement('span');
        date.className = 'project-date';
        date.textContent = formatDate(data.fecha);

        header.appendChild(title);
        header.appendChild(date);

        const course = document.createElement('p');
        course.className = 'project-course';
        const courseIcon = document.createElement('i');
        courseIcon.className = 'fa-solid fa-graduation-cap';
        course.appendChild(courseIcon);
        const courseText = document.createElement('span');
        courseText.className = 'course-text';
        courseText.textContent = data.curso;
        course.appendChild(courseText);

        const description = document.createElement('p');
        description.className = 'project-description';
        description.textContent = data.descripcion;

        const actions = document.createElement('div');
        actions.className = 'project-card-actions';

        const editButton = document.createElement('button');
        editButton.type = 'button';
        editButton.className = 'btn-action btn-edit';
        editButton.innerHTML = '<i class="fa-solid fa-pen-to-square"></i> Editar';
        editButton.addEventListener('click', function () {
            populateForm(card);
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

        actions.appendChild(editButton);
        actions.appendChild(deleteButton);

        card.appendChild(header);
        card.appendChild(course);
        card.appendChild(description);
        card.appendChild(actions);
        return card;
    }

    function updateCard(card, data) {
        card.dataset.nombre = data.nombre;
        card.dataset.curso = data.curso;
        card.dataset.descripcion = data.descripcion;
        card.dataset.fecha = data.fecha;

        card.querySelector('.project-title').textContent = data.nombre;
        card.querySelector('.project-date').textContent = formatDate(data.fecha);
        const courseText = card.querySelector('.course-text');
        if (courseText) {
            courseText.textContent = data.curso;
        }
        card.querySelector('.project-description').textContent = data.descripcion;
    }

    async function cargarProyectos() {
        try {
            const response = await fetch('/api/proyectos');
            if (!response.ok) {
                throw new Error('No se pudieron cargar los proyectos.');
            }

            const proyectos = await response.json();
            list.innerHTML = '';

            proyectos.forEach((p) => {
                const card = createCardElements({
                    nombre: p.nombre,
                    curso: p.curso,
                    descripcion: p.descripcion,
                    fecha: p.fechaEntrega?.split('T')[0] ?? ''
                });
                list.appendChild(card);
            });

            ensureEmptyState();
        } catch (error) {
            console.error(error);
            ensureEmptyState();
        }
    }

    form.addEventListener('submit', async function (event) {
        event.preventDefault();

        const data = {
            nombre: inputs.nombre.value.trim(),
            curso: inputs.curso.value.trim(),
            descripcion: inputs.descripcion.value.trim(),
            fechaEntrega: inputs.fecha.value
        };

        if (!data.nombre || !data.curso || !data.descripcion || !data.fechaEntrega) {
            return;
        }

        try {
            const response = await fetch('/api/proyectos', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            });

            if (!response.ok) {
                throw new Error('No se pudo guardar el proyecto.');
            }

            const saved = await response.json();

            if (editingCard) {
                updateCard(editingCard, {
                    nombre: saved.nombre,
                    curso: saved.curso,
                    descripcion: saved.descripcion,
                    fecha: saved.fechaEntrega?.split('T')[0] ?? data.fechaEntrega
                });
            } else {
                const emptyEl = document.getElementById('emptyState');
                if (emptyEl) {
                    emptyEl.remove();
                }
                const card = createCardElements({
                    nombre: saved.nombre,
                    curso: saved.curso,
                    descripcion: saved.descripcion,
                    fecha: saved.fechaEntrega?.split('T')[0] ?? data.fechaEntrega
                });
                list.appendChild(card);
            }

            clearForm();
            ensureEmptyState();
        } catch (error) {
            console.error(error);
        }
    });

    cargarProyectos();
    ensureEmptyState();
});