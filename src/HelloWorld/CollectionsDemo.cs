namespace MyDemo.Collections;

using System.Globalization;

public static class CollectionsDemo
{
    public static void Run()
    {
        Console.WriteLine("=== [3.1 集合表达式与切片内存实操] ===");
        DemonstrateCollectionExpressionsAndSlices();

        Console.WriteLine("\n=== [3.2 核心集合容器实操] ===");
        DemonstrateCoreCollections();

        Console.WriteLine("\n=== [3.3 声明式数据流 LINQ 实操] ===");
        DemonstrateLinq();
    }

    // ==========================================
    // 3.1 集合表达式、倒数索引与切片差异
    // ==========================================
    private static void DemonstrateCollectionExpressionsAndSlices()
    {
        // 1. C# 12+ 统一集合表达式 [JS: [1, 2, 3] / Go: []int{1, 2, 3}]
        int[] partA = [1, 2, 3];
        int[] partB = [4, 5, 6];

        // 展开操作符 (Spread Operator) [JS: [...partA, 99, ...partB]]
        int[] merged = [.. partA, 99, .. partB];
        Console.WriteLine($"Merged Length: {merged.Length}, Middle: {merged[3]}");

        // 2. 现代倒数索引 (Index from end: ^) [JS: at(-1) / Go: len(s)-1]
        int lastItem = merged[^1];  // 等价于 merged[merged.Length - 1]，即 6
        int secondLast = merged[^2]; // 5
        Console.WriteLine($"Last: {lastItem}, 2nd Last: {secondLast}");

        // 3. 陷阱：普通数组切片 (触发堆分配与深拷贝)
        int[] heapCopiedSlice = merged[1..4]; // 取索引 1, 2, 3 构成新数组 [JS: slice(1, 4)]
        Console.WriteLine($"Heap Copied Slice Len: {heapCopiedSlice.Length}");

        // 4. 极致性能：Span 切片 (完全等价于 Go 切片，纯栈引用指针，零拷贝，Native AOT 核心)
        ReadOnlySpan<int> zeroCopySlice = merged.AsSpan()[1..4]; // [Go: merged[1:4]]
        Console.WriteLine($"Zero-copy Span Slice Len: {zeroCopySlice.Length}, First: {zeroCopySlice[0]}");
    }

    // ==========================================
    // 3.2 核心集合容器 (List, Dictionary, HashSet)
    // ==========================================
    private static void DemonstrateCoreCollections()
    {
        // 1. 动态数组 List<T> [Go: []T 动态扩容切片 / JS: Array]
        // 性能技巧：已知数据量时指定 Capacity 避免扩容复制 [Go: make([]int, 0, 10)]
        List<string> frameworks = new(capacity: 10) { "ASP.NET Core", "React" };
        frameworks.Add("Vue");
        frameworks.AddRange(["Svelte", "Next.js"]); // 批量添加
        Console.WriteLine($"List Count: {frameworks.Count}, First: {frameworks[0]}");

        // 2. 哈希字典 Dictionary<TKey, TValue> [Go: map[K]V / JS: Map]
        // 也支持集合表达式初始化
        Dictionary<string, int> memoryUsage = new()
        {
            ["Kernel"] = 64,
            ["CliApp"] = 12
        };
        memoryUsage["CliApp"] = 15; // 覆盖赋值

        // 安全取值 (类比 Go: val, ok := m[key])
        if (memoryUsage.TryGetValue("CliApp", out int usageMb))
        {
            Console.WriteLine($"CliApp Memory: {usageMb.ToString(CultureInfo.InvariantCulture)} MB");
        }

        // 3. 无序唯一集 HashSet<T> [JS: Set / Go: map[T]struct{}]
        HashSet<string> uniqueTags = ["backend", "performance", "aot"];
        bool addedNew = uniqueTags.Add("aot"); // 重复添加，返回 false
        Console.WriteLine($"HashSet Count: {uniqueTags.Count}, Added duplicate: {addedNew}");
    }

    // ==========================================
    // 3.3 声明式数据查询 (LINQ 入门)
    // ==========================================
    private static void DemonstrateLinq()
    {
        // 准备只读数据源
        List<int> rawScores = [45, 82, 95, 60, 30, 88, 100, 74];

        // 链式声明式查询
        // [JS: scores.filter(...).sort(...).map(...)]
        var processedScores = rawScores
            .Where(score => score >= 60)              // 过滤及格分数 [JS: .filter()]
            .OrderByDescending(score => score)        // 降序排序 [JS: .sort((a, b) => b - a)]
            .Select(score => $"Ranked-{score}")       // 映射转换 [JS: .map()]
            .ToList();                                // 立即求值，物化为 List<string>

        Console.WriteLine("Passing Scores (Ranked):");
        foreach (var item in processedScores)
        {
            Console.Write($"{item} ");
        }
        Console.WriteLine();

        // 谓词断言测试 [JS: .some() / .every()]
        bool hasPerfectScore = rawScores.Any(score => score == 100); // [JS: .some()]
        bool allPassed = rawScores.All(score => score >= 60);         // [JS: .every()]

        Console.WriteLine($"Any Perfect (100): {hasPerfectScore}");
        Console.WriteLine($"All Passed (>=60): {allPassed}");
    }
}
