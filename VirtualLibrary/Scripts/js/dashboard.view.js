var reservationsView = {};
reservationsView.table = '';
reservationsView.table_row_data = '';
$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip({
        placement: 'bottom'
    });
    reservationsView.table = $('#reservations_overview_table').DataTable({
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

$('#checkin_button').on('click', function () {
    if (reservationsView.table.rows('.selected').data().length == 0) {
        $('#checkin_button').prop('disabled', true);
        $('#reservations_view_notification').notify({
            message: { text: 'No reservation has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {
                $('#checkin_button').prop('disabled', false);
            }
        }).show();
    }
    else {
        var url = $('#checkin_modal').data('url') + '/' + reservationsView.table_row_data.id;
        $.get(url, function (data) {
            $('#checkin_modal_body').html(data);
            $('#checkin_modal').modal('show');
        });
    }
});

$('#checkout_button').on('click', function () {
    if (reservationsView.table.rows('.selected').data().length == 0) {
        $('#checkout_button').prop('disabled', true);
        $('#reservations_view_notification').notify({
            message: { text: 'No reservation has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {
                $('#checkout_button').prop('disabled', false);
            }
        }).show();
    }
    else if (reservationsView.table_row_data.Check_In == 'No'){
        $('#checkout_button').prop('disabled', true);
        $('#reservations_view_notification').notify({
            message: { text: 'Cannot checkout before checkin' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {
                $('#checkout_button').prop('disabled', false);
            }
        }).show();
    }
    else {
        var url = $('#checkout_modal').data('url') + '/' + reservationsView.table_row_data.id;
        $.get(url, function (data) {
            $('#checkout_modal_body').html(data);
            $('#checkout_modal').modal('show');
        });
    }
});


$('#checkin_modal_submit_button').on('click', function () {
    $('#checkin_form').submit();
});


$('#checkout_modal_submit_button').on('click', function () {
    $('#checkout_form').submit();
});