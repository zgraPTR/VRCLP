using DllBase;
using SSVRCNJ.Core;
using SSVRCNJ.Service;
using SSVRCNJ.Utils;
using System.Text.RegularExpressions;

namespace SSVRCNJ
{
    public class SSVRCNJ : IPlugin
    {
        // フィールド
        private OSCSendercs osc = new OSCSendercs();                // OSCクラス
        private GameInfo gameData = GameInfo.Instance;              // ゲーム情報クラス

        // プロパティ
        public string Name => "SlashcoSence-VRC-Net-Jpn";           // プラグイン名
        public string AppName => GlobalUtils.AppName;               // アプリケーション名
        public string Version => GlobalUtils.AppVersion;            // プラグインバージョン

        /// <summary>
        /// 初期化
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
            int fuelNum = 0;           // 投入済燃料数

            if (gameData.InGame == false)
            {                       // ゲーム中フラグ無効
                if (ParseFuelReduction(line) == true)
                {                   // 調節された燃料数抽出 成功
                    return;
                }
                else if (line.EndsWith(LogUtils.GameStartStr) == true)
                {                   // ゲーム中フラグ有効化
                    gameData.InGame = true;                     // ゲーム中フラグ

                    for (int genNum = 1; genNum <= GameUtils.GenNum; genNum++)
                    {                                           // 全ジェネ
                        fuelNum = gameData.Generators[genNum - 1].FilledFuel;      // 投入済燃料数
                        PUtils.CSLog(GlobalUtils.AppName ,$"ジェネレーター{genNum}: 投入済燃料数 {fuelNum}"); // デバッグ出力
                        osc.SendFuelNum(genNum, fuelNum);       // 投入済燃料数送信
                        osc.SendBatteryStatus(genNum, false);   // バッテリー未投入状態送信
                    }

                    return;
                }
            }

            if (line.EndsWith(LogUtils.GameEndStr) == true)
            {                       // ゲーム中フラグ無効化
                gameData.Reset();   // ゲーム情報リセット
                return;
            }

            if (ParseSlasherName(line) == true)
            {                       // スラッシャー名抽出 成功
                return;
            }
            else if (ParseGeneratorUpdate(line) == true)
            {                        // ジェネ更新状態抽出 (GM以外)
                return;
            }
            else if (ParseGeneratorUpdateGM(line) == true)
            {                        // ジェネ更新状態抽出 (GM)
                return;
            }
        }

        /// <summary>
        /// 調節された燃料数抽出
        /// </summary>
        /// <param name="line">ログ内容</param>
        /// <returns>抽出成功</returns>
        private bool ParseFuelReduction(string line)
        {
            int filledFuel = 0;             // 調節された燃料数
            int genNum = 0;                // ジェネ番号

            Match match = LogUtils.FuelReductionRegex.Match(line);          // 調節された燃料数抽出

            if (match.Success == false)
            {                               // 調節された燃料抽出失敗
                return false;
            }

            GetNumber("調節済燃料数 PFR", match.Groups[1].Value, out filledFuel);    // 燃料数取得
            GetNumber("ジェネ番号 PFR", match.Groups[2].Value, out genNum);          // ジェネ番号取得

            gameData.Generators[genNum - 1].FilledFuel += filledFuel;   // 燃料数格納

            return true;                    // 抽出内容検知
        }

        /// <summary>
        /// スラッシャー名抽出
        /// </summary>
        /// <param name="line">ログ内容</param>
        /// <returns>抽出成功</returns>
        private bool ParseSlasherName(string line)
        {
            Match match = LogUtils.SlasherRegex.Match(line);    // スラッシャー名抽出

            if (match.Success == false)
            {                                                   // スラッシャー名抽出失敗
                return false;
            }

            gameData.SlasherName = match.Groups[1].Value;       // スラッシャー名格納
            osc.SendSlasherID();                                // スラッシャーID送信

            return true;
        }

