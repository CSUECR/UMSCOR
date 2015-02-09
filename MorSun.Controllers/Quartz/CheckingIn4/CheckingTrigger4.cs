using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MorSun.Controllers.Quartz
{
    public class CheckingTrigger4
    {
        public virtual void Run()
        {            

            IJobDetail job = JobBuilder.Create<CheckingJob4>()
                .WithIdentity("jobzyb44", "groupzyb44")//.RequestRecovery(true)//服务重启之后不用再执行任务 应用重启之后时候忽略过期任务，默认false
                .Build();

            ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create()
                                                      .WithIdentity("triggerzyb44", "groupzyb44")
                                                      .WithCronSchedule("0/15/30/45 * * * * ?")//.WithCronSchedule("20 30 9,14,22 * * ?")
                                                      .Build();

            DateTimeOffset ft = MorSunScheduler.Instance.SchedulerJob(job, trigger);            
        }
    }
}
