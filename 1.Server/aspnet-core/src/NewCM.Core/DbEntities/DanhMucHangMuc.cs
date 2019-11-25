using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewCM.DbEntities
{
    [Table("DanhMucHangMuc")]
    public class DanhMucHangMuc : FullAuditedEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public virtual string Name { get; set; }
        public virtual int? DichVuId { get; set; }
        public virtual string DonViTinh { get; set; }
        public virtual double DonGia { get; set; }
        public virtual string Description { get; set; }
        public virtual bool IsActive { get; set; }
    }
}
