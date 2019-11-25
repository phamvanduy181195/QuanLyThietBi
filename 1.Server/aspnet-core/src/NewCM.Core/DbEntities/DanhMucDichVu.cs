using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewCM.DbEntities
{
    [Table("DanhMucDichVu")]
    public class DanhMucDichVu : FullAuditedEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public virtual string Name { get; set; }
        public virtual int? NhomDichVuId { get; set; }
        public virtual string Description { get; set; }
        public virtual bool IsActive { get; set; }
    }
}
