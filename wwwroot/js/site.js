// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Modal
document.addEventListener('DOMContentLoaded', () => {
    const estados = [
        { value: 1, text: 'ToDo'},
        { value: 2, text: 'Doing'},
        { value: 3, text: 'Review'},
        { value: 4, text: 'Done'},
    ];

    document.querySelectorAll('[data-estado-de-tarea]').forEach(container => {
        const estadoDeTarea = container.dataset.estadoDeTarea;
        
        const select = container.nextElementSibling.querySelector('#estado-tarea-select');
        if (select) {
            select.innerHTML = '';

            estados.forEach(estado => {
                const option = document.createElement('option');
                option.value = estado.value;
                option.text = estado.text;
                if (estado.text === estadoDeTarea) {
                    option.selected = true; 
                }
                select.appendChild(option);
            });
        }
    });
});

function openModal(autorizarEdyEl, nombreTarea, descripcion, usuarioAsignado, nombreTablero, idTarea, idTablero) {
    document.getElementById('modal-nombre-tarea').innerText = nombreTarea;
    document.getElementById('modal-nombre-tablero').innerText = `( ${nombreTablero} )`;
    document.getElementById('modal-usuario-asignado').innerText = usuarioAsignado;
    document.getElementById('modal-descripcion').innerText = descripcion;

    if(autorizarEdyEl)
    {
        const editarEnlace = document.getElementById('modal-editar');
        editarEnlace.href = `/Tarea/EditarTarea?idTarea=${idTarea}`;

        const eliminarEnlace = document.getElementById('modal-eliminar');
        eliminarEnlace.href = `/Tarea/EliminarTarea?idTarea=${idTarea}&idTablero=${idTablero}`;
    }

    document.getElementById('tareaModal').classList.remove('hidden');
}

function closeModal() {
    document.getElementById('tareaModal').classList.add('hidden');
}

function toggleAccordion(index) {
    const content = document.getElementById(`content-${index}`);
    const icon = document.getElementById(`icon-${index}`);

    const downSVG = `
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 16 16" fill="currentColor" class="w-4 h-4">
            <path fill-rule="evenodd" d="M4.22 6.22a.75.75 0 0 1 1.06 0L8 8.94l2.72-2.72a.75.75 0 1 1 1.06 1.06l-3.25 3.25a.75.75 0 0 1-1.06 0L4.22 7.28a.75.75 0 0 1 0-1.06Z" clip-rule="evenodd" />
        </svg>
    `;

    const upSVG = `
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 16 16" fill="currentColor" class="w-4 h-4">
            <path fill-rule="evenodd" d="M11.78 9.78a.75.75 0 0 1-1.06 0L8 7.06 5.28 9.78a.75.75 0 0 1-1.06-1.06l3.25-3.25a.75.75 0 0 1 1.06 0l3.25 3.25a.75.75 0 0 1 0 1.06Z" clip-rule="evenodd" />
        </svg>
    `;

    if (content.style.maxHeight && content.style.maxHeight !== '0px') {
        content.style.maxHeight = '0';
        icon.innerHTML = upSVG;
    } else {
        content.style.maxHeight = content.scrollHeight + 'px';
        icon.innerHTML = downSVG;
    }
}