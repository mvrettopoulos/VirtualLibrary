var loadsView = {};

loadsView.table = '';
loadsView.table_row_data = '';

$(document).ready(function () {

    $('[data-toggle="tooltip"]').tooltip({

        placement: 'bottom'
    });

    loadsView.table = $('#loads_overview_table').DataTable({

        select: {
            style: 'single'
        },
        columns: [
            { "data": "id" },
            { "data": "description" },
            {
                "data": "caseStudyId",
                "defaultContent": ""
            }
        ]
    });
});

$('#loads_overview_table tr').not(':first').hover(
    function () {
        $(this).css("background-color", "red");
        $(this).css("color", "white");
    },
    function () {
        $(this).css("background-color", "");
        $(this).css("color", "");
    }
);

$('#loads_overview_table').on('click', 'tr', function () {

    loadsView.table_row_data = loadsView.table.row(this).data();
});

$('#insert_load_button').on('click', function (e) {

    e.preventDefault();
    $('#insert_load_modal').modal('show');
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

                $('#insert_load_modal').modal('hide');
                location.reload();
            } else {
                $('#insert_load_modalContent').html(result);
                bindFormInsert();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$('#delete_load_button').on('click', function () {

    if (loadsView.table.rows('.selected').data().length == 0) {

        $('#delete_load_button').prop('disabled', true);

        $('#loads_view_notification').notify({

            message: { text: 'No load has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#delete_load_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#delete_load_modal').data('url') + '/' + loadsView.table_row_data.id;

        $.get(url, function (data) {

            $('#delete_load_modal_body').html(data);
            $('#delete_load_modal').modal('show');
        });
    }
});

$('#edit_load_button').on('click', function () {

    if (loadsView.table.rows('.selected').data().length == 0) {

        $('#edit_load_button').prop('disabled', true);

        $('#loads_view_notification').notify({

            message: { text: 'No load has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#edit_load_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#edit_load_modal').data('url') + '/' + loadsView.table_row_data.id;

        $.get(url, function (data) {

            $('#edit_load_modalContent').html(data);
            $('#edit_load_modal').modal('show');
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

                $('#edit_load_modal').modal('hide');
                location.reload();
            } else {
                $('#edit_load_modalContent').html(result);
                bindFormEdit();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$('#delete_load_modal_submit_button').on('click', function () {

    $('#delete_load_form').submit();
});