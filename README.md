# dotnet 语法基础演练

## 常用命令

```bash
# 运行当前目录的程序。
dotnet run
dotnet run ./xx
dotnet run -- --port 8080 --debug

# 快速进行类型与语法检查，不启动程序
dotnet build

# 自动寻找项目中的测试用例，并运行输出结果。
dotnet test

# 向当前项目安装第三方库。
dotnet add package xx
dotnet remove package Newtonsoft.Json

# 发布为 Windows 绿色单文件（自包含运行库，玩家电脑无需安装 .NET，双击即跑）
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./dist

# Native AOT 极致体积与零延迟秒开打包（纯机器码）
dotnet publish -c Release -r win-x64 -p:PublishAot=true -o ./dist_aot

# 当项目发生诡异的编译错误或缓存冲突时，清空 bin/ 和 obj/ 目录。
dotnet clean

# 排查 SDK 版本、运行库路径以及 DOTNET_ROOT 是否配置正确。
dotnet --info

# 修改 AXAML 界面或 C# 代码后自动热更新，无需反复重启。
dotnet watch
```

## 官网地址
