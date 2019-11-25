using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewCM.DbEntities
{
    [Table("LichSuCongViec")]
    public class LichSuCongViec : FullAuditedEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public virtual long CongViecId { get; set; }
        public virtual int? TramDichVuId { get; set; }
        public virtual long? NhanVienId { get; set; }
        public virtual long? KhachHangId { get; set; }
        public virtual int NhomTrangThaiId { get; set; }
        public virtual int TrangThaiId { get; set; }
        public virtual string Description { get; set; }
    }
}
