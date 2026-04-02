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
    $(".filter-hair-color").on("click", filterImagesByHairColor);
}

function filterImagesByGender(event) {
    const $element = $(this);
    $element.siblings(".badge.text-bg-primary").removeClass("text-bg-primary").addClass("text-bg-secondary");
    $element.removeClass("text-bg-secondary").addClass("text-bg-primary");

    let gender = $element.text();
    const $hairstyleFilters = $(".filter-hairstyle-true");
    const $hairColors = $(".filter-hair-color-true");

    fetchFilteredImages(gender, $hairstyleFilters, $hairColors);
}

function filterImagesByHairstyle(event) {
    const $element = $(this);

    if ($element.hasClass("text-bg-primary")) {
        $element.removeClass("text-bg-primary").addClass("text-bg-secondary");
        $element.removeClass("filter-hairstyle-true");
    } else if ($element.hasClass("text-bg-secondary")) {
        $element.removeClass("text-bg-secondary").addClass("text-bg-primary");
        $element.addClass("filter-hairstyle-true");
    }

    const gender = $(".filter-gender.text-bg-primary").text();
    const $hairstyleFilters = $(".filter-hairstyle-true");
    const $hairColors = $(".filter-hair-color-true");

    fetchFilteredImages(gender, $hairstyleFilters, $hairColors);
}

function filterImagesByHairColor(event) {
    const $element = $(this);

    if ($element.hasClass("text-bg-primary")) {
        $element.removeClass("text-bg-primary").addClass("text-bg-secondary");
        $element.removeClass("filter-hair-color-true");
    } else if ($element.hasClass("text-bg-secondary")) {
        $element.removeClass("text-bg-secondary").addClass("text-bg-primary");
        $element.addClass("filter-hair-color-true");
    }

    const gender = $(".filter-gender.text-bg-primary").text();
    const $hairstyleFilters = $(".filter-hairstyle-true");
    const $hairColors = $(".filter-hair-color-true");

    fetchFilteredImages(gender, $hairstyleFilters, $hairColors);
}

function createImageCards(images) {
    $("footer").addClass("d-none");

    $.each(images, function (imageIndex, value) {
        const image = value;
        const imageId = image.id;
        const thumbnailUrl = image.url; // url points to the image blob in Azure Blob Storage
        const imageUrl = null; // this should be a full resolution image that opens on the <a> tag but I've yet to implement
        const imageDescription = image.description;
        const hairstyles = image.hairstyles;
        const hairColors = image.hair_colors;

        const $gridItem = $("<div/>").addClass("col-sm-6 col-lg-3 grid-item");
        const $card = $("<div/>").addClass("card").appendTo($gridItem);
        const $galleryItem = $("<div/>").addClass("gallery-item").appendTo($card);
        const $image = $("<img/>").attr("src", thumbnailUrl).addClass("card-img-top img-fluid").appendTo($galleryItem);
        const $galleryIconContainer = $("<div/>").addClass("gallery-icon").appendTo($galleryItem);
        const $fullImageLink = $("<a/>").attr("href", thumbnailUrl)
                                        .attr("data-lightbox", "gallery")
                                        .addClass("btn btn-primary btn-lg-square img-full")
                                        .appendTo($galleryIconContainer);

        const $thumbnailIcon = $("<i/>").addClass("fa fa-eye").appendTo($fullImageLink);
        const $cardBody = $("<div/>").addClass("card-body pt-1 pb-1").attr("id", `card-body-img-${imageIndex}`).appendTo($card);
        const $hairstyleBadge = $("<div/>").addClass("hairstyle-badge mb-1").appendTo($cardBody);
        const $hairstyleBadgeLabel = $("<span/>").addClass("fw-semibold mb-0").text("Peinados: ").appendTo($hairstyleBadge);
        const $hairColorBadge = $("<div/>").addClass("hair-color-badge").appendTo($cardBody);
        const $hairColorBadgeLabel = $("<span/>").addClass("fw-semibold mb-0").text("Colores: ").appendTo($hairColorBadge);

        // This is set by Razor code in Gallery.cshtml
        if (isAdminUser === true) {
            const $adminDeleteImageLink = $("<a/>").addClass("btn btn-link p-0").attr("href", adminDeleteImageUrl + imageId).text("Delete Image").appendTo($cardBody);
        }

        if (imageDescription.trim().length !== 0) {
            $image.attr("alt", "Description: " + imageDescription);

            $fullImageLink.attr("data-title", imageDescription);
            $fullImageLink.attr("data-alt", imageDescription);

            const $decriptionText = $("<span/>").addClass("card-text mb-1").text(imageDescription).prependTo($cardBody);
            const $descriptionLabel = $("<span/>").addClass("fw-semibold mb-0").text("Description: ").prependTo($cardBody);
        } else {
            const hairstylesAltDescription = "Peinados: " + hairstyles.map(hairstyleObject => hairstyleObject.style).join(", ");
            $image.attr("alt", hairstylesAltDescription);
            $fullImageLink.attr("data-alt", hairstylesAltDescription);
        }

        // This is the Bootstrap card content the above code will create. The image description <span>s are excluded if it wasn't set.
        /*const imageCardHtmlString = `
            <div class="col-sm-6 col-lg-3 grid-item">
                <div class="card">
                    <div class="gallery-item">
                        <img src="${thumbnailUrl}" class="card-img-top img-fluid" alt="${imageDescription}" />
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
                            <span class="fw-semibold mb-0">Peinados:</span>
                        </div>

                        <div class="hair-color-badge">
                            <span class="fw-semibold mb-0">Colores:</span>
                        </div>
                    </div>
                </div>
            </div>`;*/

        // https://masonry.desandro.com/methods#adding-removing-items:~:text=demo%20on%20CodePen-,While,-jQuery%20can%20add
        // At this point, the grid item is created & laid out with Masonry, but the image blob might not be loaded.
        // We will call Masonry to lay out the grid item again once the image it contains is loaded via the imagesloaded.js library
        $grid.append($gridItem).masonry("appended", $gridItem);

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
        // the admin upload image button is inside the accordion so hide everything else (filter options)
        $("#gallery-spinner").addClass("d-none");
        $(".accordion-body > .row").addClass("d-none");
        $(".accordion-body").children(".row").last().removeClass("d-none");

        $("<p/>").addClass("m-1 text-danger text-center").text("Lo sentimos, por el momento no hay imágenes disponibles.").appendTo(".container");

        // non-admin user shouldn't see the filter option if there are no images
        if (isAdminUser === false) {
            $(".accordion").addClass("d-none");
        }

        return;
    }

    // unloaced images can throw off Masonry layouts and cause items to overlap
    // use imagesloaded.js to trigger a Masonry layout after each image loads
    // https://masonry.desandro.com/layout#:~:text=demo%20on%20CodePen-,imagesLoaded,-Unloaded%20images%20can
    $grid.imagesLoaded().progress(function (instance, image) {
        var result = image.isLoaded ? "loaded" : "broken";

        // This is true if the image blob was deleted in Azure but the server's memory cache still contained the image's URL
        if (result === "broken") {
            const $brokenGridItem = $(image.img).closest(".grid-item");
            $grid.masonry("remove", $brokenGridItem);
            $brokenGridItem.remove();
        }

        $grid.masonry("layout");
    });
}

function fetchFilteredImages(gender, $hairstyles, $hairColors) {
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

    $hairColors.each(function (index, element) {
        const queryValue = element.textContent.trim().toLowerCase();
        queryParams.append("hairColors", queryValue);
    });

    const resourceUrl = `/api/azureblobstorage/image-url?${queryParams.toString()}`;

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