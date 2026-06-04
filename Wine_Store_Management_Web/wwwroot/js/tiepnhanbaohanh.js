$(document).ready(function () {
    // Nhận dữ liệu JSON hóa đơn từ biến toàn cục do View truyền ra
    var hoaDons = window.hoaDonsJsonData || [];

    // 1. Bắt sự kiện chọn Khách Hàng
    $('#KhachHangSelect').change(function () {
        var selected = $(this).find('option:selected');
        if (selected.val() !== "") {
            // Tự động điền SĐT nhưng KH có thể tự sửa lại
            $('#SoDienThoai').val(selected.data('sdt'));
        } else {
            $('#SoDienThoai').val('');
        }
        findInvoice();
    });

    // 2. Bắt sự kiện chọn Sản Phẩm
    $('#MaSanPham').change(function () {
        var selected = $(this).find('option:selected');
        if (selected.val() !== "") {
            // Tự động điền cố định Số Seri
            $('#SoSeri').val(selected.data('seri'));
        } else {
            $('#SoSeri').val('');
        }
        findInvoice();
    });

    // 3. Thuật toán tự động tìm Mã Hóa Đơn (Dựa vào KH + SP)
    function findInvoice() {
        var maKhachHang = $('#KhachHangSelect').find('option:selected').data('ma');
        var maSanPham = $('#MaSanPham').val();

        $('#MaHoaDon').val(''); // Xóa kết quả cũ

        if (maKhachHang && maSanPham) {
            // Tìm kiếm trong mảng JSON xem có hóa đơn nào của KH này chứa SP này không
            var found = hoaDons.find(h => h.MaKhachHang === maKhachHang && h.SanPhams.includes(maSanPham));

            if (found) {
                $('#MaHoaDon').val(found.MaHoaDon);
            }
        }
    }
});