$(document).ready(function () {

    // 1. AUTO-COMPLETE TÊN KHÁCH HÀNG
    $('#HoTenNguoiMua').on('input', function () {
        var val = $(this).val();
        var $option = $('#listKhachHang option').filter(function () { return this.value === val; });
        if ($option.length) {
            $('#MaKhachHang').val($option.attr('data-ma'));
        } else {
            $('#MaKhachHang').val('');
        }
    });

    // 2. LOAD THÔNG TIN SẢN PHẨM VÀO DÒNG
    $(document).on('change', '.sp-select', function () {
        var $row = $(this).closest('tr');
        var $selectedOption = $(this).find('option:selected');

        if ($selectedOption.val() !== "") {
            $row.find('.sp-ten').val($selectedOption.attr('data-ten'));
            $row.find('.sp-dvt').val($selectedOption.attr('data-dvt'));
            $row.find('.sp-gia').val($selectedOption.attr('data-gia'));
            $row.find('.sp-soluong').attr('max', $selectedOption.attr('data-ton')).attr('title', 'Kho hiện còn: ' + $selectedOption.attr('data-ton'));
        } else {
            $row.find('input:not(.sp-soluong)').val('');
            $row.find('.sp-soluong').removeAttr('max');
        }
        tinhToanTienHang($row);
    });

    // 3. TÍNH TOÁN KHI THAY ĐỔI SỐ LƯỢNG
    $(document).on('input', '.sp-soluong', function () {
        tinhToanTienHang($(this).closest('tr'));
    });

    function tinhToanTienHang($row) {
        var sl = parseFloat($row.find('.sp-soluong').val()) || 0;
        var gia = parseFloat($row.find('.sp-gia').val()) || 0;
        var maxTon = parseFloat($row.find('.sp-soluong').attr('max')) || 0;

        if (maxTon > 0 && sl > maxTon) {
            alert('Số lượng mua vượt quá tồn kho (' + maxTon + ')!');
            $row.find('.sp-soluong').val(maxTon);
            sl = maxTon;
        }

        $row.find('.sp-thanhtien').val((sl * gia).toFixed(2));
        tinhTongHoaDon();
    }

    // =========================================================
    // TÍNH TỔNG TIỀN VÀ GỌI HÀM ĐỌC SỐ THÀNH CHỮ
    // =========================================================
    function tinhTongHoaDon() {
        var tongGiaTri = 0;
        $('.sp-thanhtien').each(function () {
            tongGiaTri += parseFloat($(this).val()) || 0;
        });

        $('#txtTongTien').text(tongGiaTri.toLocaleString('en-US'));

        // Gọi hàm đọc chữ và điền vào ô input ẩn/hiện
        var tienBangChu = DocTienBangChu(tongGiaTri);
        $('#TongTienVietBangChu').val(tienBangChu);
    }

    // =========================================================
    // CƠ CHẾ THÊM / XÓA NHIỀU MẶT HÀNG
    // =========================================================
    $('#btnAddRow').click(function () {
        var $firstRow = $('#productTable tbody tr:first');
        var $newRow = $firstRow.clone();

        // Xóa sạch dữ liệu của dòng mới nhân bản
        $newRow.find('input').val('');
        $newRow.find('.sp-soluong').val(1).removeAttr('max').removeAttr('title');
        $newRow.find('.sp-thanhtien').val('0.00');
        $newRow.find('select').prop('selectedIndex', 0);
        $newRow.find('.btn-remove-row').prop('disabled', false); // Cho phép xóa

        $('#productTable tbody').append($newRow);
        danhLaiSoThuTuVaIndex();
    });

    $(document).on('click', '.btn-remove-row', function () {
        if ($('#productTable tbody tr').length > 1) {
            $(this).closest('tr').remove();
            danhLaiSoThuTuVaIndex();
            tinhTongHoaDon();
        }
    });

    // Bắt buộc phải đánh lại Index [0], [1], [2] thì C# MVC mới nhận diện được List
    function danhLaiSoThuTuVaIndex() {
        $('#productTable tbody tr').each(function (index) {
            $(this).find('td:first').text(index + 1); // Cột STT
            $(this).find('[name^="ChiTietHoaDons["]').each(function () {
                var name = $(this).attr('name');
                var newName = name.replace(/\[\d+\]/, '[' + index + ']');
                $(this).attr('name', newName);
            });
        });
    }

    // =========================================================
    // THUẬT TOÁN ĐỌC SỐ TIỀN THÀNH CHỮ TIẾNG VIỆT
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
        if (so == 0) return "Không đồng";
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