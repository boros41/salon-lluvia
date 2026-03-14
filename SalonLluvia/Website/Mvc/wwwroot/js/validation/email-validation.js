"use strict";

// https://github.com/azanov/isMailFine/tree/master

import isMailFine from "./is-mail-fine.js";

jQuery.validator.addMethod("email", function (value, element, param) {
    return isMailFine(value, true, false);
});

jQuery.validator.unobtrusive.adapters.addBool("email");