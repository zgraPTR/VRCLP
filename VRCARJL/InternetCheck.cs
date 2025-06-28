namespace VRCARJL
{
    internal class InternetCheck
    {

        private static readonly HttpClient _httpClient = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(5)   // タイムアウトを5秒に設定
        };                                      // HTTPクライアントのインスタンスを作成

        // コンストラクター
        public InternetCheck()
        {
            // Ctrl+C に対応して安全に終了
            Console.CancelKeyPress += (sender, e) =>
            {
                _httpClient.Dispose();          // HttpClientを解放
                e.Cancel = true;
            };
        }

        /// <summary>
        /// インターネット接続完了まで待機するメソッド
        /// </summary>
        /// <returns></returns>
        public async Task WaitForInternetConnectionAsync()
        {
            bool isConnected = false;                       // 接続状態を初期化

            while (isConnected == false)
            {                                               // 接続状態がfalseの間ループ
                isConnected = await CheckInternetConnectionAsync();           // インターネット接続を確認

                if (isConnected == false)
                {                                           // 接続失敗の場合
                    await Task.Delay(5000);                 // 5秒待機
                }
            }
        }

        /// <summary>
        /// インターネット接続を確認するメソッド
        /// </summary>
        /// <returns></returns>
        private async Task<bool> CheckInternetConnectionAsync()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(GlobalUtils.AccessUrl);
                // ウェブでリクエストを送信

                return response.IsSuccessStatusCode;    // 接続が成功したかを返す
            }
            catch (Exception)
            {                                           // 例外が発生した場合
                return false;                           // 接続失敗
            }
        }

    }
}
