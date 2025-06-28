using System.Text.RegularExpressions;

namespace SSVRCNJ.Utils
{
    internal class LogUtils
    {
        public static readonly Regex FuelReductionRegex = new Regex(@"subtracted (\d) can for free from: SC_generator(\d)", RegexOptions.Compiled);
        /* 燃料調節の正規表現
         * Groups[1].Value = 調節された燃料数
         * Groups[2].Value = ジェネ番号 */

        public static readonly Regex SlasherRegex = new Regex(@"Initialized the Slasher as (\w+)", RegexOptions.Compiled);
        // スラッシャー名の正規表現

        public static readonly Regex GasFueledRegex = new Regex(@"Gas fueled to SC_generator(\d)", RegexOptions.Compiled);
        // GM用 ガソリン投入正規表現 (Groups[1].Value = ジェネ番号)
        public static readonly Regex HadBatteryRegex = new Regex(@"Generator SC_generator(\d) had battery", RegexOptions.Compiled);
        // GM用 バッテリー投入正規表現 (Groups[1].Value = ジェネ番号)

        public static readonly Regex GeneratorRegex = new Regex(@"SC_generator(\d) Progress check\. Last (\w+) value: .*?, updated \2 value: (\w+)", RegexOptions.Compiled);
        /* GM以外用 燃料投入正規表現
         * Groups[1].Value = ジェネ番号
         * Groups[2].Value = enum GeneratorStatus
         * Groups[3].Value = 更新ステータス (バッテリー投入状態(bool) ot 必要燃料数(int)) */
        public enum GeneratorStatus
        {
            HAS_BATTERY,            // バッテリー投入
            REMAINING,              // 燃料投入
        }                           // ジェネレーターの状態


        public static readonly string GameStartStr = "SLASHCO Game setup.";
        // ゲーム開始テキスト
        public static readonly string GameEndStr = "Generators reset again.";
        // ゲーム終了テキスト

        public static readonly List<string> MapNames = new List<string>()
        {
            "SlashCoHQ",
            "MalonesFarmyard",
            "PhilipsWestwoodHighSchool",
            "EastwoodGeneralHospital",
            "ResearchFacilityDelta"
        };                      // マップリスト

        public static readonly List<string> SlasherNames = new List<string>()
        {
            "Bababooey",
            "Sid",
            "Trollge",
            "Borgmire",
            "Abomignat",
            "Thirsty",
            "Elmo",
            "Watcher",
            "Beast",
            "Dolphinman",
            "Igor",
            "Grouch",
            "Princess",
            "Speedrunner"
        };                  // スラッシャーリスト
    }
}
