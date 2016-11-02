var eventsView = {};

eventsView.table = '';
eventsView.table_row_data = '';

$(document).ready(function () {

    $('[data-toggle="tooltip"]').tooltip({

        placement: 'bottom'
    });

    eventsView.table = $('#events_overview_table').DataTable({

        select: {
            style: 'single'
        },
        columns: [
            { "data": "id" },
            { "data": "description" },
            { "data": "magnitude" },
            {
                "data": "caseStudyId",
                "defaultContent": ""
            }
        ]
    });
});

$('#events_overview_table tr').not(':first').hover(

    function () {
        $(this).css('background-color', 'red');
        $(this).css('color', 'white');
    },
    function () {
        $(this).css('background-color', '');
        $(this).css('color', '');
    }
);

$('#events_overview_table').on('click', 'tr', function () {

    eventsView.table_row_data = eventsView.table.row(this).data();
});

$('#insert_event_button').on('click', function (e) {

    e.preventDefault();
    $('#insert_event_modal').modal('show');
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

                $('#insert_event_modal').modal('hide');
                location.reload();
            } else {
                $('#insert_event_modalContent').html(result);
                bindFormInsert();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$('#delete_event_button').on('click', function () {

    if (eventsView.table.rows('.selected').data().length == 0) {

        $('#delete_event_button').prop('disabled', true);

        $('#events_view_notification').notify({

            message: { text: 'No event has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#delete_event_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#delete_event_modal').data('url') + '/' + eventsView.table_row_data.id;

        $.get(url, function (data) {

            $('#delete_event_modal_body').html(data);
            $('#delete_event_modal').modal('show');
        });
    }
});

$('#edit_event_button').on('click', function () {

    if (eventsView.table.rows('.selected').data().length == 0) {

        $('#edit_event_button').prop('disabled', true);

        $('#events_view_notification').notify({

            message: { text: 'No event has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#edit_event_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#edit_event_modal').data('url') + '/' + eventsView.table_row_data.id;

        $.get(url, function (data) {

            $('#edit_event_modalContent').html(data);
            $('#edit_event_modal').modal('show');
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

                $('#edit_event_modal').modal('hide');
                location.reload();
            } else {
                $('#edit_event_modalContent').html(result);
                bindFormEdit();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$('#delete_event_modal_submit_button').on('click', function () {

    $('#delete_event_form').submit();
});



