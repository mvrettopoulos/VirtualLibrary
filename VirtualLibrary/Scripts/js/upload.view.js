$(document).ready(function () {
    $('#upload').click(function () {
        var url = $('#insert_image_modal').data('url');

        $.get(url, function (data) {
           

            $('#insert_image_modal').modal('show');
        });
    });
});