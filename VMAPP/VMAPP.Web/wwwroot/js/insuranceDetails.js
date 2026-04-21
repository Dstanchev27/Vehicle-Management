$(document).ready(function () {

    $('#vehiclesTable').DataTable({
        responsive: true,
        pageLength: 10,
        lengthMenu: [5, 10, 25, 50],
        order: [[1, 'asc']],
        language: {
            search: 'Search vehicles:',
            emptyTable: 'No insured vehicles found for this company.'
        }
    });

    const companyId = parseInt(document.getElementById('currentCompanyId').value);
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    const vinSearchInput = document.getElementById('vinSearchInput');
    const vinSearchMessage = document.getElementById('vinSearchMessage');
    const vehicleResult = document.getElementById('vehicleResult');
    const foundVehicleId = document.getElementById('foundVehicleId');
    const btnSearchVin = document.getElementById('btnSearchVin');
    const resultVin = document.getElementById('resultVin');
    const resultBrand = document.getElementById('resultBrand');
    const resultModel = document.getElementById('resultModel');
    const resultYear = document.getElementById('resultYear');
    const addPolicyNumber = document.getElementById('addPolicyNumber');
    const addStartDate = document.getElementById('addStartDate');
    const addEndDate = document.getElementById('addEndDate');
    const addStartDateError = document.getElementById('addStartDateError');
    const addEndDateError = document.getElementById('addEndDateError');
    const addPolicyError = document.getElementById('addPolicyError');
    const btnSubmitAddPolicy = document.getElementById('btnSubmitAddPolicy');

    const addPolicyModalEl = document.getElementById('addPolicyModal');
    const deletePolicyModalEl = document.getElementById('deletePolicyModal');
    const deletePolicyNumber = document.getElementById('deletePolicyNumber');
    const deletePolicyError = document.getElementById('deletePolicyError');
    const btnConfirmDeletePolicy = document.getElementById('btnConfirmDeletePolicy');

    const deleteModal = new bootstrap.Modal(deletePolicyModalEl);

    let pendingDeletePolicyId = null;

    function hideError(el) { el.classList.add('d-none'); el.textContent = ''; }
    function showError(el, msg) { el.classList.remove('d-none'); el.textContent = msg; }

    function showSearchMessage(message, type) {
        vinSearchMessage.classList.remove('d-none', 'text-danger', 'text-warning', 'text-success');
        vinSearchMessage.classList.add(`text-${type}`);
        vinSearchMessage.textContent = message;
    }

    function resetAddModal() {
        vinSearchInput.value = '';
        vinSearchMessage.classList.add('d-none');
        vinSearchMessage.textContent = '';
        vehicleResult.classList.add('d-none');
        foundVehicleId.value = '';
        [resultVin, resultBrand, resultModel, resultYear].forEach(el => el.value = '');
        addPolicyNumber.value = '';
        addStartDate.value = '';
        addEndDate.value = '';
        [addStartDateError, addEndDateError, addPolicyError].forEach(hideError);
        btnSubmitAddPolicy.classList.add('d-none');
    }

    addPolicyModalEl.addEventListener('hidden.bs.modal', resetAddModal);

    deletePolicyModalEl.addEventListener('hidden.bs.modal', () => {
        pendingDeletePolicyId = null;
        hideError(deletePolicyError);
    });

    async function searchByVin() {
        const vin = vinSearchInput.value.trim().toUpperCase();
        if (!vin) { showSearchMessage('Please enter a VIN.', 'warning'); return; }

        try {
            const response = await fetch(`/Insurance/SearchVehicleByVin?vin=${encodeURIComponent(vin)}`);
            const data = await response.json();

            if (!data.found) {
                showSearchMessage(data.message, 'danger');
                vehicleResult.classList.add('d-none');
                btnSubmitAddPolicy.classList.add('d-none');
                return;
            }

            vinSearchMessage.classList.add('d-none');
            foundVehicleId.value = data.vehicle.id;
            resultVin.value = data.vehicle.vin;
            resultBrand.value = data.vehicle.carBrand;
            resultModel.value = data.vehicle.carModel;
            resultYear.value = data.vehicle.createdOnYear;
            vehicleResult.classList.remove('d-none');
            btnSubmitAddPolicy.classList.remove('d-none');
            hideError(addPolicyError);
        } catch {
            showSearchMessage('An error occurred while searching. Please try again.', 'danger');
        }
    }

    btnSearchVin.addEventListener('click', searchByVin);
    vinSearchInput.addEventListener('keydown', e => { if (e.key === 'Enter') { e.preventDefault(); searchByVin(); } });

    btnSubmitAddPolicy.addEventListener('click', async () => {
        let valid = true;

        if (!addStartDate.value) {
            showError(addStartDateError, 'Start date is required.');
            valid = false;
        } else { hideError(addStartDateError); }

        if (!addEndDate.value) {
            showError(addEndDateError, 'End date is required.');
            valid = false;
        } else if (addEndDate.value <= addStartDate.value) {
            showError(addEndDateError, 'End date must be after start date.');
            valid = false;
        } else { hideError(addEndDateError); }

        if (!valid) return;

        try {
            const response = await fetch('/Insurance/AddPolicy', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', 'X-CSRF-TOKEN': token },
                body: JSON.stringify({
                    insuranceCompanyId: companyId,
                    vehicleId: parseInt(foundVehicleId.value),
                    policyNumber: addPolicyNumber.value.trim() || null,
                    startDate: addStartDate.value,
                    endDate: addEndDate.value
                })
            });
            const data = await response.json();
            if (!data.success) { showError(addPolicyError, data.message ?? 'Failed to add policy.'); return; }
            location.reload();
        } catch { showError(addPolicyError, 'An error occurred. Please try again.'); }
    });

    document.addEventListener('click', e => {
        const btn = e.target.closest('.btn-delete-policy');
        if (!btn) return;
        pendingDeletePolicyId = parseInt(btn.dataset.policyId);
        deletePolicyNumber.textContent = btn.dataset.policyNumber;
        deleteModal.show();
    });

    btnConfirmDeletePolicy.addEventListener('click', async () => {
        if (!pendingDeletePolicyId) return;
        try {
            const response = await fetch('/Insurance/DeletePolicy', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', 'X-CSRF-TOKEN': token },
                body: JSON.stringify({ id: pendingDeletePolicyId })
            });
            const data = await response.json();
            if (!data.success) { showError(deletePolicyError, data.message ?? 'Failed to delete policy.'); return; }
            location.reload();
        } catch { showError(deletePolicyError, 'An error occurred. Please try again.'); }
    });
});
