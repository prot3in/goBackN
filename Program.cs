using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace goBackN
{
    class Program
    {
        
        static List<Frame> frams;
        static ConcurrentQueue<Frame> sque = new ConcurrentQueue<Frame>();
        static ConcurrentQueue<Frame> rque = new ConcurrentQueue<Frame>();
        static void Main()
        {
            Console.WriteLine("设定发送帧数为16，窗口大小为4\n");
            
            //初始化发送帧
            frams = new List<Frame>();
            for (int i = 0; i <= 16; i++)
            {
                frams.Add(new Frame(i));
            }

            Sender sender = new Sender(frams, sque, rque);
            Recver recver = new Recver(frams, sque, rque,sender);
            new Thread(() => sender.Run()).Start();
            new Thread(() => recver.Run()).Start();
            
        }

    }
}
