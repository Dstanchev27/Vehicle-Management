$(document).ready(function () {

    $('#vehiclesTable').DataTable({
        responsive: true,
        pageLength: 10,
        lengthMenu: [5, 10, 25, 50],
        order: [[1, 'asc']],
        language: {
            search: 'Search vehicles:',
            emptyTable: 'No inspected vehicles found for this company.'
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
    const addReportNumber = document.getElementById('addReportNumber');
    const addInspectionDate = document.getElementById('addInspectionDate');
    const addExpiryDate = document.getElementById('addExpiryDate');
    const addInspectionDateError = document.getElementById('addInspectionDateError');
    const addExpiryDateError = document.getElementById('addExpiryDateError');
    const addPassed = document.getElementById('addPassed');
    const addNotes = document.getElementById('addNotes');
    const addReportError = document.getElementById('addReportError');
    const btnSubmitAddReport = document.getElementById('btnSubmitAddReport');

    const addReportModalEl = document.getElementById('addReportModal');
    const deleteReportModalEl = document.getElementById('deleteReportModal');
    const deleteReportNumber = document.getElementById('deleteReportNumber');
    const deleteReportError = document.getElementById('deleteReportError');
    const btnConfirmDeleteReport = document.getElementById('btnConfirmDeleteReport');

    const deleteModal = new bootstrap.Modal(deleteReportModalEl);

    let pendingDeleteReportId = null;

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
        addReportNumber.value = '';
        addInspectionDate.value = '';
        addExpiryDate.value = '';
        addPassed.checked = false;
        addNotes.value = '';
        [addInspectionDateError, addExpiryDateError, addReportError].forEach(hideError);
        btnSubmitAddReport.classList.add('d-none');
    }

    addReportModalEl.addEventListener('hidden.bs.modal', resetAddModal);

    deleteReportModalEl.addEventListener('hidden.bs.modal', () => {
        pendingDeleteReportId = null;
        hideError(deleteReportError);
    });

    async function searchByVin() {
        const vin = vinSearchInput.value.trim().toUpperCase();
        if (!vin) { showSearchMessage('Please enter a VIN.', 'warning'); return; }

        try {
            const response = await fetch(`/AnnualReview/SearchVehicleByVin?vin=${encodeURIComponent(vin)}`);
            const data = await response.json();

            if (!data.found) {
                showSearchMessage(data.message, 'danger');
                vehicleResult.classList.add('d-none');
                btnSubmitAddReport.classList.add('d-none');
                return;
            }

            vinSearchMessage.classList.add('d-none');
            foundVehicleId.value = data.vehicle.id;
            resultVin.value = data.vehicle.vin;
            resultBrand.value = data.vehicle.carBrand;
            resultModel.value = data.vehicle.carModel;
            resultYear.value = data.vehicle.createdOnYear;
            vehicleResult.classList.remove('d-none');
            btnSubmitAddReport.classList.remove('d-none');
            hideError(addReportError);
        } catch {
            showSearchMessage('An error occurred while searching. Please try again.', 'danger');
        }
    }

    btnSearchVin.addEventListener('click', searchByVin);
    vinSearchInput.addEventListener('keydown', e => { if (e.key === 'Enter') { e.preventDefault(); searchByVin(); } });

    btnSubmitAddReport.addEventListener('click', async () => {
        let valid = true;

        if (!addInspectionDate.value) {
            showError(addInspectionDateError, 'Inspection date is required.');
            valid = false;
        } else { hideError(addInspectionDateError); }

        if (!addExpiryDate.value) {
            showError(addExpiryDateError, 'Expiry date is required.');
            valid = false;
        } else if (addExpiryDate.value <= addInspectionDate.value) {
            showError(addExpiryDateError, 'Expiry date must be after inspection date.');
            valid = false;
        } else { hideError(addExpiryDateError); }

        if (!valid) return;

        try {
            const response = await fetch('/AnnualReview/AddReport', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': token },
                body: JSON.stringify({
                    annualReviewCompanyId: companyId,
                    vehicleId: parseInt(foundVehicleId.value),
                    reportNumber: addReportNumber.value.trim() || null,
                    inspectionDate: addInspectionDate.value,
                    expiryDate: addExpiryDate.value,
                    passed: addPassed.checked,
                    notes: addNotes.value.trim() || null
                })
            });
            const data = await response.json();
            if (!data.success) { showError(addReportError, data.message ?? 'Failed to add report.'); return; }
            location.reload();
        } catch { showError(addReportError, 'An error occurred. Please try again.'); }
    });

    document.addEventListener('click', e => {
        const btn = e.target.closest('.btn-delete-report');
        if (!btn) return;
        pendingDeleteReportId = parseInt(btn.dataset.reportId);
        deleteReportNumber.textContent = btn.dataset.reportNumber;
        deleteModal.show();
    });

    btnConfirmDeleteReport.addEventListener('click', async () => {
        if (!pendingDeleteReportId) return;
        try {
            const response = await fetch('/AnnualReview/DeleteReport', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': token },
                body: JSON.stringify({ id: pendingDeleteReportId })
            });
            const data = await response.json();
            if (!data.success) { showError(deleteReportError, data.message ?? 'Failed to delete report.'); return; }
            location.reload();
        } catch { showError(deleteReportError, 'An error occurred. Please try again.'); }
    });
});
