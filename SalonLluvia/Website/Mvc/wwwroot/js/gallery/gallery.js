"use strict";

$(document).ready(function () {
    const resourceUrl = "/api/azureblobstorage/image-url";

    const options = {
        method: "GET",
        headers: {
            Accept: "application/json",
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

            $.each(images, function (imageIndex, value) {
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
                        <div class="card-body pt-1 pb-1" id="card-body-img-${imageIndex}">
                            <span class="fw-semibold mb-0">Description:</span>
                            <span class="card-text mb-1">${imageDescription}</span>

                            <div class="hairstyle-badge mb-1">
                                <span class="fw-semibold mb-0">Hairstyles:</span>
                            </div>

                            <div class="hair-color-badge">
                                <span class="fw-semibold mb-0">Hair Colors:</span>
                            </div>
                        </div>
                    </div>
                </div>`;

                // Masonry.js is unable to append HTML string content so we must wrap it in a jQuery object
                // https://masonry.desandro.com/methods#adding-removing-items:~:text=demo%20on%20CodePen-,While,-jQuery%20can%20add
                const $imageCard = $(imageCardHtmlString);

                // At this point, the <img> is created, but the image blob might not be loaded, we will init Masonry.js after all are loaded to lay them out
                $grid.append($imageCard);

                $("#gallery-spinner").addClass("d-none");

                // Bootstrap badges for the image's hair color metadata
                $.each(hairColors, function (index, currentHairColor) {
                    const hairColor = currentHairColor.color;
                    const hairColorBadgeHtmlString = `<span class="badge rounded-pill text-bg-primary">${hairColor}</span>`;
                    const $badgeParent = $(`#card-body-img-${imageIndex} .hair-color-badge`);

                    $(hairColorBadgeHtmlString).appendTo($badgeParent);
                });

                // Bootstrap badges for the image's hairstyle metadata
                $.each(hairstyles, function (index, currentHairStyle) {
                    const hairstyle = currentHairStyle.style;
                    const hairstyleBadgeHtmlString = `<span class="badge rounded-pill text-bg-secondary">${hairstyle}</span>`;
                    const $badgeParent = $(`#card-body-img-${imageIndex} .hairstyle-badge`);

                    $(hairstyleBadgeHtmlString).appendTo($badgeParent);
                });

            });

            if ($grid.children().length === 0) {
                console.log("No images found!!!");
                return;
            }

            $grid.imagesLoaded(function () {
                // init Masonry.js after all images have loaded
                // https://masonry.desandro.com/layout#imagesloaded:~:text=Or%2C%20initialize%20Masonry%20after%20all%20images%20have%20been%20loaded.
                $grid.masonry({
                    percentPosition: true
                });
            })
            .progress(function (instance, image) {
                var result = image.isLoaded ? "loaded" : "broken";

                // This is true if the image blob was deleted in Azure but the server's memory cache still contained the image's URL
                if (result === "broken") {
                    console.log(`Removing broken image: ${image.img.src}`);
                    $(image.img).parents(".card").parent("div").remove();
                }
            });
        })
});
