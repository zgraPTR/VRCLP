using DllBase;
using System.Reflection;
using VRCLPC.Utils;

namespace VRCLPC.Core
{
    internal class DllLoader
    {
        public DllLoader()
        {
            GlobalUtils.plugins = new List<IPlugin>(); // dllのリストを初期化
        }

        /// <summary>
        /// pluginフォルダ内の DLL を走査し、IPlugin 実装を読み込む
        /// </summary>
        public void Load()
        {
            // dllフォルダのパスを取得

            if (Directory.Exists(GlobalUtils.DllDir) == false)
            {                               // dllフォルダが存在しない場合
                Directory.CreateDirectory(GlobalUtils.DllDir);                   // dllフォルダを作成
                PUtils.CSLog(GlobalUtils.AppName ,"dllフォルダを作成しました");

                return;
            }

            string[] dllFiles = Directory.GetFiles(GlobalUtils.DllDir, "*.dll"); // フォルダ内の DLL ファイルを取得

            foreach (string dllPath in dllFiles)
            {                                                           // 各 DLL ファイルを走査示
                try
                {
                    Assembly assembly = Assembly.LoadFrom(dllPath);     // DLL を読み込む

                    IEnumerable<Type> pluginTypes = assembly.GetTypes().Where(t => typeof(IPlugin).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);
                    // アセンブリ内の IPlugin を実装したクラスを取得

                    foreach (Type? type in pluginTypes)
                    {                                                   // 各dllの型を走査
                        if (Activator.CreateInstance(type) is IPlugin plugin)
                        {
                            GlobalUtils.plugins.Add(plugin);                                            // dllリストに追加
                            plugin.Initialize();                                                        // dllの初期化メソッドを呼び出す
                            PUtils.CSLog(GlobalUtils.AppName, $"読み込み完了 : {plugin.Name} ({plugin.Version})");
                        }
                    }
                }
                catch (ReflectionTypeLoadException rtle)
                {                                                       // 型の読み込みに失敗した場合
                    PUtils.CSLog(GlobalUtils.AppName, $"型読み込みエラー\n{dllPath}");
                    foreach (Exception? loaderEx in rtle.LoaderExceptions)
                    {                                                   // 各ローダー例外を表示
                        PUtils.CSLog(GlobalUtils.AppName, $" - {loaderEx?.Message}");
                    }
                }
                catch (Exception ex)
                {                                                       // その他の例外が発生した場合
                    PUtils.CSLog(GlobalUtils.AppName, $"読み込みエラー\n{dllPath}");
                    PUtils.CSLog(GlobalUtils.AppName, $" - {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 読み込んだすべてのdllの Execute() を呼び出す
        /// </summary>
        public static void LogPaseAll(string line)
        {
            if (GlobalUtils.plugins.Count == 0)
            {                               // dllが読み込まれていない場合
                return;
            }

            foreach (IPlugin plugin in GlobalUtils.plugins)
            {                               // 読み込んだdllを順に実行
                try
                {
                    plugin.ParseLog(line); // 各dllの LogParser メソッドを呼び出す
                }
                catch (Exception ex)
                {                           // dllの実行中に例外が発生した場合
                    PUtils.CSLog(GlobalUtils.AppName, $"実行エラー: {plugin.GetType().FullName}");
                    PUtils.CSLog(GlobalUtils.AppName, $" {ex.Message}\n");
                }
            }
        }
    }
}
