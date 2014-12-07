using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using Quartz.Impl;

namespace MorSun.Controllers.Quartz
{
    public class MorSunScheduler
    {
        static ISchedulerFactory _sf = new StdSchedulerFactory();
        static IScheduler _sched = _sf.GetScheduler();

        static MorSunScheduler _instance = null;
        static object lockObj = new object();

        public static MorSunScheduler Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new MorSunScheduler();
                        }
                    }
                }
                return _instance;
            }
        }

        public void Start()
        {
            //JobDetail job = new JobDetail("WriteLog", "Log", typeof(WriteLogJob));
            //DateTime start = TriggerUtils.GetNextGivenSecondDate(null, 5);

            //TimeSpan interval = TimeSpan.FromSeconds(10);
            //Trigger trigger = new SimpleTrigger("WriteLog", "Log", "WriteLog", "Log", start, null, 10, interval);

            //_sched.AddJob(job, true);
            //DateTime dt = _sched.ScheduleJob(trigger);
            _sched.Start();
        }

        public void Stop(bool waitForJobsToComplete)
        {
            if (waitForJobsToComplete)
                _sched.Shutdown(waitForJobsToComplete);
            else
                _sched.Shutdown();
        }

        public DateTimeOffset SchedulerJob(IJobDetail job, ICronTrigger trigger)
        {
            return _sched.ScheduleJob(job, trigger);
        }

        /// <summary>
        /// 判断矿石是否开启状态
        /// </summary>
        /// <returns></returns>
        public bool IsStart()
        {
            return _sched.IsStarted;
        }

        /// <summary>
        /// 判断矿石是否关闭状态
        /// </summary>
        /// <returns></returns>
        public bool IsStop()
        {
            return _sched.IsShutdown;
        }
        /// <summary>
        /// 清除所有记录
        /// </summary>
        public void Clear()
        {
            _sched.Clear();
        }
        /// <summary>
        /// 启用全部
        /// </summary>
        public void ResumeAll()
        {
            _sched.ResumeAll();
        }
        /// <summary>
        /// 停止某项工作
        /// </summary>
        /// <param name="name"></param>
        /// <param name="group"></param>
        public void StopJob(string name, string group)
        {
            JobKey job = new JobKey(name, group);
            _sched.Interrupt(job);
        }

        /// <summary>
        /// 触发某项工作 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="group"></param>
        public void TrggerJob(string name, string group)
        {
            JobKey job = new JobKey(name, group);
            _sched.TriggerJob(job);
        }
    }
}
