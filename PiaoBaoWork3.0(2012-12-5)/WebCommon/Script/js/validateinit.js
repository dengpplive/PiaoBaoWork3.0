$(document).ready(function () {
    $.validator.setDefaults({
        submitHandler: function (form) {
            if ($(form).valid())
                form.submit();
        }
    });
    $.metadata.setType("attr", "validate");
    $("form").each(function (index, item) {
        $(item).validate({
            errorElement: "em",
            errorClass: "error"
            //errorPlacement: function (error, element) { element.after(error.attr("class", "error")); },
            //            success: function (label) {
            //                label.text(" ").addClass("success");
            //            }
        });
    })
})