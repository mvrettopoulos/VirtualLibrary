var availabilityView = {};

availabilityView.table = '';
availabilityView.table_row_data = '';

$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip({
        placement: 'bottom'
    });
    availabilityView.table = $('#availability_overview_table').DataTable({
        select: {
            style: 'single'
        },
        columns: [
            { "data": "id" },
            { "data": "book_title" },
            { "data": "university_name" },
            { "data": "quantity" },
            { "data": "reserved" },
            { "data": "available" }

        ]
    });
});
$('#availability_overview_table tr').not(':first').hover(
    function () {
        $(this).css("background-color", "red");
        $(this).css("color", "white");
    },
    function () {
        $(this).css("background-color", "");
        $(this).css("color", "");
    }
);
$('#availability_overview_table').on('click', 'tr', function () {
    availabilityView.table_row_data = availabilityView.table.row(this).data();
});
$('#insert_availability_button').on('click', function (e) {
    e.preventDefault();
    $('#insert_availability_modal').modal('show');
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
                $('#insert_availability_modal').modal('hide');
                location.reload();
            } else {
                $('#insert_availability_modalContent').html(result);
                bindFormInsert();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}
$('#delete_availability_button').on('click', function () {
    if (availabilityView.table.rows('.selected').data().length == 0) {
        $('#delete_availability_button').prop('disabled', true);
        $('#availability_view_notification').notify({
            message: { text: 'No availability has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {
                $('#delete_availability_button').prop('disabled', false);
            }
        }).show();
    }
    else {
        var url = $('#delete_availability_modal').data('url') + '/' + availabilityView.table_row_data.id;
        $.get(url, function (data) {
            $('#delete_availability_modal_body').html(data);
            $('#delete_availability_modal').modal('show');
        });
    }
});
$('#edit_availability_button').on('click', function () {
    if (availabilityView.table.rows('.selected').data().length == 0) {
        $('#edit_availability_button').prop('disabled', true);
        $('#availability_view_notification').notify({
            message: { text: 'No availability has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {
                $('#edit_availability_button').prop('disabled', false);
            }
        }).show();
    }
    else {
        var url = $('#edit_availability_modal').data('url') + '/' + availabilityView.table_row_data.id;
        $.get(url, function (data) {
            $('#edit_availability_modalContent').html(data);
            $('#edit_availability_modal').modal('show');
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
                $('#edit_availability_modal').modal('hide');
                location.reload();
            } else {
                $('#edit_availability_modalContent').html(result);
                bindFormEdit();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}
$('#delete_availability_modal_submit_button').on('click', function () {
    $('#delete_availability_form').submit();
});