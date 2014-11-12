using Common.Logging;
using MorSun.Controllers.Quartz.CheckingIn;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MorSun.Controllers.Quartz
{
    public class CheckingTrigger3
    {
        public virtual void Run()
        {
            ILog log = LogManager.GetLogger(typeof(CheckingTrigger));
            log.Info("------- Initializing -------------------");

            log.Info("------- Initialization Complete --------");

            log.Info("------- Scheduling Jobs ----------------");

            IJobDetail job = JobBuilder.Create<CheckingJob3>()
                .WithIdentity("job33", "group33")//.RequestRecovery(true)//服务重启之后不用再执行任务 应用重启之后时候忽略过期任务，默认false
                .Build();

            ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create()
                                                      .WithIdentity("trigger33", "group33")
                                                      .WithCronSchedule("00 00 02 * * ?")//.WithCronSchedule("20 30 9,14,22 * * ?")
                                                      .Build();

            DateTimeOffset ft = MorSunScheduler.Instance.SchedulerJob(job, trigger);
            log.Info(job.Key + " has been scheduled to run at: " + ft
                     + " and repeat based on expression: "
                     + trigger.CronExpressionString);
        }
    }
}
