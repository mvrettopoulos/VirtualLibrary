var reservationsView = {};
reservationsView.table = '';
reservationsView.table_row_data = '';
$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip({
        placement: 'bottom'
    });
    reservationsView.table = $('#reservations_overview_table').DataTable({
        scrollX: true,
        select: {
            style: 'single'
        },
        columns: [
            { "data": "id" },
            { "data": "Book_title" },
            { "data": "UserName" },
            { "data": "Library" },
            { "data": "Reserved_Date" },
            { "data": "Return_Date" },
            { "data": "Check_In" },
            { "data": "Check_Out" },
            { "data": "renew_times" }
        ]
    });
});
$('#reservations_overview_table tr').not(':first').hover(
    function () {
        $(this).css("background-color", "red");
        $(this).css("color", "white");
    },
    function () {
        $(this).css("background-color", "");
        $(this).css("color", "");
    }
);
$('#reservations_overview_table').on('click', 'tr', function () {
    reservationsView.table_row_data = reservationsView.table.row(this).data();
});

$('#delete_reservation_button').on('click', function () {
    if (reservationsView.table.rows('.selected').data().length == 0) {
        $('#delete_reservation_button').prop('disabled', true);
        $('#reservations_view_notification').notify({
            message: { text: 'No reservation has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {
                $('#delete_reservation_button').prop('disabled', false);
            }
        }).show();
    }
    else {
        var url = $('#delete_reservation_modal').data('url') + '/' + reservationsView.table_row_data.id;
        $.get(url, function (data) {
            $('#delete_reservation_modal_body').html(data);
            $('#delete_reservation_modal').modal('show');
        });
    }
});
$('#edit_reservation_button').on('click', function () {
    if (reservationsView.table.rows('.selected').data().length == 0) {
        $('#edit_reservation_button').prop('disabled', true);
        $('#reservations_view_notification').notify({
            message: { text: 'No reservation has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {
                $('#edit_reservation_button').prop('disabled', false);
            }
        }).show();
    }
    else {
        var url = $('#edit_reservation_modal').data('url') + '/' + reservationsView.table_row_data.id;
        $.get(url, function (data) {
            $('#edit_reservation_modalContent').html(data);
            $('#edit_reservation_modal').modal('show');
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
                $('#edit_reservation_modal').modal('hide');
                location.reload();
            } else {
                $('#edit_reservation_modalContent').html(result);
                bindFormEdit();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}
$('#delete_reservation_modal_submit_button').on('click', function () {
    $('#delete_reservation_form').submit();
});