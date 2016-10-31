var floorView = {};

floorView.table = '';
floorView.table_row_data = '';

$(document).ready(function () {

    $('[data-toggle="tooltip"]').tooltip({

        placement: 'bottom'
    });

    floorView.table = $('#floors_overview_table').DataTable({

        select: {
            style: 'single'
        },
        columns: [
            { "data": "id" },
            { "data": "description" }
        ]
    });
});

$('#floors_overview_table tr').not(':first').hover(

    function () {
        $(this).css('background-color', 'red');
        $(this).css('color', 'white');
    },
    function () {
        $(this).css('background-color', '');
        $(this).css('color', '');
    }
);

$('#floors_overview_table').on('click', 'tr', function () {

    floorView.table_row_data = floorView.table.row(this).data();
});

$('#insert_floor_button').on('click', function (e) {

    e.preventDefault();
    $('#insert_floor_modal').modal('show');
    bindFormInsert();
});

function bindFormInsert(dialog) {
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

                $('#insert_floor_modal').modal('hide');
                location.reload();
            } else {
                $('#insert_floor_modalContent').html(result);
                bindFormInsert();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$('#delete_floor_button').on('click', function () {

    if (floorView.table.rows('.selected').data().length == 0) {

        $('#delete_floor_button').prop('disabled', true);

        $('#floors_view_notification').notify({

            message: { text: 'No event has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#delete_floor_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#delete_floor_modal').data('url') + '/' + floorView.table_row_data.id;

        $.get(url, function (data) {

            $('#delete_floor_modal_body').html(data);
            $('#delete_floor_modal').modal('show');
        });
    }
});

$('#edit_floor_button').on('click', function () {

    if (floorView.table.rows('.selected').data().length == 0) {

        $('#edit_floor_button').prop('disabled', true);

        $('#floors_view_notification').notify({

            message: { text: 'No event has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#edit_floor_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#edit_floor_modal').data('url') + '/' + floorView.table_row_data.id;

        $.get(url, function (data) {

            $('#edit_floor_modalContent').html(data);
            $('#edit_floor_modal').modal('show');
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

                $('#edit_floor_modal').modal('hide');
                location.reload();
            } else {
                $('#edit_floor_modalContent').html(result);
                bindFormEdit();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$('#delete_floor_modal_submit_button').on('click', function () {

    $('#delete_floor_form').submit();
});



