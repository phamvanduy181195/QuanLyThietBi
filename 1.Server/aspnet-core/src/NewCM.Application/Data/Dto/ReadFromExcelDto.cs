using System.Collections.Generic;

namespace NewCM.Data.Dto
{
    public class ReadFromExcelDto<ResultDto>
    {
        /// <summary>
        /// Return code:
        ///     200 : OK
        ///     1   : File not found
        ///     2   : Sheet not found
        ///     3   : Cant read data
        /// </summary>
        public int ResultCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ErrorMessage { get; set; }

        public List<ResultDto> ListResult { get; set; }

        public List<List<string>> ListErrorRow { get; set; }

        public ReadFromExcelDto()
        {
            ResultCode = 200;
            ErrorMessage = "";

            ListResult = new List<ResultDto>();
            ListErrorRow = new List<List<string>>();
        }
    }
}
