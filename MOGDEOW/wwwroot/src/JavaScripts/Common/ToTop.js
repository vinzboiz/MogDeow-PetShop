document.addEventListener("DOMContentLoaded", function () {
  let backToTopBtn = document.getElementById("backToTopBtn");

  // Hiển thị nút khi cuộn xuống 200px
  window.onscroll = function () {
    if (
      document.body.scrollTop > 200 ||
      document.documentElement.scrollTop > 200
    ) {
      backToTopBtn.style.display = "block";
    } else {
      backToTopBtn.style.display = "none";
    }
  };

  // Hàm cuộn lên đầu trang
  function scrollToTop() {
    window.scrollTo({ top: 0, behavior: "smooth" });
  }

  // Gán sự kiện click cho nút
  backToTopBtn.addEventListener("click", scrollToTop);
});
