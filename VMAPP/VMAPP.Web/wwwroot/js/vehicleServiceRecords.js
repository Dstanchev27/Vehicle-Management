$(function () {
    if (!window.jQuery) {
        console.warn('jQuery not found. Skipping service records table init.');
        return;
    }
    if (!$.fn || !$.fn.DataTable) {
        console.warn('DataTables plugin not found. Skipping service records table init.');
        return;
    }

    function formatDecimalInput(value) {
        if (!value && value !== 0) return '';

        value = String(value).trim();

        console.log('formatDecimalInput - Input value:', value);

        value = value.replace(',', '.');

        value = value.replace(/[^\d.]/g, '');

        console.log('formatDecimalInput - After cleanup:', value);

        var parts = value.split('.');
        if (parts.length > 2) {
            value = parts[0] + '.' + parts.slice(1).join('');
        }

        console.log('formatDecimalInput - Final formatted value:', value);
        return value;
    }

    function validateDecimalInput(value) {
        if (!value && value !== '0') {
            console.warn('validateDecimalInput - Empty value');
            return { isValid: false, error: 'Cost is required' };
        }

        var formatted = formatDecimalInput(value);
        console.log('validateDecimalInput - Formatted:', formatted);

        if (!formatted || formatted === '') {
            console.warn('validateDecimalInput - Formatted is empty');
            return { isValid: false, error: 'Cost must be a number' };
        }

        var numValue = parseFloat(formatted);
        console.log('validateDecimalInput - Parsed number:', numValue);

        if (isNaN(numValue)) {
            console.warn('validateDecimalInput - Is NaN');
            return { isValid: false, error: 'Cost must be a valid number' };
        }

        if (numValue < 0) {
            console.warn('validateDecimalInput - Negative value');
            return { isValid: false, error: 'Cost cannot be negative' };
        }

        if (numValue > 999999999.99) {
            console.warn('validateDecimalInput - Value too large');
            return { isValid: false, error: 'Cost amount is too large' };
        }

        console.log('validateDecimalInput - Valid! Value:', numValue);
        return { isValid: true, formatted: formatted, parsed: numValue };
    }

    var $table = $('#serviceRecordsTable');
    if ($table.length) {
        $table.DataTable({
            pagingType: "simple_numbers",
            responsive: true,
            pageLength: 10,
            lengthMenu: [5, 10, 25, 50],
            columnDefs: [
                { targets: [3, 4], orderable: false, searchable: false } 
            ]
        });
    }

    $('#btnAddRecord').on('click', function () {
        $('#recordModalTitle').text('Add Service Record');
        $('#recordForm').attr('action', '/VehicleServiceCars/AddRecord');
        $('#RecordDate').val('');
        $('#Cost').val('');
        $('#Description').val('');
        $('#recordSaveBtn').prop('disabled', false);
        console.log('Add record modal opened - fields cleared');
    });

    $(document).on('click', '.btn-edit-record', function () {
        var $tr = $(this).closest('tr');
        var id = $tr.data('record-id');
        var date = $tr.data('record-date') || '';
        var cost = $tr.data('record-cost') || '';
        var desc = $tr.data('record-desc') || '';

        $('#recordModalTitle').text('Edit Service Record');
        $('#recordForm').attr('action', '/VehicleServiceCars/EditRecord/' + id);
        $('#RecordDate').val(date);
        
        var formattedCost = formatDecimalInput(cost);
        $('#Cost').val(formattedCost);
        
        $('#Description').val(desc);
        $('#recordSaveBtn').prop('disabled', false);

        console.log('Edit record loaded:');
        console.log('  - Record ID:', id);
        console.log('  - Date:', date);
        console.log('  - Cost (raw):', cost);
        console.log('  - Cost (formatted):', formattedCost);
        console.log('  - Description:', desc);
    });

    $(document).on('click', '.btn-delete-record', function () {
        var $tr = $(this).closest('tr');
        var id = $tr.data('record-id');
        $('#DeleteRecordId').val(id);
        
        console.log('Delete record ID set to:', id);
    });

    $('#addEditRecordModal').on('shown.bs.modal', function () {
        $('#RecordDate').trigger('focus');

        var costInput = $('#Cost');

        costInput.off('input.costValidation').off('blur.costValidation');

        costInput.on('input.costValidation', function () {
            var value = $(this).val();
            console.log('Cost input changed:', value);

            var validation = validateDecimalInput(value);
            var errorSpan = $(this).parent().find('.cost-error');

            if (!validation.isValid) {
                $(this).addClass('is-invalid');
                if (errorSpan.length) {
                    errorSpan.text(validation.error).show();
                }
                $('#recordSaveBtn').prop('disabled', true);
                console.warn('Cost validation failed:', validation.error);
            } else {
                $(this).removeClass('is-invalid');
                if (errorSpan.length) {
                    errorSpan.hide();
                }
                $('#recordSaveBtn').prop('disabled', false);
                console.log('Cost is valid:', validation.formatted);
            }
        });

        costInput.on('blur.costValidation', function () {
            var value = $(this).val();
            if (value) {
                var formatted = formatDecimalInput(value);
                $(this).val(formatted);
                console.log('Cost formatted on blur:', formatted);

                var validation = validateDecimalInput(formatted);
                if (!validation.isValid) {
                    $(this).addClass('is-invalid');
                    $('#recordSaveBtn').prop('disabled', true);
                }
            }
        });
    });

    $('#recordForm').on('submit', function (e) {
        var costInput = $('#Cost');
        var costValue = costInput.val();

        console.log('===== FORM SUBMIT =====');
        console.log('Original cost value:', costValue);

        var formatted = formatDecimalInput(costValue);
        console.log('Formatted cost value:', formatted);

        var validation = validateDecimalInput(costValue);
        console.log('Validation result:', validation);

        if (!validation.isValid) {
            console.error('Form submission blocked - Invalid cost:', validation.error);
            e.preventDefault();
            e.stopPropagation();
            alert('Cost Error: ' + validation.error);
            costInput.focus().addClass('is-invalid');
            return false;
        }

        costInput.val(validation.formatted);
        console.log('Cost set to formatted value:', validation.formatted);

        var finalValue = costInput.val();
        console.log('Final value being sent:', finalValue);
        console.log('Final parsed value:', parseFloat(finalValue));

        console.log('Form submitting with values:');
        console.log('  - Action:', $(this).attr('action'));
        console.log('  - RecordDate:', $('#RecordDate').val());
        console.log('  - RecordCost (final):', finalValue);
        console.log('  - RecordCost (parsed):', parseFloat(finalValue));
        console.log('  - Description:', $('#Description').val());
        console.log('===== END FORM SUBMIT =====');

        return true;
    });
});
