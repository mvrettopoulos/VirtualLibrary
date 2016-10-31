var typeView = {};

typeView.table = '';
typeView.table_row_data = '';

$(document).ready(function () {

    $('[data-toggle="tooltip"]').tooltip({

        placement: 'bottom'
    });

    typeView.table = $('#types_overview_table').DataTable({

        select: {
            style: 'single'
        },
        columns: [
            { "data": "id" },
            { "data": "description" }
        ]
    });
});

$('#types_overview_table tr').not(':first').hover(

    function () {
        $(this).css('background-color', 'red');
        $(this).css('color', 'white');
    },
    function () {
        $(this).css('background-color', '');
        $(this).css('color', '');
    }
);

$('#types_overview_table').on('click', 'tr', function () {

    typeView.table_row_data = typeView.table.row(this).data();
});

$('#insert_type_button').on('click', function (e) {

    e.preventDefault();
    $('#insert_type_modal').modal('show');
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

                $('#insert_type_modal').modal('hide');
                location.reload();
            } else {
                $('#insert_type_modalContent').html(result);
                bindFormInsert();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$('#delete_type_button').on('click', function () {

    if (typeView.table.rows('.selected').data().length == 0) {

        $('#delete_type_button').prop('disabled', true);

        $('#types_view_notification').notify({

            message: { text: 'No event has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#delete_type_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#delete_type_modal').data('url') + '/' + typeView.table_row_data.id;

        $.get(url, function (data) {

            $('#delete_type_modal_body').html(data);
            $('#delete_type_modal').modal('show');
        });
    }
});

$('#edit_type_button').on('click', function () {

    if (typeView.table.rows('.selected').data().length == 0) {

        $('#edit_type_button').prop('disabled', true);

        $('#types_view_notification').notify({

            message: { text: 'No event has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#edit_type_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#edit_type_modal').data('url') + '/' + typeView.table_row_data.id;

        $.get(url, function (data) {

            $('#edit_type_modalContent').html(data);
            $('#edit_type_modal').modal('show');
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

                $('#edit_type_modal').modal('hide');
                location.reload();
            } else {
                $('#edit_type_modalContent').html(result);
                bindFormEdit();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$('#delete_type_modal_submit_button').on('click', function () {

    $('#delete_type_form').submit();
});



