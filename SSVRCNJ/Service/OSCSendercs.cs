using DllBase;
using SSVRCNJ.Core;
using SSVRCNJ.Utils;

namespace SSVRCNJ.Service
{
    internal class OSCSendercs
    {
        // フィールド
        private readonly string ParameterUrl = "/avatar/parameters";        // OSCパラメーターURL

        private OSCTransmitter oscTransmitter = OSCTransmitter.Instance;    // OSC送信クラスのインスタンス
        private GameInfo gameInfo = GameInfo.Instance;                      // ゲーム情報クラスのインスタンス

        /// <summary>
        /// 燃料数送信処理
        /// </summary>
        /// <param name="gNumber"></param>
        /// <param name="generatorType">ジェネレーター更新状態種類</param>
        /// <param name="value">更新後内容</param>
        public void SendFuelNum(int gNumber, int fuel)
        {
            string parameterName = $"GENERATOR{gNumber}_FUEL";              // 燃料パラメータ
            SendOscMessage(parameterName, fuel);                            // OSC送信
        }

        /// <summary>
        /// 燃料数送信処理
        /// </summary>
        /// <param name="gNumber"></param>
        /// <param name="generatorType">ジェネレーター更新状態種類</param>
        /// <param name="value">更新後内容</param>
        public void SendBatteryStatus(int gNumber, bool hadBattery)
        {
            string parameterName = $"GENERATOR{gNumber}_BATTERY";           // バッテリーパラメータ
            SendOscMessage(parameterName, hadBattery);                      // OSC送信
        }

        /// <summary>
        /// スラッシャー名送信
        /// </summary>
        public void SendSlasherID()
        {
            int slasherID = 0;                      // スラッシャーID
            slasherID = LogUtils.SlasherNames.IndexOf(gameInfo.SlasherName);                // スラッシャーID取得

            PUtils.CSLog(GlobalUtils.AppName, $"スラッシャー名: {gameInfo.SlasherName}");  // スラッシャーIDログ出力

            if (slasherID != -1)
            {                                       // 取得成功
                SendOscMessage("SlasherID", slasherID);
            }
        }

        /// <summary>
        /// OSC送信
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        private void SendOscMessage(string parameterName, object sendObj)
        {
            oscTransmitter.SendOscMessage($"{ParameterUrl}/{parameterName}", sendObj);          // OSC送信
        }

    }
}
