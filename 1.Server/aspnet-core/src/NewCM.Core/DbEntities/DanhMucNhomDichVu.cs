using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewCM.DbEntities
{
    [Table("DanhMucNhomDichVu")]
    public class DanhMucNhomDichVu: FullAuditedEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
    }
}
