"use strict";

$(document).ready(function () {
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

            return response.json();
        })
        .then(response => {
            const $grid = $("#gallery");
            const images = response.images;

            $.each(images, function (index, value) {
                const image = value;
                const thumbnailUrl = image.url; // url points to the image blob in Azure Blob Storage
                const imageUrl = null; // this should be a full resolution image that opens on the <a> tag but I've yet to implement
                const imageDescription = image.description;
                const hairstyles = image.hairstyles;
                const hairColors = image.hair_colors;

                // Bootstrap card holding one image in the gallery: https://getbootstrap.com/docs/4.0/components/card/
                const imageCardHtmlString = `
                <div class="col-sm-6 col-lg-3 wow fadeIn">
                    <div class="card">
                        <div class="gallery-item">
                            <img src="${thumbnailUrl}" class="card-img-top img-thumbnail" alt="${imageDescription}"/>
                            <div class="gallery-icon">
                                <a href="${thumbnailUrl}" class="btn btn-primary btn-lg-square img-full" 
                                data-lightbox="gallery" data-title="${imageDescription}" data-alt="${imageDescription}">
                                    <i class="fa fa-eye"></i>
                                </a>
                            </div>
                        </div>
                        <div class="card-body">
                            <p class="card-text">${imageDescription}</p>
                        </div>
                    </div>
                </div>`;

                // Masonry.js is unable to append HTML string content so we must wrap it in a jQuery object
                // https://masonry.desandro.com/methods#adding-removing-items:~:text=demo%20on%20CodePen-,While,-jQuery%20can%20add
                const $imageCard = $(imageCardHtmlString);

                // At this point, the <img> tag is created, but the image blob is not loaded causing Masonry.js grid items to overlap
                $grid.append($imageCard).masonry("appended", $imageCard);

                $("#gallery-spinner").addClass("d-none");

                // Wait for the image to load then notify Masonry.js to lay out the grid item again since its size has changed
                // https://masonry.desandro.com/layout#imagesloaded:~:text=Unloaded%20images%20can,each%20image%20loads.
                $grid.imagesLoaded().progress(function () {
                    $grid.masonry("layout");
                });
            });

            if ($grid.children().length === 0) {
                console.log("No images found!!!");
            }
        })
});
