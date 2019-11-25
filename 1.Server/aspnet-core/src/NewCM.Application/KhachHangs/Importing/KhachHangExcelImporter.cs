using NewCM.Data;
using NewCM.Data.Dto;
using NewCM.DbEntities;
using System.Collections.Generic;

namespace NewCM.KhachHangs.Importing
{
    public class KhachHangExcelImporter: ExcelImporterBase, IKhachHangExcelImporter
    {
        public ReadFromExcelDto<KhachHang> ReadFromExcel(string FilePath)
        {
            ReadFromExcelDto<KhachHang> result = new ReadFromExcelDto<KhachHang>();

            var Data = ReadFile(FilePath, "DanhSachKhachHang");

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
                        string Name = Data[i][1];
                        string PhoneNumber = Data[i][2];
                        string Address = Data[i][3];

                        if (string.IsNullOrWhiteSpace(Name) || PhoneNumber.Length > 32 || Address.Length > 4000)
                        {
                            result.ListErrorRow.Add(Data[i]);
                        }
                        else
                        {
                            result.ListResult.Add(new KhachHang
                            {
                                Name = Name,
                                PhoneNumber = PhoneNumber,
                                Address = Address
                            });
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
