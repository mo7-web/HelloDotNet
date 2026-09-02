#!/usr/bin/env node

/* 
真正跨平台的 Git 自动提交脚本
基于 execa 驱动，零 Shell 依赖，Windows/macOS/Linux 完全通用
*/

import { execa } from 'execa';
import process from 'node:process';

// 1. 校验系统是否安装了 Git
try {
  await execa('git', ['--version']);
} catch {
  console.error('Error: 未在系统中检测到 Git，请确保已安装并配置到了环境变量 PATH 中。');
  process.exit(1);
}

// 2. 校验提交信息参数
const desc = process.argv[2];
if (!desc || !desc.trim()) {
  console.error('Error: 请提供有效的提交描述信息。');
  console.error('示例: pnpm run git-sync "feat: 调整脚本"');
  process.exit(1);
}

console.info('当前工作目录:', process.cwd());

try {
  // 3. 设置 Git 本地配置（参数以数组形式传递，绝对安全）
  await execa('git', ['config', 'core.ignorecase', 'false']);
  await execa('git', ['config', 'core.filemode', 'false']);
  await execa('git', ['config', 'pull.rebase', 'false']);

  // 4. 拉取远端更新（stdio: 'inherit' 让 Git 的原生进度直接实时打在当前终端）
  console.info('正在拉取远端更新...');
  await execa('git', ['pull'], { stdio: 'inherit' });

  // 5. 暂存所有变更
  console.info('正在暂存文件...');
  await execa('git', ['add', '.']);

  // 6. 执行提交（设置 reject: false，允许非 0 退出码不直接抛出异常崩溃）
  console.info(`正在提交变更: "${desc}"...`);
  const commitResult = await execa('git', ['commit', '-m', desc], { reject: false });

  // 7. 处理提交状态与推送
  if (commitResult.exitCode === 0) {
    console.info('正在推送到远端仓库...');
    await execa('git', ['push'], { stdio: 'inherit' });
    console.info(' 同步完成并已推送到远端！');
  } else if (
    commitResult.exitCode === 1 &&
    (commitResult.stdout.includes('nothing to commit') ||
      commitResult.stderr.includes('nothing to commit'))
  ) {
    console.info('ℹ️ 本地无需提交 (nothing to commit)，工作区干净。');
  } else {
    console.error(`❌ Git commit 失败 (退出码: ${commitResult.exitCode})`);
    if (commitResult.stdout) console.error(commitResult.stdout);
    if (commitResult.stderr) console.error(commitResult.stderr);
    process.exit(1);
  }
} catch (err) {
  console.error('❌ 执行过程中遇到不可恢复的异常:', err.message);
  process.exit(1);
}
