var startDate, endDate, ranges = [];

$("#librarySelect").change(function(){
    var thisvalue = $(this).find("option:selected").val();

    for (var i = 0; i < reservationDatesLibraries.length;i++) {
        if (reservationDatesLibraries[i].library == thisvalue) {
            var dateRangesList = reservationDatesLibraries[i].datesRange;
            for (var k = 0; k < dateRangesList.length; k++) {


                var startDateArray = dateRangesList[k].startDate.split('-');
                // rearrange the month and day
                startDateArray.splice(1, 0, startDateArray.shift());
                // create a date object called date and pass our array to the constructor
                var startDate = new Date(startDateArray);

                var endDateArray = dateRangesList[k].endDate.split('-');
                // rearrange the month and day
                endDateArray.splice(1, 0, endDateArray.shift());
                // create a date object called date and pass our array to the constructor
                var endDate = new Date(endDateArray);
                ranges.push({ start: startDate, end: endDate });

                //endDate = jQuery.datepicker.formatDate('dd-mm-yy', dateRangesList[k].endDate);
                //for (var d = new Date(jQuery.datepicker.formatDate('dd-mm-yy', dateRangesList[k].startDate)) ;
                //    d <= new Date(jQuery.datepicker.formatDate('dd-mm-yy', dateRangesList[k].endDate)) ;
                //    d.setDate(d.getDate() + 1)) {
                //    
                //}
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

            var tmpDate = Date.parse(date2);
            console.log("Datepicker :"+tmpDate);
            for (var i = 0; i < ranges.length; i++) {
                var startDate = Date.parse(ranges[i].start);
               // var endDate = Date.parse(ranges[i].end);
                if (tmpDate >= startDate) {
                    
                    $('#datepickerReturn').datepicker('option', 'maxDate', ranges[i].end);
                    
                    
                }
            }
           
        },
        beforeShowDay: function (date) {
            for (var i = 0; i < ranges.length; i++) {
                if (date >= ranges[i].start && date <= ranges[i].end) return [false, ''];
            }
            return [true, ''];
        }
    });
}


function setReturnDatepicker() {
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
}