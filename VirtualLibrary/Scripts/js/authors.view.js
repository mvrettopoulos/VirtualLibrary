var authorsView = {};
authorsView.table = '';
authorsView.table_row_data = '';
$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip({
        placement: 'bottom'
    });
    authorsView.table = $('#authors_overview_table').DataTable({
        scrollX: true,
        select: {
            style: 'single'
        },
        columns: [
            { "data": "id" },
            { "data": "author_name" }
        ]
    });
});
$('#authors_overview_table tr').not(':first').hover(
    function () {
        $(this).css("background-color", "red");
        $(this).css("color", "white");
    },
    function () {
        $(this).css("background-color", "");
        $(this).css("color", "");
    }
);
$('#authors_overview_table').on('click', 'tr', function () {
    authorsView.table_row_data = authorsView.table.row(this).data();
});
$('#insert_author_button').on('click', function (e) {
    e.preventDefault();
    $('#insert_author_modal').modal('show');
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
                $('#insert_author_modal').modal('hide');
                location.reload();
            } else {
                $('#insert_author_modalContent').html(result);
                bindFormInsert();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}
$('#delete_author_button').on('click', function () {
    if (authorsView.table.rows('.selected').data().length == 0) {
        $('#delete_author_button').prop('disabled', true);
        $('#authors_view_notification').notify({
            message: { text: 'No author has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {
                $('#delete_author_button').prop('disabled', false);
            }
        }).show();
    }
    else {
        var url = $('#delete_author_modal').data('url') + '/' + authorsView.table_row_data.id;
        $.get(url, function (data) {
            $('#delete_author_modal_body').html(data);
            $('#delete_author_modal').modal('show');
        });
    }
});
$('#edit_author_button').on('click', function () {
    if (authorsView.table.rows('.selected').data().length == 0) {
        $('#edit_author_button').prop('disabled', true);
        $('#authors_view_notification').notify({
            message: { text: 'No author has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {
                $('#edit_author_button').prop('disabled', false);
            }
        }).show();
    }
    else {
        var url = $('#edit_author_modal').data('url') + '/' + authorsView.table_row_data.id;
        $.get(url, function (data) {
            $('#edit_author_modalContent').html(data);
            $('#edit_author_modal').modal('show');
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
                $('#edit_author_modal').modal('hide');
                location.reload();
            } else {
                $('#edit_author_modalContent').html(result);
                bindFormEdit();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}
$('#delete_author_modal_submit_button').on('click', function () {
    $('#delete_author_form').submit();
});