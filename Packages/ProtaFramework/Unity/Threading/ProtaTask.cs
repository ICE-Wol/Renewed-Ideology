using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Prota
{
    public static class ProtaTask
    {
        public static void ProtectedWait(this Task task)
        {
            if(task.Status == TaskStatus.Running) task.Wait();
            else if(task.Status == TaskStatus.WaitingForActivation) { task.Start(); task.Wait(); }
            else if(task.Status == TaskStatus.WaitingToRun) { task.Start(); task.Wait(); }
            else if(task.Status == TaskStatus.Created) { task.Start(); task.Wait(); }
            else if(task.Status == TaskStatus.WaitingForChildrenToComplete) task.Wait();
            else if(task.Status == TaskStatus.RanToCompletion) return;
            else if(task.Status == TaskStatus.Faulted) throw task.Exception;
        }
        
        public static T ProtectedWait<T>(this Task<T> task)
        {
            if(task.Status == TaskStatus.Running) task.Wait();
            else if(task.Status == TaskStatus.WaitingForActivation) task.Start();
            else if(task.Status == TaskStatus.WaitingToRun) task.Start();
            else if(task.Status == TaskStatus.Created) task.Start();
            else if(task.Status == TaskStatus.WaitingForChildrenToComplete) task.Wait();
            else if(task.Status == TaskStatus.RanToCompletion) return task.Result;
            else if(task.Status == TaskStatus.Faulted) throw task.Exception;
            return task.Result;
        }
        
        public static Task Then(this Task task, Action action)
        {
            return Task.Run(async () => {
                try
                {
                    await task;
                    action();
                }
                catch(Exception e)
                {
                    Debug.LogException(e);
                }
            });
        }
        
        public static Task<Ret> Then<Ret>(this Task<Ret> task, Func<Ret> action)
        {
            return Task.Run(async () => {
                try
                {
                    await task;
                    return action();
                }
                catch(Exception e)
                {
                    Debug.LogException(e);
                    return default;
                }
            });
        }
        
        public static Task Run(Action action)
        {
            return Task.Run(() => {
                try
                {
                    action();
                }
                catch(Exception e)
                {
                    Debug.LogException(e);
                }
            });
        }
        
        public static Task<Ret> Run<Ret>(Func<Ret> func)
        {
            return Task.Run(() => {
                try
                {
                    return func();
                }
                catch(Exception e)
                {
                    Debug.LogException(e);
                    return default;
                }
            });
        }
        
        public static Task RunAsync(Func<Task> func)
        {
            return Task.Run(async () => {
                try
                {
                    await func();
                }
                catch(Exception e)
                {
                    Debug.LogException(e);
                }
            });
        }
        
        public static Task<Ret> RunAsync<Ret>(Func<Task<Ret>> func)
        {
            return Task.Run(async () => {
                try
                {
                    return await func();
                }
                catch(Exception e)
                {
                    Debug.LogException(e);
                    return default;
                }
            });
        }   
    }
}


