document.addEventListener("DOMContentLoaded", function () {
  // Các biến toàn cục
  let currentIndex = 0;
  const slides = document.querySelectorAll(".slide");
  const dots = document.querySelectorAll(".dot");
  const totalSlides = slides.length;
  const intervalTime = 5000; // Thời gian chuyển slide: 5 giây
  let slideInterval;

  // Hàm hiển thị slide theo index
  function showSlide(index) {
    slides.forEach((slide, i) => {
      slide.classList.remove("active");
      if (dots[i]) {
        dots[i].classList.remove("active-dot");
      }
    });
    slides[index].classList.add("active");
    if (dots[index]) {
      dots[index].classList.add("active-dot");
    }
    currentIndex = index;
  }

  // Chuyển sang slide kế tiếp
  function nextSlide() {
    let nextIndex = currentIndex + 1;
    if (nextIndex >= totalSlides) {
      nextIndex = 0;
    }
    showSlide(nextIndex);
  }

  // Chuyển sang slide trước đó
  function prevSlide() {
    let prevIndex = currentIndex - 1;
    if (prevIndex < 0) {
      prevIndex = totalSlides - 1;
    }
    showSlide(prevIndex);
  }

  // Bắt đầu tự động chuyển slide
  function startSlider() {
    slideInterval = setInterval(nextSlide, intervalTime);
  }

  // Dừng chuyển slide tự động
  function stopSlider() {
    clearInterval(slideInterval);
  }

  // Xử lý sự kiện click nút chuyển slide
  const sliderLeft = document.querySelector(".slider-left");
  const sliderRight = document.querySelector(".slider-right");
  if (sliderLeft) {
    sliderLeft.addEventListener("click", () => {
      stopSlider();
      prevSlide();
      startSlider();
    });
  }
  if (sliderRight) {
    sliderRight.addEventListener("click", () => {
      stopSlider();
      nextSlide();
      startSlider();
    });
  }

  // Xử lý sự kiện click vào chấm tròn
  dots.forEach((dot) => {
    dot.addEventListener("click", function () {
      stopSlider();
      const index = parseInt(this.getAttribute("data-index"), 10);
      showSlide(index);
      startSlider();
    });
  });

  // Khởi chạy slider
  startSlider();
});