        /// <summary>
        /// ジュネ更新種類抽出
        /// </summary>
        /// <param name="line">ログ内容</param>
        /// <returns>抽出成功</returns>
        private bool ParseGeneratorUpdate(string line)
        {
            int genNum = 0;                                         // ジェネID
            string typeStr = string.Empty;                          // ジェネ状態更新種類
            string statusStr = string.Empty;                        // ジェネ状態更新内容
            bool hadBattery = false;                                // ジェネ状態更新内容

            LogUtils.GeneratorStatus generatorStatus;               // ジェネ状態更新種類

            Match match = LogUtils.GeneratorRegex.Match(line);      // ジェネ状態更新抽出

            if (match.Success == false)
            {                                                       // ジェネ状態更新抽出 失敗
                return false;
            }

            typeStr = match.Groups[2].Value;                        // ジェネ状態更新種類
            statusStr = match.Groups[3].Value;                      // ジェネ状態更新内容 (バッテリー投入状態 or 燃料数)

            GetNumber("ジェネ番号 PGU", match.Groups[1].Value, out genNum); // ジェネ番号取得

            if (Enum.TryParse(typeStr, true, out generatorStatus) == false)
            {                                                       // ジェネ状態更新種類 取得失敗
                ShowParseError("ジェネ状態更新種類 PGU", typeStr, "Enum 取得失敗");
                return true;
            }

            if (generatorStatus == LogUtils.GeneratorStatus.HAS_BATTERY)
            {                                                       // バッテリー投入
                hadBattery = bool.Parse(statusStr);                         // バッテリー投入状態
                gameData.Generators[genNum - 1].HasBattery = hadBattery;    // バッテリー状態格納

                osc.SendBatteryStatus(genNum, hadBattery);                  // バッテリー状態送信
            }
            else if (generatorStatus == LogUtils.GeneratorStatus.REMAINING)
            {                                                       // 燃料投入
                GetNumber("必要燃料数 PGU", statusStr, out int fuelCnt);     // 必要燃料数取得
                fuelCnt = (GameUtils.TotalFuel - fuelCnt);                  // 投入済燃料数
                gameData.Generators[genNum - 1].FilledFuel = fuelCnt;       // 燃料数格納

                osc.SendFuelNum(genNum, fuelCnt);                   // 必要燃料数送信
            }
            else
            {                                                       // ジェネ状態更新種類 分岐未存在
                ShowParseError("ジェネ状態更新種類 PGU", typeStr, "ジェネ状態状態 分岐無し");  // ジェネ状態更新種類 エラー表示
            }

            return true;
        }


        /// <summary>
        /// ジュネ更新種類抽出
        /// </summary>
        /// <param name="line">ログ内容</param>
        private bool ParseGeneratorUpdateGM(string line)
        {
            int genNum = 0;             // ジェネID
            int fuelCnt = 0;            // 投入済燃料数

            Match batteryMatch = LogUtils.HadBatteryRegex.Match(line); // バッテリー投入抽出
            Match fuelMatch = LogUtils.GasFueledRegex.Match(line);     // 燃料投入抽出


            if (batteryMatch.Success == true)
            {                                                           // バッテリー投入 抽出成功
                GetNumber("ジェネ番号 PGUGM Battery", batteryMatch.Groups[1].Value, out genNum); // ジェネ番号取得
                gameData.Generators[genNum - 1].HasBattery = true;      // バッテリー状態格納

                osc.SendBatteryStatus(genNum, true);                    // バッテリー状態送信
            }
            else if (fuelMatch.Success == true)
            {                                                           // 燃料投入 抽出成功
                GetNumber("ジェネ番号 PGUGM Fuel", fuelMatch.Groups[1].Value, out genNum); // ジェネ番号取得
                fuelCnt = (++gameData.Generators[genNum - 1].FilledFuel);   // 投入済燃料数

                osc.SendFuelNum(genNum, fuelCnt);                       // 必要燃料数送信
            }
            else
            {                       // ジェネ状態更新抽出 失敗
                return false;       // 抽出失敗
            }

            return true;
        }

        /// <summary>
        /// 数字取得
        /// </summary>
        /// <param name="itemName">名前</param>
        /// <param name="number">数字</param>
        private bool GetNumber(string itemName, string snumber, out int inumber)
        {
            bool result = false;    // 変換結果
            result = int.TryParse(snumber, out inumber);    // 数値へ変換

            if (result == false)
            {                       // 変換失敗
                ShowParseError(itemName, snumber);          // 変換エラー表示
            }

            return result;          // 変換結果
        }

        /// <summary>
        /// 変換エラー表示
        /// </summary>
        /// <param name="itemName">名前</param>
        /// <param name="value">内容</param>
        private void ShowParseError(string itemName, string value, string title = "変換エラー")
        {
            PUtils.CSLog(GlobalUtils.AppName, $"{itemName} 変換エラー: {value}");
            // 変換エラー表示
        }

    }
}
