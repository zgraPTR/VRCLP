using System.Diagnostics;
using DllBase;

namespace VRCARJL
{
    internal class DisconnectManager
    {
        // フィールド
        private InternetCheck internetCheck = new InternetCheck();     // インターネット接続確認クラスのインスタンス

        //// <summary>
        /// 切断時に実行する非同期処理
        /// </summary>
        public async Task Disconnected()
        {
            PUtils.CSLog(GlobalUtils.AppName, "VRChat が切断されました。");

            await internetCheck.WaitForInternetConnectionAsync();       // 接続復旧まで待機

            KillVRChat();                                               // VRChatプロセスを強制終了
            Rejoin();                                                   // リジョイン処理を実行
        }

        /// <summary>
        /// VRChatを強制終了するメソッド
        /// </summary>
        private void KillVRChat()
        {
            PUtils.CSLog(GlobalUtils.AppName, "VRChatを強制終了します。");
            Process? vrchatProcess = Process.GetProcessesByName("VRChat").FirstOrDefault();     // VRChatプロセスを取得
            vrchatProcess?.Kill();                                                              // VRChatプロセスが存在する場合は強制終了
        }

        /// <summary>
        /// VRChatにリジョインするメソッド
        /// </summary>
        private void Rejoin()
        {
            if (string.IsNullOrEmpty(GlobalUtils.JoinUrl))
            {                                                               // リジョイン用のURLが設定されていない場合
                return;                                                     // 処理を終了
            }

            PUtils.CSLog(GlobalUtils.AppName, "リジョイン中");            // リジョインのURLを表示

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = GlobalUtils.JoinUrl,
                UseShellExecute = true
            };                                                              // リジョイン用のURLを起動するためのプロセス情報を設定
            Process.Start(startInfo);                                       // リジョイン用のURLを起動
        }

    }
}
