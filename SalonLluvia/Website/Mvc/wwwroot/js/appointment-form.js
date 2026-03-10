"use strict";
//https://flatpickr.js.org/

const url = "https://localhost:7187/api/calendly/available-days";

const options = {
    method: 'GET',
    headers: {
        "Content-Type": "application/json",
        Accept: "application/json",
        Authorization: "Bearer "

    }
};

const fp = $("#Date").flatpickr();
$("#Date").prop("disabled", true);
$("#Date").val("Fetching available days...");

fetch(url, options)
    .then(response => response.json())
    .then(response => {
        console.log(response);

        const availableDays = response;

        const config = {
            enable: availableDays, // this has to be specified first or else the dates will be offset by 1 for some reason
            altInput: true,
            altFormat: "F j, Y",
            dateFormat: "Z",
        };

        $("#Date").prop("disabled", false);
        const fp = $("#Date").flatpickr(config);

        console.log(fp.dateFormat);
    })
    .catch(err => console.error(err));