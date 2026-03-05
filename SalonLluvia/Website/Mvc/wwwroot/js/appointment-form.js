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
    let phoneNumber;

    try {
        phoneNumber = parsePhoneNumber(rawPhoneNumber, "US");
    } catch (error) {
        $input.val("");

        return;
    }

    let formattedPhoneNumber;
    if (phoneNumber.isValid()) {
        formattedPhoneNumber = phoneNumber.format("NATIONAL");
    } else {
        const asYouType = new AsYouType("US");
        formattedPhoneNumber = asYouType.input(phoneNumber.nationalNumber)
    }

    $input.val(formattedPhoneNumber);
}