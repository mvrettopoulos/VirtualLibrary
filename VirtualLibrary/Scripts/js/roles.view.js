var rolesView = {};

rolesView.table = '';
rolesView.table_row_data = '';

$(document).ready(function () {

    $('[data-toggle="tooltip"]').tooltip({

        placement: 'bottom'
    });

    rolesView.table = $('#roles_overview_table').DataTable({
        scrollX: true,
        select: {
            style: 'single'
        },
        columns: [
            { "data": "id" },
            { "data": "name" }
        ]
    });
});

$('#roles_overview_table tr').not(':first').hover(
    function () {
        $(this).css("background-color", "red");
        $(this).css("color", "white");
    },
    function () {
        $(this).css("background-color", "");
        $(this).css("color", "");
    }
);

$('#roles_overview_table').on('click', 'tr', function () {

    rolesView.table_row_data = rolesView.table.row(this).data();
});


$('#delete_role_button').on('click', function () {

    if (rolesView.table.rows('.selected').data().length == 0) {

        $('#delete_role_button').prop('disabled', true);

        $('#roles_view_notification').notify({

            message: { text: 'No role has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#delete_role_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#delete_role_modal').data('url') + '/' + rolesView.table_row_data.id;

        $.get(url, function (data) {

            $('#delete_role_modal_body').html(data);
            $('#delete_role_modal').modal('show');
        });
    }
});

$('#edit_role_button').on('click', function () {

    if (rolesView.table.rows('.selected').data().length == 0) {

        $('#edit_role_button').prop('disabled', true);

        $('#roles_view_notification').notify({

            message: { text: 'No role has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#edit_role_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#edit_role_modal').data('url') + '/' + rolesView.table_row_data.id;

        $.get(url, function (data) {

            $('#edit_role_modalContent').html(data);
            $('#edit_role_modal').modal('show');
            bindFormEdit();
        });
    }
});


$('#delete_role_modal_submit_button').on('click', function () {

    $('#delete_role_form').submit();
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

                $('#edit_role_modal').modal('hide');
                location.reload();
            } else {
                $('#edit_role_modalContent').html(result);
                bindFormEdit();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}




$('#insert_role_button').on('click', function (e) {
    e.preventDefault();
    $('#insert_role_modal').modal('show');
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

                $('#insert_role_modal').modal('hide');
                location.reload();
            } else {
                $('#insert_role_modalContent').html(result);
                bindFormInsert();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

