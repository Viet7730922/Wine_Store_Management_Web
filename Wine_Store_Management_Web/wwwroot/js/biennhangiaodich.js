$(document).ready(function () {
    var dataGiaoDich = window.giaoDichData || [];

    $('#MaDoiChieuSelect').change(function () {
        var maChon = $(this).val();

        if (maChon !== "") {
            // Tìm kiếm dữ liệu tương ứng trong mảng JSON
            var gd = dataGiaoDich.find(g => g.Ma === maChon);

            if (gd) {
                // Đổ dữ liệu vào các ô tương ứng
                $('#LoaiBienNhan').val(gd.Loai);
                $('#TenKhachHang').val(gd.TenKH);
                $('#SoDienThoai').val(gd.SDT);
                $('#ChiTietSanPham').val(gd.ChiTiet);
                $('#TongGiaTriDoiChieu').val(gd.TongTien);
            }
        } else {
            // Xóa trắng nếu không chọn
            $('#LoaiBienNhan, #TenKhachHang, #SoDienThoai, #ChiTietSanPham, #TongGiaTriDoiChieu').val('');
        }
    });
});