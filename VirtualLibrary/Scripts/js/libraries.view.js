var librariesView = {};

librariesView.table = '';
librariesView.table_row_data = '';

$(document).ready(function () {

    $('[data-toggle="tooltip"]').tooltip({

        placement: 'bottom'
    });

    librariesView.table = $('#libraries_overview_table').DataTable({
        select: {
            style: 'single'
        },
        columns: [
            { "data": "id"},
            { "data": "University Name" },
            { "data": "Building" },
            { "data": "Location" },
            { "data": "Librarians"}
        ]
    });
});

$('#libraries_overview_table tr').not(':first').hover(
    function () {
        $(this).css("background-color", "red");
        $(this).css("color", "white");
    },
    function () {
        $(this).css("background-color", "");
        $(this).css("color", "");
    }
);

$('#libraries_overview_table').on('click', 'tr', function () {

    librariesView.table_row_data = librariesView.table.row(this).data();
});

$('#insert_library_button').on('click', function (e) {

    e.preventDefault();
    $('#insert_library_modal').modal('show');
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

                $('#insert_library_modal').modal('hide');
                location.reload();
            } else {
                $('#insert_library_modalContent').html(result);
                bindFormInsert();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$('#delete_library_button').on('click', function () {

    if (librariesView.table.rows('.selected').data().length == 0) {

        $('#delete_library_button').prop('disabled', true);

        $('#libraries_view_notification').notify({

            message: { text: 'No library has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#delete_library_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#delete_library_modal').data('url') + '/' + librariesView.table_row_data.id;

        $.get(url, function (data) {

            $('#delete_library_modal_body').html(data);
            $('#delete_library_modal').modal('show');
        });
    }
});

$('#edit_library_button').on('click', function () {

    if (librariesView.table.rows('.selected').data().length == 0) {

        $('#edit_library_button').prop('disabled', true);

        $('#libraries_view_notification').notify({

            message: { text: 'No library has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#edit_library_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#edit_library_modal').data('url') + '/' + librariesView.table_row_data.id;

        $.get(url, function (data) {

            $('#edit_library_modalContent').html(data);
            $('#edit_library_modal').modal('show');
            bindFormEdit();
        });
    }
});

$('#librarians_button').on('click', function () {

    if (librariesView.table.rows('.selected').data().length == 0) {

        $('#librarians_button').prop('disabled', true);

        $('#libraries_view_notification').notify({

            message: { text: 'No library has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#librarians_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#librarians_modal').data('url') + '/' + librariesView.table_row_data.id;

        $.get(url, function (data) {

            $('#librarians_modalContent').html(data);
            $('#librarians_modal').modal('show');
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

                $('#edit_library_modal').modal('hide');
                location.reload();
            } else {
                $('#edit_library_modalContent').html(result);
                bindFormEdit();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$('#delete_library_modal_submit_button').on('click', function () {

    $('#delete_library_form').submit();
});