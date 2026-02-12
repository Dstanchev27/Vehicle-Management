$(document).ready(function () {
    // Guard: ensure DataTables plugin is present and table exists
    if (!window.jQuery) {
        console.warn('jQuery not found. Skipping table init.');
        return;
    }

    if (!$.fn || !$.fn.DataTable) {
        console.warn('DataTables plugin not found. Skipping table init.');
        return;
    }

    var $table = $('#servicesTable');
    if (!$table.length) {
        return;
    }

    var table = $table.DataTable({
        pagingType: "simple_numbers",
        responsive: true,
        pageLength: 10,
        lengthMenu: [5, 10, 25, 50],
        // table has 6 columns: indices 0..5 -> last two are action columns
        columnDefs: [
            { targets: [4, 5], orderable: false, searchable: false }
        ]
    });

    $('.dataTables_length').addClass('bs-select');
});