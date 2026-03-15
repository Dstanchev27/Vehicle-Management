$(document).ready(function () {

    $('#serviceRecordsTable').DataTable({
        responsive: true,
        pageLength: 10,
        lengthMenu: [5, 10, 25, 50],
        order: [[1, 'desc']],
        language: {
            search: 'Search records:',
            emptyTable: 'No service records found for this vehicle.'
        }
    });

    const vehicleId = parseInt(document.getElementById('hiddenVehicleId').value);
    const serviceId = parseInt(document.getElementById('hiddenServiceId').value);
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
    let pendingDeleteId = null;

    const addRecordModalEl = document.getElementById('addRecordModal');
    const addDescription = document.getElementById('addDescription');
    const addServiceDate = document.getElementById('addServiceDate');
    const addCost = document.getElementById('addCost');
    const addDescriptionError = document.getElementById('addDescriptionError');
    const addServiceDateError = document.getElementById('addServiceDateError');
    const addCostError = document.getElementById('addCostError');
    const addRecordError = document.getElementById('addRecordError');
    const btnSubmitAdd = document.getElementById('btnSubmitAdd');

    const detailsRecordModalEl = document.getElementById('detailsRecordModal');
    const detailsDescription = document.getElementById('detailsDescription');
    const detailsServiceDate = document.getElementById('detailsServiceDate');
    const detailsCost = document.getElementById('detailsCost');

    const editRecordModalEl = document.getElementById('editRecordModal');
    const editRecordId = document.getElementById('editRecordId');
    const editDescription = document.getElementById('editDescription');
    const editServiceDate = document.getElementById('editServiceDate');
    const editCost = document.getElementById('editCost');
    const editDescriptionError = document.getElementById('editDescriptionError');
    const editServiceDateError = document.getElementById('editServiceDateError');
    const editCostError = document.getElementById('editCostError');
    const editRecordError = document.getElementById('editRecordError');
    const btnSubmitEdit = document.getElementById('btnSubmitEdit');

    const deleteRecordModalEl = document.getElementById('deleteRecordModal');
    const deleteRecordDescription = document.getElementById('deleteRecordDescription');
    const deleteRecordError = document.getElementById('deleteRecordError');
    const btnConfirmDelete = document.getElementById('btnConfirmDelete');

    const addModal = new bootstrap.Modal(addRecordModalEl);
    const detailsModal = new bootstrap.Modal(detailsRecordModalEl);
    const editModal = new bootstrap.Modal(editRecordModalEl);
    const deleteModal = new bootstrap.Modal(deleteRecordModalEl);

    function hideError(el) { el.classList.add('d-none'); el.textContent = ''; }
    function showError(el, message) { el.classList.remove('d-none'); el.textContent = message; }

    function formatDateDisplay(iso) {
        if (!iso) return '';
        const [y, m, d] = iso.split('-');
        return `${d}.${m}.${y}`;
    }

    function resetAddModal() {
        addDescription.value = '';
        addServiceDate.value = '';
        addCost.value = '';
        [addDescriptionError, addServiceDateError, addCostError, addRecordError].forEach(hideError);
    }

    function resetEditModal() {
        editRecordId.value = '';
        editDescription.value = '';
        editServiceDate.value = '';
        editCost.value = '';
        [editDescriptionError, editServiceDateError, editCostError, editRecordError].forEach(hideError);
    }

    addRecordModalEl.addEventListener('hidden.bs.modal', resetAddModal);
    editRecordModalEl.addEventListener('hidden.bs.modal', resetEditModal);
    deleteRecordModalEl.addEventListener('hidden.bs.modal', () => {
        pendingDeleteId = null;
        hideError(deleteRecordError);
    });

    function validateForm(descEl, dateEl, costEl, descErr, dateErr, costErr) {
        let valid = true;

        if (!descEl.value.trim() || descEl.value.trim().length < 3) {
            showError(descErr, 'Description must be at least 3 characters.');
            valid = false;
        } else { hideError(descErr); }

        if (!dateEl.value) {
            showError(dateErr, 'Service date is required.');
            valid = false;
        } else { hideError(dateErr); }

        if (costEl.value === '' || parseFloat(costEl.value) < 0) {
            showError(costErr, 'Cost must be 0 or greater.');
            valid = false;
        } else { hideError(costErr); }

        return valid;
    }

    btnSubmitAdd.addEventListener('click', async () => {
        if (!validateForm(addDescription, addServiceDate, addCost,
            addDescriptionError, addServiceDateError, addCostError)) return;
        try {
            const response = await fetch('/VehicleServices/AddServiceRecord', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': token },
                body: JSON.stringify({
                    vehicleId, vehicleServiceId: serviceId,
                    description: addDescription.value.trim(),
                    serviceDate: addServiceDate.value,
                    cost: parseFloat(addCost.value)
                })
            });
            const data = await response.json();
            if (!data.success) { showError(addRecordError, data.message ?? 'Failed to add record.'); return; }
            location.reload();
        } catch { showError(addRecordError, 'An error occurred. Please try again.'); }
    });

    document.addEventListener('click', async e => {
        const btn = e.target.closest('.btn-record-details');
        if (!btn) return;
        try {
            const response = await fetch(`/VehicleServices/GetServiceRecord?id=${btn.dataset.id}`);
            const data = await response.json();
            detailsDescription.value = data.description;
            detailsServiceDate.value = formatDateDisplay(data.serviceDate);
            detailsCost.value = parseFloat(data.cost).toFixed(2);
            detailsModal.show();
        } catch { alert('Failed to load service record details.'); }
    });

    document.addEventListener('click', async e => {
        const btn = e.target.closest('.btn-record-edit');
        if (!btn) return;
        try {
            const response = await fetch(`/VehicleServices/GetServiceRecord?id=${btn.dataset.id}`);
            const data = await response.json();
            editRecordId.value = data.id;
            editDescription.value = data.description;
            editServiceDate.value = data.serviceDate;
            editCost.value = data.cost;
            editModal.show();
        } catch { alert('Failed to load service record.'); }
    });

    btnSubmitEdit.addEventListener('click', async () => {
        if (!validateForm(editDescription, editServiceDate, editCost,
            editDescriptionError, editServiceDateError, editCostError)) return;
        try {
            const response = await fetch('/VehicleServices/EditServiceRecord', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': token },
                body: JSON.stringify({
                    id: parseInt(editRecordId.value), vehicleId, vehicleServiceId: serviceId,
                    description: editDescription.value.trim(),
                    serviceDate: editServiceDate.value,
                    cost: parseFloat(editCost.value)
                })
            });
            const data = await response.json();
            if (!data.success) { showError(editRecordError, data.message ?? 'Failed to update record.'); return; }
            location.reload();
        } catch { showError(editRecordError, 'An error occurred. Please try again.'); }
    });

    document.addEventListener('click', e => {
        const btn = e.target.closest('.btn-record-delete');
        if (!btn) return;
        pendingDeleteId = parseInt(btn.dataset.id);
        deleteRecordDescription.textContent = btn.dataset.description;
        deleteModal.show();
    });

    btnConfirmDelete.addEventListener('click', async () => {
        if (!pendingDeleteId) return;
        try {
            const response = await fetch('/VehicleServices/DeleteServiceRecord', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': token },
                body: JSON.stringify({ id: pendingDeleteId })
            });
            const data = await response.json();
            if (!data.success) { showError(deleteRecordError, data.message ?? 'Failed to delete record.'); return; }
            location.reload();
        } catch { showError(deleteRecordError, 'An error occurred. Please try again.'); }
    });
});