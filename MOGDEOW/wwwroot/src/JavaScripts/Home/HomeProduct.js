document.addEventListener("DOMContentLoaded", async function () {
    // ✅ Lấy các phần tử HTML
    const dogShopContainer = document.getElementById("dogContainer");
    const catShopContainer = document.getElementById("catContainer");
    const petShopContainer = document.getElementById("productContainer");

    let allProducts = []; // Danh sách sản phẩm từ API

    // ✅ Gọi API lấy danh sách sản phẩm
    async function fetchProducts() {
        try {
            const response = await fetch("http://localhost:5216/api/products/all");
            if (!response.ok) throw new Error(`❌ Lỗi API: ${response.status}`);

            const data = await response.json();
            console.log("🔥 Dữ liệu từ API:", data);

            allProducts = data.map(product => ({
                ProductID: product.productID || null,
                ProductName: product.productName || "Sản phẩm không có tên",
                ProductPrice: product.productPrice || 0,
                ProductImage: (product.images && product.images.length > 0)
                    ? fixImagePath(product.images[0])
                    : "https://via.placeholder.com/150",
                ProductTypeID: product.productType?.productTypeID?.toString() || "0"
            }));

            console.log("✅ Dữ liệu sản phẩm sau xử lý:", allProducts);
            loadShopSections();
        } catch (error) {
            console.error("❌ Lỗi khi lấy dữ liệu sản phẩm:", error);
        }
    }

    // ✅ Sửa lỗi đường dẫn ảnh
    function fixImagePath(imageUrl) {
        return imageUrl.startsWith("http") ? imageUrl : `http://localhost:5216/${imageUrl}`;
    }

    // ✅ Load dữ liệu vào Shop Chó Cưng, Shop Mèo Cưng & Dành cho Pet Yêu
    function loadShopSections() {
        renderShopProducts(dogShopContainer, getRandomProductsByCategory("1", 5)); // Chó
        renderShopProducts(catShopContainer, getRandomProductsByCategory("2", 5)); // Mèo
        renderShopProducts(petShopContainer, getRandomProductsExcluding(["1", "2"], 5)); // Phụ kiện, thức ăn,...
    }

    // ✅ Lấy sản phẩm ngẫu nhiên theo danh mục
    function getRandomProductsByCategory(categoryID, count) {
        let filteredProducts = allProducts.filter(product => product.ProductTypeID === categoryID);
        return shuffleArray(filteredProducts).slice(0, count);
    }

    // ✅ Lấy sản phẩm ngẫu nhiên nhưng loại bỏ các danh mục cụ thể (VD: loại bỏ chó/mèo)
    function getRandomProductsExcluding(excludeCategories, count) {
        let filteredProducts = allProducts.filter(product => !excludeCategories.includes(product.ProductTypeID));
        return shuffleArray(filteredProducts).slice(0, count);
    }

    // ✅ Hiển thị sản phẩm trong các mục SHOP
    function renderShopProducts(container, products) {
        container.innerHTML = "";
        products.forEach(product => {
            const productCard = document.createElement("div");
            productCard.classList.add("product-card");
            productCard.innerHTML = `
                <img src="${product.ProductImage}" alt="${product.ProductName}" class="product-image"/>
                <h3 class="product-title">${product.ProductName}</h3>
                <p class="product-price">${product.ProductPrice.toLocaleString()} đ</p>
                <button class="detail-btn" onclick="viewProductDetail(${product.ProductID})">Chi tiết</button>
            `;
            container.appendChild(productCard);
        });
    }

    // ✅ Điều hướng khi nhấn vào danh mục trên HEADER hoặc SIDEBAR
    //function initCategoryNavigation() {
    //    document.querySelectorAll(".choose-menu-list a, .choose-dropdown-item a").forEach(item => {
    //        item.addEventListener("click", function (event) {
    //            event.preventDefault(); // Ngăn chặn hành vi mặc định của thẻ <a>
    //            let category = this.getAttribute("data-category");
    //            if (!category) return;

    //            console.log("📌 Điều hướng sang danh mục:", category);

    //            // ✅ Lưu danh mục vào LocalStorage để Search.html lấy ra
    //            localStorage.setItem("selectedCategory", category);
    //            window.location.href = `../Pages/search.html?category=${encodeURIComponent(category)}`;
    //        });
    //    });
    //}

    // ✅ Khi nhấn "Xem thêm", điều hướng đến trang tìm kiếm với danh mục tương ứng
    function initSeeMoreButtons() {
        document.querySelectorAll(".btn").forEach(button => {
            button.addEventListener("click", function () {
                let category = this.getAttribute("data-category");
                if (!category) return;

                console.log("📌 Chuyển hướng từ nút 'Xem thêm' đến danh mục:", category);

                localStorage.setItem("selectedCategory", category);
                window.location.href = `../Pages/search.html?category=${category}`;
            });
        });
    }

    // ✅ Xáo trộn danh sách sản phẩm (Fisher-Yates Algorithm)
    function shuffleArray(array) {
        for (let i = array.length - 1; i > 0; i--) {
            const j = Math.floor(Math.random() * (i + 1));
            [array[i], array[j]] = [array[j], array[i]];
        }
        return array;
    }

    // ✅ Điều hướng sang trang chi tiết sản phẩm
    window.viewProductDetail = function (productId) {
        window.location.href = `DetailProduct.html?id=${productId}`;
    };

    // ✅ Gọi API khi trang tải
    fetchProducts();
    //initCategoryNavigation();
    initSeeMoreButtons();
});
