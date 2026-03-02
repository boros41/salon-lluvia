let isAscending = true; // Appointment controller initially returns ascending order
const $sortBtn = $("#appointment-sort-date");
const ascendingIcon = "fas fa-sort-numeric-down";
const descendingIcon = "fas fa-sort-numeric-up-alt";


$(document).ready(function () {
    $sortBtn.on("click", function (e) {
        e.preventDefault();
        SortDate();
    });
});

function SortDate() {
    const rows = $(".appointment-row").get();
    if (rows.length < 2) return;

    rows.sort((rowA, rowB) => {
        const timeStringA = $(rowA).find(".appointment-date").text().trim();
        const timeStringB = $(rowB).find(".appointment-date").text().trim();

        const timeA = new Date(timeStringA).getTime();
        const timeB = new Date(timeStringB).getTime();

        if (isAscending) {
            return SortDescending(timeA, timeB);
        } else {
            return SortAscending(timeA, timeB);
        }
    });

    $("#appointment-table-body").append(rows);

    isAscending = !isAscending;
}

function SortDescending(a, b) {
    $sortBtn.removeClass(ascendingIcon).addClass(descendingIcon);
    return b - a;
}

function SortAscending(a, b) {
    $sortBtn.removeClass(descendingIcon).addClass(ascendingIcon);
    return a - b;
}