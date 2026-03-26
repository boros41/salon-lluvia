"use strict";

const resourceUrl = "/api/azureblobstorage/image-url";

const options = {
    method: "GET",
    headers: {
        Accept: "text/plain",
    }
};

fetch(resourceUrl, options)
    .then(response => {
        if (!response.ok) {
            throw new Error(`HTTP ${response.status}`);
        }

        return response.text();
    })
    .then(response => {
        console.log(`The image url is: ${response}`)
    })