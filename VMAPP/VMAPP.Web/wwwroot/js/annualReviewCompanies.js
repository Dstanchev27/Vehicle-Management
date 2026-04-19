$(document).ready(function () {
    const deleteCompanyButtons = document.querySelectorAll('.btn-delete-company');
    const companyIdInput = document.getElementById('companyId');
    const companyNameSpan = document.getElementById('companyName');

    deleteCompanyButtons.forEach(button => {
        button.addEventListener('click', () => {
            const companyId = button.getAttribute('data-company-id');
            const companyName = button.getAttribute('data-company-name');

            companyIdInput.value = companyId;
            companyNameSpan.innerText = companyName;
        });
    });

    $('#annualReviewCompaniesTable').DataTable({
        responsive: true,
        searchable: true,
        pageLength: 10,
        lengthMenu: [5, 10, 25, 50],
        order: [[0, 'asc']],
        language: {
            search: "Search annual review companies:",
            emptyTable: "No annual review companies found."
        }
    });
});
