using System;
using System.IO;
using System.Text;

namespace EmilePlugins.Windows.ScriptService
{
    public class CSharpScriptActuator
    {
        public enum Commond
        {
            None,
            Error,
            ClearDevFiles
        }

        public static void Main(string[] args)
        {
            if (args == null) return;
            if (args.Length!=2) return;

            var path = args[0];
            var action = args[1] ;
            var dirs = args[2];

            var cmd = GetCommond(action);
            var condition = GetCondition(cmd);
            var after = GetAfter(cmd);

            Console.Write("开始");

            ForeachDir(path, condition, after);

            //  var path=

            Console.ReadLine();
        }


        static Commond GetCommond(string action)
        {
            var cmd = Commond.None;
            if (Enum.TryParse<Commond>(action, out cmd) == false) cmd = Commond.Error;
            return cmd;
        }

        static void ForeachDir(string path,Func<string,bool> condition,Action<string> after)
        {
            var files = Directory.GetDirectories(path);
            foreach (var one in files)
            {
                if (condition(one)==true) {
                    after(one);
                    continue;
                }
                ForeachDir(one, condition, after);
            }
        }


        static void DeleteFiles(string path)
        {

        }

        static Func<string, bool> GetCondition(Commond cmd)
        {
            return null;
        }

        static Action<string> GetAfter(Commond cmd)
        {
            return null;
        }

    }
}


