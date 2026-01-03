document.addEventListener("DOMContentLoaded", async function () {
    // ✅ Lấy các phần tử HTML từ DOM
    const categoryList = document.getElementById("categoryList");
    const searchResultsContainer = document.getElementById("searchResults");
    const priceRangeMin = document.getElementById("priceRangeMin");
    const priceRangeMax = document.getElementById("priceRangeMax");
    const priceValue = document.getElementById("priceValue");
    const filterBtn = document.getElementById("filterBtn");
    const sortPrice = document.getElementById("sortPrice");
    const loadMoreBtn = document.getElementById("loadMoreBtn");
    const searchResultText = document.getElementById("searchResultText");

    // ✅ Biến toàn cục
    let allProducts = []; // Dữ liệu API
    let displayedProducts = []; // Dữ liệu đã lọc
    let currentCategory = "all"; // Danh mục mặc định
    let minPrice = 0, maxPrice = 20000000;
    let itemsPerPage = 12;
    let currentPage = 1;

    // ✅ Lấy tham số từ URL
    const params = new URLSearchParams(window.location.search);
    const category = params.get("category") || "all"; // Mặc định "all"
    currentCategory = category;

    console.log("🔍 Danh mục được chọn:", currentCategory);
   

    // ✅ Gọi API lấy sản phẩm
    async function fetchProducts() {
        try {
            const response = await fetch("http://localhost:5216/api/products/all");
            if (!response.ok) {
                throw new Error(`❌ Lỗi API: ${response.status}`);
            }

            const data = await response.json();
            console.log("🔥 Dữ liệu từ API:", data);

            // ✅ Xử lý dữ liệu sản phẩm
            allProducts = data.map(product => ({
                ProductID: product.productID || null,
                ProductName: product.productName || "Sản phẩm không có tên",
                ProductPrice: product.productPrice || 0,
                ProductImage: (product.images && product.images.length > 0)
                    ? fixImagePath(product.images[0])  // ✅ Sửa lỗi URL ảnh
                    : "https://via.placeholder.com/150",
                ProductTypeID: product.productType?.productTypeID || null
            }));

            console.log("✅ Dữ liệu sản phẩm sau xử lý:", allProducts);
            filterAndRenderProducts();
        } catch (error) {
            console.error("❌ Lỗi khi lấy dữ liệu sản phẩm:", error);
        }
    }

    // ✅ Sửa lỗi đường dẫn ảnh
    function fixImagePath(imageUrl) {
        return imageUrl.startsWith("http") ? imageUrl : `http://localhost:5216/${imageUrl}`;
    }

    // ✅ Lọc và hiển thị sản phẩm ngay khi nhận được danh mục từ URL
    function filterAndRenderProducts() {
        //console.log("📌 Đang lọc sản phẩm theo danh mục:", currentCategory);

        //if (currentCategory === "all") {
        //    displayedProducts = allProducts; // ✅ Hiển thị tất cả sản phẩm
        //} else {
        //    displayedProducts = allProducts.filter(product => {
        //        return product.ProductTypeID === mapCategoryToID(currentCategory);
        //    });
        //}

        //console.log("🔍 Số sản phẩm sau khi lọc:", displayedProducts.length);
        //renderProducts();
        console.log("📌 Đang lọc sản phẩm theo danh mục:", currentCategory);

        let categoryID = mapCategoryToID(currentCategory); // Lấy ID danh mục từ tên hoặc số

        if (categoryID === "all") {
            displayedProducts = allProducts; // ✅ Hiển thị tất cả sản phẩm
        } else {
            displayedProducts = allProducts.filter(product => {
                let productCategory = product.ProductTypeID;
                return (
                    productCategory == categoryID ||  // So sánh số với chuỗi
                    productCategory.toString() === categoryID.toString() // Kiểm tra cả số lẫn chuỗi
                );
            });
        }

        console.log("🔍 Số sản phẩm sau khi lọc:", displayedProducts.length);
        renderProducts();
    }

    // ✅ Hiển thị danh sách sản phẩm
    function renderProducts() {
        console.log("📌 Dữ liệu từ API trước khi lọc:", displayedProducts);
        searchResultsContainer.innerHTML = ""; // Xóa danh sách cũ

        if (displayedProducts.length === 0) {
            searchResultsContainer.innerHTML = "<p>Không tìm thấy sản phẩm nào.</p>";
            return;
        }

        displayedProducts.forEach(product => {
            console.log("🎯 Hiển thị ảnh:", product.ProductImage); // Kiểm tra URL ảnh

            const productCard = document.createElement("div");
            productCard.classList.add("product-card");
            productCard.innerHTML = `
                <img src="${product.ProductImage}" alt="${product.ProductName}" class="product-image"/>
                <h3 class="product-title">${product.ProductName}</h3>
                <p class="product-price">${product.ProductPrice.toLocaleString()} đ</p>
                <button class="detail-btn" onclick="viewProductDetail(${product.ProductID})">Chi tiết</button>
            `;
            searchResultsContainer.appendChild(productCard);
        });

        // ✅ Kiểm tra hiển thị nút "Xem thêm"
        loadMoreBtn.style.display = displayedProducts.length > itemsPerPage ? "block" : "none";
    }

    // ✅ Khi nhấn vào danh mục Sidebar
    categoryList.addEventListener("click", function (event) {
        //let clickedCategory = event.target.closest("li")?.getAttribute("data-category");
        //if (!clickedCategory) return;

        //console.log("📌 Chọn danh mục:", clickedCategory);

        //currentCategory = mapCategoryToID(clickedCategory); // ✅ Chuyển tên danh mục thành `ProductTypeID`
        //// ✅ Cập nhật URL khi nhấn danh mục Sidebar
        //const newUrl = new URL(window.location);
        //newUrl.searchParams.set("category", currentCategory);
        //window.history.pushState({}, "", newUrl);
        //filterAndRenderProducts();

        let clickedCategory = event.target.closest("li")?.getAttribute("data-category");
        if (!clickedCategory) return;

        console.log("📌 Chọn danh mục từ Sidebar:", clickedCategory);

        currentCategory = mapCategoryToID(clickedCategory); // Chuyển danh mục thành ID

        if (currentCategory !== "all") {
            console.log(`✅ Đã chọn danh mục: ${currentCategory}`);
        } else {
            console.warn("⚠️ Danh mục bị reset về 'all', kiểm tra lại sự kiện click.");
        }

        // ✅ Cập nhật URL khi nhấn danh mục Sidebar
        const newUrl = new URL(window.location);
        newUrl.searchParams.set("category", currentCategory);
        window.history.pushState({}, "", newUrl);

        filterAndRenderProducts(); // 🔥 Gọi lại bộ lọc ngay lập tức
    });

    // ✅ Hàm ánh xạ danh mục từ Sidebar sang `ProductTypeID`
    function mapCategoryToID(categoryName) {
        //const categoryMap = {
        //    "all": "all",
        //    "dog": 1,
        //    "cat": 2,
        //    "food": 3,
        //    "hygiene": 4,
        //    "accessories": 5,
        //    "housing": 6,
        //    "medicine": 7
        //};
        //return categoryMap[categoryName] || "all";
        const categoryMap = {
            "all": "all",
            "dog": "1",
            "cat": "2",
            "food": "3",
            "hygiene": "4",
            "accessories": "5",
            "housing": "6",
            "medicine": "7"
        };

        let categoryID = categoryMap[categoryName] || categoryName; // Giữ nguyên nếu đã là số
        return categoryID.toString(); // Trả về dạng chuỗi để đảm bảo so sánh chính xác

    }

    // ✅ Lọc sản phẩm theo giá
    filterBtn.addEventListener("click", function () {
        // ✅ Lấy giá trị trực tiếp từ input
        let minPrice = parseInt(priceRangeMin.value) || 0;
        let maxPrice = parseInt(priceRangeMax.value) || 20000000;

        // ✅ Cập nhật giá hiển thị ngay khi lọc
        priceValue.innerText = `${minPrice.toLocaleString()} đ - ${maxPrice.toLocaleString()} đ`;

        console.log(`🔍 Lọc theo giá từ ${minPrice} đ đến ${maxPrice} đ`);

        // ✅ Lọc sản phẩm theo giá trị mới
        displayedProducts = allProducts.filter(product =>
            product.ProductPrice >= minPrice && product.ProductPrice <= maxPrice
        );

        console.log("📌 Sản phẩm sau khi lọc giá:", displayedProducts);
        renderProducts();
    });

    // ✅ Cập nhật giá trị khi kéo thanh trượt mà không cần bấm nút
    priceRangeMin.addEventListener("input", function () {
        let minPrice = parseInt(priceRangeMin.value) || 0;
        let maxPrice = parseInt(priceRangeMax.value) || 20000000;
        priceValue.innerText = `${minPrice.toLocaleString()} đ - ${maxPrice.toLocaleString()} đ`;
    });

    priceRangeMax.addEventListener("input", function () {
        let minPrice = parseInt(priceRangeMin.value) || 0;
        let maxPrice = parseInt(priceRangeMax.value) || 20000000;
        priceValue.innerText = `${minPrice.toLocaleString()} đ - ${maxPrice.toLocaleString()} đ`;
    });


    // ✅ Sắp xếp sản phẩm theo giá
    sortPrice.addEventListener("change", function () {
        if (sortPrice.value === "asc") {
            displayedProducts.sort((a, b) => a.ProductPrice - b.ProductPrice);
        } else if (sortPrice.value === "desc") {
            displayedProducts.sort((a, b) => b.ProductPrice - a.ProductPrice);
        }
        renderProducts();
    });

    // ✅ Xử lý chuyển đến trang chi tiết sản phẩm
    window.viewProductDetail = function (productId) {
        window.location.href = `DetailProduct.html?id=${productId}`;
    }

    // ✅ Hàm xáo trộn danh sách sản phẩm
    function shuffleArray(array) {
        for (let i = array.length - 1; i > 0; i--) {
            const j = Math.floor(Math.random() * (i + 1));
            [array[i], array[j]] = [array[j], array[i]];
        }
        return array;
    }

    // ✅ Gọi API khi trang tải
    fetchProducts();
});
