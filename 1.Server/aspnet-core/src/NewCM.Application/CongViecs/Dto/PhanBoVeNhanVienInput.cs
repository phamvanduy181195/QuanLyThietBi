using System;
using System.Collections.Generic;
using System.Text;

namespace NewCM.CongViecs.Dto
{
    public class PhanBoVeNhanVienInput
    {
        public int TramDichVuId { get; set; }

        public long NhanVienId { get; set; }

        public long[] CongViecIds { get; set; }

        public string GhiChuQuanLy { get; set; }
    }
}
