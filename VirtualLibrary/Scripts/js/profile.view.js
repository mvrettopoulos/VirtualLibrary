$(document).ready(function () {
    $('#profile').click(function () {
        var url = $('#edit_profile_modal').data('url');

        $.get(url, function (data) {


            $('#edit_profile_modal').modal('show');
        });
    });
});