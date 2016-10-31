var caseStudiesView = {};

caseStudiesView.table = '';
caseStudiesView.table_row_data = '';

$(document).ready(function () {
caseStudiesView.table = $('#case_studies_overview_table').DataTable({
    columns: [
        { "data": "id" },
        { "data": "description" },
        { "data": "version" },
        { "data": "scene" },
        { "data": "multimedia" },
        { "data": "documentation" }
    ]
});

});
$('#case_studies_overview_table tr').not(':first').hover(

    function () {
        $(this).css('background-color', '#bce8f1');
        $(this).css('color', 'black');
    },
    function () {
        $(this).css('background-color', '');
        $(this).css('color', '');
    }
);

function showpdf(name) {
    $('#show_pdf_modal_' + name).modal();
}