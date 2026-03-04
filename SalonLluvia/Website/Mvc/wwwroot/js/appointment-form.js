"use strict";
// https://www.npmjs.com/package/libphonenumber-js

const { parsePhoneNumber, AsYouType } = libphonenumber;

$(document).ready(function () {
    // I would prefer to use the input event, but I can't solve the "smart caret problem"
    $("#phone-number-wrapper > input").on("blur", formatPhone);
});

function formatPhone() {
    const $input = $(this);
    const rawPhoneNumber = $input.val();

    if (rawPhoneNumber.length < 2) {
        return;
    }

    const phoneNumber = parsePhoneNumber(rawPhoneNumber, "US");

    let formattedPhoneNumber;
    if (phoneNumber.isValid()) {
        formattedPhoneNumber = phoneNumber.format("NATIONAL");
    } else {
        const asYouType = new AsYouType("US");
        formattedPhoneNumber = asYouType.input(phoneNumber.nationalNumber)
    }

    console.log("Formatted number: " + formattedPhoneNumber);
    console.log("Is valid: " + phoneNumber.isValid());

    $input.val(formattedPhoneNumber);
}