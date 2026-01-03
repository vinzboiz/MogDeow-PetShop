function goBack() {
    if (document.referrer) {
        window.location.href = document.referrer; // Quay về trang trước
    } else {
        window.location.href = '/'; // Nếu không có trang trước, chuyển về trang chủ
    }
}
