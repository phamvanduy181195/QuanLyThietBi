namespace NewCM.CongViecs.Dto
{
    public class DanhSachCongViecInput
    {
        public int[] TrangThaiIds { get; set; }

        public int Page { get; set; }

        public DanhSachCongViecInput()
        {
            Page = 1;
        }
    }
}
