﻿var basementView = {};

basementView.table = '';
basementView.table_row_data = '';

$(document).ready(function () {

    $('[data-toggle="tooltip"]').tooltip({

        placement: 'bottom'
    });

    basementView.table = $('#basements_overview_table').DataTable({

        select: {
            style: 'single'
        },
        columns: [
            { "data": "id" },
            { "data": "description" }
        ]
    });
});

$('#basements_overview_table tr').not(':first').hover(

    function () {
        $(this).css('background-color', 'red');
        $(this).css('color', 'white');
    },
    function () {
        $(this).css('background-color', '');
        $(this).css('color', '');
    }
);

$('#basements_overview_table').on('click', 'tr', function () {

    basementView.table_row_data = basementView.table.row(this).data();
});

$('#insert_basement_button').on('click', function (e) {

    e.preventDefault();
    $('#insert_basement_modal').modal('show');
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

                $('#insert_basement_modal').modal('hide');
                location.reload();
            } else {
                $('#insert_basement_modalContent').html(result);
                bindFormInsert();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$('#delete_basement_button').on('click', function () {

    if (basementView.table.rows('.selected').data().length == 0) {

        $('#delete_basement_button').prop('disabled', true);

        $('#basements_view_notification').notify({

            message: { text: 'No event has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#delete_basement_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#delete_basement_modal').data('url') + '/' + basementView.table_row_data.id;

        $.get(url, function (data) {

            $('#delete_basement_modal_body').html(data);
            $('#delete_basement_modal').modal('show');
        });
    }
});

$('#edit_basement_button').on('click', function () {

    if (basementView.table.rows('.selected').data().length == 0) {

        $('#edit_basement_button').prop('disabled', true);

        $('#basements_view_notification').notify({

            message: { text: 'No event has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#edit_basement_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#edit_basement_modal').data('url') + '/' + basementView.table_row_data.id;

        $.get(url, function (data) {

            $('#edit_basement_modalContent').html(data);
            $('#edit_basement_modal').modal('show');
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

                $('#edit_basement_modal').modal('hide');
                location.reload();
            } else {
                $('#edit_basement_modalContent').html(result);
                bindFormEdit();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$('#delete_basement_modal_submit_button').on('click', function () {

    $('#delete_basement_form').submit();
});



