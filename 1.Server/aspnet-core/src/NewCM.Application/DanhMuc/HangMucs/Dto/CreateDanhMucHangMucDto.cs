using Abp.AutoMapper;
using NewCM.DbEntities;

namespace NewCM.DanhMuc.HangMucs.Dto
{
    [AutoMap(typeof(DanhMucHangMuc))]
    public class CreateDanhMucHangMucDto
    {
        public string Name { get; set; }
        public int? DichVuId { get; set; }
        public string DonViTinh { get; set; }
        public double DonGia { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
