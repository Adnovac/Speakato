using System;

namespace SpeakatoVoiceAssistant
{
    internal class CommandResolver
    {
        private string currentPath;
        public CommandResolver(string startingPath)
        {
            currentPath = startingPath;
        }

        public void ResolveCommnad(Command command, string content)
        {
            switch(command)
            {
                case Command.Greeting: Greeting(content); break;
                case Command.Cd: OpenDirectory(content); break;
                case Command.Open: OpenFile(content); break;
            };
        }

        private void Greeting(string content)
        {

        }

        private void OpenDirectory(string content)
        {

        }

        private void OpenFile(string content)
        {

        }
    }
}
