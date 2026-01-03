document.addEventListener("DOMContentLoaded", async function () {
    // ✅ Lấy ID sản phẩm từ URL
    const params = new URLSearchParams(window.location.search);
    const productId = params.get("id");

    if (!productId) {
        console.error("❌ Không tìm thấy ID sản phẩm!");
        return;
    }

    console.log("🔍 Đang tải thông tin sản phẩm, ID:", productId);

    // ✅ Gọi API lấy thông tin sản phẩm
    async function fetchProductDetails() {
        try {
            const response = await fetch(`http://localhost:5216/api/products/${productId}`);
            if (!response.ok) {
                throw new Error(`❌ Lỗi API: ${response.status}`);
            }

            const product = await response.json();
            if (!product || Object.keys(product).length === 0) {
                console.error("❌ Không tìm thấy sản phẩm với ID:", productId);
                return;
            }

            console.log("📌 Sản phẩm được chọn:", product);

            // ✅ Cập nhật thông tin sản phẩm trên giao diện
            document.getElementById("name-product").textContent = product.productName;
            document.getElementById("price-product").textContent = `${product.productPrice.toLocaleString()} đ`;
            document.getElementById("desc-product").textContent =
                product.productDetail || product.description || "Không có mô tả sản phẩm.";
            document.getElementById("category-product").textContent = product.productType?.productTypeName || "Danh mục không xác định";
            document.getElementById("id-product").textContent = product.productID;

            // ✅ Xử lý ảnh sản phẩm
            updateProductImages(product);
        } catch (error) {
            console.error("❌ Lỗi khi lấy dữ liệu sản phẩm:", error);
        }
    }

    // ✅ Hiển thị hình ảnh sản phẩm
    function updateProductImages(product) {
        const slideContainer = document.getElementById("slidesContainer");
        const thumbnailContainer = document.getElementById("thumbnailContainer");

        slideContainer.innerHTML = "";
        thumbnailContainer.innerHTML = "";

        // ✅ Lấy danh sách ảnh từ API hoặc sử dụng ảnh mặc định
        let productImages = product.images && product.images.length > 0 ? product.images : ["https://via.placeholder.com/500"];

        productImages.forEach((image, index) => {
            // ✅ Ảnh lớn
            let slideDiv = document.createElement("div");
            slideDiv.classList.add("mySlides");
            slideDiv.innerHTML = `<img src="${fixImagePath(image)}" alt="Sản phẩm">`;
            slideContainer.appendChild(slideDiv);

            // ✅ Thumbnail nhỏ
            let thumbnailImg = document.createElement("img");
            thumbnailImg.classList.add("demo", "cursor");
            thumbnailImg.src = fixImagePath(image);
            thumbnailImg.onclick = () => currentSlide(index + 1);

            let thumbnailDiv = document.createElement("div");
            thumbnailDiv.classList.add("thumbnail-item");
            thumbnailDiv.appendChild(thumbnailImg);

            thumbnailContainer.appendChild(thumbnailDiv);
        });

        console.log("✅ Ảnh sản phẩm đã cập nhật thành công!");
        showSlides(1); // Hiển thị ảnh đầu tiên
    }

    // ✅ Sửa lỗi đường dẫn ảnh
    function fixImagePath(imageUrl) {
        return imageUrl.startsWith("http") ? imageUrl : `http://localhost:5216/${imageUrl}`;
    }

    // ✅ Xử lý slide ảnh
    let slideIndex = 1;
    function showSlides(n) {
        let slides = document.getElementsByClassName("mySlides");
        let dots = document.getElementsByClassName("demo");

        if (n > slides.length) slideIndex = 1;
        if (n < 1) slideIndex = slides.length;

        for (let i = 0; i < slides.length; i++) {
            slides[i].style.display = "none";
        }
        for (let i = 0; i < dots.length; i++) {
            dots[i].className = dots[i].className.replace(" active", "");
        }

        slides[slideIndex - 1].style.display = "block";
        dots[slideIndex - 1].className += " active";
    }

    function plusSlides(n) {
        showSlides((slideIndex += n));
    }

    function currentSlide(n) {
        showSlides((slideIndex = n));
    }

    // ✅ Xử lý số lượng sản phẩm (Sửa lỗi không tăng giảm được)
    const quantityInput = document.getElementById("quantity");
    const btnDecrease = document.querySelector(".btn-decrease");
    const btnIncrease = document.querySelector(".btn-increase");

    btnDecrease.addEventListener("click", function () {
        let currentValue = parseInt(quantityInput.value);
        if (currentValue > 1) quantityInput.value = currentValue - 1;
    });

    btnIncrease.addEventListener("click", function () {
        let currentValue = parseInt(quantityInput.value);
        quantityInput.value = currentValue + 1;
    });

    // ✅ Xử lý thêm vào giỏ hàng
    document.querySelector(".add-to-cart").addEventListener("click", function () {
        let quantity = parseInt(document.getElementById("quantity").value);
        console.log(`🛒 Đã thêm vào giỏ hàng: Sản phẩm ID ${productId} - Số lượng: ${quantity}`);

        alert(`🎉 Đã thêm vào giỏ hàng! Sản phẩm: ${document.getElementById("name-product").textContent}`);
    });

    // ✅ Xử lý bình luận sản phẩm
    document.getElementById("submit-comment").addEventListener("click", function () {
        const selectedRating = document.querySelector('input[name="rating"]:checked');
        const commentText = document.getElementById("comment-input").value;

        if (!selectedRating) {
            alert("⚠️ Vui lòng chọn số sao đánh giá!");
            return;
        }
        if (commentText.trim() === "") {
            alert("⚠️ Vui lòng nhập bình luận!");
            return;
        }

        console.log("📝 Đánh giá sao:", selectedRating.value);
        console.log("💬 Bình luận:", commentText);
        alert("🎉 Cảm ơn bạn đã đánh giá sản phẩm!");
    });

    // ✅ Gọi API để lấy sản phẩm khi trang tải
    fetchProductDetails();
});
