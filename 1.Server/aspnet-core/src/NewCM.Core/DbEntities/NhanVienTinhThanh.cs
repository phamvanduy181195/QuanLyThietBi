using Abp.Domain.Entities;
using NewCM.Authorization.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewCM.DbEntities
{
    [Table("NhanVienTinhThanh")]
    public class NhanVienTinhThanh : Entity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public virtual long NhanVienId { get; set; }

        public virtual int TinhThanhId { get; set; }

        public virtual User NhanVien { get; set; }
    }
}
