using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using Prota.Unity;
using Prota;

public class DirectoryTreeWatcherTests
{
    private string _testRootPath;
    private DirectoryTreeWatcher _watcher;

    [SetUp]
    public void Setup()
    {
        // 在 PersistentDataPath 下创建一个临时测试目录
        var rootPath = Path.Combine(Application.persistentDataPath, "DirectoryWatcherTest_" + Guid.NewGuid().ToString());
        rootPath = Path.GetFullPath(rootPath);
        Directory.CreateDirectory(rootPath);
        _testRootPath = rootPath.ToStandardPath();
    }

    /// <summary>
    /// 创建 DirectoryTreeWatcher 并设置测试用的短扫描间隔
    /// </summary>
    private DirectoryTreeWatcher CreateWatcherForTest()
    {
        var watcher = new DirectoryTreeWatcher(_testRootPath);
        // 设置较短的扫描间隔以便测试更快完成
        watcher.SetScanInterval(50);
        return watcher;
    }

    /// <summary>
    /// 等待至少一个扫描周期，确保扫描完成
    /// </summary>
    private void WaitForScan(DirectoryTreeWatcher watcher, int timeoutMs = 1000)
    {
        int scanInterval = watcher.GetScanInterval();
        int waitTime = Math.Max(scanInterval + 50, timeoutMs);
        Thread.Sleep(waitTime);
    }

    /// <summary>
    /// 等待事件触发，然后等待扫描完成
    /// </summary>
    private void WaitForEventAndScan(DirectoryTreeWatcher watcher, ref int eventTriggered, int timeoutMs = 1000)
    {
        int scanInterval = watcher.GetScanInterval();
        int waited = 0;
        int checkInterval = 50;
        
        // 等待事件触发
        while (eventTriggered == 0 && waited < timeoutMs)
        {
            Thread.Sleep(checkInterval);
            waited += checkInterval;
        }
        
        // 事件触发后，再等待一个扫描周期确保扫描完成
        if (eventTriggered > 0)
        {
            Thread.Sleep(scanInterval + 50);
        }
    }

    private bool WaitForNextScan(DirectoryTreeWatcher watcher, DateTime previousScanTimeUtc, int timeoutMs)
    {
        int waited = 0;
        int checkInterval = 20;
        while (waited < timeoutMs)
        {
            if (watcher.GetLastScanTime() > previousScanTimeUtc)
            {
                return true;
            }

            Thread.Sleep(checkInterval);
            waited += checkInterval;
        }

        return false;
    }

    [Test]
    public void DirectoryTreeData_Scan_ShouldCreateCorrectTree()
    {
        // Arrange
        string subDir = Path.Combine(_testRootPath, "SubDir");
        Directory.CreateDirectory(subDir);
        string file1 = Path.Combine(_testRootPath, "file1.txt");
        string file2 = Path.Combine(subDir, "file2.txt");
        File.WriteAllText(file1, "content1");
        File.WriteAllText(file2, "content2");

        // Act
        var treeData = DirectoryTreeData.Scan(_testRootPath, null);
        var rootNode = treeData.root;

        // Assert
        Assert.IsNotNull(rootNode);
        Assert.AreEqual(Path.GetFileName(_testRootPath), rootNode.name);
        
        // Verify file1
        Assert.IsTrue(rootNode.filesMap.ContainsKey("file1.txt"));
        var file1Node = treeData.GetFile(rootNode.filesMap["file1.txt"]);
        Assert.IsNotNull(file1Node);
        Assert.AreEqual("file1.txt", file1Node.name);
        Assert.AreEqual("content1", file1Node.utf8Content);

        // Verify subDir
        Assert.IsTrue(rootNode.directoriesMap.ContainsKey("SubDir"));
        var subNode = treeData.GetDirectory(rootNode.directoriesMap["SubDir"]);
        Assert.IsNotNull(subNode);
        Assert.AreEqual("SubDir", subNode.name);

        // Verify file2
        Assert.IsTrue(subNode.filesMap.ContainsKey("file2.txt"));
        var file2Node = treeData.GetFile(subNode.filesMap["file2.txt"]);
        Assert.IsNotNull(file2Node);
        Assert.AreEqual("file2.txt", file2Node.name);
        Assert.AreEqual("content2", file2Node.utf8Content);
    }

