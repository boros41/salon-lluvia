"use strict";

/*  Gallery page will have a lot of images so a footer doesn't make sense.
    This will improve the CLS score, otherwise the footer will constantly shift as the Masonry grid items transition.
    Also, I will want this page as an infinite scroll.
*/
$("footer").remove();

const $grid = $("#gallery");

const options = {
    method: "GET",
    headers: {
        Accept: "application/json",
    }
};

$(document).ready(function () {

    // init Masonry.js after all images have loaded
    // https://masonry.desandro.com/layout#imagesloaded:~:text=Or%2C%20initialize%20Masonry%20after%20all%20images%20have%20been%20loaded.
    $grid.masonry({
        itemSelector: ".grid-item",
        percentPosition: true,
        transitionDuration: 0
    });

    fetchAllImages();

    addFilterClickListeners();
});

function fetchAllImages() {
    const resourceUrl = "/api/azureblobstorage/image-url";

    fetch(resourceUrl, options)
    .then(response => {
        if (!response.ok) {
            throw new Error(`HTTP ${response.status}`);
        }

        return response.json();
    })
    .then(response => {
        const images = response.images;

        createImageCards(images);
    });
}

function addFilterClickListeners() {
    $(".filter-gender").on("click", filterImagesByGender);
    $(".filter-hairstyle").on("click", filterImagesByHairstyle);
}

function filterImagesByGender(event) {
    const $element = $(this);
    $element.siblings(".badge.text-bg-primary").removeClass("text-bg-primary").addClass("text-bg-secondary");
    $element.removeClass("text-bg-secondary").addClass("text-bg-primary");

    let gender = $element.text();
    const $hairstyleFilters = $(".filter-hairstyle-true");

    fetchFilteredImages(gender, $hairstyleFilters, null);
}

function filterImagesByHairstyle(event) {
    const $element = $(this);

    if ($element.hasClass("text-bg-primary")) {
        $element.removeClass("text-bg-primary").addClass("text-bg-secondary");
        $element.removeClass("filter-hairstyle-true");
    } else if ($element.hasClass("text-bg-secondary")) {
        $element.removeClass("text-bg-secondary").addClass("text-bg-primary");
        $element.removeClass("filter-hairstyle-false").addClass("filter-hairstyle-true");
    }

    const gender = $(".filter-gender.text-bg-primary").text();
    const $hairstyleFilters = $(".filter-hairstyle-true");

    fetchFilteredImages(gender, $hairstyleFilters, null);
}

function createImageCards(images) {
    $("footer").addClass("d-none");

    $.each(images, function (imageIndex, value) {
        const image = value;
        const thumbnailUrl = image.url; // url points to the image blob in Azure Blob Storage
        const imageUrl = null; // this should be a full resolution image that opens on the <a> tag but I've yet to implement
        const imageDescription = image.description;
        const hairstyles = image.hairstyles;
        const hairColors = image.hair_colors;

        // Bootstrap card holding one image in the gallery: https://getbootstrap.com/docs/4.0/components/card/
        const imageCardHtmlString = `
            <div class="col-sm-6 col-lg-3 grid-item">
                <div class="card">
                    <div class="gallery-item">
                        <img src="${thumbnailUrl}" class="card-img-top img-thumbnail" alt="${imageDescription}" />
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

        // Masonry.js is unable to add HTML string content so we must wrap it in a jQuery object
        // https://masonry.desandro.com/methods#adding-removing-items:~:text=demo%20on%20CodePen-,While,-jQuery%20can%20add
        const $imageCard = $(imageCardHtmlString);

        // At this point, the grid item is created & registered with Masonry, but the image blob might not be loaded nor has Masonry laid it out
        // We will call Masonry to lay out the grid item once the image it contains is loaded via the imagesloaded.js library
        $grid.append($imageCard);
        $grid.masonry("addItems", $imageCard);

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

    // unloaced images can throw off Masonry layouts and cause items to overlap
    // use imagesloaded.js to trigger a Masonry layout after each image loads
    // https://masonry.desandro.com/layout#:~:text=demo%20on%20CodePen-,imagesLoaded,-Unloaded%20images%20can
    $grid.imagesLoaded().progress(function (instance, image) {
        var result = image.isLoaded ? "loaded" : "broken";

        // This is true if the image blob was deleted in Azure but the server's memory cache still contained the image's URL
        if (result === "broken") {
            console.log(`Removing broken image: ${image.img.src}`);

            const $brokenGridItem = $(image.img).closest(".grid-item");
            $grid.masonry("remove", $brokenGridItem);
            $brokenGridItem.remove();
        }

        $grid.masonry("layout");
    });
}

function fetchFilteredImages(gender, $hairstyles, hairColors) {
    const queryParams = new URLSearchParams();

    // These are how the server represents genders. I should add proper localization...
    switch (gender) {
        case "Ambos":
            gender = "both";
            break;
        case "Mujer":
            gender = "F";
            break;
        case "Hombre":
            gender = "M";
            break;
    }

    queryParams.append("gender", gender);

    $hairstyles.each(function (index, element) {
        const queryValue = element.textContent.trim().toLowerCase();
        queryParams.append("hairstyles", queryValue);
    });


    const resourceUrl = `/api/azureblobstorage/image-url?${queryParams.toString()}`;

    console.log(`Hairstyles filter URL: ${resourceUrl}`);

    fetch(resourceUrl, options)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP ${response.status}`);
            }

            return response.json();
        })
        .then(response => {
            const images = response.images;

            const $allGridItems = $grid.children(".grid-item");
            $grid.masonry("remove", $allGridItems).masonry("layout");
            $grid.empty(); // simply recreate the newly filtered images

            createImageCards(images);
        });
}