// VehicleServiceCars DataTable initialization.
// Assumes jQuery and DataTables scripts are already loaded.
$(document).ready(function () {
    // Initialize DataTable after brief delay so DOM is stable (optional).
    var table = $('#servicesTable').DataTable({
        pagingType: "simple_numbers",
        responsive: true,
        pageLength: 10,
        lengthMenu: [5, 10, 25, 50],
        dom: 'lrtip', // remove built-in search input (we use external)
        columnDefs: [
            { targets: [6, 7], orderable: false, searchable: false } // action columns
        ]
    });

    // wire external search input to DataTable
    $('#external-search').on('input', function () {
        table.search(this.value).draw();
    });

    // apply initial server-provided query if present in the input
    var initialQuery = $('#external-search').val() ? $('#external-search').val().toString().trim() : '';
    if (initialQuery) {
        table.search(initialQuery).draw();
    }

    // UI: hide loading and show table section
    $('.loading').css('display', 'none');
    $('#sectionServicesTable').css('display', 'block');

    // small cosmetic tweak: bootstrap select class
    $('.dataTables_length').addClass('bs-select');
});