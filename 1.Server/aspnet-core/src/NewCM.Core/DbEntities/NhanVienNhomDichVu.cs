using Abp.Domain.Entities;
using NewCM.Authorization.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewCM.DbEntities
{
    [Table("NhanVienNhomDichVu")]
    public class NhanVienNhomDichVu: Entity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public virtual long NhanVienId { get; set; }

        public virtual int NhomDichVuId { get; set; }

        public virtual User NhanVien { get; set; }
    }
}
