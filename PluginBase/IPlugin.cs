namespace DllBase
{
    public interface IPlugin
    {
        public string Name { get; }         // dll名
        public string AppName { get; }      // アプリケーション名
        public string Version { get; }      // バージョン

        /// <summary>
        /// ロード時に初期化を行う関数
        /// </summary>
        public void Initialize();

        /// <summary>
        /// ログの処理を行う関数
        /// </summary>
        public void ParseLog(string line);
    }
}
