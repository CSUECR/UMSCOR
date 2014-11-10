using System;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using log4net;
using HOHO18.Common.WEB;

namespace HOHO18.Common.Web
{    
    /// <summary>  
    /// 发送邮件的类  
    /// </summary>
    public sealed class SendMail
    {
        private MailMessage mailMessage;
        private SmtpClient smtpClient;
        private string password;//发件人密码  
        
        /// <summary>  
        /// 处审核后类的实例  
        /// </summary>  
        /// <param name="To">收件人地址</param>  
        /// <param name="From">发件人地址</param>  
        /// <param name="Body">邮件正文</param>  
        /// <param name="Title">邮件的主题</param>  
        /// <param name="Password">发件人密码</param>  
        public SendMail(string To, string From, string Body, string Title, string Password, string MailFromName, string MailToName)
        {            
            Encoding chtEnc = Encoding.BigEndianUnicode;

            mailMessage = new MailMessage();
            mailMessage.To.Add(new MailAddress(To, MailToName, chtEnc));
            mailMessage.From = new MailAddress(From, MailFromName, chtEnc);
            mailMessage.Subject = Title;
            mailMessage.SubjectEncoding = chtEnc;
            mailMessage.Body = Body;
            mailMessage.IsBodyHtml = true;
            mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
            mailMessage.Priority = MailPriority.Normal;

            #region 服务器上要加这些内容
            ////set the mail to be certify needed            

            //mailMessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", 1);

            ////set the user to be certified 

            //mailMessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpusername", sendUsr);

            ////the password of the account 

            //mailMessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtppassword", sendPwd); 
            #endregion

            this.password = Password;
        }
        
        /// <summary>  
        /// 添加附件  
        /// </summary>  
        public void Attachments(string Path)  
        {  
            string[] path = Path.Split(',');  
            Attachment data;  
            ContentDisposition disposition;  
            for (int i = 0; i < path.Length; i++)  
            {  
                data = new Attachment(path[i], MediaTypeNames.Application.Octet);//实例化附件  
                disposition = data.ContentDisposition;  
                disposition.CreationDate = System.IO.File.GetCreationTime(path[i]);//获取附件的创建日期  
                disposition.ModificationDate = System.IO.File.GetLastWriteTime(path[i]);//获取附件的修改日期  
                disposition.ReadDate = System.IO.File.GetLastAccessTime(path[i]);//获取附件的读取日期  
                mailMessage.Attachments.Add(data);//添加到附件中  
            }  
        }
        
        /// <summary>  
        /// 异步发送邮件  
        /// </summary>  
        /// <param name="CompletedMethod"></param>  
        public void SendAsync(SendCompletedEventHandler CompletedMethod)
        {
            if (mailMessage != null)
            {
                smtpClient = new SmtpClient();
                smtpClient.Credentials = new System.Net.NetworkCredential(mailMessage.From.Address, password);//设置发件人身份的票据  
                smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtpClient.Host = "smtp." + mailMessage.From.Host;
                smtpClient.SendCompleted += new SendCompletedEventHandler(CompletedMethod);//注册异步发送邮件完成时的事件  
                smtpClient.SendAsync(mailMessage, mailMessage.Body);
            }
        }
        
        /// <summary>  
        /// 发送邮件  
        /// </summary>  
        public void Send()
        {
            if (mailMessage != null)
            {
                smtpClient = new SmtpClient();                
                smtpClient.Host = "smtp." + mailMessage.From.Host;
                //smtpClient.Host = "mxdomain.qq.com.";//"aspmx.l.google.com.";//
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential(mailMessage.From.Address, password);//设置发件人身份的票据  
                smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtpClient.EnableSsl = true;
                smtpClient.Port = 587;
                try { smtpClient.Send(mailMessage); }
                catch (Exception ex)
                {
                    LogHelper.Write(ex.Message.ToString(), LogHelper.LogMessageType.Error);//记录错误日志
                }
                
            }
        }

        public void Send(string host, int? port,string errMessage)
        {
            if (mailMessage != null)
            {
                smtpClient = new SmtpClient();
                if (string.IsNullOrEmpty(host))
                    host = "smtp.";
                var thost = mailMessage.From.Host == "bungma.com" ? "qq.com" : mailMessage.From.Host;
                smtpClient.Host = host + thost;
                //smtpClient.Host = "mxdomain.qq.com.";//"aspmx.l.google.com.";//
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential(mailMessage.From.Address, password);//设置发件人身份的票据  
                smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtpClient.EnableSsl = true;
                if (port == null)
                    port = 587;
                smtpClient.Port = port.Value;
                try { smtpClient.Send(mailMessage); }
                catch (Exception ex)
                {
                    LogHelper.Write(errMessage + ex.Message.ToString(), LogHelper.LogMessageType.Error);//记录错误日志
                    //for (int i = 0; i < 20;i++ )
                    //{LogHelper.Write(errMessage + ex.Message.ToString(), LogHelper.LogMessageType.Error);//记录错误日志 
                    //}                        
                }

            }
        } 
    }
    
}
