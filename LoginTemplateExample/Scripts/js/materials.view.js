var materialsView = {};

materialsView.table = '';
materialsView.table_row_data = '';

$(document).ready(function () {

    $('[data-toggle="tooltip"]').tooltip({

        placement: 'bottom'
    });

    materialsView.table = $('#materials_overview_table').DataTable({

        select: {
            style: 'single'
        },
        columns: [
            { "data": "id" },
            { "data": "description" },
            {
                "data": "caseStudyId",
                "defaultContent": ""
            }
        ]
    });
});

$('#materials_overview_table tr').not(':first').hover(
    function () {
        $(this).css("background-color", "red");
        $(this).css("color", "white");
    },
    function () {
        $(this).css("background-color", "");
        $(this).css("color", "");
    }
);

$('#materials_overview_table').on('click', 'tr', function () {

    materialsView.table_row_data = materialsView.table.row(this).data();
});

$('#insert_material_button').on('click', function (e) {

    e.preventDefault();
    $('#insert_material_modal').modal('show');
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

                $('#insert_material_modal').modal('hide');
                location.reload();
            } else {
                $('#insert_material_modalContent').html(result);
                bindFormInsert();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$('#delete_material_button').on('click', function () {

    if (materialsView.table.rows('.selected').data().length == 0) {

        $('#delete_material_button').prop('disabled', true);

        $('#materials_view_notification').notify({

            message: { text: 'No material has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#delete_material_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#delete_material_modal').data('url') + '/' + materialsView.table_row_data.id;

        $.get(url, function (data) {

            $('#delete_material_modal_body').html(data);
            $('#delete_material_modal').modal('show');
        });
    }
});

$('#edit_material_button').on('click', function () {

    if (materialsView.table.rows('.selected').data().length == 0) {

        $('#edit_material_button').prop('disabled', true);

        $('#materials_view_notification').notify({

            message: { text: 'No material has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#edit_material_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#edit_material_modal').data('url') + '/' + materialsView.table_row_data.id;

        $.get(url, function (data) {

            $('#edit_material_modalContent').html(data);
            $('#edit_material_modal').modal('show');
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

                $('#edit_material_modal').modal('hide');
                location.reload();
            } else {
                $('#edit_material_modalContent').html(result);
                bindFormEdit();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$('#delete_material_modal_submit_button').on('click', function () {

    $('#delete_material_form').submit();
});