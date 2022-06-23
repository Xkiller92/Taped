using OpenTK.Windowing.Desktop;
using System.Reflection;
using Taped.Core;

namespace Taped
{
    public class Program
    {
        static void Main(string[] args)
        {
            string gameProjetName = "E";

            if (args.Length > 0)
            {
                gameProjetName = args[0];
            }

            Assembly ass = Assembly.LoadFrom(@$"D:\dev\csharp\E\E\bin\Debug\net6.0\{gameProjetName}.dll");
            Type[] assTypes = ass.GetTypes();

            for (int i = 0; i < assTypes.Length; i++)
            {
                if (assTypes[i].IsSubclassOf(typeof(GameObject)))
                {
                    ass.CreateInstance(assTypes[i].FullName);
                }
            }

            GameWindowSettings settings = new GameWindowSettings();
            //settings.IsMultiThreaded = true;

            using (MainWindow game = new MainWindow(settings))
            {
                //game.RenderFrequency = 60;
                //game.UpdateFrequency = 60;
                game.Run();
            }
        }
    }
}