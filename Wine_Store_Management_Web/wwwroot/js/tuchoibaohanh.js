$(document).ready(function () {
    // Trích xuất dữ liệu danh sách hóa đơn từ biến môi trường toàn cục
    var danhSachHoaDon = window.invoiceJsonDataCollection || [];

    // 1. Khi thay đổi lựa chọn Khách hàng
    $('#KhachHangSelect').change(function () {
        var selectedOpt = $(this).find('option:selected');
        if (selectedOpt.val() !== "") {
            // Tự động điền Họ tên Khách hàng lên TextBox
            $('#HoTenKhachHang').val(selectedOpt.data('ten'));
        } else {
            $('#HoTenKhachHang').val('');
        }
        kiemTraRanhBuocMuaHang();
    });

    // 2. Khi thay đổi lựa chọn Sản phẩm lỗi
    $('#SanPhamSelect').change(function () {
        var selectedOpt = $(this).find('option:selected');
        if (selectedOpt.val() !== "") {
            // Tự động điền Tên sản phẩm và Số Seri thực tế từ lô kho
            $('#TenSanPham').val(selectedOpt.data('ten'));
            $('#SoSeri').val(selectedOpt.data('seri'));
        } else {
            $('#TenSanPham').val('');
            $('#SoSeri').val('');
        }
        kiemTraRanhBuocMuaHang();
    });

    // 3. Logic bật/tắt ô nhập khi chọn lý do "Khác"
    $('input[name="LyDoTuChoi"]').change(function () {
        if ($('#ld4').is(':checked')) {
            $('#LyDoChiTietKhac').prop('disabled', false).removeClass('bg-light').focus();
        } else {
            $('#LyDoChiTietKhac').prop('disabled', true).addClass('bg-light').val('');
        }
    });

    // 4. Thuật toán tự động kiểm tra xem Khách hàng có thực sự từng mua sản phẩm này chưa
    function kiemTraRanhBuocMuaHang() {
        var maKH = $('#KhachHangSelect').val();
        var maSP = $('#SanPhamSelect').val();

        if (maKH && maSP) {
            // Dò trong mảng hóa đơn hệ thống
            var tungMuaHang = danhSachHoaDon.some(h => h.MaKhachHang === maKH && h.SanPhams.includes(maSP));

            // Nếu phát hiện khách hàng chưa từng mua mặt hàng này, tự động chọn lý do thứ 3 (Mất hóa đơn / mua ngoài hệ thống)
            if (!tungMuaHang) {
                $('#ld3').prop('checked', true).trigger('change');
                alert('Hệ thống kiểm tra nhanh phát hiện Khách hàng này chưa từng mua sản phẩm rượu trên tại cửa hàng! Tự động chuyển hình thức sang vi phạm mua ngoài hệ thống.');
            }
        }
    }
});