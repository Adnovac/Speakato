using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SpeakatoVoiceAssistant
{
    internal class CommandResolver
    {
        private string currentPath;
        private string basicPath;
        private Process currentProcess = null;
        public CommandResolver(string startingPath)
        {
            basicPath = startingPath;
            currentPath = startingPath;
        }

        public string ResolveCommnad(Command command, string content)
        {
            return command switch
            {
                Command.Greeting => Greeting(content),
                Command.Open => OpenFile(content),
                Command.Close => CloseFile(content),
                _ => "Nic nie zrobiłem"
            };
        }

        private string Greeting(string content)
        {
            return "Cześć!";
        }

        private string CloseFile(string content)
        {
            if (currentProcess != null)
            {
                currentProcess.Close();
                currentPath = basicPath;
                return $"Zamknięto {currentProcess.ProcessName}";
            }
            return $"Nie mam nic do zamknięcia";
        }

        private string OpenFile(string content)
        {
            List<(string, int)> paths = Directory.GetFiles(currentPath).Select(m => (m, 0)).ToList();
            paths.AddRange(Directory.GetDirectories(currentPath).Select(m => (m, 1)));

            foreach (var path in paths.OrderByDescending(m => m.Item1.Length))
            {
                Console.WriteLine(path);
                var name = Path.GetFileNameWithoutExtension(path.Item1);
                if (content.ToLower().Contains(name.ToLower()))
                {
                    if (currentProcess != null)
                    {
                        currentProcess.Close();
                    }

                    if (path.Item2 == 1)
                    {
                        currentPath = path.Item1;
                        currentProcess = Process.Start("explorer.exe", path.Item1);
                        return $"Otwieram folder {name}";
                    }
                    else
                    {
                        currentProcess = Process.Start(@"cmd.exe", $"/c {path.Item1}");
                        return $"Otwieram aplikację {name}";
                    }
                }
            }
            return "Nie udało mi się znaleźć takiego folderu i pliku";
        }
    }
}
