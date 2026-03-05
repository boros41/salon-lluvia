"use strict";
// https://www.npmjs.com/package/libphonenumber-js

const { parsePhoneNumber, AsYouType } = libphonenumber;

jQuery.validator.addMethod("phonenumber", function (value, element, param) {
    let phoneNumber;
    try {
        phoneNumber = parsePhoneNumber(value, "US");
    } catch (error) {
        element.value = value;

        return false;
    }

    if (!phoneNumber.isValid()) {
        const asYouType = new AsYouType("US");
        const partiallyFormattedPhoneNumber = asYouType.input(phoneNumber.nationalNumber)

        element.value = partiallyFormattedPhoneNumber;

        return false;
    }

    const formattedPhoneNumber = phoneNumber.format("NATIONAL");
    element.value = formattedPhoneNumber;

    return true;
});

jQuery.validator.unobtrusive.adapters.addBool("phonenumber");