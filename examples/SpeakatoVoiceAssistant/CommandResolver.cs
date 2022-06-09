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
        private readonly string basicPath;
        private Process? currentProcess = null;
        public CommandResolver(string startingPath)
        {
            basicPath = startingPath;
            currentPath = startingPath;
        }

        public string ResolveCommnad(Command command, string content)
        {
            content = content.ToLower().Replace("odtwórz", "otwórz");
            if(content.Contains("stwórz")) return CreateDirectory(content);
            return command switch
            {
                Command.Greeting => Greeting(),
                Command.Open => OpenFile(content),
                Command.Create => CreateDirectory(content),
                Command.Close => CloseFile(),
                _ => "Nic nie zrobiłem"
            };
        }

        private static string Greeting()
        {
            return "Cześć!";
        }

        private string CloseFile()
        {
            if (currentProcess != null && !currentProcess.HasExited)
            {
                currentProcess.Kill();
                return $"Zamknięto {currentProcess.ProcessName}. Jestem w folderze {currentPath.Split().Last()}";
            }
            return $"Nie mam nic do zamknięcia";
        }

        private string CreateDirectory(string content)
        {
            var name = content.Split().Last();
            Directory.CreateDirectory(Path.Combine(currentPath, name));
            return $"Stworzono folder {name}";
        }

        private string OpenFile(string content)
        {
            List<(string, int)> paths = Directory.GetFiles(currentPath).Select(m => (m, 0)).ToList();
            paths.AddRange(Directory.GetDirectories(currentPath).Select(m => (m, 1)));

            foreach (var path in paths.OrderByDescending(m => m.Item1.Length))
            {
                Console.WriteLine(path);
                var name = Path.GetFileNameWithoutExtension(path.Item1);
                if (!string.IsNullOrWhiteSpace(name) && content.ToLower().Contains(name.ToLower()))
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
                        if (Path.GetExtension(path.Item1) == ".exe")
                            currentProcess = Process.Start(path.Item1);
                        else
                            currentProcess = Process.Start(@"cmd.exe", $"/c {path.Item1}");
                        return $"Otwieram aplikację {name}";
                    }
                }
            }
            return "Nie udało mi się znaleźć takiego folderu i pliku";
        }
    }
}
