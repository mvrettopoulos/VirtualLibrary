var buildingView = {};

buildingView.table = '';
buildingView.table_row_data = '';

$(document).ready(function () {

    $('[data-toggle="tooltip"]').tooltip({

        placement: 'bottom'
    });

    buildingView.table = $('#buildings_overview_table').DataTable({

        select: {
            style: 'single'
        },
        columns: [
            { "data": "id" },
            { "data": "description" },
            { "data": "BasementType.description" },
            { "data": "BuildingAgeType.description" },
            { "data": "FloorType.description" },
            { "data": "RoofType.description" },
            { "data": "BuildingType1.description" },
            { "data": "WallsColumnsType.description" },
            { "data": "ShearwallType.description" },
            { "data": "floorNumbers" },
            { "data": "collapseType" },
            { "data": "percentFailure" },
            { "data": "prexistingDamageType" },
            { "data": "prexistingDamageMagnitude" },
            { "data": "caseStudies" }
        ]
    });
});

$('#buildings_overview_table tr').not(':first').hover(

    function () {
        $(this).css('background-color', 'red');
        $(this).css('color', 'white');
    },
    function () {
        $(this).css('background-color', '');
        $(this).css('color', '');
    }
);

$('#buildings_overview_table').on('click', 'tr', function () {

    buildingView.table_row_data = buildingView.table.row(this).data();
});

$('#insert_building_button').on('click', function (e) {

    e.preventDefault();
    $('#insert_building_modal').modal('show');
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

                $('#insert_building_modal').modal('hide');
                location.reload();
            } else {
                $('#insert_building_modalContent').html(result);
                bindFormInsert();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$('#delete_building_button').on('click', function () {

    if (buildingView.table.rows('.selected').data().length == 0) {

        $('#delete_building_button').prop('disabled', true);

        $('#buildings_view_notification').notify({

            message: { text: 'No event has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#delete_building_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#delete_building_modal').data('url') + '/' + buildingView.table_row_data.id;

        $.get(url, function (data) {

            $('#delete_building_modal_body').html(data);
            $('#delete_building_modal').modal('show');
        });
    }
});

$('#edit_building_button').on('click', function () {

    if (buildingView.table.rows('.selected').data().length == 0) {

        $('#edit_building_button').prop('disabled', true);

        $('#buildings_view_notification').notify({

            message: { text: 'No event has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#edit_building_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#edit_building_modal').data('url') + '/' + buildingView.table_row_data.id;

        $.get(url, function (data) {

            $('#edit_building_modalContent').html(data);
            $('#edit_building_modal').modal('show');
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

                $('#edit_building_modal').modal('hide');
                location.reload();
            } else {
                $('#edit_building_modalContent').html(result);
                bindFormEdit();
            }
        }).fail(function (xhr, status, error) {
            alert('failed');
        });
    });
}

$('#delete_building_modal_submit_button').on('click', function () {

    $('#delete_building_form').submit();
});



