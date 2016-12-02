
$("#datepickerReserve").datepicker({
    dateFormat: "dd-mm-yy",
    minDate: 0,
    onSelect: function (date) {

        var date2 = $('#datepickerReserve').datepicker('getDate');
        date2.setDate(date2.getDate());
        $('#datepickerReturn').datepicker('setDate', date2);
        $('#datepickerReturn').datepicker('option', 'minDate', date2);
        $('#datepickerReturn').datepicker('option', 'maxDate', date_add_days(date2,7));
    }
});


$('#datepickerReturn').datepicker({
    dateFormat: "dd-mm-yy",
    onClose: function () {
        var dt1 = $('#datepickerReserve').datepicker('getDate');

        var dt2 = $('#datepickerReturn').datepicker('getDate');
        if (dt2 <= dt1) {
            var minDate = $('#datepickerReturn').datepicker('option', 'minDate');
            $('#datepickerReturn').datepicker('setDate', minDate);
        }
        if (dt2 > date_add_days(dt1, 7)) {
            var maxDate = date_add_days(dt1, 7);
            $('#datepickerReturn').datepicker('setDate', maxDate);
        }
    }
});


function date_add_days(date, days) {
    return new Date(
        date.getFullYear(),
        date.getMonth(),
        date.getDate() + days,
        date.getHours(),
        date.getMinutes(),
        date.getSeconds(),
        date.getMilliseconds()
    );
}


