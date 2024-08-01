using BD.Inventory.Common;
using BD.Inventory.Entities;
using BD.Inventory.WebApi.Common;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using System;
using System.Collections.Generic;

namespace BD.Inventory.WebApi.QuartzJob
{
    public class SchedulerServer
    {
        /// <summary>
        /// 调用定时任务
        /// </summary>
        public static void SynChronizeStart()
        {
            var playload = new JWTPlayloadInfo { LoginIP = "0.0.0.0", UserName = "主机" };
            try
            {
                StartQuartz();
            }
            catch (Exception ex)
            {
                LogHelper.LogError(playload, ex, Constant.ActionEnum.Quartz, "创建定时任务");
            }

        }

        /// <summary>
        /// 开始定时任务
        /// </summary>
        private static void StartQuartz()
        {

            // 创建调度器工厂
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();


            IScheduler scheduler = schedulerFactory.GetScheduler();
            scheduler.Clear();

            // 获取所有实现 IJobModel 接口的类型
            var typeList = GetClassByInterface(typeof(IJobModel));

            foreach (var type in typeList)
            {
                var obj = Activator.CreateInstance(type) as IJobModel;
                if (obj == null)
                {
                    continue;
                }

                string jobName = obj.JobName;
                string jobTime = obj.JobTime;

                if (string.IsNullOrEmpty(jobTime))
                {
                    Console.WriteLine($"Job time has not been set for job {jobName}.");
                    continue; // 跳过这个作业
                }

                // 尝试解析 Cron 表达式，如果解析失败，则跳过该作业
                try
                {
                    var cronExpression = new CronExpression(jobTime);
                }
                catch
                {
                    Console.WriteLine($"Invalid cron expression for job {jobName}.");
                    continue; // 跳过这个作业
                }

                // 检查下一次执行时间是否是未来的时间
                DateTime? nextFireTime = GetNextFireTime(jobTime);
                if (!nextFireTime.HasValue || nextFireTime.Value <= DateTime.UtcNow)
                {
                    Console.WriteLine($"The scheduled time for job {jobName} is in the past or invalid.");
                    continue; // 跳过这个作业
                }

                // 2. 创建一个具体的作业
                IJobDetail job = JobBuilder.Create(type)
                    .WithIdentity(jobName, "JobGroup")
                    .Build();

                // 3. 创建并配置一个触发器
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity(jobName, "JobGroup")
                    .StartNow()
                    .WithCronSchedule(jobTime) // 时间表达式
                    .Build();


                // 4. 加入作业调度池中
                scheduler.ScheduleJob(job, trigger);
            }

            // 启动调度器
            scheduler.Start();

        }

        // 辅助方法，用于获取 Cron 表达式的下一次执行时间
        private static DateTime? GetNextFireTime(string cronExpression)
        {
            try
            {
                // 假设你的服务器在中国，东八区时区
                string timeZoneId = "China Standard Time";
                // 获取时区信息
                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

                var cron = new CronExpression(cronExpression);
                DateTimeOffset? nextFireTimeUtc = cron.GetTimeAfter(DateTime.UtcNow);
                // 将下一个 UTC 触发时间转换为本地时间
                DateTime? nextFireTimeLocal = nextFireTimeUtc.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(nextFireTimeUtc.Value.DateTime, timeZone) : (DateTime?)null;
                //return nextFireTimeUtc.HasValue ? nextFireTimeUtc.Value.DateTime : (DateTime?)null;
                return nextFireTimeLocal;
            }
            catch (FormatException)
            {
                // 处理无效的 Cron 表达式
                //Console.WriteLine($"The cron expression '{cronExpression}' is invalid.");
                return null;
            }
        }

        /// <summary>
        /// 获取所有继承 IJobModel接口的类
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetClassByInterface(Type interfaceType)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var t in type.GetInterfaces())
                    {
                        if (t == interfaceType)
                        {
                            yield return type;
                            break;
                        }
                    }
                }
            }
        }

    }
}