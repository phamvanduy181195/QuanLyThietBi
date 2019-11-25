using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewCM.DbEntities
{
    [Table("DanhMucTinhThanh")]
    public class DanhMucTinhThanh : Entity, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public virtual string Code { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
    }
}
