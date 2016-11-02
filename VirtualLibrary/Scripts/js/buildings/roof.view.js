var roofView = {};

roofView.table = '';
roofView.table_row_data = '';

$(document).ready(function () {

    $('[data-toggle="tooltip"]').tooltip({

        placement: 'bottom'
    });

    roofView.table = $('#roofs_overview_table').DataTable({

        select: {
            style: 'single'
        },
        columns: [
            { "data": "id" },
            { "data": "description" }
        ]
    });
});

$('#roofs_overview_table tr').not(':first').hover(

    function () {
        $(this).css('background-color', 'red');
        $(this).css('color', 'white');
    },
    function () {
        $(this).css('background-color', '');
        $(this).css('color', '');
    }
);

$('#roofs_overview_table').on('click', 'tr', function () {

    roofView.table_row_data = roofView.table.row(this).data();
});

$('#insert_roof_button').on('click', function (e) {

    e.preventDefault();
    $('#insert_roof_modal').modal('show');
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

                $('#insert_roof_modal').modal('hide');
                location.reload();
            } else {
                $('#insert_roof_modalContent').html(result);
                bindFormInsert();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$('#delete_roof_button').on('click', function () {

    if (roofView.table.rows('.selected').data().length == 0) {

        $('#delete_roof_button').prop('disabled', true);

        $('#roofs_view_notification').notify({

            message: { text: 'No event has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#delete_roof_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#delete_roof_modal').data('url') + '/' + roofView.table_row_data.id;

        $.get(url, function (data) {

            $('#delete_roof_modal_body').html(data);
            $('#delete_roof_modal').modal('show');
        });
    }
});

$('#edit_roof_button').on('click', function () {

    if (roofView.table.rows('.selected').data().length == 0) {

        $('#edit_roof_button').prop('disabled', true);

        $('#roofs_view_notification').notify({

            message: { text: 'No event has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#edit_roof_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#edit_roof_modal').data('url') + '/' + roofView.table_row_data.id;

        $.get(url, function (data) {

            $('#edit_roof_modalContent').html(data);
            $('#edit_roof_modal').modal('show');
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

                $('#edit_roof_modal').modal('hide');
                location.reload();
            } else {
                $('#edit_roof_modalContent').html(result);
                bindFormEdit();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$('#delete_roof_modal_submit_button').on('click', function () {

    $('#delete_roof_form').submit();
});



