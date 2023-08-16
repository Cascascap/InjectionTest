using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcheageLibrary
{
    public class Client
    {
        public static string Test(String channelName)
        {
            Console.WriteLine("Injected successfully!");
            Console.WriteLine($"Channel Name: {channelName}");
            return "Hooked";
        }
    }
}
