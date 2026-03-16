$(document).ready(function () {

    $('#vehiclesTable').DataTable({
        responsive: true,
        pageLength: 10,
        lengthMenu: [5, 10, 25, 50],
        order: [[1, 'asc']],
        language: {
            search: 'Search vehicles:'
        }
    });

    const vehicleDisplayName = document.getElementById('vehicleDisplayName');
    const vehicleIdInput     = document.getElementById('vehicleIdInput');

    document.addEventListener('click', e => {
        const btn = e.target.closest('.btn-delete-vehicle');

        if (!btn)
        {
            return;
        }

        vehicleDisplayName.textContent = btn.dataset.vehicleName;
        vehicleIdInput.value           = btn.dataset.vehicleId;
    });
});