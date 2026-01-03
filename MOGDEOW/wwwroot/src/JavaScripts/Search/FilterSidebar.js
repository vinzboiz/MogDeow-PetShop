document.addEventListener("DOMContentLoaded", function () {
    const filterToggleBtn = document.getElementById("filterToggleBtn");
    const sidebar = document.querySelector(".sidebar");
    const closeSidebarBtn = document.createElement("button");
    const layoutSidebar = document.getElementById("layout-sidebar-search");

    if (!filterToggleBtn || !sidebar || !layoutSidebar) {
        console.error(
            "Một hoặc nhiều phần tử không tồn tại. Kiểm tra lại ID trong HTML!"
        );
        return;
    }

    // Tạo nút đóng sidebar (chỉ thêm nếu chưa có)
    if (!document.getElementById("closeSidebar")) {
        closeSidebarBtn.id = "closeSidebar";
        closeSidebarBtn.textContent = "Đóng bộ lọc";
        sidebar.appendChild(closeSidebarBtn);
    }

    // Mở sidebar khi nhấn "Lọc sản phẩm"
    filterToggleBtn.addEventListener("click", function () {
        sidebar.classList.add("show");
        layoutSidebar.classList.add("show"); // Hiện layout mở
    });

    // Đóng sidebar khi nhấn vào layout mở
    layoutSidebar.addEventListener("click", function () {
        sidebar.classList.remove("show");
        layoutSidebar.classList.remove("show"); // Ẩn layout mở
    });

    // Đóng sidebar khi nhấn nút "Đóng bộ lọc"
    closeSidebarBtn.addEventListener("click", function () {
        sidebar.classList.remove("show");
        layoutSidebar.classList.remove("show"); // Ẩn layout mở
    });
});
