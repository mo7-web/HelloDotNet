# HelloDotNet

## 目录说明

- `obj` 编译过程中的临时半成品
- `bin` 最终二进制成品
- `publish` 目录下的文件是编译器生成的发布文件

## 常用开发命令速查

```bash
# 诊断环境与 SDK、运行时路径
dotnet --info

# 运行当前目录项目
dotnet run
# 运行指定子项目
dotnet run --project ./HelloWorld
dotnet run --project ./NameSpaceDemo

# 向程序传递启动参数（使用 -- 分隔）
dotnet run -- --port 8080 --debug

# 热重载开发模式（修改代码自动生效，无需重启）
dotnet watch

# 仅执行编译与类型检查（零运行）
dotnet build

# 清理 bin 和 obj 缓存
dotnet clean

# 代码自动化规范格式化 (等同于 prettier --write)
dotnet format

# 包与依赖管理
# 安装第三方包 (pnpm add)
dotnet add package <NuGet包名>
# 卸载第三方包 (pnpm remove)
dotnet remove package <NuGet包名>
# 引用本地其他模块 (Monorepo 内部依赖)
dotnet add reference <子项目.csproj>
# 还原并下载所有依赖 (pnpm install)
dotnet restore

# 自动化测试
dotnet test                         # 运行全部单测
dotnet watch test                   # 监听模式运行单测

```

## 全平台 Native AOT 极速编译矩阵

> **前置条件：** `.csproj` 中已开启 `<PublishAot>true</PublishAot>`
> **核心参数：** `-c Release`（开启最高性能优化）、`-r`（指定目标平台 RID）、`-o`（指定输出目录）

### Windows 平台

(在 Windows PC 宿主机上执行)

```bash
# Windows x64 (主流 Intel/AMD 电脑)
# [环境要求]: Visual Studio 安装 "使用 C++ 的桌面开发"
dotnet publish -c Release -r win-x64 -o ./publish/win_x64

# Windows ARM64 (高通骁龙 X Elite / Surface Copilot+ PC)
# [环境要求]: 在 VS 安装器中额外勾选 "MSVC ARM64/ARM64EC 生成工具"
dotnet publish -c Release -r win-arm64 -o ./publish/win_arm64

```

### Linux 平台

```bash
# 1. Linux x64 (主流 Linux 云服务器 / 桌面)
# [编译环境]: Windows 上的 Hyper-V (x64 Ubuntu)
# [环境依赖]: sudo apt install -y clang zlib1g-dev
dotnet publish -c Release -r linux-x64 -o ./publish/linux_x64

# 2. Linux ARM64 (树莓派 4/5、AWS Graviton、ARM 云服务器)
# [编译环境]: Mac M1/M2/M3/M4 上的 Linux ARM64 虚拟机 (OrbStack / Docker / UTM)
# [环境依赖]: sudo apt install -y clang zlib1g-dev
dotnet publish -c Release -r linux-arm64 -o ./publish/linux_arm64

```

> `glibc` “向前兼容”铁律，建议使用 Ubuntu 22.04 LTS 作为编译宿主

### macOS 平台

(在 Apple Silicon Mac 宿主机上执行，需安装 xcode-select --install)

```bash
# 1. macOS ARM64 (Apple Silicon M系列芯片 - 原生编译)
dotnet publish -c Release -r osx-arm64 -o ./publish/mac_arm64

# 2. macOS x64 (老款 Intel 芯片 Mac - 原生交叉编译)
dotnet publish -c Release -r osx-x64 -o ./publish/mac_x64

# 3. 合并为 Universal 2 通用二进制 (双架构合一，任意 Mac 均可原生秒开)
mkdir -p ./publish/mac_universal
lipo -create -output ./publish/mac_universal/MyApp ./publish/mac_arm64/MyApp ./publish/mac_x64/MyApp

```

## 官方权威参考

- C# 官方语言文档：<https://learn.microsoft.com/zh-cn/dotnet/csharp/>
- .NET CLI 官方命令参考：<https://learn.microsoft.com/zh-cn/dotnet/core/tools/>
- Native AOT 官方部署指南：<https://learn.microsoft.com/zh-cn/dotnet/core/deploying/native-aot/>
