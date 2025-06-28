
using DllBase;
using System.Text.RegularExpressions;

namespace VRCARJL
{
    internal class VRCARJML : IPlugin
    {
        // フィールド
        private DisconnectManager disconnectManager = new DisconnectManager();  // 切断管理クラスのインスタンス

        // プロパティ
        public string Name => "VRChat Auto ReConnect";              // dll名
        public string AppName => GlobalUtils.AppName;               // アプリケーション名

        public string Version => GlobalUtils.AppVersion;            // dllのバージョン

        /// <summary>
        /// 初期化メソッド
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        /// ログのパースを行うメソッド
        /// </summary>
        /// <param name="line"></param>
        public void ParseLog(string line)
        {
            Match match = GlobalUtils.JoinRegex.Match(line);                                // インスタンス入室 正規表現
            string worldId = string.Empty;                                                  // ワールドIDの初期化
            string instanceId = string.Empty;                                               // インスタンスIDの初期化

            if (line.EndsWith(GlobalUtils.DisconnectedMessage))
            {                                                                               // 切断メッセージの場合
                Task.Run(disconnectManager.Disconnected);                                   // 切断処理を実行
            }
            else if (match.Groups.Count == 3)
            {                                                                               // ジョインログの場合
                worldId = match.Groups[1].Value;                                            // ワールドIDを取得
                instanceId = match.Groups[2].Value;                                         // インスタンスIDを取得
                GlobalUtils.JoinUrl = $"{GlobalUtils.JoinUrlBase}{worldId}:{instanceId}";   // リジョイン用のURLを生成
            }
        }
        
    }
}