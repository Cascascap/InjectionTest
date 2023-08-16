using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcheageBot2
{
    public interface IRemoteServer
    {
        void ExecuteCommand(string command);
    }

    public class RemoteServer : MarshalByRefObject, IRemoteServer
    {
        public void ExecuteCommand(string command)
        {
            Console.WriteLine($"Executing command: {command}");
            // Implement the logic to execute the command within the game client
        }
    }
}
