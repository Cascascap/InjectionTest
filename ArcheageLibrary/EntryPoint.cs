using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyHook;

namespace ArcheageLibrary
{

    public class EntryPoint : IEntryPoint
    {
        private string _channelName;

        public EntryPoint(RemoteHooking.IContext context, string channelName)
        {
            // Constructor code here
            _channelName = channelName;
        }

        public EntryPoint(EasyHook.RemoteHooking.IContext context)
        {
        }


        public void Run(EasyHook.RemoteHooking.IContext context)
        {
            Console.WriteLine("Running");
        }

        public void Run(RemoteHooking.IContext context, string channelName)
        {
            Console.WriteLine("Running with channelName");
        }

    }

}
