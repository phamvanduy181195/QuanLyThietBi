using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewCM.DbEntities
{
    [Table("CongViecHangMuc")]
    public class CongViecHangMuc : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public virtual long CongViecId { get; set; }
        public virtual int HangMucId { get; set; }
        public virtual double SoLuong { get; set; }
        public virtual double DonGia { get; set; }
        public virtual double ThanhTien { get; set; }

        public virtual CongViec CongViec { get; set; }
    }
}
