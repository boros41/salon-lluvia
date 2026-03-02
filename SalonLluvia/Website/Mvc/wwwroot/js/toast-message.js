"use strict";

const $toast = $("#liveToast");

if ($toast.length) {
    const toastBootstrap = bootstrap.Toast.getOrCreateInstance($toast[0]);
    toastBootstrap.show();
}