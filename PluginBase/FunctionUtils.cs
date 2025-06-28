namespace DllBase
{
    public class PUtils
    {
        /// <summary>
        /// コンソールにログを出力するメソッド
        /// </summary>
        /// <param name="pluginName"></param>
        /// <param name="message"></param>
        public static void CSLog(string pluginName,string message)
        {
            Console.WriteLine($"[{pluginName}] {message}");
            // コンソールにログ出力
        }
    }
}
