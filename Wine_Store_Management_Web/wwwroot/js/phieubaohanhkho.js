$(document).ready(function () {

    // 1. Sự kiện thay đổi Khách hàng -> Tự động điền SĐT
    $('#KhachHangSelect').change(function () {
        var selectedOption = $(this).find('option:selected');
        if (selectedOption.val() !== "") {
            // Trích xuất dữ liệu từ thuộc tính data-sdt cấu hình ở View
            $('#SoDienThoai').val(selectedOption.data('sdt'));
        } else {
            $('#SoDienThoai').val('');
        }
    });

    // 2. Sự kiện thay đổi Sản phẩm -> Tự động tra và gán Số Seri 16 số bảo mật
    $('#SanPhamSelect').change(function () {
        var selectedOption = $(this).find('option:selected');
        if (selectedOption.val() !== "") {
            // Tra cứu số Seri gốc của sản phẩm đổ vào ô input
            $('#SoSeri').val(selectedOption.data('seri'));
        } else {
            $('#SoSeri').val('');
        }
    });

    // 3. Thuật toán tự động tính toán LUÔN NGÀY HẸN TRẢ SẢN PHẨM PHÒNG KHO
    // Công thức: Ngày hẹn trả = Ngày tiếp nhận + Số ngày xử lý dự kiến
    function tuDongTinhNgayHenTra() {
        var ngayTiepNhanStr = $('#NgayTiepNhan').val();
        var soNgayXuLy = parseInt($('#ThoiGianXuLy').val()) || 0;

        if (ngayTiepNhanStr) {
            var dateObj = new Date(ngayTiepNhanStr);
            // Cộng thêm số ngày dự kiến vào mốc thời gian tiếp nhận
            dateObj.setDate(dateObj.getDate() + soNgayXuLy);

            // Định dạng lại theo chuẩn yyyy-mm-dd để gán ngược lại thẻ input date của HTML5
            var yyyy = dateObj.getFullYear();
            var mm = String(dateObj.getMonth() + 1).padStart(2, '0');
            var dd = String(dateObj.getDate()).padStart(2, '0');

            $('#NgayHenTra').val(yyyy + '-' + mm + '-' + dd);
        }
    }

    // Bắt sự kiện thay đổi mốc thời gian hoặc số ngày xử lý để chạy thuật toán tính toán
    $('#NgayTiepNhan, #ThoiGianXuLy').on('input change', function () {
        tuDongTinhNgayHenTra();
    });
});