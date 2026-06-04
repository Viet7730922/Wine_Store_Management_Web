$(document).ready(function () {

    // 1. KHI THAY ĐỔI LỰA CHỌN SẢN PHẨM RƯỢU
    $(document).on('change', '.sp-select', function () {
        var $row = $(this).closest('tr');
        var $selectedOption = $(this).find('option:selected');

        if ($selectedOption.val() !== "") {
            // Tự động load Tên và ĐVT tương ứng lên dòng dữ liệu
            $row.find('.sp-ten').val($selectedOption.data('ten'));
            $row.find('.sp-dvt').val($selectedOption.data('dvt'));
        } else {
            $row.find('.sp-ten, .sp-dvt').val('');
        }
        tinhTongTienPhieuNhap();
    });

    // 2. KHI THAY ĐỔI SỐ LƯỢNG HOẶC ĐƠN GIÁ NHẬP
    $(document).on('input', '.sp-soluong, .sp-gia', function () {
        tinhTongTienPhieuNhap();
    });

    // 3. THUẬT TOÁN TÍNH TỔNG TIỀN PHIẾU VÀ TỰ DỊCH SỐ THÀNH CHỮ HỆ THỐNG
    function tinhTongTienPhieuNhap() {
        var tongTienPhieu = 0;

        $('#nhapKhoTable tbody tr').each(function () {
            var sl = parseFloat($(this).find('.sp-soluong').val()) || 0;
            var gia = parseFloat($(this).find('.sp-gia').val()) || 0;
            tongTienPhieu += (sl * gia);
        });

        // Gọi hàm dịch số thành chữ tiếng Việt
        var chuVietThuong = DocTienBangChu(tongTienPhieu);
        $('#TongTienVietBangChu').val(chuVietThuong);
    }

    // 4. CƠ CHẾ NHÂN BẢN DÒNG (THÊM SẢN PHẨM MỚI COPPY THEO INDEX ĐÚNG CHUẨN C#)
    $('#btnAddRow').click(function () {
        var $firstRow = $('#nhapKhoTable tbody tr:first');
        var $newRow = $firstRow.clone();

        // Làm sạch trắng dữ liệu của hàng mới nhân bản
        $newRow.find('input').val('');
        $newRow.find('.sp-ten').attr('placeholder', 'Hệ thống tự load...');
        $newRow.find('.sp-soluong').val(1);
        $newRow.find('select').prop('selectedIndex', 0);
        $newRow.find('.btn-remove-row').prop('disabled', false); // Mở khóa nút xóa dòng

        $('#nhapKhoTable tbody').append($newRow);
        danhLaiSoThuTuPhieuNhap();
    });

    // 5. CƠ CHẾ XÓA DÒNG
    $(document).on('click', '.btn-remove-row', function () {
        if ($('#nhapKhoTable tbody tr').length > 1) {
            $(this).closest('tr').remove();
            danhLaiSoThuTuPhieuNhap();
            tinhTongTienPhieuNhap();
        }
    });

    // Ép gán lại thuộc tính chỉ mục Index [0], [1], [2] để Model Binder C# ánh xạ đúng vào List<ChiTietPhieuNhap>
    function danhLaiSoThuTuPhieuNhap() {
        $('#nhapKhoTable tbody tr').each(function (index) {
            $(this).find('.row-stt').text(index + 1); // Cập nhật cột STT hiển thị
            $(this).find('[name^="ChiTietPhieuNhaps["]').each(function () {
                var name = $(this).attr('name');
                var newName = name.replace(/\[\d+\]/, '[' + index + ']');
                $(this).attr('name', newName);
            });
        });
    }

    // =========================================================
    // HÀM TIỆN ÍCH DỊCH CHỮ TIẾNG VIỆT CHUẨN XÁC
    // =========================================================
    var mangso = ['không', 'một', 'hai', 'ba', 'bốn', 'năm', 'sáu', 'bảy', 'tám', 'chín'];
    function dochangchuc(so, daydu) {
        var chuoi = "";
        var chuc = Math.floor(so / 10);
        var donvi = so % 10;
        if (chuc > 1) {
            chuoi = " " + mangso[chuc] + " mươi";
            if (donvi == 1) chuoi += " mốt";
        } else if (chuc == 1) {
            chuoi = " mười";
            if (donvi == 1) chuoi += " một";
        } else if (daydu && donvi > 0) {
            chuoi = " lẻ";
        }
        if (donvi == 5 && chuc >= 1) chuoi += " lăm";
        else if (donvi > 1 || (donvi == 1 && chuc == 0)) chuoi += " " + mangso[donvi];
        return chuoi;
    }
    function docblock(so, daydu) {
        var chuoi = "";
        var tram = Math.floor(so / 100);
        so = so % 100;
        if (daydu || tram > 0) {
            chuoi = " " + mangso[tram] + " trăm";
            chuoi += dochangchuc(so, true);
        } else {
            chuoi = dochangchuc(so, false);
        }
        return chuoi;
    }
    function dochangtrieu(so, daydu) {
        var chuoi = "";
        var trieu = Math.floor(so / 1000000);
        so = so % 1000000;
        if (trieu > 0) {
            chuoi = docblock(trieu, daydu) + " triệu";
            daydu = true;
        }
        var nghin = Math.floor(so / 1000);
        so = so % 1000;
        if (nghin > 0) {
            chuoi += docblock(nghin, daydu) + " nghìn";
            daydu = true;
        }
        if (so > 0) chuoi += docblock(so, daydu);
        return chuoi;
    }
    function DocTienBangChu(so) {
        if (so == 0) return "Không đồng.";
        var chuoi = "", hauto = "";
        do {
            var ty = so % 1000000000;
            so = Math.floor(so / 1000000000);
            if (so > 0) {
                chuoi = dochangtrieu(ty, true) + hauto + chuoi;
            } else {
                chuoi = dochangtrieu(ty, false) + hauto + chuoi;
            }
            hauto = " tỷ";
        } while (so > 0);

        chuoi = chuoi.trim();
        if (chuoi.length > 0) {
            chuoi = chuoi.charAt(0).toUpperCase() + chuoi.slice(1);
        }
        return chuoi + " đồng chẵn.";
    }
});