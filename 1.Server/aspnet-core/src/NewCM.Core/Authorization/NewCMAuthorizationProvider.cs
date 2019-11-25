using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace NewCM.Authorization
{
    public class NewCMAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
            context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));

            context.CreatePermission(PermissionNames.Pages_Users, L("mnuNhanVien"), multiTenancySides: MultiTenancySides.Tenant);
            context.CreatePermission(PermissionNames.Pages_KhachHang, L("mnuKhachHang"), multiTenancySides: MultiTenancySides.Tenant);
            context.CreatePermission(PermissionNames.Pages_TramDichVu, L("TramDichVu"), multiTenancySides: MultiTenancySides.Tenant);

            var perQuanLyCongViec = context.CreatePermission(PermissionNames.Pages_QuanLyCongViec, L("mnuQuanLyCongViec"), multiTenancySides: MultiTenancySides.Tenant);
            perQuanLyCongViec.CreateChildPermission(PermissionNames.Pages_QuanLyCongViec_PhanBoVeTram, L("QuanLyCongViec_PhanBoVeTram"), multiTenancySides: MultiTenancySides.Tenant);
            perQuanLyCongViec.CreateChildPermission(PermissionNames.Pages_QuanLyCongViec_PhanBoVeNhanVien, L("QuanLyCongViec_PhanBoVeNhanVien"), multiTenancySides: MultiTenancySides.Tenant);

            context.CreatePermission(PermissionNames.Pages_DanhMucNhomDichVu, L("mnuNhomDichVu"), multiTenancySides: MultiTenancySides.Tenant);
            context.CreatePermission(PermissionNames.Pages_DanhMucDichVu, L("mnuDichVu"), multiTenancySides: MultiTenancySides.Tenant);
            context.CreatePermission(PermissionNames.Pages_DanhMucHangMuc, L("mnuHangMuc"), multiTenancySides: MultiTenancySides.Tenant);
            context.CreatePermission(PermissionNames.Pages_DanhMucHang, L("mnuHang"), multiTenancySides: MultiTenancySides.Tenant);

            context.CreatePermission(PermissionNames.Pages_QuanLyKhuyenMai, L("QuanLyKhuyenMai"), multiTenancySides: MultiTenancySides.Tenant);
            context.CreatePermission(PermissionNames.Pages_QuanLyHeThong, L("QuanLyHeThong"), multiTenancySides: MultiTenancySides.Tenant);
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, NewCMConsts.LocalizationSourceName);
        }
    }
}
