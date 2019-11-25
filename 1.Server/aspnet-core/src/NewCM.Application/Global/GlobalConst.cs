namespace NewCM.Global
{
    public class GlobalConst
    {
        public enum StaticRole
        {
            Admin = 2,
            TramTruong,
            NhanVien
        }

        public enum TrangThaiCongViec
        {
            ChoPhanBo = 0,
            DaPhanBo,
            DaNhan,
            DangXuLy,
            DaHoanThanh,
            KhachHangHuy,
            BiTuChoi,
            ChoLinhKien,
            ChoXacNhanHoanThanh
        }

        public enum ReadExcelResultCode
        {
            FileNotFound = 1,
            SheetNotFound,
            CantReadData,
            OK = 200
        }

        public enum LoaiTinTuc
        {
            KhuyenMai = 0,
            ThongTinHeThong,
            DieuKhoanSuDung
        }
    }
}
