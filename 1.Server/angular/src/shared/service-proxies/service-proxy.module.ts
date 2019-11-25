import { NgModule } from '@angular/core';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AbpHttpInterceptor } from '@abp/abpHttpInterceptor';

import * as ApiServiceProxies from './service-proxies';

@NgModule({
    providers: [
        ApiServiceProxies.HeThongServiceProxy,
        ApiServiceProxies.QuanLyKhuyenMaiServiceProxy,
        ApiServiceProxies.DanhMucHangServiceProxy,
        ApiServiceProxies.DanhMucHangMucServiceProxy,
        ApiServiceProxies.DanhMucDichVuServiceProxy,
        ApiServiceProxies.DanhMucNhomDichVuServiceProxy,
        ApiServiceProxies.KhachHangServiceProxy,
        ApiServiceProxies.QuanLyCongViecServiceProxy,
        ApiServiceProxies.TramDichVuServiceProxy,
        ApiServiceProxies.LookupTableServiceProxy,
        ApiServiceProxies.RoleServiceProxy,
        ApiServiceProxies.SessionServiceProxy,
        ApiServiceProxies.UserServiceProxy,
        ApiServiceProxies.TokenAuthServiceProxy,
        ApiServiceProxies.AccountServiceProxy,
        ApiServiceProxies.ConfigurationServiceProxy,
        { provide: HTTP_INTERCEPTORS, useClass: AbpHttpInterceptor, multi: true }
    ]
})
export class ServiceProxyModule { }
