# ProfilerScope - RAII Profiler 工具

## 概述

`ProfilerScope` 是一个 RAII (Resource Acquisition Is Initialization) 风格的 Unity Profiler 工具，提供自动的 `BeginSample`/`EndSample` 管理.

## 特性

- ✅ **RAII 模式**: 自动管理 Profiler 生命周期
- ✅ **条件编译**: 只在开发版本中启用，发布版本零开销
- ✅ **性能优化**: 提供缓存版本避免重复字符串分配
- ✅ **多种用法**: 支持基本用法、对象上下文、扩展方法等
- ✅ **嵌套支持**: 支持嵌套的 Profiler 作用域

## 基本用法

### 1. 简单用法

```csharp
using (new ProfilerScope("MyOperation"))
{
    // 你的代码
    DoSomeWork();
}
```

### 2. 带对象上下文

```csharp
using (new ProfilerScope("TransformOperation", transform))
{
    // 你的代码
    DoTransformWork(transform);
}
```

### 3. 缓存版本（性能优化）

```csharp
// 对于频繁使用的 Profiler 名称，使用缓存版本
using (new CachedProfilerScope("FrequentOperation"))
{
    // 你的代码
    DoFrequentWork();
}
```

### 4. 扩展方法

```csharp
using (ProfilerScopeExtensions.BeginProfiler("ExtensionMethod"))
{
    // 你的代码
    DoExtensionWork();
}

// 缓存版本
using (ProfilerScopeExtensions.BeginCachedProfiler("CachedExtension"))
{
    // 你的代码
    DoCachedExtensionWork();
}
```

## 高级用法

### 嵌套 Profiler 作用域

```csharp
using (new ProfilerScope("OuterOperation"))
{
    DoOuterWork();
    
    using (new ProfilerScope("InnerOperation"))
    {
        DoInnerWork();
    }
    
    DoMoreOuterWork();
}
```

### 条件 Profiling

```csharp
if (shouldProfile)
{
    using (new ProfilerScope("ConditionalOperation"))
    {
        DoConditionalWork();
    }
}
else
{
    DoConditionalWork();
}
```

### 性能关键循环

```csharp
for (int i = 0; i < 1000; i++)
{
    // 在性能关键循环中使用缓存版本
    using (new CachedProfilerScope("LoopIteration"))
    {
        DoLoopWork(i);
    }
}
```

## 性能考虑

### 条件编译

所有 Profiler 调用都被 `#if UNITY_EDITOR || DEVELOPMENT_BUILD` 包围，这意味着：

- **编辑器**: 总是启用
- **开发版本**: 启用
- **发布版本**: 完全禁用，零开销

### 字符串缓存

`CachedProfilerScope` 使用内部字符串缓存来避免重复的字符串分配：

```csharp
// 第一次调用会缓存字符串
using (new CachedProfilerScope("MyOperation")) { }

// 后续调用会使用缓存的字符串
using (new CachedProfilerScope("MyOperation")) { }
```

### 内存管理

如果需要清理缓存（通常不需要），可以调用：

```csharp
ProfilerScopeCache.ClearCache();
```

## 最佳实践

1. **使用基本版本**: 对于一次性或偶尔使用的 Profiler 名称
2. **使用缓存版本**: 对于频繁使用的 Profiler 名称（如循环中）
3. **合理嵌套**: 避免过深的嵌套层级
4. **命名规范**: 使用清晰、描述性的名称
5. **条件使用**: 在性能关键路径中考虑条件使用

## 注意事项

- 确保 `using` 语句正确使用，避免忘记 `using` 关键字
- 在异常情况下，`Dispose` 方法仍会被调用，确保 Profiler 正确结束
- 缓存版本适合频繁使用的场景，一次性使用建议使用基本版本
- 发布版本中所有 Profiler 调用都会被移除，无需担心性能影响
