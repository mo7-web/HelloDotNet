namespace MyDemo.Threading;

using System.Globalization;
using System.Threading.Channels; // 核心：对标 Go channel 的高性能通道库

public static class ThreadConcurrencyDemo
{
    // C# 13+ / .NET 9+ 引入的全新原生专用锁对象（取代老旧的 object lock，性能更强且防止死锁）
    // [Go: mu sync.Mutex]
    private static readonly Lock ThreadLock = new();

    // 共享计数器
    private static int _unsafeCounter;
    private static int _atomicCounter;

    public static async Task RunAsync()
    {
        Console.WriteLine("=== [多线程本质：验证 C# 真实的物理线程切换] ===");
        await DemonstrateThreadSwitchingAsync();

        Console.WriteLine("\n=== [多线程数据传递：System.Threading.Channels (对标 Go chan)] ===");
        await DemonstrateChannelsAsync();

        Console.WriteLine("\n=== [多线程竞态与同步：Interlocked vs 现代 Lock] ===");
        await DemonstrateSynchronizationAsync();
    }

    // ==========================================
    // 1. 实锤 C# 的多线程切换（打破 JS 单线程直觉）
    // ==========================================
    private static async Task DemonstrateThreadSwitchingAsync()
    {
        // 打印当前执行的物理内核线程 ID
        Console.WriteLine($"[Thread Audit] await 前物理线程 ID: {Environment.CurrentManagedThreadId.ToString(CultureInfo.InvariantCulture)}");

        // 模拟一个非阻塞 IO，底层将控制权还给线程池
        await Task.Delay(50);

        // 观察：await 唤醒后，极大概率已经被调度到了另外一个完全不同的物理线程！
        Console.WriteLine($"[Thread Audit] await 后物理线程 ID: {Environment.CurrentManagedThreadId.ToString(CultureInfo.InvariantCulture)} (证明 C# 异步天生处于多线程环境)");
    }

    // ==========================================
    // 2. 现代 C# 推荐通道传值：Channel<T> (100% 对标 Go Channel)
    // ==========================================
    private static async Task DemonstrateChannelsAsync()
    {
        // 创建一个有界通道（容量为 5），背压（Backpressure）策略为等待
        // [Go: ch := make(chan string, 5)]
        var channel = Channel.CreateBounded<string>(new BoundedChannelOptions(capacity: 5)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleWriter = false,
            SingleReader = false
        });

        // 启动生产者线程任务 (模拟并发推送)
        // [Go: go func() { ... }()]
        var producerTask = Task.Run(async () =>
        {
            for (int i = 1; i <= 3; i++)
            {
                string msg = $"Telemetry-Event-{i.ToString(CultureInfo.InvariantCulture)}";
                // 写入通道 [Go: ch <- msg]
                await channel.Writer.WriteAsync(msg);
                Console.WriteLine($"[Producer Thread {Environment.CurrentManagedThreadId.ToString(CultureInfo.InvariantCulture)}] Pushed: {msg}");
                await Task.Delay(20);
            }

            // 完成发送，关闭通道写入端 [Go: close(ch)]
            channel.Writer.Complete();
        });

        // 启动消费者任务 (跨线程消费)
        // [Go: for msg := range ch { ... }]
        var consumerTask = Task.Run(async () =>
        {
            // ReadAllAsync: 优雅消费通道，通道关闭且读空后自动退出循环
            await foreach (var item in channel.Reader.ReadAllAsync())
            {
                Console.WriteLine($"[Consumer Thread {Environment.CurrentManagedThreadId.ToString(CultureInfo.InvariantCulture)}] Consumed: {item}");
            }
        });

        // 等待生产者与消费者全流程结束 [Go: sync.WaitGroup]
        await Task.WhenAll(producerTask, consumerTask);
    }

    // ==========================================
    // 3. 共享状态同步：Interlocked（原子操作）与 Lock
    // ==========================================
    private static async Task DemonstrateSynchronizationAsync()
    {
        _unsafeCounter = 0;
        _atomicCounter = 0;

        // 开启 10 个并发线程任务，每个任务累加 1,000 次
        const int taskCount = 10;
        const int incrementsPerTask = 1000;
        var tasks = new Task[taskCount];

        for (int i = 0; i < taskCount; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < incrementsPerTask; j++)
                {
                    // 场景 A：无保护裸写（引发数据竞态 Data Race）
                    _unsafeCounter++;

                    // 场景 B：硬件级原子操作 [Go: atomic.AddInt32(&_atomicCounter, 1)]
                    // 编译为单个 CPU 级别的 LOCK XADD 汇编指令，零锁，纳秒级
                    Interlocked.Increment(ref _atomicCounter);

                    // 场景 C：现代 Lock 语法糖
                    // [Go: mu.Lock(); defer mu.Unlock()]
                    lock (ThreadLock)
                    {
                        // 临界区代码，多线程强互斥排队进入
                    }
                }
            });
        }

        await Task.WhenAll(tasks);

        Console.WriteLine($"[竞态导致数据丢失] Unsafe Counter: {_unsafeCounter.ToString(CultureInfo.InvariantCulture)} (理论应为 10000)");
        Console.WriteLine($"[原子操作安全保障] Atomic Counter: {_atomicCounter.ToString(CultureInfo.InvariantCulture)} (理论应为 10000)");
    }
}
