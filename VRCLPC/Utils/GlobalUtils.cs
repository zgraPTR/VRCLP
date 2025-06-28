using DllBase;

namespace VRCLPC.Utils
{
    internal class GlobalUtils
    {
        public static readonly string LogFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low", "VRChat", "VRChat");
        // VRChatのログのフォルダパス
        private static readonly string DllFolderName = "dll";         // dllフォルダ名
        public static readonly string DllDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DllFolderName);
        // dllフォルダのパス

        public static readonly string AppName = "VRCLPC";               // アプリケーション名
        public static List<IPlugin> plugins = new List<IPlugin>();      // dllのリスト
    }
}
