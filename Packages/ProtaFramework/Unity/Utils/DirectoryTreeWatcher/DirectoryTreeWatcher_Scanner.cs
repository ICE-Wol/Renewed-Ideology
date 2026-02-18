using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using Prota;

namespace Prota.Unity
{
    public partial class DirectoryTreeWatcher
    {
        private readonly Thread _scanThread;
        private bool _shouldStop = false;
        private int _scanIntervalMs = 500;
        
        private readonly ManualResetEvent _scanTrigger = new(false);
        private int _scanningCount = 0;
        private readonly List<Action> _scanFinishedCallbacks = new();
        
        private int _scanLoop = 0;
        public int scanLoop => _scanLoop;
        
        public TimeSpan lastScanTimeConsume { get; private set; } = TimeSpan.Zero;

        readonly System.Diagnostics.Stopwatch stopwatch = new();

        private void ScanLoop()
        {
            while (!_shouldStop)
            {
                try
                {
                    int interval = _scanIntervalMs;
                    
                    // 等待触发信号或超时
                    int waitTime = interval.Max(100);
                    _scanTrigger.WaitOne(waitTime);
                    
                    if (_shouldStop) break;

                    // 开始扫描
                    lock (_scanFinishedCallbacks)
                    {
                        Interlocked.Increment(ref _scanningCount);
                    }

                    try
                    {
                        UnityEngine.Profiling.Profiler.BeginSample($"DirectoryTreeWatcher-Scan [{scanLoop}] {_rootPath}");
                        stopwatch.Restart();
                        PerformScan();
                    }
                    catch(ThreadAbortException)
                    {
                        // abort是正常的.
                        return;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[DirectoryTreeWatcher] Error in scan loop: {ex}");
                        onChanged?.Invoke(new ChangeEvent(ex));
                    }
                    finally
                    {
                        UnityEngine.Profiling.Profiler.EndSample();
                        stopwatch.Stop();
                        lastScanTimeConsume = stopwatch.Elapsed;

                        lock (_scanFinishedCallbacks)
                        {
                            foreach(var callback in _scanFinishedCallbacks)
                            {
                                callback?.Invoke();
                            }
                            _scanFinishedCallbacks.Clear();

                            Interlocked.Decrement(ref _scanningCount);
                            
                            // 重置信号，准备下一次等待
                            _scanTrigger.Reset();

                        }
                    }

                    Interlocked.Increment(ref _scanLoop);
                }
                catch(ThreadAbortException)
                {
                    // abort是正常的.
                    return;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[DirectoryTreeWatcher] Critical error in scan loop: {ex.Message}");
                }
            }
        }

        private void PerformScan()
        {
            if (!Directory.Exists(_rootPath))
            {
                return;
            }

            try
            {
                var oldTreeData = _treeData;
                var newTreeData = DirectoryTreeData.Scan(_rootPath, oldTreeData);
                
                _treeData = newTreeData;
                _lastScanTime = DateTime.UtcNow;
                
                DirectoryTreeData.CompareAndNotify(oldTreeData, newTreeData, onChanged);
            }
            catch(ThreadAbortException)
            {
                // abort是正常的.
                return;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[DirectoryTreeWatcher] Error performing scan: {ex.Message}");
                onChanged?.Invoke(new ChangeEvent(ex));
            }
        }
    }
}
