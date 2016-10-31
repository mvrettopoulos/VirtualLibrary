var shearwallView = {};

shearwallView.table = '';
shearwallView.table_row_data = '';

$(document).ready(function () {

    $('[data-toggle="tooltip"]').tooltip({

        placement: 'bottom'
    });

    shearwallView.table = $('#shearwalls_overview_table').DataTable({

        select: {
            style: 'single'
        },
        columns: [
            { "data": "id" },
            { "data": "description" }
        ]
    });
});

$('#shearwalls_overview_table tr').not(':first').hover(

    function () {
        $(this).css('background-color', 'red');
        $(this).css('color', 'white');
    },
    function () {
        $(this).css('background-color', '');
        $(this).css('color', '');
    }
);

$('#shearwalls_overview_table').on('click', 'tr', function () {

    shearwallView.table_row_data = shearwallView.table.row(this).data();
});

$('#insert_shearwall_button').on('click', function (e) {

    e.preventDefault();
    $('#insert_shearwall_modal').modal('show');
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

                $('#insert_shearwall_modal').modal('hide');
                location.reload();
            } else {
                $('#insert_shearwall_modalContent').html(result);
                bindFormInsert();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$('#delete_shearwall_button').on('click', function () {

    if (shearwallView.table.rows('.selected').data().length == 0) {

        $('#delete_shearwall_button').prop('disabled', true);

        $('#shearwalls_view_notification').notify({

            message: { text: 'No event has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#delete_shearwall_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#delete_shearwall_modal').data('url') + '/' + shearwallView.table_row_data.id;

        $.get(url, function (data) {

            $('#delete_shearwall_modal_body').html(data);
            $('#delete_shearwall_modal').modal('show');
        });
    }
});

$('#edit_shearwall_button').on('click', function () {

    if (shearwallView.table.rows('.selected').data().length == 0) {

        $('#edit_shearwall_button').prop('disabled', true);

        $('#shearwalls_view_notification').notify({

            message: { text: 'No event has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#edit_shearwall_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#edit_shearwall_modal').data('url') + '/' + shearwallView.table_row_data.id;

        $.get(url, function (data) {

            $('#edit_shearwall_modalContent').html(data);
            $('#edit_shearwall_modal').modal('show');
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

                $('#edit_shearwall_modal').modal('hide');
                location.reload();
            } else {
                $('#edit_shearwall_modalContent').html(result);
                bindFormEdit();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$('#delete_shearwall_modal_submit_button').on('click', function () {

    $('#delete_shearwall_form').submit();
});



