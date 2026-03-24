"use strict";

// hack to override jQuery's validation for the checkboxes generated with the @Html.CheckBoxFor() in Gallery.cshtml.
// the logic is to pass validation if at least one checkbox in Hairstyles & Hair colors section is checked.
$(document).ready(function () {
    const $hairstyleCheckboxes = $(".gallery-hairstyle-checkbox");
    const $hairColorCheckboxes = $(".gallery-haircolor-checkbox");
    const $description = $("#Description");

    // MVC generated these for the ICheckbox.IsChecked property since the type is a non-nullable bool but we don't need them here.
    $hairstyleCheckboxes.removeAttr("data-val data-val-required");
    $hairColorCheckboxes.removeAttr("data-val data-val-required");

    $("#upload-image-form").on("submit", function (event) {
        // jQuery still adds .valid & .field-validation-valid even if no checkboxes are set which goes against the expected validation logic.
        $hairstyleCheckboxes.removeClass("valid");
        $hairColorCheckboxes.removeClass("valid");
        $("#female").removeClass("valid");
        $("#male").removeClass("valid");

        if ($description.val().trim().length === 0) {
            $description.removeClass("valid");
        }

        if ($hairstyleCheckboxes.is(":checked")) {
            $("#hairstyle-error").text("");
        } else {
            $("#hairstyle-error").text("Please enter at least one hairstyle.").removeClass("field-validation-valid");
        }

        if ($hairColorCheckboxes.is(":checked")) {
            $("#hair-color-error").text("");
        } else {
            $("#hair-color-error").text("Please enter at least one hair color.").removeClass("field-validation-valid");
        }
    });

    $hairstyleCheckboxes.change(function () {
        $hairstyleCheckboxes.removeClass("valid");

        if ($hairstyleCheckboxes.is(":checked")) {
            $("#hairstyle-error").text("");
        } else {
            $("#hairstyle-error").text("Please enter at least one hairstyle.").removeClass("field-validation-valid");
        }
    });

    $hairColorCheckboxes.change(function () {
        $hairColorCheckboxes.removeClass("valid");

        if ($hairColorCheckboxes.is(":checked")) {
            $("#hair-color-error").text("");
        } else {
            $("#hair-color-error").text("Please enter at least one hair color.").removeClass("field-validation-valid");
        }
    });

    $description.on("blur", function () {
        if ($description.val().trim().length === 0) {
            $description.removeClass("valid"); 
        }
    });
});