$(document).ready(function () {
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
        columnDefs: [
            { targets: [4, 5], orderable: false, searchable: false }
        ]
    });

    $('.dataTables_length').addClass('bs-select');
});