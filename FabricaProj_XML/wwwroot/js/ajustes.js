const input = document.querySelector("#competencia");

input.addEventListener("click", () => {
    if (typeof input.showPicker === "function") {
        input.showPicker();
    }
});