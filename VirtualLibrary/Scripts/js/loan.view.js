﻿var loansView = {};
loansView.table = '';
loansView.table_row_data = '';
$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip({
        placement: 'bottom'
    });
    loansView.table = $('#loans_overview_table').DataTable({
        select: {
            style: 'single'
        },
        columns: [
            { "data": "id" },
            { "data": "Book title" },
            { "data": "Library" },
            { "data": "Reserved Date" },
            { "data": "Return Date" },
            { "data": "Available renew times" }
        ]
    });
});
$('#loans_overview_table tr').not(':first').hover(
    function () {
        $(this).css("background-color", "red");
        $(this).css("color", "white");
    },
    function () {
        $(this).css("background-color", "");
        $(this).css("color", "");
    }
);
$('#loans_overview_table').on('click', 'tr', function () {
    loansView.table_row_data = loansView.table.row(this).data();
});

$('#edit_loan_button').on('click', function () {
    if (loansView.table.rows('.selected').data().length == 0) {
        $('#edit_loan_button').prop('disabled', true);
        $('#loans_view_notification').notify({
            message: { text: 'No loan has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {
                $('#edit_loan_button').prop('disabled', false);
            }
        }).show();
    }
    else {
        var url = $('#edit_loan_modal').data('url') + '/' + loansView.table_row_data.id;
        $.get(url, function (data) {
            $('#edit_loan_modalContent').html(data);
            $('#edit_loan_modal').modal('show');
            bindFormEdit();
        });
    }
});
function bindFormEdit(dialog) {
    $('form', dialog).submit(function (e) {
        e.preventDefault();
        if (!$(this).valid()) {
            return false;
        }
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize()
        }).done(function (result) {
            if (result.success) {
                $('#edit_loan_modal').modal('hide');
                location.reload();
            } else {
                $('#edit_loan_modalContent').html(result);
                bindFormEdit();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$("#datepicker").datepicker({
    dateFormat: "dd/MM/yyyy",
    min: minDate,
    max: maxDate
    
    
    
});