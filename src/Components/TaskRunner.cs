using System;
using System.Collections.Concurrent;
using RandomEncounters.Patch;
using RandomEncounters.Utils;
using Unity.Entities;

namespace RandomEncounters.Components
{
    public class TaskRunner
    {
        private static readonly ConcurrentQueue<RisingTask> TaskQueue = new();
        private static readonly ConcurrentDictionary<Guid, RisingTaskResult> TaskResults = new();

        public static void Initialize()
        {
            ServerEvents.OnUpdate += Update;
        }

        public static Guid Start(Func<World, object> func, TimeSpan startAfter)
        {
            var risingTask = new RisingTask
            {
                ResultFunction = func,
                StartAfter = DateTime.UtcNow.Add(startAfter),
                TaskId = Guid.NewGuid()
            };
            TaskQueue.Enqueue(risingTask);
            return risingTask.TaskId;
        }

        public static object GetResult(Guid taskId)
        {
            return TaskResults.TryGetValue(taskId, out var result) ? result : null;
        }

        private static void Update(World world)
        {
            if (!TaskQueue.TryDequeue(out var task))
            {
                return;
            }

            if (task.StartAfter > DateTime.UtcNow)
            {
                TaskQueue.Enqueue(task);
                return;
            }
            object result;
            try
            {
                Logger.LogDebug("Executing task");
                result = task.ResultFunction.Invoke(world);
            }
            catch (Exception e)
            {
                TaskResults[task.TaskId] = new RisingTaskResult { Exception = e };
                return;
            }

            TaskResults[task.TaskId] = new RisingTaskResult { Result = result };
        }

        public static void Destroy()
        {
            ServerEvents.OnUpdate-=Update;
            TaskQueue.Clear();
            TaskResults.Clear();
        }
        
        private class RisingTask
        {
            public Guid TaskId { get; set; } = Guid.NewGuid();
            public DateTime StartAfter { get; set; }
            public Func<World, object> ResultFunction { get; set; }
        }
    }

    public class RisingTaskResult
    {
        public object Result { get; set; }
        public Exception Exception { get; set; }
    }
}
