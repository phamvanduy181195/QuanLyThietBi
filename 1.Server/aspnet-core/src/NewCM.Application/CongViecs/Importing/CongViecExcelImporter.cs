using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using NewCM.Data;
using NewCM.Data.Dto;
using NewCM.DbEntities;
using NewCM.Global;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewCM.CongViecs.Importing
{
    public class CongViecExcelImporter: ExcelImporterBase, ICongViecExcelImporter
    {
        private readonly IRepository<TramDichVu> _tramDichVuRepository;
        private readonly GlobalCache _globalCache;

        public CongViecExcelImporter(IRepository<TramDichVu> tramDichVuRepository,
            GlobalCache globalCache)
        {
            _tramDichVuRepository = tramDichVuRepository;
            _globalCache = globalCache;
        }

        public async Task<ReadFromExcelDto<CongViec>> ReadFromExcel(string FilePath)
        {
            // Lấy thông tin trạm dịch vụ để tra cứu Id từ mã trạm
            var TramDichVus = await _tramDichVuRepository.GetAll().ToListAsync();
            SortedList<string, int> SortedTramDichVu = new SortedList<string, int>();
            foreach (var tram in TramDichVus)
            {
                if (!SortedTramDichVu.ContainsKey(tram.Code))
                    SortedTramDichVu.Add(tram.Code, tram.Id);
            }

            ReadFromExcelDto<CongViec> result = new ReadFromExcelDto<CongViec>();

            var Data = ReadFile(FilePath, "DanhSachCongViec");

            if (Data.Count <= 0 || Data[0].Count < 4)
            {
                result.ResultCode = (int)Global.GlobalConst.ReadExcelResultCode.CantReadData;
                result.ErrorMessage = "Không đọc được dữ liệu, sai cấu trúc file hoặc dữ liệu rỗng!";
            }
            else
            {
                for (int i = 0; i < Data.Count; i++)
                {
                    try
                    {
                        DateTime? NgayHen = null;
                        if (!string.IsNullOrWhiteSpace(Data[i][1]))
                            NgayHen = DateTime.FromOADate(double.Parse(Data[i][1]));

                        string SoGiaoNhan = Data[i][2];
                        string MaTram = Data[i][3];
                        //string TenTram = Data[i][4];
                        string TenKhachHang = Data[i][5];
                        string SoDienThoai = Data[i][6];
                        string DiaChi = Data[i][7];
                        string TenSanPham = Data[i][8];
                        string Model = Data[i][9];
                        string Serial = Data[i][10];
                        string HienTuongLoi = Data[i][11];

                        if (string.IsNullOrWhiteSpace(SoDienThoai))
                        {
                            result.ListErrorRow.Add(Data[i]);
                        }
                        else
                        {
                            var newCongViec = new CongViec
                            {
                                NgayGioHen = NgayHen,
                                SoGiaoNhan = SoGiaoNhan,
                                KhachHangName = TenKhachHang,
                                SoDienThoai = SoDienThoai,
                                DiaChi = DiaChi,
                                SanPhamName = TenSanPham,
                                SanPhamModel = Model,
                                SanPhamSerial = Serial,
                                NoiDung = HienTuongLoi,

                                // các trường bắt buộc
                                TieuDe = "",
                                ThanhTien = 0,
                                TrangThaiId = 0,
                                DaCapNhatLocation = false,
                                TimeId = string.Format("{0:yyyyMM}", DateTime.Now)
                            };

                            // Kiểm tra nếu có mã trạm thì tìm TramDichVuId
                            if (!string.IsNullOrWhiteSpace(MaTram) && SortedTramDichVu.ContainsKey(MaTram))
                            {
                                newCongViec.TramDichVuId = SortedTramDichVu[MaTram];
                            }

                            // Kiểm tra Địa chỉ khác null thì tính Location
                            if (!string.IsNullOrWhiteSpace(DiaChi))
                            {
                                newCongViec.Location = await Global.GlobalFunction.GetLongLatFromAddress(_globalCache.GetGoogleApiKey(), DiaChi);
                            }

                            result.ListResult.Add(newCongViec);
                        }
                    }
                    catch
                    {
                        result.ListErrorRow.Add(Data[i]);
                    }
                }
            }

            return result;
        }
    }
}
