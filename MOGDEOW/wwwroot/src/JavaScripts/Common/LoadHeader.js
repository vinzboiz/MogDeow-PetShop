document.addEventListener("DOMContentLoaded", function () {
    console.log("🚀 Đang tải các sự kiện của Header...");

    // ✅ Kiểm tra trước khi khởi chạy
    if (!document.getElementById("open-cart") &&
        !document.getElementById("open-menu") &&
        !document.getElementById("open-drop-accesories") &&
        !document.getElementById("open-drop-pet")) {
        console.error("⚠️ Không tìm thấy phần tử cần thiết! Kiểm tra lại HTML.");
        return;
    }

    // ✅ Khởi tạo sự kiện sau khi DOM tải xong
    initHeaderCartEvents();
    initHeaderMenuEvents();
    initHeaderDropDownMenu();
});

// ✅ Xử lý sự kiện mở và đóng giỏ hàng
function initHeaderCartEvents() {
    const openCartBtn = document.getElementById("open-cart");
    const cartSidebar = document.getElementById("cart-sidebar");
    const closeCartBtn = document.getElementById("close-cart");
    const layoutSidebar = document.getElementById("layout-sidebar");

    if (!openCartBtn || !cartSidebar || !closeCartBtn || !layoutSidebar) {
        console.warn("⚠️ Không tìm thấy phần tử giỏ hàng! Kiểm tra lại HTML.");
        return;
    }

    openCartBtn.addEventListener("click", function (e) {
        e.preventDefault();
        cartSidebar.classList.toggle("active");
        layoutSidebar.classList.toggle("active");
    });

    closeCartBtn.addEventListener("click", function () {
        cartSidebar.classList.remove("active");
        layoutSidebar.classList.remove("active");
    });

    layoutSidebar.addEventListener("click", function () {
        cartSidebar.classList.remove("active");
        layoutSidebar.classList.remove("active");
    });

    console.log("✅ Sự kiện giỏ hàng đã được gán!");
}

// ✅ Xử lý sự kiện mở và đóng menu
function initHeaderMenuEvents() {
    const openMenuBtn = document.getElementById("open-menu");
    const menuSidebar = document.getElementById("menu-sidebar");
    const closeMenuBtn = document.getElementById("close-menu");
    const layoutSidebar = document.getElementById("layout-sidebar");

    if (!openMenuBtn || !menuSidebar || !closeMenuBtn || !layoutSidebar) {
        console.warn("⚠️ Không tìm thấy phần tử menu! Kiểm tra lại HTML.");
        return;
    }

    openMenuBtn.addEventListener("click", function (e) {
        e.preventDefault();
        menuSidebar.classList.toggle("active");
        layoutSidebar.classList.toggle("active");
    });

    closeMenuBtn.addEventListener("click", function () {
        menuSidebar.classList.remove("active");
        layoutSidebar.classList.remove("active");
    });

    layoutSidebar.addEventListener("click", function () {
        menuSidebar.classList.remove("active");
        layoutSidebar.classList.remove("active");
    });

    console.log("✅ Sự kiện menu đã được gán!");
}

// ✅ Xử lý menu thả xuống (dropdown)
function initHeaderDropDownMenu() {
    const openAccesories = document.getElementById("open-drop-accesories");
    const dropDownAccesoires = document.getElementsByClassName("dropdown-accesories");
    const openPet = document.getElementById("open-drop-pet");
    const dropDownPet = document.getElementsByClassName("dropdown-pet");

    if (!openAccesories && !openPet) {
        console.warn("⚠️ Không tìm thấy phần tử dropdown!");
        return;
    }

    if (openAccesories) {
        openAccesories.addEventListener("click", function (e) {
            e.preventDefault();
            Array.from(dropDownAccesoires).forEach((item) => item.classList.toggle("active"));
            console.log("✅ Dropdown phụ kiện đã được bật/tắt!");
        });
    }

    if (openPet) {
        openPet.addEventListener("click", function (e) {
            e.preventDefault();
            Array.from(dropDownPet).forEach((item) => item.classList.toggle("active"));
            console.log("✅ Dropdown thú cưng đã được bật/tắt!");
        });
    }
}
