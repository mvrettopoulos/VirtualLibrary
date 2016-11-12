var booksView = {};

booksView.table = '';
booksView.table_row_data = '';

$(document).ready(function () {

    $('[data-toggle="tooltip"]').tooltip({

        placement: 'bottom'
    });

    booksView.table = $('#books_overview_table').DataTable({

        select: {
            style: 'single'
        },
        columns: [
            { "data": "id" },
            { "data": "title" },
            { "data": "description" },
            { "data": "image" },
            { "data": "isbn" },
            { "data": "publisher" },
            { "data": "view" },
            { "data": "Author" },
            { "data": "Category" }


        ]
    });
});

$('#books_overview_table tr').not(':first').hover(
    function () {
        $(this).css("background-color", "red");
        $(this).css("color", "white");
    },
    function () {
        $(this).css("background-color", "");
        $(this).css("color", "");
    }
);

$('#books_overview_table').on('click', 'tr', function () {

    booksView.table_row_data = booksView.table.row(this).data();
});

$('#insert_book_button').on('click', function (e) {

    e.preventDefault();
    $('#insert_book_modal').modal('show');
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

                $('#insert_book_modal').modal('hide');
                location.reload();
            } else {
                $('#insert_book_modalContent').html(result);
                bindFormInsert();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$('#delete_book_button').on('click', function () {

    if (booksView.table.rows('.selected').data().length == 0) {

        $('#delete_book_button').prop('disabled', true);

        $('#books_view_notification').notify({

            message: { text: 'No book has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#delete_book_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#delete_book_modal').data('url') + '/' + booksView.table_row_data.id;

        $.get(url, function (data) {

            $('#delete_book_modal_body').html(data);
            $('#delete_book_modal').modal('show');
        });
    }
});

$('#edit_book_button').on('click', function () {

    if (booksView.table.rows('.selected').data().length == 0) {

        $('#edit_book_button').prop('disabled', true);

        $('#books_view_notification').notify({

            message: { text: 'No book has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#edit_book_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#edit_book_modal').data('url') + '/' + booksView.table_row_data.id;

        $.get(url, function (data) {

            $('#edit_book_modalContent').html(data);
            $('#edit_book_modal').modal('show');
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

                $('#edit_book_modal').modal('hide');
                location.reload();
            } else {
                $('#edit_book_modalContent').html(result);
                bindFormEdit();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$('#delete_book_modal_submit_button').on('click', function () {

    $('#delete_book_form').submit();
});