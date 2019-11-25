using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using NewCM.DbEntities;

namespace NewCM.DanhMuc.HangMucs.Dto
{
    [AutoMap(typeof(DanhMucHangMuc))]
    public class DanhMucHangMucDto : EntityDto<int>
    {
        public string Name { get; set; }
        public int? DichVuId { get; set; }
        public string DonViTinh { get; set; }
        public double DonGia { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        // Thông tin thêm
        public string DichVuName { get; set; }
        public string NhomDichVuName { get; set; }
        public int? NhomDichVuId { get; set; }
    }
}