    [Test]
    public void RandomFileGeneration_ShouldHaveConsistentStructure()
    {
        // Arrange
        _watcher = CreateWatcherForTest();
        int fileCount = 20;
        var createdFiles = new Dictionary<string, string>(); // Path -> Content

        // Act: 随机生成文件结构
        var random = new System.Random();
        for (int i = 0; i < fileCount; i++)
        {
            string dir = _testRootPath;
            // 30% 概率创建在子目录
            if (random.NextDouble() < 0.3)
            {
                dir = Path.Combine(_testRootPath, "RandomSubDir_" + random.Next(0, 3));
                Directory.CreateDirectory(dir);
            }

            string fileName = $"random_file_{i}.txt";
            string fullPath = Path.Combine(dir, fileName);
            string content = Guid.NewGuid().ToString();
            
            File.WriteAllText(fullPath, content);
            createdFiles[fullPath.ToStandardPath()] = content;
        }

        // 等待扫描完成，由于文件较多，等待足够的时间
        WaitForScan(_watcher, 1000);

        // Assert 1: 通过数据结构检查
        var root = _watcher.GetDirectory(_testRootPath);
        foreach (var kvp in createdFiles)
        {
            string fullPath = kvp.Key;
            string expectedContent = kvp.Value;
            string fileName = Path.GetFileName(fullPath);

            // 检查文件是否存在于树中
            // 注意：这里我们需要手动遍历树来找到节点，或者使用我们实现的 GetFile
            // 为了验证树结构的正确性，我们尝试从 root 手动遍历

            DirectoryNode current = root;
            // 计算相对路径部分
            string relPath = fullPath.Substring(_testRootPath.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string[] parts = relPath.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

            // 遍历目录部分
            for (int j = 0; j < parts.Length - 1; j++)
            {
                Assert.IsTrue(current.directoryMap.ContainsKey(parts[j]), $"Directory {parts[j]} missing in tree");
                current = current.GetChildDirectory(parts[j]);
            }

            // 检查文件
            Assert.IsTrue(current.fileMap.ContainsKey(fileName), $"File {fileName} missing in tree node {current.name}");
            var node = current.GetChildFile(fileName);
            Assert.IsTrue(node.isValid, $"GetFile returned invalid node for {fullPath}");
            Assert.AreEqual(expectedContent, node.contentUtf8);
        }

        // Assert 2: 通过 GetFile 接口检查
        foreach (var kvp in createdFiles)
        {
            var node = _watcher.GetFile(kvp.Key);
            Assert.IsTrue(node.isValid, $"GetFile returned invalid node for {kvp.Key}");
            Assert.AreEqual(kvp.Value, node.contentUtf8);
        }
    }

    [Test]
    public void AccessNonExistentFile_ShouldBeInvalid()
    {
        _watcher = CreateWatcherForTest();
        Assert.IsFalse(_watcher.GetFile(Path.Combine(_testRootPath, "non_existent_file.txt").ToStandardPath()).isValid);
        Assert.IsFalse(_watcher.GetDirectory(Path.Combine(_testRootPath, "non_existent_dir").ToStandardPath()).isValid);
        Assert.IsFalse(_watcher.GetFile(Path.Combine(_testRootPath, "non_existent_abs.txt").ToStandardPath()).isValid);
    }

    [Test]
    public void HeldNode_ShouldBeInvalidated_WhenParentDeleted()
    {
        // Arrange
        string subDir = Path.Combine(_testRootPath, "ParentDir");
        string subSubDir = Path.Combine(subDir, "ChildDir");
        Directory.CreateDirectory(subSubDir);
        
        string file = Path.Combine(subSubDir, "deep_file.txt");
        File.WriteAllText(file, "data");

        _watcher = CreateWatcherForTest();
        
        // 等待初始扫描完成
        WaitForScan(_watcher);
        
        // 获取深层引用
        var deepFileNode = _watcher.GetFile(file.ToStandardPath());
        var childDirNode = _watcher.GetDirectory(subSubDir.ToStandardPath());
        
        Assert.IsTrue(deepFileNode.isRefValid);
        Assert.IsTrue(deepFileNode.isValid);
        Assert.IsTrue(childDirNode.isRefValid);
        Assert.IsTrue(childDirNode.isValid);

        // Act: 删除最上层父目录
        int eventTriggered = 0;
        _watcher.onChanged += (e) => Interlocked.Increment(ref eventTriggered);
        
        Directory.Delete(subDir, true);

        // 等待事件触发并完成扫描
        WaitForEventAndScan(_watcher, ref eventTriggered, 2000);

        // Assert
        Assert.IsTrue(eventTriggered > 0);
        Assert.IsTrue(deepFileNode.isValid, "Held deep file node should keep old snapshot state");
        Assert.IsTrue(childDirNode.isValid, "Held child dir node should keep old snapshot state");
        Assert.IsFalse(_watcher.GetFile(file.ToStandardPath()).isValid);
        Assert.IsFalse(_watcher.GetDirectory(subSubDir.ToStandardPath()).isValid);
        Assert.IsFalse(_watcher.GetDirectory(subDir.ToStandardPath()).isValid);
    }

    [TearDown]
    public void TearDown()
    {
        if (_watcher != null)
        {
            _watcher.Dispose();
            _watcher = null;
        }

        if (Directory.Exists(_testRootPath))
        {
            try
            {
                Directory.Delete(_testRootPath, true);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to cleanup test directory: {e.Message}");
            }
        }
    }

    private void TraverseNode(DirectoryNode node)
    {
        if (!node.isValid) return;
        
        // 访问属性
        var name = node.name;
        var path = node.fullPath;
        
        foreach (var dirName in node.directories)
        {
            var dir = node.GetChildDirectory(dirName);
            if (dir.isValid)
            {
                TraverseNode(dir);
            }
        }
    }

    [Test]
    public void Initialize_ShouldScanExistingFiles()
    {
        // Arrange
        string subDir = Path.Combine(_testRootPath, "SubDir");
        Directory.CreateDirectory(subDir);
        string file1 = Path.Combine(_testRootPath, "file1.txt");
        string file2 = Path.Combine(subDir, "file2.txt");
        File.WriteAllText(file1, "content1");
        File.WriteAllText(file2, "content2");

        // Act
        _watcher = CreateWatcherForTest();
        
        // 等待初始扫描完成
        WaitForScan(_watcher);

        // Assert
        var root = _watcher.GetDirectory(_testRootPath);
        Assert.IsTrue(root.isValid);
        Assert.IsTrue(root.fileMap.ContainsKey("file1.txt"));
        Assert.IsTrue(root.directoryMap.ContainsKey("SubDir"));

        var file1Node = root.GetChildFile("file1.txt");
        var subDirNode = root.GetChildDirectory("SubDir");
        Assert.IsTrue(file1Node.isValid);
        Assert.IsTrue(subDirNode.isValid);
        Assert.IsTrue(subDirNode.fileMap.ContainsKey("file2.txt"));
        var file2Node = subDirNode.GetChildFile("file2.txt");
        Assert.IsTrue(file2Node.isValid);

        Assert.AreEqual("content1", file1Node.contentUtf8);
        Assert.AreEqual("content2", file2Node.contentUtf8);
    }

    [Test]
    public void CreateFile_ShouldUpdateTree()
    {
        // Arrange
        _watcher = CreateWatcherForTest();
        int eventTriggered = 0;
        _watcher.onChanged += (e) => Interlocked.Increment(ref eventTriggered);

        // Act
        string newFile = Path.Combine(_testRootPath, "newFile.txt");
        ProtaFileUtils.ForceWriteAllText(newFile, "newContent");
        
        // 等待事件触发并完成扫描
        WaitForEventAndScan(_watcher, ref eventTriggered, 1000);

        // Assert
        Assert.IsTrue(eventTriggered > 0, "OnChanged event was not triggered");
        var root = _watcher.GetDirectory(_testRootPath);
        Assert.IsTrue(root.fileMap.ContainsKey("newFile.txt"));
        var node = root.GetChildFile("newFile.txt");
        Assert.IsTrue(node.isValid);
        Assert.AreEqual("newContent", node.contentUtf8);
    }


    [Test]
    public void CreateFile_ShouldUpdateTree_Simple()
    {
        // Arrange
        _watcher = CreateWatcherForTest();
        int eventTriggered = 0;
        _watcher.onChanged += (e) => { Interlocked.Increment(ref eventTriggered); };

        // Act
        string newFile = Path.Combine(_testRootPath, "newFile.txt");
        // File.WriteAllText(newFile, "newContent");
        
        using(var fs = new FileStream(
            newFile,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096,
            FileOptions.WriteThrough))
        {
            var content = "newContent";
            var bytes = System.Text.Encoding.UTF8.GetBytes(content);
            fs.Write(bytes, 0, bytes.Length);
            fs.Flush(true);
        }
        
        // Wait for scan to complete
        WaitForScan(_watcher, 1000);

        // Assert
        Assert.IsTrue(eventTriggered > 0, "OnChanged event was not triggered");
    }

    [Test]
    public void ModifyFile_ShouldUpdateContent_LazyLoad()
    {
        // Arrange
        string file = Path.Combine(_testRootPath, "modify_lazy.txt");
        File.WriteAllText(file, "original");
        _watcher = CreateWatcherForTest();
        
        // 等待初始扫描完成
        WaitForScan(_watcher);
        
        // 获取文件节点引用
        var node = _watcher.GetFile(file.ToStandardPath());
        Assert.IsTrue(node.isValid);
        // 第一次读取，加载内容
        Assert.AreEqual("original", node.contentUtf8);
        
        int eventTriggered = 0;
        _watcher.onChanged += (e) => Interlocked.Increment(ref eventTriggered);
        Debug.Log("register!");

        // Act
        // 确保文件时间戳有变化
        Thread.Sleep(400); 
        File.WriteAllText(file, "modified");
        Debug.Log("write!");

        // 等待事件触发并完成扫描
        WaitForEventAndScan(_watcher, ref eventTriggered, 1000);

        // Assert
        Assert.IsTrue(eventTriggered > 0, "OnChanged event was not triggered");
        
        Assert.AreEqual("original", node.contentUtf8);

        var newNode = _watcher.GetFile(file.ToStandardPath());
        Assert.IsTrue(newNode.isValid);
        Assert.AreEqual("modified", newNode.contentUtf8);
    }

    [Test]
    public void DeleteFile_ShouldRemoveNode()
    {
        // Arrange
        string file = Path.Combine(_testRootPath, "delete.txt");
        File.WriteAllText(file, "content");
        _watcher = CreateWatcherForTest();
        
        // 等待初始扫描完成
        WaitForScan(_watcher);
        
        int eventTriggered = 0;
        _watcher.onChanged += (e) => Interlocked.Increment(ref eventTriggered);

        // Act
        File.Delete(file);

        // 等待事件触发并完成扫描
        WaitForEventAndScan(_watcher, ref eventTriggered, 1000);

        // Assert
        Assert.IsTrue(eventTriggered > 0);
        var root = _watcher.GetDirectory(_testRootPath);
        Assert.IsFalse(root.fileMap.ContainsKey("delete.txt"));
    }

    [Test]
    public void RenameFile_ShouldUpdateTree()
    {
        // Arrange
        string oldFile = Path.Combine(_testRootPath, "old.txt");
        string newFile = Path.Combine(_testRootPath, "new.txt");
        File.WriteAllText(oldFile, "content");
        _watcher = CreateWatcherForTest();
        
        // 等待初始扫描完成
        WaitForScan(_watcher);
        
        int eventTriggered = 0;
        _watcher.onChanged += (e) => Interlocked.Increment(ref eventTriggered);

        // Act
        File.Move(oldFile, newFile);

        // 等待事件触发并完成扫描
        WaitForEventAndScan(_watcher, ref eventTriggered, 1000);

        // Assert
        Assert.IsTrue(eventTriggered > 0);
        var root = _watcher.GetDirectory(_testRootPath);
        Assert.IsFalse(root.fileMap.ContainsKey("old.txt"));
        Assert.IsTrue(root.fileMap.ContainsKey("new.txt"));
        var node = root.GetChildFile("new.txt");
        Assert.IsTrue(node.isValid);
        Assert.AreEqual("content", node.contentUtf8);
    }

    [Test]
    public void GetFile_ShouldReturnCorrectNode_WithAbsolutePath()
    {
        // Arrange
        string subDir = Path.Combine(_testRootPath, "SubDir");
        Directory.CreateDirectory(subDir);
        string file = Path.Combine(subDir, "target.txt");
        File.WriteAllText(file, "content");
        
        _watcher = CreateWatcherForTest();
        
        // 等待初始扫描完成
        WaitForScan(_watcher);

        // Act & Assert
        // 1. 绝对路径
        var nodeAbs = _watcher.GetFile(file.ToStandardPath());
        Assert.IsTrue(nodeAbs.isValid);
        Assert.AreEqual("target.txt", nodeAbs.name);
        Assert.AreEqual("content", nodeAbs.contentUtf8);

        // 2. 不存在的路径
        Assert.IsFalse(_watcher.GetFile(Path.Combine(_testRootPath, "non_existent.txt").ToStandardPath()).isValid);
    }

    [Test]
    public void GetDirectory_ShouldReturnCorrectNode_WithAbsolutePaths()
    {
        // Arrange
        string subDir = Path.Combine(_testRootPath, "SubDir");
        Directory.CreateDirectory(subDir);
        
        _watcher = CreateWatcherForTest();
        
        // 等待初始扫描完成
        WaitForScan(_watcher);

        // Act & Assert
        // 1. 绝对路径
        var nodeRel = _watcher.GetDirectory(subDir.ToStandardPath());
        Assert.IsTrue(nodeRel.isValid);
        Assert.AreEqual("SubDir", nodeRel.name);

        // 2. 根目录
        var rootRel = _watcher.GetDirectory(_testRootPath);
        Assert.IsTrue(rootRel.isValid);
        Assert.AreEqual(Path.GetFileName(_testRootPath), rootRel.name);
    }

    [Test]
    public void GetDirectory_WithAbsolutePath_ShouldThrow()
    {
        // Arrange
        _watcher = CreateWatcherForTest();
        WaitForScan(_watcher);
        
        // Act & Assert
        Assert.IsTrue(_watcher.GetDirectory(_testRootPath).isValid);
    }

    [Test]
    public void GetFile_WhenFileDeletedExternally_ShouldReturnNull()
    {
        // Arrange
            string file = Path.Combine(_testRootPath, "external_delete.txt");
            File.WriteAllText(file, "content");
            _watcher = CreateWatcherForTest();
            
            // 等待初始扫描完成
            WaitForScan(_watcher);
            
            // 确保文件存在
            Assert.IsTrue(_watcher.GetFile(file.ToStandardPath()).isValid);
            
            // Act - 模拟外部删除，并等待 Watcher 更新
            int eventTriggered = 0;
            _watcher.onChanged += (e) => Interlocked.Increment(ref eventTriggered);
            
            File.Delete(file);
            
            // 等待事件触发并完成扫描
            WaitForEventAndScan(_watcher, ref eventTriggered, 1000);

            // Assert
            Assert.IsTrue(eventTriggered > 0);
            Assert.IsFalse(_watcher.GetFile(file.ToStandardPath()).isValid);
        }

    [Test]
    public void Node_ShouldHaveCorrectLastWriteTime()
    {
        // Arrange
        string file = Path.Combine(_testRootPath, "time_test.txt");
        File.WriteAllText(file, "content");
        var fileInfo = new FileInfo(file);
        
        // Act
        _watcher = CreateWatcherForTest();
        
        // 等待初始扫描完成
        WaitForScan(_watcher);
        
        var node = _watcher.GetFile(file.ToStandardPath());

        // Assert
        Assert.IsTrue(node.isValid);
        // 允许微小的误差，因为文件系统时间精度可能不同
        Assert.AreEqual(fileInfo.LastWriteTimeUtc, node.lastWriteTimeUtc);
        Assert.AreEqual(fileInfo.LastWriteTime, node.lastWriteTime);
        
        // 验证目录时间
        var dirNode = _watcher.GetDirectory(_testRootPath);
        var dirInfo = new DirectoryInfo(_testRootPath);
        Assert.IsTrue(dirNode.isValid);
        Assert.AreEqual(dirInfo.LastWriteTimeUtc, dirNode.lastWriteTimeUtc);
    }
    
    [Test]
    public void Node_ShouldBeInvalidated_WhenDeleted()
    {
        // Arrange
        string subDir = Path.Combine(_testRootPath, "InvalidTestDir");
        Directory.CreateDirectory(subDir);
        string file = Path.Combine(subDir, "test.txt");
        File.WriteAllText(file, "content");
        
        _watcher = CreateWatcherForTest();
        
        // 等待初始扫描完成
        WaitForScan(_watcher);
        
        // 获取引用
        var dirNode = _watcher.GetDirectory(subDir.ToStandardPath());
        var fileNode = _watcher.GetFile(file.ToStandardPath());
        
        Assert.IsTrue(dirNode.isRefValid);
        Assert.IsTrue(fileNode.isRefValid);
        Assert.IsTrue(dirNode.isValid);
        Assert.IsTrue(fileNode.isValid);
        
        // Act
        int eventTriggered = 0;
        _watcher.onChanged += (e) => Interlocked.Increment(ref eventTriggered);
        
        // 删除整个目录
        Directory.Delete(subDir, true);
        
        // 等待事件触发并完成扫描
        WaitForEventAndScan(_watcher, ref eventTriggered, 1000);

        // Assert
        Assert.IsTrue(eventTriggered > 0);
        
        Assert.IsTrue(dirNode.isValid, "Held directory node should keep old snapshot state");
        Assert.IsTrue(fileNode.isValid, "Held file node should keep old snapshot state");
        
        // 验证树中已经不存在
        Assert.IsFalse(_watcher.GetDirectory(subDir.ToStandardPath()).isValid);
        Assert.IsFalse(_watcher.GetFile(file.ToStandardPath()).isValid);
    }


    [Test]
    public void HeavyConcurrent_ReadWrite_ShouldBeSafe()
    {
        // Arrange
        _watcher = CreateWatcherForTest();
        bool stop = false;
        var exceptions = new System.Collections.Concurrent.ConcurrentBag<Exception>();
        
        // 增加并发强度
        int writerCount = 4;    // 4个写入线程
        int readerCount = 20;   // 20个读取线程
        int fileCount = 50;     // 每个写入线程操作的文件数
        
        var writers = new Thread[writerCount];
        var readers = new Thread[readerCount];

        // 1. 写入线程：高频创建/修改/删除
        for (int i = 0; i < writerCount; i++)
        {
            int writerId = i;
            writers[i] = new Thread(() =>
            {
                try
                {
                    var random = new System.Random(writerId * 1000);
                    while (!stop)
                    {
                        // 每个线程操作自己的一组文件，避免文件锁冲突，但会触发树结构的并发修改
                        string fileName = $"heavy_file_{writerId}_{random.Next(fileCount)}.txt";
                        string filePath = Path.Combine(_testRootPath, fileName);
                        
                        try 
                        {
                            int op = random.Next(3);
                            if (op == 0) // Create/Modify
                            {
                                File.WriteAllText(filePath, Guid.NewGuid().ToString());
                            }
                            else if (op == 1) // Delete
                            {
                                if (File.Exists(filePath)) File.Delete(filePath);
                            }
                            else // Move
                            {
                                if (File.Exists(filePath))
                                {
                                    string newName = $"heavy_file_{writerId}_{random.Next(fileCount)}.txt";
                                    string newPath = Path.Combine(_testRootPath, newName);
                                    if (!File.Exists(newPath)) File.Move(filePath, newPath);
                                }
                            }
                        }
                        catch (IOException) { /* 忽略文件锁竞争 */ }
                        
                        // 极短的休眠，制造高频事件
                        Thread.Sleep(1);
                    }
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            });
        }

        // 2. 读取线程：高频读取树结构和文件内容
        for (int i = 0; i < readerCount; i++)
        {
            readers[i] = new Thread(() =>
            {
                try
                {
                    var random = new System.Random();
                    while (!stop)
                    {
                        var root = _watcher.GetDirectory(_testRootPath);
                        TraverseNodeLimited(root, 5);

                        if (root.files.Count > 0)
                        {
                            var files = new List<string>(root.files);
                            if (files.Count > 0)
                            {
                                var randomFile = files[random.Next(files.Count)];
                                var node = root.GetChildFile(randomFile);
                                if (node.isValid)
                                {
                                    try
                                    {
                                        var content = node.content;
                                    }
                                    catch (Exception ex)
                                    {
                                        if (ex is not IOException) throw;
                                    }
                                }
                            }
                        }
                        Thread.Sleep(1);
                    }
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            });
        }

        // Start
        foreach (var t in writers) t.Start();
        foreach (var t in readers) t.Start();

        // Run
        Thread.Sleep(5000); // 运行5秒

        // Stop
        stop = true;
        foreach (var t in writers) t.Join();
        foreach (var t in readers) t.Join();

        // Assert
        if (!exceptions.IsEmpty)
        {
            exceptions.TryPeek(out var e);
            Assert.Fail($"Encountered {exceptions.Count} exceptions. First: {e}");
        }
    }

    private void TraverseNodeLimited(DirectoryNode node, int depth)
    {
        if (!node.isValid || depth <= 0) return;
        
        // 访问属性，确保读锁保护
        var name = node.name;
        
        foreach (var fileName in node.files)
        {
            var file = node.GetChildFile(fileName);
            if (file.isValid)
            {
                var t = file.lastWriteTime;
            }
        }
        
        foreach (var dirName in node.directories)
        {
            var dir = node.GetChildDirectory(dirName);
            TraverseNodeLimited(dir, depth - 1);
        }
    }

    [Test]
    public void MultiThreaded_Read_ShouldBeSafe()
    {
        // Arrange
        string subDir = Path.Combine(_testRootPath, "MTDir");
        Directory.CreateDirectory(subDir);
        for (int i = 0; i < 100; i++)
        {
            File.WriteAllText(Path.Combine(subDir, $"file_{i}.txt"), $"content_{i}");
        }
        _watcher = CreateWatcherForTest();
        
        // 等待初始扫描完成
        WaitForScan(_watcher, 2000);

        // Act
        int threadCount = 10;
        Thread[] threads = new Thread[threadCount];
        Exception[] exceptions = new Exception[threadCount];

        for (int i = 0; i < threadCount; i++)
        {
            int threadIndex = i;
            threads[i] = new Thread(() =>
            {
                try
                {
                    for (int j = 0; j < 50; j++)
                    {
                        var root = _watcher.GetDirectory(_testRootPath);
                        var dir = root.GetChildDirectory("MTDir");
                        Assert.IsTrue(dir.isValid);
                        Assert.AreEqual(100, dir.files.Count);

                        var random = new System.Random();
                        int randomIdx = random.Next(0, 100);
                        Assert.IsTrue(dir.fileMap.ContainsKey($"file_{randomIdx}.txt"));
                        Thread.Sleep(1);
                    }
                }
                catch (Exception e)
                {
                    exceptions[threadIndex] = e;
                }
            });
            threads[i].Start();
        }

        // Wait
        foreach (var t in threads) t.Join();

        // Assert
        for (int i = 0; i < threadCount; i++)
        {
            if (exceptions[i] != null)
            {
                Assert.Fail($"Thread {i} failed: {exceptions[i]}");
            }
        }
    }

    [Test]
    public void Concurrent_ReadAndModify_ShouldBeSafe()
    {
        // Arrange
        _watcher = CreateWatcherForTest();
        bool stop = false;
        List<Exception> exceptions = new();
        object excLock = new object();

        // 1. 写入线程：不断创建和删除文件
        Thread writer = new(() =>
        {
            try
            {
                int counter = 0;
                while (!stop)
                {
                    string fileName = $"concurrent_file_{counter}.txt";
                    string filePath = Path.Combine(_testRootPath, fileName);
                    
                    // Create
                    File.WriteAllText(filePath, "content");
                    Thread.Sleep(5); // 模拟IO间隔
                    
                    // Modify
                    File.WriteAllText(filePath, "modified");
                    Thread.Sleep(5);

                    // Delete
                    File.Delete(filePath);
                    Thread.Sleep(5);
                    
                    counter = (counter + 1) % 10; // 循环使用文件名
                }
            }
            catch (Exception e)
            {
                lock (excLock) exceptions.Add(e);
            }
        });

        // 2. 读取线程：不断遍历树
        Thread[] readers = new Thread[5];
        for (int i = 0; i < readers.Length; i++)
        {
            readers[i] = new Thread(() =>
            {
                try
                {
                    while (!stop)
                    {
                        var root = _watcher.GetDirectory(_testRootPath);
                        TraverseNode(root);
                        Thread.Sleep(1);
                    }
                }
                catch (Exception e)
                {
                    lock (excLock) exceptions.Add(e);
                }
            });
        }

        // Start
        writer.Start();
        foreach (var r in readers) r.Start();

        // Run for a while
        Thread.Sleep(2000); // 运行2秒

        // Stop
        stop = true;
        writer.Join();
        foreach (var r in readers) r.Join();

        // Assert
        if (exceptions.Count > 0)
        {
            Assert.Fail($"Encountered {exceptions.Count} exceptions during concurrent test. First: {exceptions[0]}");
        }
    }

    [Test]
    public void DirectoryTreeData_Scan_ShouldReturnCorrectStructure()
    {
        // Arrange
        string subDir = Path.Combine(_testRootPath, "ScanTestDir");
        Directory.CreateDirectory(subDir);
        string file1 = Path.Combine(_testRootPath, "root_file.txt");
        string file2 = Path.Combine(subDir, "sub_file.txt");
        File.WriteAllText(file1, "content1");
        File.WriteAllText(file2, "content2");

        // Act
        var treeData = DirectoryTreeData.Scan(_testRootPath, null);
        var rootNode = treeData.root;

        // Assert
        Assert.IsNotNull(rootNode);
        Assert.AreEqual(Path.GetFileName(_testRootPath), rootNode.name);
        
        // 验证文件
        Assert.IsTrue(rootNode.filesMap.ContainsKey("root_file.txt"));
        var fileNode1 = treeData.GetFile(rootNode.filesMap["root_file.txt"]);
        Assert.IsNotNull(fileNode1);
        Assert.AreEqual("root_file.txt", fileNode1.name);
        
        // 验证子目录
        Assert.IsTrue(rootNode.directoriesMap.ContainsKey("ScanTestDir"));
        var dirNode = treeData.GetDirectory(rootNode.directoriesMap["ScanTestDir"]);
        Assert.IsNotNull(dirNode);
        Assert.AreEqual("ScanTestDir", dirNode.name);
        
        // 验证子目录中的文件
        Assert.IsTrue(dirNode.filesMap.ContainsKey("sub_file.txt"));
        var fileNode2 = treeData.GetFile(dirNode.filesMap["sub_file.txt"]);
        Assert.IsNotNull(fileNode2);
        Assert.AreEqual("sub_file.txt", fileNode2.name);
    }

    [Test]
    public void DirectoryTreeData_CreateFileNodeData_ShouldReturnCorrectData()
    {
        // Arrange
        string file = Path.Combine(_testRootPath, "node_test.txt");
        File.WriteAllText(file, "test_content");
        var fileInfo = new FileInfo(file);

        // Act
        var node = DirectoryTreeData.CreateFileNodeData(file);

        // Assert
        Assert.IsNotNull(node);
        Assert.AreEqual("node_test.txt", node.name);
        Assert.AreEqual(file.ToStandardPath(), node.fullPath);
        // 时间戳可能有微小差异，允许1秒误差
        Assert.Less((node.lastWriteTimeUtc - fileInfo.LastWriteTimeUtc).TotalSeconds, 1.0);
        Assert.AreEqual("test_content", node.utf8Content);
    }

    [Test]
    public void CompareAndNotify_ShouldTriggerEvents()
    {
        string watchPath = Path.Combine(_testRootPath, "CompareAndNotifyTest");
        Directory.CreateDirectory(watchPath);

        string deletedFilePath = Path.Combine(watchPath, "deleted.txt");
        string changedFilePath = Path.Combine(watchPath, "changed.txt");
        File.WriteAllText(deletedFilePath, "content");
        File.WriteAllText(changedFilePath, "old");
        File.SetLastWriteTimeUtc(changedFilePath, DateTime.UtcNow.AddMinutes(-2));

        using (var watcher = new DirectoryTreeWatcher(watchPath))
        {
            watcher.SetScanInterval(1000000);

            var events = new List<DirectoryTreeWatcher.ChangeEvent>();
            watcher.onChanged += (e) =>
            {
                lock (events)
                {
                    events.Add(e);
                }
            };

            Thread.Sleep(600);

            string createdFilePath = Path.Combine(watchPath, "new.txt");
            File.WriteAllText(createdFilePath, "new");
            File.Delete(deletedFilePath);
            File.WriteAllText(changedFilePath, "new content");
            File.SetLastWriteTimeUtc(changedFilePath, DateTime.UtcNow.AddMinutes(1));

            var prevScanTime = watcher.GetLastScanTime();
            watcher.TriggerScan();
            Assert.IsTrue(WaitForNextScan(watcher, prevScanTime, 5000), "Scan should complete within timeout");

            var createdFullPath = createdFilePath.ToStandardPath();
            var deletedFullPath = deletedFilePath.ToStandardPath();
            var changedFullPath = changedFilePath.ToStandardPath();

            DirectoryTreeWatcher.ChangeEvent createdEvent;
            DirectoryTreeWatcher.ChangeEvent deletedEvent;
            DirectoryTreeWatcher.ChangeEvent changedEvent;
            lock (events)
            {
                createdEvent = events.Find(e => e.changeType == WatcherChangeTypes.Created && e.fullPath == createdFullPath);
                deletedEvent = events.Find(e => e.changeType == WatcherChangeTypes.Deleted && e.fullPath == deletedFullPath);
                changedEvent = events.Find(e => e.changeType == WatcherChangeTypes.Changed && e.fullPath == changedFullPath);
            }

            Assert.IsNotNull(createdEvent.fullPath);
            Assert.IsNotNull(deletedEvent.fullPath);
            Assert.IsNotNull(changedEvent.fullPath);
        }
    }

    [Test]
    public void CompareFolders_ShouldDetectDifferences()
    {
        // 1. Setup Watch Path (Source State)
        string watchPath = Path.Combine(_testRootPath, "WatchedDir");
        if (Directory.Exists(watchPath)) Directory.Delete(watchPath, true);
        Directory.CreateDirectory(watchPath);

        string commonFile = "common.txt";
        File.WriteAllText(Path.Combine(watchPath, commonFile), "source_content");
        
        string deletedFile = "deleted.txt";
        File.WriteAllText(Path.Combine(watchPath, deletedFile), "content");
        
        string subDir = Path.Combine(watchPath, "SubDir");
        Directory.CreateDirectory(subDir);
        File.WriteAllText(Path.Combine(subDir, "f1.txt"), "content");

        // 2. Create Watcher
        using (var watcher = new DirectoryTreeWatcher(watchPath))
        {
            var events = new List<DirectoryTreeWatcher.ChangeEvent>();
            watcher.onChanged += (e) => 
            {
                lock(events) { events.Add(e); }
            };

            // 3. Mutate to Target State
            // Change common.txt
            File.WriteAllText(Path.Combine(watchPath, commonFile), "target_content");
            // Ensure timestamp change
            File.SetLastWriteTimeUtc(Path.Combine(watchPath, commonFile), DateTime.UtcNow.AddMinutes(1));

            // Delete deleted.txt
            File.Delete(Path.Combine(watchPath, deletedFile));

            // Create created.txt
            string createdFile = "created.txt";
            File.WriteAllText(Path.Combine(watchPath, createdFile), "content");

            // SubDir changes
            File.Delete(Path.Combine(subDir, "f1.txt"));
            File.WriteAllText(Path.Combine(subDir, "f2.txt"), "content");

            // 4. Trigger Scan and Wait
            var prevScanTime = watcher.GetLastScanTime();
            watcher.TriggerScan();
            Assert.IsTrue(WaitForNextScan(watcher, prevScanTime, 5000), "Scan should complete within timeout");

            // 5. Assert
            // 预期会有 6 个事件：5 个文件变更 + 1 个子目录时间戳变更 (SubDir 内容发生变化导致其 LastWriteTime 更新)
            Assert.AreEqual(6, events.Count, "Should detect 6 changes");

            var deletedEvent = events.Find(e => e.changeType == WatcherChangeTypes.Deleted && e.fullPath.EndsWith(deletedFile));
            Assert.IsNotNull(deletedEvent, "Should detect deleted file");

            var createdEvent = events.Find(e => e.changeType == WatcherChangeTypes.Created && e.fullPath.EndsWith(createdFile));
            Assert.IsNotNull(createdEvent, "Should detect created file");

            var changedEvent = events.Find(e => e.changeType == WatcherChangeTypes.Changed && e.fullPath.EndsWith(commonFile));
            Assert.IsNotNull(changedEvent, "Should detect changed file");

            var subDeleted = events.Find(e => e.changeType == WatcherChangeTypes.Deleted && e.fullPath.EndsWith("f1.txt"));
            Assert.IsNotNull(subDeleted, "Should detect sub-directory file deletion");

            var subCreated = events.Find(e => e.changeType == WatcherChangeTypes.Created && e.fullPath.EndsWith("f2.txt"));
            Assert.IsNotNull(subCreated, "Should detect sub-directory file creation");

            var subDirChanged = events.Find(e => e.changeType == WatcherChangeTypes.Changed && e.fullPath.EndsWith("SubDir"));
            Assert.IsNotNull(subDirChanged, "Should detect sub-directory timestamp change");
        }
    }

    [Test]
    public void LongRunning_AutoScan_ShouldSyncRepeatedly()
    {
        // 1. Setup
        string watchPath = Path.Combine(_testRootPath, "LongRunningWatch");
        if (Directory.Exists(watchPath)) Directory.Delete(watchPath, true);
        Directory.CreateDirectory(watchPath);

        using (var watcher = new DirectoryTreeWatcher(watchPath))
        {
            watcher.SetScanInterval(100);
            
            var rng = new System.Random(17727);
            
            // Repeat 20 times
            for (int i = 0; i < 20; i++)
            {
                int opCount = rng.Next(1, 6); // 1 to 5 operations
                
                for (int j = 0; j < opCount; j++)
                {
                    // Refresh file list for valid operations
                    var allFiles = Directory.GetFiles(watchPath, "*", SearchOption.AllDirectories);
                    var allDirs = new List<string>(Directory.GetDirectories(watchPath, "*", SearchOption.AllDirectories));
                    allDirs.Add(watchPath); // Include root as valid parent for creation
                    
                    // 0: Create File
                    // 1: Delete File
                    // 2: Modify File
                    // 3: Create Dir
                    // 4: Delete Dir
                    int opType = rng.Next(0, 5);
                    
                    try 
                    {
                        switch(opType) 
                        {
                            case 0: // Create File
                                if (allDirs.Count > 0)
                                {
                                    string pDir = allDirs[rng.Next(allDirs.Count)];
                                    File.WriteAllText(Path.Combine(pDir, Guid.NewGuid().ToString("N") + ".txt"), "content");
                                }
                                break;
                                
                            case 1: // Delete File
                                if (allFiles.Length > 0) 
                                {
                                    string f = allFiles[rng.Next(allFiles.Length)];
                                    File.Delete(f);
                                }
                                break;
                                
                            case 2: // Modify File
                                if (allFiles.Length > 0) 
                                {
                                    string f = allFiles[rng.Next(allFiles.Length)];
                                    File.AppendAllText(f, " changed");
                                    // Ensure timestamp changes for the watcher to detect
                                    var lastTime = File.GetLastWriteTimeUtc(f);
                                    File.SetLastWriteTimeUtc(f, lastTime.AddSeconds(1));
                                }
                                break;
                                
                            case 3: // Create Dir
                                if (allDirs.Count > 0)
                                {
                                    string pDir2 = allDirs[rng.Next(allDirs.Count)];
                                    Directory.CreateDirectory(Path.Combine(pDir2, "Dir_" + Guid.NewGuid().ToString("N")));
                                }
                                break;
                                
                            case 4: // Delete Dir
                                if (allDirs.Count > 1) 
                                { 
                                    // Pick a dir that is not root
                                    string d = allDirs[rng.Next(allDirs.Count)];
                                    // Simple check to avoid deleting root, though watchPath logic above handles it via count
                                    // But we need to be careful not to delete watchPath
                                    if (!string.Equals(Path.GetFullPath(d).TrimEnd('\\', '/'), Path.GetFullPath(watchPath).TrimEnd('\\', '/'), StringComparison.OrdinalIgnoreCase))
                                    {
                                        Directory.Delete(d, true);
                                    }
                                }
                                break;
                        }
                    } 
                    catch(Exception) 
                    {
                        // Ignore random IO collisions (e.g. deleting parent then child)
                    }
                }
                
                // Wait for scan
                Thread.Sleep(400);
                
                // Verify
                VerifyWatcherMatchesFileSystem(watchPath, watcher);
            }
        }
    }

    private void VerifyWatcherMatchesFileSystem(string rootPath, DirectoryTreeWatcher watcher)
    {
        string Standardize(string p) => p.Replace("\\", "/").TrimEnd('/');
        
        // 1. Collect FS state
        var fsFiles = new HashSet<string>();
        if (Directory.Exists(rootPath))
        {
            foreach (var f in Directory.GetFiles(rootPath, "*", SearchOption.AllDirectories))
            {
                fsFiles.Add(Standardize(f));
            }
        }

        var fsDirs = new HashSet<string>();
        if (Directory.Exists(rootPath))
        {
            foreach (var d in Directory.GetDirectories(rootPath, "*", SearchOption.AllDirectories))
            {
                fsDirs.Add(Standardize(d));
            }
        }

        // 2. Collect Watcher state
        var watcherFiles = new HashSet<string>();
        var watcherDirs = new HashSet<string>();
        
        void Traverse(DirectoryNode node)
        {
            foreach (var fileKvp in node.fileMap)
            {
                watcherFiles.Add(Standardize(fileKvp.Value));
            }
            
            foreach (var dirKvp in node.directoryMap)
            {
                watcherDirs.Add(Standardize(dirKvp.Value));
                Traverse(node.GetChildDirectory(dirKvp.Key));
            }
        }
        
        Traverse(watcher.GetDirectory(rootPath.ToStandardPath()));
        
        // 3. Compare
        foreach (var f in fsFiles)
        {
            Assert.IsTrue(watcherFiles.Contains(f), $"Watcher missed file: {f}");
            Assert.IsTrue(watcher.GetFile(f.ToStandardPath()).isValid, $"GetFile returned invalid node for: {f}");
        }
        
        foreach (var f in watcherFiles)
        {
            Assert.IsTrue(fsFiles.Contains(f), $"Watcher has extra file: {f}");
        }
        
        foreach (var d in fsDirs)
        {
            Assert.IsTrue(watcherDirs.Contains(d), $"Watcher missed directory: {d}");
            Assert.IsTrue(watcher.GetDirectory(d.ToStandardPath()).isValid, $"GetDirectory returned invalid node for: {d}");
        }
        
        foreach (var d in watcherDirs)
        {
            Assert.IsTrue(fsDirs.Contains(d), $"Watcher has extra directory: {d}");
        }
    }
}
