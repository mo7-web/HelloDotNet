export default {
  plugins: ['@prettier/plugin-xml', 'prettier-plugin-ini'],
  iniSpaceAroundEquals: true, // ini 文件中等号两边添加空格
  bracketSameLine: true, // JSX 中的 > 是否另起一行
  singleQuote: true, // 使用单引号
  printWidth: 100, // 每行代码长度
  semi: true, // 句尾添加分号
};
