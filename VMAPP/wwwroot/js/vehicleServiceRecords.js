// Handles DataTable init and modal population for Add/Edit/Delete service records.
$(function () {
    // Guard: ensure jQuery & DataTables are available
    if (!window.jQuery) {
        console.warn('jQuery not found. Skipping service records table init.');
        return;
    }
    if (!$.fn || !$.fn.DataTable) {
        console.warn('DataTables plugin not found. Skipping service records table init.');
        return;
    }

    var $table = $('#serviceRecordsTable');
    if ($table.length) {
        $table.DataTable({
            pagingType: "simple_numbers",
            responsive: true,
            pageLength: 10,
            lengthMenu: [5, 10, 25, 50],
            columnDefs: [
                { targets: [3, 4], orderable: false, searchable: false } // edit/delete columns (updated indices)
            ]
        });
    }

    // Add button: clear modal for new record and set form action to AddRecord
    $('#btnAddRecord').on('click', function () {
        $('#recordModalTitle').text('Add Service Record');
        $('#recordForm').attr('action', '/VehicleServiceCars/AddRecord');
        $('#RecordDate').val('');
        $('#Cost').val('');
        $('#Description').val('');
        // ensure Save button is enabled
        $('#recordSaveBtn').prop('disabled', false);
    });

    // Edit button: populate modal from row data attributes and set form action to EditRecord
    $(document).on('click', '.btn-edit-record', function () {
        var $tr = $(this).closest('tr');
        var id = $tr.data('record-id');
        var date = $tr.data('record-date') || '';
        var cost = $tr.data('record-cost') || '';
        var desc = $tr.data('record-desc') || '';

        $('#recordModalTitle').text('Edit Service Record');
        $('#recordForm').attr('action', '/VehicleServiceCars/EditRecord/' + id);
        $('#RecordDate').val(date);
        $('#Cost').val(cost);
        $('#Description').val(desc);
        $('#recordSaveBtn').prop('disabled', false);
    });

    // Delete button: set id on delete form
    $(document).on('click', '.btn-delete-record', function () {
        var $tr = $(this).closest('tr');
        var id = $tr.data('record-id');
        $('#DeleteRecordId').val(id);
    });

    // Optional: focus first input when modal shown
    $('#addEditRecordModal').on('shown.bs.modal', function () {
        $('#RecordDate').trigger('focus');
    });
});