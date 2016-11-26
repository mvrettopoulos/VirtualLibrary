var categoriesView = {};
categoriesView.table = '';
categoriesView.table_row_data = '';
$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip({
        placement: 'bottom'
    });
    categoriesView.table = $('#categories_overview_table').DataTable({
        select: {
            style: 'single'
        },
        columns: [
            { "data": "id" },
            { "data": "description" }
        ]
    });
});
$('#categories_overview_table tr').not(':first').hover(
    function () {
        $(this).css("background-color", "red");
        $(this).css("color", "white");
    },
    function () {
        $(this).css("background-color", "");
        $(this).css("color", "");
    }
);
$('#categories_overview_table').on('click', 'tr', function () {
    categoriesView.table_row_data = categoriesView.table.row(this).data();
});
$('#insert_category_button').on('click', function (e) {
    e.preventDefault();
    $('#insert_category_modal').modal('show');
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
                $('#insert_category_modal').modal('hide');
                location.reload();
            } else {
                $('#insert_category_modalContent').html(result);
                bindFormInsert();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}
$('#delete_category_button').on('click', function () {
    if (categoriesView.table.rows('.selected').data().length === 0) {
        $('#delete_category_button').prop('disabled', true);
        $('#categories_view_notification').notify({
            message: { text: 'No category has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {
                $('#delete_category_button').prop('disabled', false);
            }
        }).show();
    }
    else {
        var url = $('#delete_category_modal').data('url') + '/' + categoriesView.table_row_data.id;
        $.get(url, function (data) {
            $('#delete_category_modal_body').html(data);
            $('#delete_category_modal').modal('show');
        });
    }
});
$('#edit_category_button').on('click', function () {
    if (categoriesView.table.rows('.selected').data().length == 0) {
        $('#edit_category_button').prop('disabled', true);
        $('#categories_view_notification').notify({
            message: { text: 'No category has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {
                $('#edit_category_button').prop('disabled', false);
            }
        }).show();
    }
    else {
        var url = $('#edit_category_modal').data('url') + '/' + categoriesView.table_row_data.id;
        $.get(url, function (data) {
            $('#edit_category_modalContent').html(data);
            $('#edit_category_modal').modal('show');
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
                $('#edit_category_modal').modal('hide');
                location.reload();
            } else {
                $('#edit_category_modalContent').html(result);
                bindFormEdit();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}
$('#delete_category_modal_submit_button').on('click', function () {
    $('#delete_category_form').submit();
});