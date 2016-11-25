//var startDate, endDate, ranges = [];

//$("#librarySelect").change(function () {
//    var thisvalue = $(this).find("option:selected").val();

//    $("#datepickerReserve").datepicker("destroy");
//    $("#datepickerReturn").datepicker("destroy");
//    ranges = [];
//    for (var i = 0; i < reservationDatesLibraries.length; i++) {
//        if (reservationDatesLibraries[i].library == thisvalue) {
//            var dateRangesList = reservationDatesLibraries[i].datesRange;
//            for (var k = 0; k < dateRangesList.length; k++) {


//                var startDateArray = dateRangesList[k].startDate.split('-');
//                // rearrange the month and day
//                startDateArray.splice(1, 0, startDateArray.shift());
//                // create a date object called date and pass our array to the constructor
//                var startDate = new Date(startDateArray);

//                var endDateArray = dateRangesList[k].endDate.split('-');
//                // rearrange the month and day
//                endDateArray.splice(1, 0, endDateArray.shift());
//                // create a date object called date and pass our array to the constructor
//                var endDate = new Date(endDateArray);
//                ranges.push({ start: startDate, end: endDate });

//            }
//            setReserveDatepicker();
//            setReturnDatepicker();
//            $("#dateReserve").removeClass("hidden");
//            $("#dateReturn").removeClass("hidden");
//        }

//    }
//});




$("#datepickerReserve").datepicker({
    dateFormat: "dd-mm-yy",
    minDate: 0,
    onSelect: function (date) {

        var date2 = $('#datepickerReserve').datepicker('getDate');
        date2.setDate(date2.getDate());
        $('#datepickerReturn').datepicker('setDate', date2);
        //sets minDate to dt1 date + 1
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


