$(document).ready(function () {

    $('#vehiclesTable').DataTable({
        responsive: true,
        pageLength: 10,
        lengthMenu: [5, 10, 25, 50],
        order: [[1, 'asc']],
        language: {
            search: 'Search vehicles:',
            emptyTable: 'No vehicles assigned to this service.'
        }
    });

    const vinSearchInput = document.getElementById('vinSearchInput');
    const vinSearchMessage = document.getElementById('vinSearchMessage');
    const vehicleResult = document.getElementById('vehicleResult');
    const foundVehicleId = document.getElementById('foundVehicleId');
    const btnAddVehicle = document.getElementById('btnAddVehicle');
    const btnSearchVin = document.getElementById('btnSearchVin');
    const btnCancelModal = document.getElementById('btnCancelModal');
    const addVehicleModal = document.getElementById('addVehicleModal');
    const addVehicleError = document.getElementById('addVehicleError');
    const currentServiceId = document.getElementById('currentServiceId');
    const resultVin = document.getElementById('resultVin');
    const resultBrand = document.getElementById('resultBrand');
    const resultModel = document.getElementById('resultModel');
    const resultYear = document.getElementById('resultYear');
    const resultColor = document.getElementById('resultColor');
    const resultType = document.getElementById('resultType');

    const removeVehicleModal = document.getElementById('removeVehicleModal');
    const removeVehicleVin = document.getElementById('removeVehicleVin');
    const removeVehicleError = document.getElementById('removeVehicleError');
    const btnConfirmRemove = document.getElementById('btnConfirmRemove');
    const removeModal = new bootstrap.Modal(removeVehicleModal);

    let pendingRemoveVehicleId = null;

    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    function resetModal() {
        vinSearchInput.value = '';
        vinSearchMessage.textContent = '';
        vinSearchMessage.classList.add('d-none');
        vehicleResult.classList.add('d-none');
        foundVehicleId.value = '';
        addVehicleError.classList.add('d-none');
        addVehicleError.textContent = '';
        btnAddVehicle.classList.add('d-none');

        [resultVin, resultBrand, resultModel, resultYear, resultColor, resultType]
            .forEach(el => el.value = '');
    }

    addVehicleModal.addEventListener('hidden.bs.modal', resetModal);
    btnCancelModal.addEventListener('click', resetModal);

    removeVehicleModal.addEventListener('hidden.bs.modal', () => {
        pendingRemoveVehicleId = null;
        removeVehicleError.classList.add('d-none');
        removeVehicleError.textContent = '';
    });

    async function searchByVin() {
        const vin = vinSearchInput.value.trim().toUpperCase();

        if (!vin) {
            showSearchMessage('Please enter a VIN.', 'warning');
            return;
        }

        try {
            const response = await fetch(`/VehicleServices/SearchVehicleByVin?vin=${encodeURIComponent(vin)}`);
            const data = await response.json();

            if (!data.found) {
                showSearchMessage(data.message, 'danger');
                vehicleResult.classList.add('d-none');
                btnAddVehicle.classList.add('d-none');
                return;
            }

            const v = data.vehicle;
            vinSearchMessage.classList.add('d-none');
            foundVehicleId.value = v.id;
            resultVin.value = v.vin;
            resultBrand.value = v.carBrand;
            resultModel.value = v.carModel;
            resultYear.value = v.createdOnYear;
            resultColor.value = v.color;
            resultType.value = v.vehicleType;
            vehicleResult.classList.remove('d-none');
            btnAddVehicle.classList.remove('d-none');
            addVehicleError.classList.add('d-none');
            addVehicleError.textContent = '';

        } catch {
            showSearchMessage('An error occurred while searching. Please try again.', 'danger');
        }
    }

    function showSearchMessage(message, type) {
        vinSearchMessage.classList.remove('d-none', 'text-danger', 'text-warning', 'text-success');
        vinSearchMessage.classList.add(`text-${type}`);
        vinSearchMessage.textContent = message;
    }

    btnSearchVin.addEventListener('click', searchByVin);

    vinSearchInput.addEventListener('keydown', e => {
        if (e.key === 'Enter') {
            e.preventDefault();
            searchByVin();
        }
    });

    btnAddVehicle.addEventListener('click', async () => {
        const serviceId = parseInt(currentServiceId.value);
        const vehicleId = parseInt(foundVehicleId.value);

        try {
            const response = await fetch('/VehicleServices/AddVehicleToService', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'X-CSRF-TOKEN': token
                },
                body: JSON.stringify({ serviceId, vehicleId })
            });

            const data = await response.json();

            if (!data.success) {
                addVehicleError.classList.remove('d-none');
                addVehicleError.textContent = data.message;
                return;
            }

            location.reload();

        } catch {
            addVehicleError.classList.remove('d-none');
            addVehicleError.textContent = 'An error occurred while adding the vehicle. Please try again.';
        }
    });

    document.addEventListener('click', e => {
        const btn = e.target.closest('.btn-remove-vehicle');
        if (!btn) return;

        pendingRemoveVehicleId = parseInt(btn.dataset.vehicleId);
        removeVehicleVin.textContent = btn.dataset.vehicleVin;
        removeModal.show();
    });

    btnConfirmRemove.addEventListener('click', async () => {
        if (!pendingRemoveVehicleId) return;

        const serviceId = parseInt(currentServiceId.value);

        try {
            const response = await fetch('/VehicleServices/RemoveVehicleFromService', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'X-CSRF-TOKEN': token
                },
                body: JSON.stringify({ serviceId, vehicleId: pendingRemoveVehicleId })
            });

            const data = await response.json();

            if (!data.success) {
                removeVehicleError.classList.remove('d-none');
                removeVehicleError.textContent = data.message;
                return;
            }

            location.reload();

        } catch {
            removeVehicleError.classList.remove('d-none');
            removeVehicleError.textContent = 'An error occurred while removing the vehicle. Please try again.';
        }
    });
});