$(document).ready(function () {

    $('#claimsTable').DataTable({
        responsive: true,
        pageLength: 10,
        lengthMenu: [5, 10, 25, 50],
        order: [[0, 'desc']],
        language: {
            search: 'Search claims:',
            emptyTable: 'No insurance claims found for this policy.'
        }
    });

    const policyId = parseInt(document.getElementById('currentPolicyId').value);
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
    let pendingDeleteClaimId = null;

    const addClaimModalEl = document.getElementById('addClaimModal');
    const detailsClaimModalEl = document.getElementById('detailsClaimModal');
    const deleteClaimModalEl = document.getElementById('deleteClaimModal');

    const addClaimDate = document.getElementById('addClaimDate');
    const addClaimDescription = document.getElementById('addClaimDescription');
    const addClaimAmount = document.getElementById('addClaimAmount');
    const addClaimDateError = document.getElementById('addClaimDateError');
    const addClaimDescriptionError = document.getElementById('addClaimDescriptionError');
    const addClaimAmountError = document.getElementById('addClaimAmountError');
    const addClaimError = document.getElementById('addClaimError');
    const btnSubmitAddClaim = document.getElementById('btnSubmitAddClaim');

    const detailsClaimDate = document.getElementById('detailsClaimDate');
    const detailsClaimDescription = document.getElementById('detailsClaimDescription');
    const detailsClaimAmount = document.getElementById('detailsClaimAmount');

    const deleteClaimDescription = document.getElementById('deleteClaimDescription');
    const deleteClaimError = document.getElementById('deleteClaimError');
    const btnConfirmDeleteClaim = document.getElementById('btnConfirmDeleteClaim');

    const detailsModal = new bootstrap.Modal(detailsClaimModalEl);
    const deleteModal = new bootstrap.Modal(deleteClaimModalEl);

    function hideError(el) { el.classList.add('d-none'); el.textContent = ''; }
    function showError(el, msg) { el.classList.remove('d-none'); el.textContent = msg; }

    function formatDateDisplay(iso) {
        if (!iso) return '';
        const [y, m, d] = iso.split('-');
        return `${d}.${m}.${y}`;
    }

    function resetAddModal() {
        addClaimDate.value = '';
        addClaimDescription.value = '';
        addClaimAmount.value = '';
        [addClaimDateError, addClaimDescriptionError, addClaimAmountError, addClaimError].forEach(hideError);
    }

    addClaimModalEl.addEventListener('hidden.bs.modal', resetAddModal);

    deleteClaimModalEl.addEventListener('hidden.bs.modal', () => {
        pendingDeleteClaimId = null;
        hideError(deleteClaimError);
    });

    btnSubmitAddClaim.addEventListener('click', async () => {
        let valid = true;

        if (!addClaimDate.value) {
            showError(addClaimDateError, 'Claim date is required.');
            valid = false;
        } else { hideError(addClaimDateError); }

        if (!addClaimDescription.value.trim() || addClaimDescription.value.trim().length < 3) {
            showError(addClaimDescriptionError, 'Description must be at least 3 characters.');
            valid = false;
        } else { hideError(addClaimDescriptionError); }

        if (addClaimAmount.value === '' || parseFloat(addClaimAmount.value) < 0) {
            showError(addClaimAmountError, 'Amount must be 0 or greater.');
            valid = false;
        } else { hideError(addClaimAmountError); }

        if (!valid) return;

        try {
            const response = await fetch('/Insurance/AddClaim', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': token },
                body: JSON.stringify({
                    insurancePolicyId: policyId,
                    claimDate: addClaimDate.value,
                    description: addClaimDescription.value.trim(),
                    amount: parseFloat(addClaimAmount.value)
                })
            });
            const data = await response.json();
            if (!data.success) { showError(addClaimError, data.message ?? 'Failed to add claim.'); return; }
            location.reload();
        } catch { showError(addClaimError, 'An error occurred. Please try again.'); }
    });

    document.addEventListener('click', async e => {
        const btn = e.target.closest('.btn-claim-details');
        if (!btn) return;
        try {
            const response = await fetch(`/Insurance/GetClaim?id=${btn.dataset.id}`);
            const data = await response.json();
            detailsClaimDate.value = formatDateDisplay(data.claimDate);
            detailsClaimDescription.value = data.description;
            detailsClaimAmount.value = parseFloat(data.amount).toFixed(2);
            detailsModal.show();
        } catch { alert('Failed to load claim details.'); }
    });

    document.addEventListener('click', e => {
        const btn = e.target.closest('.btn-claim-delete');
        if (!btn) return;
        pendingDeleteClaimId = parseInt(btn.dataset.id);
        deleteClaimDescription.textContent = btn.dataset.description;
        deleteModal.show();
    });

    btnConfirmDeleteClaim.addEventListener('click', async () => {
        if (!pendingDeleteClaimId) return;
        try {
            const response = await fetch('/Insurance/DeleteClaim', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': token },
                body: JSON.stringify({ id: pendingDeleteClaimId })
            });
            const data = await response.json();
            if (!data.success) { showError(deleteClaimError, data.message ?? 'Failed to delete claim.'); return; }
            location.reload();
        } catch { showError(deleteClaimError, 'An error occurred. Please try again.'); }
    });
});
