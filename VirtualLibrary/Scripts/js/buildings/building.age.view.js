var ageView = {};

ageView.table = '';
ageView.table_row_data = '';

$(document).ready(function () {

    $('[data-toggle="tooltip"]').tooltip({

        placement: 'bottom'
    });

    ageView.table = $('#ages_overview_table').DataTable({

        select: {
            style: 'single'
        },
        columns: [
            { "data": "id" },
            { "data": "description" }
        ]
    });
});

$('#ages_overview_table tr').not(':first').hover(

    function () {
        $(this).css('background-color', 'red');
        $(this).css('color', 'white');
    },
    function () {
        $(this).css('background-color', '');
        $(this).css('color', '');
    }
);

$('#ages_overview_table').on('click', 'tr', function () {

    ageView.table_row_data = ageView.table.row(this).data();
});

$('#insert_age_button').on('click', function (e) {

    e.preventDefault();
    $('#insert_age_modal').modal('show');
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

                $('#insert_age_modal').modal('hide');
                location.reload();
            } else {
                $('#insert_age_modalContent').html(result);
                bindFormInsert();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$('#delete_age_button').on('click', function () {

    if (ageView.table.rows('.selected').data().length == 0) {

        $('#delete_age_button').prop('disabled', true);

        $('#ages_view_notification').notify({

            message: { text: 'No event has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#delete_age_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#delete_age_modal').data('url') + '/' + ageView.table_row_data.id;

        $.get(url, function (data) {

            $('#delete_age_modal_body').html(data);
            $('#delete_age_modal').modal('show');
        });
    }
});

$('#edit_age_button').on('click', function () {

    if (ageView.table.rows('.selected').data().length == 0) {

        $('#edit_age_button').prop('disabled', true);

        $('#ages_view_notification').notify({

            message: { text: 'No event has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#edit_age_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#edit_age_modal').data('url') + '/' + ageView.table_row_data.id;

        $.get(url, function (data) {

            $('#edit_age_modalContent').html(data);
            $('#edit_age_modal').modal('show');
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

                $('#edit_age_modal').modal('hide');
                location.reload();
            } else {
                $('#edit_age_modalContent').html(result);
                bindFormEdit();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$('#delete_age_modal_submit_button').on('click', function () {

    $('#delete_age_form').submit();
});



