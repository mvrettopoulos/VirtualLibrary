var walls_columnsView = {};

walls_columnsView.table = '';
walls_columnsView.table_row_data = '';

$(document).ready(function () {

    $('[data-toggle="tooltip"]').tooltip({

        placement: 'bottom'
    });

    walls_columnsView.table = $('#walls_columnss_overview_table').DataTable({

        select: {
            style: 'single'
        },
        columns: [
            { "data": "id" },
            { "data": "description" }
        ]
    });
});

$('#walls_columnss_overview_table tr').not(':first').hover(

    function () {
        $(this).css('background-color', 'red');
        $(this).css('color', 'white');
    },
    function () {
        $(this).css('background-color', '');
        $(this).css('color', '');
    }
);

$('#walls_columnss_overview_table').on('click', 'tr', function () {

    walls_columnsView.table_row_data = walls_columnsView.table.row(this).data();
});

$('#insert_walls_columns_button').on('click', function (e) {

    e.preventDefault();
    $('#insert_walls_columns_modal').modal('show');
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

                $('#insert_walls_columns_modal').modal('hide');
                location.reload();
            } else {
                $('#insert_walls_columns_modalContent').html(result);
                bindFormInsert();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$('#delete_walls_columns_button').on('click', function () {

    if (walls_columnsView.table.rows('.selected').data().length == 0) {

        $('#delete_walls_columns_button').prop('disabled', true);

        $('#walls_columnss_view_notification').notify({

            message: { text: 'No event has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#delete_walls_columns_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#delete_walls_columns_modal').data('url') + '/' + walls_columnsView.table_row_data.id;

        $.get(url, function (data) {

            $('#delete_walls_columns_modal_body').html(data);
            $('#delete_walls_columns_modal').modal('show');
        });
    }
});

$('#edit_walls_columns_button').on('click', function () {

    if (walls_columnsView.table.rows('.selected').data().length == 0) {

        $('#edit_walls_columns_button').prop('disabled', true);

        $('#walls_columnss_view_notification').notify({

            message: { text: 'No event has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#edit_walls_columns_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#edit_walls_columns_modal').data('url') + '/' + walls_columnsView.table_row_data.id;

        $.get(url, function (data) {

            $('#edit_walls_columns_modalContent').html(data);
            $('#edit_walls_columns_modal').modal('show');
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

                $('#edit_walls_columns_modal').modal('hide');
                location.reload();
            } else {
                $('#edit_walls_columns_modalContent').html(result);
                bindFormEdit();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$('#delete_walls_columns_modal_submit_button').on('click', function () {

    $('#delete_walls_columns_form').submit();
});



