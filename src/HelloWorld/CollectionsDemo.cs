namespace MyDemo.Collections;

using System;
using System.Collections.Generic;
using System.Globalization;

public static class CollectionsDemo
{
    public static void Run()
    {
        Console.WriteLine("=== [3.1 集合表达式与切片内存实操] ===");
        CollectionsDemo.DemonstrateCollectionExpressionsAndSlices();

        Console.WriteLine("\n=== [3.2 核心集合容器实操] ===");
        CollectionsDemo.DemonstrateCoreCollections();

        Console.WriteLine("\n=== [3.3 声明式数据处理（扁平平铺展开）] ===");
        CollectionsDemo.DemonstrateDataProcessing();
    }

    // ==========================================
    // 3.1 集合表达式、倒数索引与切片差异
    // ==========================================
    private static void DemonstrateCollectionExpressionsAndSlices()
    {
        // 1. 统一集合表达式与展开操作符 (Spread Operator)
        int[] partA = [1, 2, 3];
        int[] partB = [4, 5, 6];

        int[] merged = [.. partA, 99, .. partB];
        Console.WriteLine($"Merged Length: {merged.Length}, Middle: {merged[3]}");

        // 2. 现代倒数索引 (Index from end: ^)
        int lastItem = merged[^1];
        int secondLast = merged[^2];
        Console.WriteLine($"Last: {lastItem}, 2nd Last: {secondLast}");

        // 3. 普通数组切片：触发新数组分配与深拷贝（有 GC 堆压力）
        int[] heapCopiedSlice = merged[1..4];
        Console.WriteLine($"Heap Copied Slice Len: {heapCopiedSlice.Length}");

        // 4. Span 切片：纯栈上指针与长度结构体，零堆拷贝（Native AOT 核心基石，对标 Go 切片）
        ReadOnlySpan<int> zeroCopySlice = merged.AsSpan()[1..4];
        Console.WriteLine($"Zero-copy Span Slice Len: {zeroCopySlice.Length}, First: {zeroCopySlice[0]}");
    }

    // ==========================================
    // 3.2 核心集合容器 (List, Dictionary, HashSet)
    // ==========================================
    private static void DemonstrateCoreCollections()
    {
        // 1. 动态数组 List<T>：显式类型实例化，禁止 new()
        List<string> frameworks = new List<string>(capacity: 10) { "ASP.NET Core", "React" };
        frameworks.Add("Vue");
        frameworks.AddRange(["Svelte", "Next.js"]);
        Console.WriteLine($"List Count: {frameworks.Count}, First: {frameworks[0]}");

        // 2. 哈希字典 Dictionary<TKey, TValue>：显式类型实例化
        Dictionary<string, int> memoryUsage = new Dictionary<string, int>()
        {
            ["Kernel"] = 64,
            ["CliApp"] = 12
        };
        memoryUsage["CliApp"] = 15;

        // 安全取值 (类比 Go: val, ok := m[key])
        if (memoryUsage.TryGetValue("CliApp", out int usageMb))
        {
            Console.WriteLine($"CliApp Memory: {usageMb.ToString(CultureInfo.InvariantCulture)} MB");
        }

        // 3. 无序唯一集 HashSet<T>
        HashSet<string> uniqueTags = ["backend", "performance", "aot"];
        bool addedNew = uniqueTags.Add("aot");
        Console.WriteLine($"HashSet Count: {uniqueTags.Count}, Added duplicate: {addedNew}");
    }

    // ==========================================
    // 3.3 数据处理：平铺展开（彻底消除长链式 LINQ 与盲盒推导）
    // ==========================================
    private static void DemonstrateDataProcessing()
    {
        List<int> rawScores = [45, 82, 95, 60, 30, 88, 100, 74];

        // 1. 重构长链式 LINQ 为平铺 foreach 管道（Go 风格）
        // 显式声明容量，杜绝多次扩容重分配
        List<int> passingScores = new List<int>(capacity: rawScores.Count);
        foreach (int score in rawScores)
        {
            if (score >= 60)
            {
                passingScores.Add(score);
            }
        }

        // 原地排序（In-place），避免 OrderByDescending 创建额外堆包装对象
        passingScores.Sort();
        passingScores.Reverse();

        // 映射格式化
        List<string> processedScores = new List<string>(capacity: passingScores.Count);
        foreach (int score in passingScores)
        {
            processedScores.Add($"Ranked-{score}");
        }

        Console.WriteLine("Passing Scores (Ranked):");
        // 显式声明类型 string，杜绝 foreach (var ...)
        foreach (string item in processedScores)
        {
            Console.Write($"{item} ");
        }
        Console.WriteLine();

        // 2. 状态断言平铺展开：短路循环替代 LINQ Any/All 委托开销
        bool hasPerfectScore = false;
        bool allPassed = true;

        foreach (int score in rawScores)
        {
            if (score == 100)
            {
                hasPerfectScore = true;
            }

            if (score < 60)
            {
                allPassed = false;
            }
        }

        Console.WriteLine($"Any Perfect (100): {hasPerfectScore}");
        Console.WriteLine($"All Passed (>=60): {allPassed}");
    }
}
