using MorSun.Bll;
using MorSun.Common.类别;
using MorSun.Controllers.CommonController;
using MorSun.Controllers.ViewModel;
using MorSun.Model;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MorSun.Controllers.Quartz
{
    public class CheckingJob3:IJob
    {

        private BaseBll<gcTask> bll = new BaseBll<gcTask>();

        public void Execute(IJobExecutionContext context)
        {
            #region 在登录成功的时候，对造价预算任务进行延迟 add by timfeng 2014-9-28。

            var taskCtr = new gcTaskController();
            var taskRef = Guid.Parse(Reference.下达任务_造价预算);
            var tkTaskRef=Guid.Parse(Reference.下达任务_踏勘评审);
            var result=Guid.Parse(Reference.下达任务状态_完成);
            var budegtTasks = bll.All.Where(u=>u.TransferRef==taskRef&&u.PlanTime<DateTime.Now);
            foreach (var budget in budegtTasks)
            {
                var hasTkTask = bll.All.Any(u => u.ProjectId == budget.ProjectId && u.TransferRef == tkTaskRef && u.Result == result);
                if (hasTkTask)
                {
                    taskCtr.DelayTask(false, budget);
                }
                //else
                //{//add by you 有时候会用到，不经常用，先注释掉
                //    taskCtr.DelayTask(tkTask, false, budget);
                //}
            }
            bll.UpdateChanges();
            #endregion
        }
    }
}
