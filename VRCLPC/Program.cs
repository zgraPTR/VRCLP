using VRCLPC.Core;

namespace VRCLPC
{
    internal class Program
    {
        private static DllLoader dllLoader = new DllLoader(); // dllローダー

        /// <summary>
        /// メインエントリポイント
        /// </summary>
        /// <param name="args">コマンドライン引数</param>
        static void Main(string[] args)
        {
            InitializeLogWatcher();                     // ログ監視の初期化 & 開始
            WaitConsoleReadLine();                      // コンソール入力を待機
        }

        /// <summary>
        /// ログ監視の初期化と開始する関数
        /// </summary>
        private static void InitializeLogWatcher()
        {
            LogWatcher logWatcher = new LogWatcher();   // ログ監視インスタンス
            logWatcher.RaisingEvents = true;            // ログ監視イベントを有効化
        }

        /// <summary>
        /// コンソール入力を待機する関数
        /// </summary>
        private static void WaitConsoleReadLine()
        {
            dllLoader.Load();                       // dllロード

            Console.WriteLine("");                      // コンソールにメッセージを表示
            while (true)
            {                                           // コンソール入力を待機
                string? input = Console.ReadLine();     // コンソールからの入力を取得
            }
        }

    }
}