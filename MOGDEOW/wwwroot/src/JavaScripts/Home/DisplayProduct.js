document.addEventListener("DOMContentLoaded", function () {
    function getProductCount() {
        if (window.innerWidth >= 1024) {
            return 10; // Desktop
        } else if (window.innerWidth >= 768) {
            return 9; // iPad
        } else {
            return 8; // Mobile
        }
    }

    function displayProducts() {
        displayRandomProducts("dog", "dogContainer", 7, "scroll");
        displayRandomProducts("cat", "catContainer", 7, "scroll");
        displayRandomListProduct(
            ["hygiene", "accessories", "food", "housing", "medicine"],
            "productContainer",
            getProductCount(),
            "no-scroll"
        );
    }

    // Load products on page load
    displayProducts();

    // Listen for window resize and update product count dynamically
    window.addEventListener("resize", function () {
        displayProducts();
    });
});
