using Rug.Osc;
using System.Net;

namespace DllBase
{
    public class OSCTransmitter
    {
        // フィールド
        private static OSCTransmitter _instance;     // インスタンス
        private IPAddress _sendAddress = IPAddress.Parse("127.0.0.1");      // 送信先IPアドレス
        private int _port = 1;                              // 送信先ポート
        private OscSender oscSender;                        // OSC送信インスタンス

        // コンストラクタ
        public OSCTransmitter()
        {
            SendAddress = _sendAddress;                     // 送信先IPアドレス初期化
            Port = 9000;                                    // 送信先ポート初期化

            oscSender = new OscSender(SendAddress, 0, Port);    // OSC送信インスタンス
        }

        // プロパティ
        public static OSCTransmitter Instance
        {
            get
            {
                if (_instance == null)                      // インスタンスが存在しない場合
                {
                    _instance = new OSCTransmitter();       // 新規インスタンス作成
                }
                return _instance;                           // シングルトンインスタンスを返す
            }                                               // シングルトンインスタンス取得
        }                                                   // インスタンス (シングルトン)

        public IPAddress SendAddress { get; set; }          // 送信先IPアドレス
        public int Port { get; set; }                       // 送信先ポート

        /// <summary>
        /// OSC送信
        /// </summary>
        /// <param name="parameterUrl"></param>
        /// <param name="value"></param>
        public void SendOscMessage(string parameterUrl, object sendObj)
        {
            if ((SendAddress != _sendAddress) || (Port != _port))
            {                                                       // 送信先IPアドレスまたはポートが変更された場合
                _sendAddress = SendAddress;                         // 送信先IPアドレス更新
                _port = Port;                                       // 送信先ポート更新

                oscSender = new OscSender(SendAddress, 0, Port);    // OSC送信インスタンス
                oscSender.Connect();                                // 接続
            }

            OscMessage oscMessage = new OscMessage(parameterUrl, sendObj);      // OSCメッセージ作成
            oscSender.Send(oscMessage);      // OSC送信
        }

    }
}
