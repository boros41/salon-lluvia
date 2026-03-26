"use strict";
//https://flatpickr.js.org/

const url = "/api/calendly/available-days";

const options = {
    method: 'GET',
    headers: {
        "Content-Type": "application/json",
        Accept: "application/json",
    }
};

const fp = $("#Date").flatpickr();
$("#Date").prop("disabled", true);
$("#Date").val("Fetching available days...");

fetch(url, options)
    .then(response => {
        if (!response.ok) {
            throw new Error(`HTTP ${response.status}`);
        }

        return response.json()
    })
    .then(response => {
        console.log(response);

        const availableDays = response;

        const config = {
            enable: availableDays, // this has to be specified first or else the dates will be offset by 1 for some reason
            altInput: true,
            altFormat: "F j, Y",
            dateFormat: "Z",
            onChange: onDateChange,
            onClose: onDateClose
        };

        $("#Date").prop("disabled", false);
        const fp = $("#Date").flatpickr(config);
    })
    .catch(err => console.error(err));

// flatpickr sets the original date input to hidden meaning jQuery-validate libraries
// won't run so I have to manually validate flatpickr's date picker
function onDateChange(selectedDates, dateStr, instance) {
    if (dateStr) {
        $("#Date + input").removeClass("input-validation-error").addClass("valid");
        $(".date-validation-error").removeClass("field-validation-error")
                                   .addClass("field-validation-valid")
                                   .text("");
    }
}

function onDateClose(selectedDates, dateStr, instance) {
    if (!dateStr) {
        $("#Date + input").addClass("input-validation-error").removeClass("valid");

        $(".date-validation-error").addClass("field-validation-error")
                                   .removeClass("field-validation-valid")
                                   .text($("#Date").data("valRequired"));
    }
}

function onSubmit(event) {
    const date = $("#Date").val();
    if (!date) {
        event.preventDefault();
        $("#Date + input").addClass("input-validation-error").removeClass("valid");
        $(".date-validation-error").addClass("field-validation-error")
                                   .removeClass("field-validation-valid")
                                   .text($("#Date").data("valRequired"));
    }
}