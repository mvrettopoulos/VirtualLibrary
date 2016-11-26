var usersView = {};

usersView.table = '';
usersView.table_row_data = '';

$(document).ready(function () {

    $('[data-toggle="tooltip"]').tooltip({

        placement: 'bottom'
    });

    usersView.table = $('#users_overview_table').DataTable({
        select: {
            style: 'single'
        },
        columns: [
            { "data": "id" },
            { "data": "UserName" },
            { "data": "Email" },
            { "data": "Roles" }
        ]
    });
});

$('#users_overview_table tr').not(':first').hover(
    function () {
        $(this).css("background-color", "red");
        $(this).css("color", "white");
    },
    function () {
        $(this).css("background-color", "");
        $(this).css("color", "");
    }
);

$('#users_overview_table').on('click', 'tr', function () {

    usersView.table_row_data = usersView.table.row(this).data();
});

$('#insert_user_button').on('click', function (e) {

    e.preventDefault();
    $('#insert_user_modal').modal('show');
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

                $('#insert_user_modal').modal('hide');
                location.reload();
            } else {
                $('#insert_user_modalContent').html(result);
                bindFormInsert();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$('#delete_user_button').on('click', function () {

    if (usersView.table.rows('.selected').data().length == 0) {

        $('#delete_user_button').prop('disabled', true);

        $('#users_view_notification').notify({

            message: { text: 'No user has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#delete_user_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#delete_user_modal').data('url') + '/' + usersView.table_row_data.id;

        $.get(url, function (data) {

            $('#delete_user_modal_body').html(data);
            $('#delete_user_modal').modal('show');
        });
    }
});

$('#edit_user_button').on('click', function () {

    if (usersView.table.rows('.selected').data().length == 0) {

        $('#edit_user_button').prop('disabled', true);

        $('#users_view_notification').notify({

            message: { text: 'No user has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#edit_user_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#edit_user_modal').data('url') + '/' + usersView.table_row_data.id;

        $.get(url, function (data) {

            $('#edit_user_modalContent').html(data);
            $('#edit_user_modal').modal('show');
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

                $('#edit_user_modal').modal('hide');
                location.reload();
            } else {
                $('#edit_user_modalContent').html(result);
                bindFormEdit();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$('#delete_user_modal_submit_button').on('click', function () {

    $('#delete_user_form').submit();
});