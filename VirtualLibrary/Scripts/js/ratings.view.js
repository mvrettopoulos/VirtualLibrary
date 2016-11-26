var ratingsView = {};
ratingsView.table = '';
ratingsView.table_row_data = '';
$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip({
        placement: 'bottom'
    });
    ratingsView.table = $('#ratings_overview_table').DataTable({
        scrollX: true,
        select: {
            style: 'single'
        },
        columns: [
            { "data": "id" },
            { "data": "title" },
            { "data": "comment" },
            { "data": "rating" },
            { "data": "username" }
        ]
    });
});
$('#ratings_overview_table tr').not(':first').hover(
    function () {
        $(this).css("background-color", "red");
        $(this).css("color", "white");
    },
    function () {
        $(this).css("background-color", "");
        $(this).css("color", "");
    }
);
$('#ratings_overview_table').on('click', 'tr', function () {
    ratingsView.table_row_data = ratingsView.table.row(this).data();
});
$('#delete_rating_button').on('click', function () {
    if (ratingsView.table.rows('.selected').data().length == 0) {
        $('#delete_rating_button').prop('disabled', true);
        $('#ratings_view_notification').notify({
            message: { text: 'No rating has been selected' },
            fadeOut: { enabled: true, delay: 2000 },
            closable: false,
            type: 'blackgloss',
            onClose: function () {
                $('#delete_rating_button').prop('disabled', false);
            }
        }).show();
    }
    else {
        var url = $('#delete_rating_modal').data('url') + '/' + ratingsView.table_row_data.id;
        $.get(url, function (data) {
            $('#delete_rating_modal_body').html(data);
            $('#delete_rating_modal').modal('show');
        });
    }
});
$('#delete_rating_modal_submit_button').on('click', function () {
    $('#delete_rating_form').submit();
});