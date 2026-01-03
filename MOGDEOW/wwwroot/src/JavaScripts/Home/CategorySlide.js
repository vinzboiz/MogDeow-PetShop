document.addEventListener("DOMContentLoaded", function () {
  const scrollContainer = document.querySelector(".category-scrollbar");

  // Xử lý cuộn ngang khi sử dụng bánh xe chuột
  scrollContainer.addEventListener("wheel", function (event) {
    event.preventDefault();
    scrollContainer.scrollLeft += event.deltaY * 2; // Tăng tốc độ cuộn
  });

  // Xử lý kéo chuột để cuộn
  let isDragging = false;
  let startX;
  let scrollLeftPos;

  scrollContainer.addEventListener("mousedown", (e) => {
    isDragging = true;
    scrollContainer.classList.add("dragging");
    startX = e.pageX - scrollContainer.offsetLeft;
    scrollLeftPos = scrollContainer.scrollLeft;
  });

  scrollContainer.addEventListener("mouseleave", () => {
    isDragging = false;
    scrollContainer.classList.remove("dragging");
  });

  scrollContainer.addEventListener("mouseup", () => {
    isDragging = false;
    scrollContainer.classList.remove("dragging");
  });

  scrollContainer.addEventListener("mousemove", (e) => {
    if (!isDragging) return;
    e.preventDefault();
    const x = e.pageX - scrollContainer.offsetLeft;
    const walk = (x - startX) * 2; // Tăng tốc độ kéo
    scrollContainer.scrollLeft = scrollLeftPos - walk;
  });
});
