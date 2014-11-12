using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using Common.Logging;
using MorSun.Model;
using HOHO18.Common.Model;
using MorSun.Bll;
using MorSun.Controllers.CommonController;
using MorSun.Controllers;
using HOHO18.Common.Base;
using HOHO18.Common;
using HOHO18.Common.Web;


namespace MorSun.Controllers.Quartz.CheckingIn
{
    public class CheckingJob : IJob
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CheckingJob));
        private zkemkeeper.CZKEMClass axCZKEM = new zkemkeeper.CZKEMClass();
        private int iMachineNumber = 1;
        private bool bIsConnected = false;
        
        /// <summary>
        /// 连接考勤机
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        private string ConnectKQ(string ip,int port)
        {
            int idwErrorCode = 0;
            bool bIsConnected = axCZKEM.Connect_Net(ip, port);
            string msg="";
            if (bIsConnected)
            {
                bIsConnected = true;
                axCZKEM.RegEvent(iMachineNumber, 65535);
                msg = "true";
            }
            else
            {
                axCZKEM.GetLastError(ref idwErrorCode);
                msg = "连接失败，错误代码：" + idwErrorCode.ToString();
            }
            return msg;
        }

        /// <summary>
        /// 删除连接
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        private void DisConnectKQ()
        {
            axCZKEM.Disconnect();
            bIsConnected = false;
        }

        /// <summary>
        /// 获取考勤数据
        /// </summary>
        /// <param name="id">考勤机ID</param>
        /// <param name="clearData">是否清除考勤机数据</param>
        /// <returns></returns>
        public string GetGeneralLogData(Guid id, bool clearData)
        {
            string msg = "";

            string sdwEnrollNumber = "";
            int idwTMachineNumber = 0;
            int idwEMachineNumber = 0;
            int idwVerifyMode = 0;
            int idwInOutMode = 0;
            int idwYear = 0;
            int idwMonth = 0;
            int idwDay = 0;
            int idwHour = 0;
            int idwMinute = 0;
            int idwSecond = 0;
            int idwWorkcode = 0;

            string sName = "";
            string sPassword = "";
            int iPrivilege = 0;
            bool bEnabled = false;
            Dictionary<string, string> dRecord = new Dictionary<string, string>();

            int idwErrorCode = 0;

            var bll = new BaseBll<kqRecord>();
            var l = new List<kqRecord>();
            var RList = bll.All;
            Int64 sort = Convert.ToInt64(RList.Max(r => r.Sort));
            int iValue = 0;
            axCZKEM.GetDeviceStatus(iMachineNumber, 6, ref iValue);
            var recordList = new List<kqRecord>();
            var kqRecordCount = 4000;
            var filepath = webConfigHelp.GetWebConfigValueNoCache("ServicePath"); ;
            var recordMachineModel = new BaseBll<kqPort>().All.Where(p => p.ID == id).FirstOrDefault();
            if (recordMachineModel != null)
            {
                kqRecordCount = recordMachineModel.MaxRecord.HasValue ? recordMachineModel.MaxRecord.Value : kqRecordCount;
            }
            axCZKEM.EnableDevice(iMachineNumber, false);//disable the device
            if (axCZKEM.ReadGeneralLogData(iMachineNumber))//read all the attendance records to the memory
            {
                if (iValue < kqRecordCount)
                {
                    axCZKEM.EnableDevice(iMachineNumber, true);//enable the device  当记录数小于最大记录数时，才马上启用，否则，要备份完之后才启用。读取完数据之后就启用考勤机
                }
                else
                {
                    axCZKEM.ReadAllUserID(iMachineNumber);
                    while (axCZKEM.SSR_GetAllUserInfo(iMachineNumber, out sdwEnrollNumber, out sName, out sPassword, out iPrivilege, out bEnabled))
                    {
                        int index = sName.IndexOf("\0");
                        if (index != -1)
                        {
                            sName = sName.Substring(0, index);
                        }
                        dRecord.Add(sdwEnrollNumber, sName);
                    }
                }
                while (axCZKEM.SSR_GetGeneralLogData(iMachineNumber, out sdwEnrollNumber, out idwVerifyMode,
                           out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get records from the memory
                {

                    //lvLogs.Items.Add(iGLCount.ToString());
                    //lvLogs.Items[iIndex].SubItems.Add(sdwEnrollNumber);//modify by Darcy on Nov.26 2009
                    //lvLogs.Items[iIndex].SubItems.Add(idwVerifyMode.ToString());
                    //lvLogs.Items[iIndex].SubItems.Add(idwInOutMode.ToString());
                    //lvLogs.Items[iIndex].SubItems.Add(idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString());
                    //lvLogs.Items[iIndex].SubItems.Add(idwWorkcode.ToString());

                    DateTime RTime = Convert.ToDateTime(idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString());
                    String CId = sdwEnrollNumber;
                    if (RList.Where(r => r.CId == CId && r.RecordTime == RTime && r.PId == id).FirstOrDefault() == null)//考勤机的数据是还没有添加进数据库的话才添加进去
                    {
                        kqRecord model = new kqRecord();
                        model.PId = id;
                        model.RecordTime = RTime;
                        model.CId = CId;
                        model.ID = Guid.NewGuid();
                        model.RegTime = DateTime.Now;
                        model.Sort = sort;
                        sort++;
                        l.Add(model);
                    }                   
                    if (iValue >= kqRecordCount)
                    {
                        recordList.Add(new kqRecord() { CId = CId, RecordTime = RTime, userName = dRecord[CId] });                        
                    }

                }
                if (l.Count() > 0)
                {
                    foreach (var m in l)
                    {
                        RList.AddObject(m);
                    }
                    bll.UpdateChanges();
                    msg = "true";
                    if (clearData)
                        axCZKEM.ClearGLog(iMachineNumber);  //这句会删掉考勤机所有数据
                }
                else
                {
                    msg = "截止上次读取数据为止没有新的打卡记录或数据读取失败!";
                }
                if (recordList.Count > 0)
                {
                    var ds = new KQController().ToDataSet(recordList);
                    new FileObj().FolderCreateNew(filepath + "UploadFile/kqRecord/" + id);
                    var fileName = DateTime.Now.ToString("yyyy-MM-ddHHmmss") + ".xls";
                    var oldFileName = fileName;
                    fileName = filepath + "UploadFile/kqRecord/" + id + "/" + fileName;
                    new ExcleHelper().DataSetToExcle(ds, fileName);
                    var mailModel = new wmfMail();
                    mailModel.MailContent = "考勤机数据已经达到要清空的标准，系统已经做好备份，并清空考勤机数据。点击这里下载备份文件。<a href=\"javascript:void(0);\" onclick=\"DownloadFile('/KQ','/UploadFile/kqRecord/" + id + "/" + oldFileName + "','" + oldFileName + "')\">" + oldFileName + "</a>";
                    new SysEmailController().SentSystemInfo(mailModel);
                    axCZKEM.ClearGLog(iMachineNumber);//清空考勤记录
                    axCZKEM.EnableDevice(iMachineNumber, true);//启用考勤机
                }
            }
            else
            {
                axCZKEM.GetLastError(ref idwErrorCode);

                if (idwErrorCode != 0)
                {
                    msg = "从设备读取数据失败，错误代码:" + idwErrorCode.ToString();
                }
                else
                {
                    msg = "设备中没有数据！";
                }
                var mailModel = new wmfMail();
                mailModel.MailContent = "考勤机数据已经超过要清空的标准，系统无法进行进一步操作，请您做好备份并清空考勤机数据。";
                new SysEmailController().SentSystemInfo(mailModel);
            }
            //axCZKEM.EnableDevice(iMachineNumber, true);//enable the device            
            return msg;
        }


        public virtual void Execute(IJobExecutionContext context)
        {
            // This job simply prints out its job name and the
            // date and time that it is running
            //JobKey jobKey = context.JobDetail.Key;
            //log.InfoFormat("SimpleJob says: {0} executing at {1}", jobKey, DateTime.Now.ToString("r"));

            string id = "9092ccd9-4089-4a15-adb5-0e18ac7db3be";//"f1547562-dda5-4fba-9d53-4ad44a898d42";//晨曦考勤机ID
            string msg = "";//返回消息

            var model = new BaseBll<kqPort>().GetModel(id);

            if (model == null)
            {
                log.InfoFormat("未找到考勤机记录");
            }

            //没有取到考勤机IP
            if (model.PortIP == null || model.PortIP.Equals(""))
                log.InfoFormat("考勤机IP为空");

            //中控考勤机OP1000默认端口号
            if (model.PortNumber == null)
                model.PortNumber = 4370;

            string connectMsg = ConnectKQ(model.PortIP, Convert.ToInt32(model.PortNumber));
            if (connectMsg.Equals("true"))
            {
                msg = GetGeneralLogData(model.ID, false);
                //关闭连接
                DisConnectKQ();
            }
            else
            {
                msg = connectMsg;
            }
            log.InfoFormat(msg);
        }
    }
}
