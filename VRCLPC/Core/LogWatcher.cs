using VRCLPC.Utils;

namespace VRCLPC.Core
{
    internal class LogWatcher
    {
        private string logPath = string.Empty;                 // 読み込み中のログ名
        private long txtPosition = 0;               // 読み込み済の内容の位置

        private FileSystemWatcher watcher;      // ファイルの監視を行うクラス

        public LogWatcher()
        {                       // コンストラクター
            watcher = new FileSystemWatcher(GlobalUtils.LogFolderPath)
            {
                NotifyFilter = NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Size,                      // 通知のフィルターを設定

                Filter = "output_*.txt",        // ログファイルをフィルターに設定
            };                                  // クラスインスタンス生成

            watcher.Changed += OnChanged;       // ファイル生成時のイベントハンドラ
        }

        public bool RaisingEvents
        {                                       // イベントを発火するかどうか
            get => watcher.EnableRaisingEvents;
            set => watcher.EnableRaisingEvents = value;
        }

        /// <summary>
        /// ファイル内容 変更時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (logPath != e.FullPath)
            {                                   // 読み込んでいたファイルパスと別
                logPath = e.FullPath;             // 新しいログのファイルパスを指定
                txtPosition = 0;                // 読み込み済の位置をリセット
            }

            ReadNewLines();      // ログを読み込む
        }

        /// <summary>
        /// ログのファイルの内容を一行ずつ読み込む
        /// </summary>
        private void ReadNewLines()
        {
            using FileStream fs = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            // ファイルストリームを開く
            using StreamReader sr = new StreamReader(fs);   // 内容を読み込むストリームを生成
            string? line = "";                   // 1行の内容

            if (txtPosition == 0)
            {                                   // 初回読み込み時
                fs.Seek(0, SeekOrigin.End);     // 末尾に移動
            }
            else
            {                                   // 2回目以降の読み込み時
                fs.Seek(txtPosition, SeekOrigin.Begin);     // 読み込み済の位置に移動
            }

            while ((line = sr.ReadLine()) is not null)
            {                                   // 1行ずつ末尾まで内容を読み込む
                DllLoader.LogPaseAll(line); // スラシュコのログか内容を確認する関数
            }

            txtPosition = fs.Position;          // 読み込み済の位置を保存
        }

    }
}
