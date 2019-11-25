using Microsoft.AspNetCore.Hosting;
using System.Net;
using NewCM.Configuration;
using Abp.Dependency;
using System.Threading.Tasks;
using System.Net.Mail;

namespace NewCM.Global
{
    public class SendEmail : ISingletonDependency
    {
        private readonly string ServerIp;
        private readonly int ServerPort;
        private readonly string Username;
        private readonly string Password;

        public SendEmail(IHostingEnvironment env)
        {
            //
            // Kết nối https không cần trusted cert
            //
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            var _appConfiguration = env.GetAppConfiguration();
            ServerIp = _appConfiguration["App:EmailInfo:ServerIp"] ?? "smtp.gmail.com";
            if (int.TryParse(_appConfiguration["App:EmailInfo:ServerPort"], out int Port))
                ServerPort = Port;
            else
                ServerPort = 587;
            Username = _appConfiguration["App:EmailInfo:Username"] ?? "dichvusuachua.server@gmail.com";
            Password = _appConfiguration["App:EmailInfo:Password"] ?? "dichvusuachua@123";
        }

        public async Task ActiveCodeEmail(string MailTo, string ActiveCode)
        {
            string Subject = "Mã kích hoạt tài khoản";
            string Message = "Xin chào!\r\n";
            Message += "Bạn hoặc ai đó đã sử dụng email này để đăng ký tài khoản trên hệ thống dịch vụ bảo hành.\r\n";
            Message += "Mã kích hoạt tài khoản của bạn là: " + ActiveCode;
            Message += "\r\n\r\nCảm ơn Bạn đã sử dụng dịch vụ của chúng tôi.";

            await Send(MailTo, Subject, Message);
        }

        public async Task ResetPasswordEmail(string MailTo, string NewPassword)
        {
            string Subject = "Reset mật khẩu hệ thống";
            string Message = "Xin chào!\r\n";
            Message += "Bạn hoặc ai đó đã sử dụng chức năng reset mật khẩu trên hệ thống dịch vụ bảo hành.\r\n";
            Message += "Mật khẩu mới của Bạn là: " + NewPassword;
            Message += "\r\n\r\nCảm ơn Bạn đã sử dụng dịch vụ của chúng tôi.";

            await Send(MailTo, Subject, Message);
        }
        private async Task Send(string MailTo, string Subject, string Message)
        {
            MailMessage message = new MailMessage(Username, MailTo)
            {
                Subject = Subject,
                Body = Message
            };

            using (SmtpClient client = new SmtpClient())
            {
                client.Host = ServerIp;
                client.Port = ServerPort;
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(Username, Password);

                await client.SendMailAsync(message);
            }
        }
    }
}
