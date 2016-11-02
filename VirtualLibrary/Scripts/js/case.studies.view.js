var caseStudiesView = {};

caseStudiesView.table = '';
caseStudiesView.table_row_data = '';

$(document).ready(function () {

    $('[data-toggle="tooltip"]').tooltip({

        placement: 'bottom'
    });

    $('[data-toggle="tooltip"]').tooltip({

        placement: 'bottom'
    });

    caseStudiesView.table = $('#case_studies_overview_table').DataTable({

        select: {
            style: 'single'
        },
        columns: [
            { "data": "id" },
            { "data": "description" },
            { "data": "version" },
            { "data": "scene" },
            { "data": "multimedia" },
            { "data": "documentation" },
            { "data": "dxf" },
            { "data": "els" }
        ]
    });
});

$('#case_studies_overview_table tr').not(':first').hover(

    function () {
        $(this).css('background-color', 'red');
        $(this).css('color', 'white');
    },
    function () {
        $(this).css('background-color', '');
        $(this).css('color', '');
    }
);

$('#case_studies_overview_table').on('click', 'tr', function () {

    caseStudiesView.table_row_data = caseStudiesView.table.row(this).data();
});

$('#insert_case_study_button').on('click', function (e) {

    e.preventDefault();
    $('#insert_case_study_modal').modal('show');
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

                $('#insert_case_study_modal').modal('hide');
                location.reload();
            } else {
                $('#insert_case_study_modalContent').html(result);
                bindFormInsert();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$('#delete_case_study_button').on('click', function () {

    if (caseStudiesView.table.rows('.selected').data().length == 0) {

        $('#delete_case_study_button').prop('disabled', true);

        $('#case_studies_view_notification').notify({

            message: { text: 'No case study has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#delete_case_study_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#delete_case_study_modal').data('url') + '/' + caseStudiesView.table_row_data.id;

        $.get(url, function (data) {

            $('#delete_case_study_modal_body').html(data);
            $('#delete_case_study_modal').modal('show');
        });
    }
});

$('#edit_case_study_button').on('click', function () {

    if (caseStudiesView.table.rows('.selected').data().length == 0) {

        $('#edit_case_study_button').prop('disabled', true);

        $('#case_studies_view_notification').notify({

            message: { text: 'No case study has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#edit_case_study_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#edit_case_study_modal').data('url') + '/' + caseStudiesView.table_row_data.id;

        $.get(url, function (data) {

            $('#edit_case_study_modalContent').html(data);
            $('#edit_case_study_modal').modal('show');
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

                $('#edit_case_study_modal').modal('hide');
                location.reload();
            } else {
                $('#edit_case_study_modalContent').html(result);
                bindFormEdit();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$('#delete_case_study_modal_submit_button').on('click', function () {

    $('#delete_case_study_form').submit();
});