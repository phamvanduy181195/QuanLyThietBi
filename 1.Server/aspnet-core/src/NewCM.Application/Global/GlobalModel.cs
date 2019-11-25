using System.Collections.Generic;

namespace NewCM.Global
{
    public class GlobalModel
    {
        public static SortedList<int, string> TrangThaiCongViec = new SortedList<int, string>
        {
            { 0, "Chờ phân bổ" },
            { 1, "Đã phân bổ" },
            { 2, "Đã nhận" },
            { 3, "Đang xử lý" },
            { 4, "Đã hoàn thành" },
            { 5, "Khách hàng hủy" },
            { 6, "Bị từ chối" },
            { 7, "Chờ linh kiện" },
            { 8, "Chờ đóng ca" }
        };
    }
}
