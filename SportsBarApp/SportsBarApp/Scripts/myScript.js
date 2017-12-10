$(function () {

    
    $(".datePicker").datepicker({

        dateFormat: "dd/mm/yy",
        changeYear: true,
        maxDate : '0'
    });        
        
   
    //$('div.profile-links a').click(function () {
    //    changeColorOnClick('div.profile-links a', $(this));
    //});
    //$('section.profile-links a').click(function () {
    //    changeColorOnClick('section.profile-links a', $(this));
    //});   
    

    //function changeColorOnClick(arr, link) {
    //    var clicked = $(link).text();
    //    $(link).css('color', 'red');
    //    $(arr).each(function () {             
    //        if ($(this).text() !== clicked) {
    //            $(this).css('color', '#4d7499');
    //        }
    //    });
    //}

})
