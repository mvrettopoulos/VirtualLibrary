var usersdashboardView = {};

usersdashboardView.table = '';
usersdashboardView.table_row_data = '';

usersdashboardView.activatetable = '';
usersdashboardView.activatetable_row_data = '';

$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip({
        placement: 'bottom'
    });
    usersdashboardView.table = $('#usersdashboard_overview_table').DataTable({
        scrollX: true,
        select: {
            style: 'single'
        },
        columns: [
            { "data": "id"},
            { "data": "username" },
            { "data": "membership_id" },
            { "data": "bad_user" },
            { "data": "first_name" },
            { "data": "last_name" },
            { "data": "email" }
        ]
    });

    usersdashboardView.activatetable = $('#inactive_usersdashboard_overview_table').DataTable({
        scrollX: true,
        select: {
            style: 'single'
        },
        columns: [
            { "data": "id" },
            { "data": "username" },
            { "data": "date_of_registration" },
            { "data": "membership_id" },
            { "data": "first_name" },
            { "data": "last_name" },
            { "data": "email" }
        ]
    });

});

$('#usersdashboard_overview_table tr').not(':first').hover(
    function () {
        $(this).css("background-color", "red");
        $(this).css("color", "white");
    },
    function () {
        $(this).css("background-color", "");
        $(this).css("color", "");
    }
);

$('#usersdashboard_overview_table').on('click', 'tr', function () {
    usersdashboardView.table_row_data = usersdashboardView.table.row(this).data();
});


$('#inactive_usersdashboard_overview_table tr').not(':first').hover(
    function () {
        $(this).css("background-color", "red");
        $(this).css("color", "white");
    },
    function () {
        $(this).css("background-color", "");
        $(this).css("color", "");
    }
);

$('#inactive_usersdashboard_overview_table').on('click', 'tr', function () {
    usersdashboardView.activate_table_row_data = usersdashboardView.activatetable.row(this).data();
});


$('#activate_user_button').on('click', function (e) {
    if (usersdashboardView.activatetable.rows('.selected').data().length == 0) {

        $('#activate_user_button').prop('disabled', true);

        $('#usersdashboard_view_notification').notify({

            message: { text: 'No user has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#activate_user_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#activate_usersdashboard_modal').data('url') + '/' + usersdashboardView.activate_table_row_data.id;

        $.get(url, function (data) {

            $('#activate_usersdashboard_modal_body').html(data);
            $('#activate_usersdashboard_modal').modal('show');

        });
    }
});

$('#activate_usersdashboard_modal_submit_button').on('click', function () {

    $('#activate_users_form').submit();
});


$('#switchstate_user_button').on('click', function (e) {
    if (usersdashboardView.table.rows('.selected').data().length == 0) {

        $('#switchstate_user_button').prop('disabled', true);

        $('#usersdashboard_view_notification').notify({

            message: { text: 'No user has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {

                $('#switchstate_user_button').prop('disabled', false);
            }

        }).show();
    }
    else {

        var url = $('#switch_usersdashboard_modal').data('url') + '/' + usersdashboardView.table_row_data.id;

        $.get(url, function (data) {

            $('#switch_usersdashboard_modal_body').html(data);
            $('#switch_usersdashboard_modal').modal('show');

        });
    }
});

$('#switch_usersdashboard_modal_submit_button').on('click', function () {

    $('#switch_users_form').submit();
});