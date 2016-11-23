var startDate, endDate, dateRange = [];

$("#librarySelect").change(function(){
    var thisvalue = $(this).find("option:selected").val();

    for (var i = 0; i < reservationDatesLibraries.length;i++) {
        if (reservationDatesLibraries[i].library == thisvalue) {
            var dateRangesList = reservationDatesLibraries[i].datesRange;
            for (var k = 0; k < dateRangesList.length; k++) {
                endDate = jQuery.datepicker.formatDate('dd-mm-yy', dateRangesList[k].endDate);
                for (var d = new Date(jQuery.datepicker.formatDate('dd-mm-yy', dateRangesList[k].startDate)) ;
                    d <= new Date(jQuery.datepicker.formatDate('dd-mm-yy', dateRangesList[k].endDate)) ;
                    d.setDate(d.getDate() + 1)) {
                    dateRange.push($.datepicker.formatDate('yy-mm-dd', d));
                }
            }
            setReserveDatepicker();
            setReturnDatepicker();
            $("#dateReserve").removeClass("hidden");
            $("#dateReturn").removeClass("hidden");
        }

    }
});



function setReserveDatepicker(){
    $("#datepickerReserve").datepicker({
        dateFormat: "dd-mm-yy",
        minDate: 0,
        onSelect: function (date) {
            var date2 = $('#datepickerReserve').datepicker('getDate');
            date2.setDate(date2.getDate() + 1);
            $('#datepickerReturn').datepicker('setDate', date2);
            //sets minDate to dt1 date + 1
            $('#datepickerReturn').datepicker('option', 'minDate', date2);
            $('#datepickerReturn').datepicker('option', 'maxDate', date2.getDate() - 11);
        },
        beforeShowDay: function (date) {
            var dateString = jQuery.datepicker.formatDate('yy-mm-dd', date);
            return [dateRange.indexOf(dateString) == -1];
        }
    });
}


function setReturnDatepicker() {
    $('#datepickerReturn').datepicker({
        dateFormat: "dd-mm-yy",
        onClose: function () {
            var dt1 = $('#datepickerReserve').datepicker('getDate');
            console.log(dt1);
            var dt2 = $('#datepickerReturn').datepicker('getDate');
            if (dt2 <= dt1) {
                var minDate = $('#datepickerReturn').datepicker('option', 'minDate');
                $('#datepickerReturn').datepicker('setDate', minDate);
            }
        },
        beforeShowDay: function (date) {
            var dateString = jQuery.datepicker.formatDate('yy-mm-dd', date);
            return [dateRange.indexOf(dateString) == -1];
        }
    });
}