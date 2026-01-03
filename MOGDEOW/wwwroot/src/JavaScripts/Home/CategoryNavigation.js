document.addEventListener("DOMContentLoaded", function () {
  // ✅ Lấy tất cả thẻ <a> có thuộc tính data-category
  const categoryLinks = document.querySelectorAll("[data-category]");
  // ✅ Chọn tất cả các phần tử `.category-item`
  const categoryItems = document.querySelectorAll(".category-item");
  // ✅ Lấy tất cả các nút "XEM THÊM" có data-category
  const viewMoreButtons = document.querySelectorAll(".btn[data-category]");

  // ✅ Thêm sự kiện click vào từng thẻ <a> có data-category
  categoryLinks.forEach((link) => {
    link.addEventListener("click", function (event) {
      event.preventDefault(); // Ngăn chặn hành vi mặc định của thẻ <a>

      const category = this.getAttribute("data-category"); // Lấy giá trị của data-category
      console.log("📌 Chuyển hướng đến danh mục:", category);

      if (category) {
        window.location.href = `search.html?category=${encodeURIComponent(
          category
        )}`;
      }
    });
  });

  // ✅ Thêm sự kiện click vào từng phần tử `.category-item`
  categoryItems.forEach((item) => {
    item.addEventListener("click", function () {
      // Tìm phần tử chứa data-category trong category-info
      const categoryName = item.querySelector(".category-name");
      if (!categoryName) return;

      const category = categoryName.getAttribute("data-category");
      console.log("📌 Chuyển hướng đến danh mục:", category);

      if (category) {
        window.location.href = `search.html?category=${encodeURIComponent(
          category
        )}`;
      }
    });
  });

  // ✅ Gán sự kiện click cho từng nút "XEM THÊM"
  viewMoreButtons.forEach((button) => {
    button.addEventListener("click", function (event) {
      event.preventDefault(); // Ngăn chặn hành vi mặc định của <a>

      const category = this.getAttribute("data-category"); // Lấy category
      console.log("📌 Chuyển hướng đến danh mục:", category);

      if (category) {
        window.location.href = `search.html?category=${encodeURIComponent(
          category
        )}`;
      }
    });
  });
});
