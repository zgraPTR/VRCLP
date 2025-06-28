namespace SSVRCNJ.Core
{
    internal class GeneratorInfo
    {
        public GeneratorInfo()
        {
            Reset();
        }

        // プロパティ

        public int FilledFuel { get; set; }     // 必要燃料数
        public bool HasBattery { get; set; }    // バッテリー挿入状態

        /// <summary>
        /// ジェネレーター情報をリセット
        /// </summary>
        public void Reset()
        {
            FilledFuel = 0;                     // 必要燃料数
            HasBattery = false;                 // バッテリー挿入状態
        }
    }
}
