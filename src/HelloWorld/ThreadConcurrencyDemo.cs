namespace MyDemo.Threading;

using System;
using System.Threading;
using System.Threading.Channels;

public static class ThreadConcurrencyDemo
{
    // C# 13+ / .NET 9+ 原生专用锁对象（禁用隐式 new()，显式写明 new Lock()）
    // 对标 Go: mu sync.Mutex
    private static readonly Lock ThreadLock = new Lock();

    // 共享静态计数器
    private static int unsafeCounter;
    private static int atomicCounter;

    public static async Task RunAsync()
    {
        Console.WriteLine("=== [多线程本质：验证 C# 真实的物理线程切换] ===");
        await ThreadConcurrencyDemo.DemonstrateThreadSwitchingAsync();

        Console.WriteLine("\n=== [多线程数据传递：System.Threading.Channels (对标 Go chan)] ===");
        await ThreadConcurrencyDemo.DemonstrateChannelsAsync();

        Console.WriteLine("\n=== [多线程竞态与同步：Interlocked vs 现代 Lock] ===");
        await ThreadConcurrencyDemo.DemonstrateSynchronizationAsync();
    }

    // ==========================================
    // 1. 实锤 C# 的多线程切换（打破 JS 单线程直觉）
    // ==========================================
    private static async Task DemonstrateThreadSwitchingAsync()
    {
        // 插值字符串内部直接填变量名，消灭 .ToString() 带来的多余堆内存分配
        Console.WriteLine($"[Thread Audit] await 前物理线程 ID: {Environment.CurrentManagedThreadId}");

        // 模拟非阻塞 IO，底层将控制权还给线程池工作调度器
        await Task.Delay(50);

        // await 唤醒后大概率处于线程池的另外一个物理线程
        Console.WriteLine($"[Thread Audit] await 后物理线程 ID: {Environment.CurrentManagedThreadId} (证明 C# 异步天生处于多线程环境)");
    }

    // ==========================================
    // 2. 现代 C# 推荐通道传值：Channel<T> (100% 对标 Go Channel)
    // ==========================================
    private static async Task DemonstrateChannelsAsync()
    {
        // 显式声明 Channel<string>，拒绝盲盒 var 推导
        // 对标 Go: ch := make(chan string, 5)
        Channel<string> channel = Channel.CreateBounded<string>(new BoundedChannelOptions(capacity: 5)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleWriter = false,
            SingleReader = false
        });

        // 启动生产者线程任务（对标 Go: go func() { ... }()）
        Task producerTask = Task.Run(async () =>
        {
            for (int i = 1; i <= 3; i++)
            {
                string msg = $"Telemetry-Event-{i}";
                // 写入通道（对标 Go: ch <- msg）
                await channel.Writer.WriteAsync(msg);
                Console.WriteLine($"[Producer Thread {Environment.CurrentManagedThreadId}] Pushed: {msg}");
                await Task.Delay(20);
            }

            // 完成发送，关闭通道写入端（对标 Go: close(ch)）
            channel.Writer.Complete();
        });

        // 启动消费者任务
        Task consumerTask = Task.Run(async () =>
        {
            // ReadAllAsync: 优雅消费通道，读空且关闭后自动跳出循环（对标 Go: for msg := range ch）
            // 显式声明 string item，拒绝 var 盲盒
            await foreach (string item in channel.Reader.ReadAllAsync())
            {
                Console.WriteLine($"[Consumer Thread {Environment.CurrentManagedThreadId}] Consumed: {item}");
            }
        });

        // 协同等待生产者与消费者退出（对标 Go: sync.WaitGroup）
        await Task.WhenAll(producerTask, consumerTask);
    }

    // ==========================================
    // 3. 共享状态同步：Interlocked（原子操作）与 Lock
    // ==========================================
    private static async Task DemonstrateSynchronizationAsync()
    {
        // 静态成员严格携带宿主类名前缀
        ThreadConcurrencyDemo.unsafeCounter = 0;
        ThreadConcurrencyDemo.atomicCounter = 0;

        const int taskCount = 10;
        const int incrementsPerTask = 1000;
        Task[] tasks = new Task[taskCount];

        for (int i = 0; i < taskCount; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < incrementsPerTask; j++)
                {
                    // 场景 A：无保护裸写（引发数据竞态 Data Race）
                    ThreadConcurrencyDemo.unsafeCounter++;

                    // 场景 B：硬件级原子操作
                    // 底层编译为单个 CPU 级别的 LOCK XADD 汇编指令（对标 Go: atomic.AddInt32）
                    Interlocked.Increment(ref ThreadConcurrencyDemo.atomicCounter);

                    // 场景 C：现代原生 Lock（进入临界区，对标 Go: mu.Lock() / defer mu.Unlock()）
                    lock (ThreadConcurrencyDemo.ThreadLock)
                    {
                        // 临界区代码，多线程强互斥排队进入
                    }
                }
            });
        }

        await Task.WhenAll(tasks);

        Console.WriteLine($"[竞态导致数据丢失] Unsafe Counter: {ThreadConcurrencyDemo.unsafeCounter} (理论应为 10000)");
        Console.WriteLine($"[原子操作安全保障] Atomic Counter: {ThreadConcurrencyDemo.atomicCounter} (理论应为 10000)");
    }
}
