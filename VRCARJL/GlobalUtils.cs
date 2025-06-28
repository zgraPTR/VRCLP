using System.Text.RegularExpressions;

namespace VRCARJL
{
    /// <summary>
    /// グローバルユーティリティクラス（定数や共通ツール）
    /// </summary>
    internal static class GlobalUtils
    {
        public static readonly string AppName = "VRCARC";           // プラグイン名
        public static readonly string AppVersion = "v1.0.1";        // プラグインバージョン

        public static readonly string DisconnectedMessage = "[Behaviour] OnDisconnected: Exception";
        // 切断ログメッセージ
        public static readonly string AccessUrl = "https://google.com/";
        // インターネット接続確認用のURL
        public static readonly string JoinUrlBase = "\"vrchat://launch?id=";
        // リジョイン用URLのベース (ワールドID:インスタンスID)

        public static readonly Regex JoinRegex = new Regex(
            @"Joining\s+(wrld_[0-9a-f\-]+):(\d+)",
            RegexOptions.Compiled
        );  // リジョイン用の正規表現

        public static string JoinUrl { get; set; } = string.Empty;
        // リジョイン用URL
    }
}
