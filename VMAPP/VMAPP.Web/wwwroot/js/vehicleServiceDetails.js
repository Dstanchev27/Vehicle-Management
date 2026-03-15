$(document).ready(function () {
    $('#vehiclesTable').DataTable({
        responsive: true,
        searchable: true,
        pageLength: 10,
        lengthMenu: [5, 10, 25, 50],
        order: [[1, 'asc']],
        language: {
            search: "Search vehicles:",
            emptyTable: "No vehicles assigned to this service."
        }
    });
});