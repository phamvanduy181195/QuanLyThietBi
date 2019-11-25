using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using NewCM.Authorization.Roles;
using NewCM.Authorization.Users;
using NewCM.MultiTenancy;
using NewCM.DbEntities;

namespace NewCM.EntityFrameworkCore
{
    public class NewCMDbContext : AbpZeroDbContext<Tenant, Role, User, NewCMDbContext>
    {
        /* Define a DbSet for each entity of the application */
        public DbSet<CongViec> CongViec { get; set; }
        public DbSet<CongViecHangMuc> CongViecHangMuc { get; set; }
        public DbSet<DanhMucHang> DanhMucHang { get; set; }
        public DbSet<DanhMucNhomDichVu> DanhMucNhomDichVu { get; set; }
        public DbSet<DanhMucDichVu> DanhMucDichVu { get; set; }
        public DbSet<DanhMucHangMuc> DanhMucHangMuc { get; set; }
        public DbSet<DanhMucQuanHuyen> DanhMucQuanHuyen { get; set; }
        public DbSet<DanhMucTinhThanh> DanhMucTinhThanh { get; set; }
        public DbSet<KhachHang> KhachHang { get; set; }
        public DbSet<LichSuCongViec> LichSuCongViec { get; set; }
        public DbSet<NhanVienNhomDichVu> NhanVienNhomDichVu { get; set; }
        public DbSet<NhanVienQuanHuyen> NhanVienQuanHuyen { get; set; }
        public DbSet<NhanVienTinhThanh> NhanVienTinhThanh { get; set; }
        public DbSet<TinTuc> TinTuc { get; set; }
        public DbSet<TramDichVu> TramDichVu { get; set; }
        public DbSet<SoBaoHanh> SoBaoHanh { get; set; }

        public NewCMDbContext(DbContextOptions<NewCMDbContext> options)
            : base(options)
        {
        }
    }
}
