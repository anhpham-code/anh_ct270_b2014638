//Plugin table
var js = jQuery.noConflict(true);
js(document).ready(function () {
    js('#myTable').DataTable({
        language: {
            url: "//cdn.datatables.net/plug-ins/1.10.25/i18n/Vietnamese.json"
        }
    });
});

//popup xoa
function deleteHangSanXuat() {
    var MAHSX = $(".delete-button").data("mahsx");

    $.ajax({
        url: "/HangSanXuat/XoaSanXuat",
        type: "POST",
        data: { id: MAHSX },
        success: function (response) {
            // Xử lý phản hồi từ Controller (nếu cần)
            window.location.href = "/HangSanXuat/DanhSachHangSanXuat";
        },
        error: function (xhr, status, error) {
            // Xử lý lỗi (nếu có)
        }
    });
}
js(document).ready(function () {
    console.log("Mã JavaScript đã được thực thi");

    // Các đoạn mã xử lý sự kiện khác ở đây.
});

//Popup thêm hãng sx
function showAddHangSanXuatPopup() {
    document.getElementById("addHangSanXuatPopup").style.display = "block";
}

function hideAddHangSanXuatPopup() {
    document.getElementById("addHangSanXuatPopup").style.display = "none";
}

document.getElementById("addHangSanXuatForm").addEventListener("submit", function (event) {
    event.preventDefault(); // Ngăn chặn gửi form mặc định

    var TENHSX = document.getElementById("TENHSX").value;
    var URL = document.getElementById("URL").value;
    var GIOITHIEU = document.getElementById("GIOITHIEU").value;
    var HINH = document.getElementById("HINH").files[0]; // Lấy tệp ảnh

    var KEYWORD = document.getElementById("KEYWORD").value;

    // Kiểm tra các trường dữ liệu trước khi gửi yêu cầu
    if (!TENHSX || !URL || !GIOITHIEU || !KEYWORD) {
        alert("Vui lòng điền đầy đủ thông tin.");
        return; // Không gửi yêu cầu nếu có trường dữ liệu thiếu
    }

    // Sử dụng FormData để gửi dữ liệu bao gồm tệp ảnh
    var formData = new FormData();
    formData.append("TENHSX", TENHSX);
    formData.append("URL", URL);
    formData.append("GIOITHIEU", GIOITHIEU);
    formData.append("HINH", HINH);
    formData.append("KEYWORD", KEYWORD);

    // Gửi dữ liệu đến Controller bằng AJAX
    $.ajax({
        url: "/HangSanXuat/ThemHangSanXuat",
        type: "POST",
        data: formData,
        processData: false, // Không xử lý dữ liệu tự động
        contentType: false, // Không đặt kiểu dữ liệu tự động
        success: function (response) {
            // Xử lý phản hồi từ Controller (nếu cần)
            hideAddHangSanXuatPopup(); // Ẩn popup sau khi gửi thành công
            window.location.href = "/HangSanXuat/DanhSachHangSanXuat"; // Chuyển hướng đến trang DanhSachHangSanXuat.cshtml
        },
        error: function (xhr, status, error) {
            // Xử lý lỗi (nếu có)
        }
    });
});