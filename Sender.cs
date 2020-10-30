using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace goBackN
{
    public partial class Sender
    {
        public List<Frame> Frames { get; set; }
        public ConcurrentQueue<Frame> Sque { get; set; }
        public ConcurrentQueue<Frame> Rque { get; set; }
        
        public int Sf, Sn, Ssize;
        //public bool recvAck;
        //public List<Timer> timers = new List<Timer>();
        public Sender(List<Frame> frames, ConcurrentQueue<Frame> sque, ConcurrentQueue<Frame> rque)
        {
            Sf = Sn = 0;
            Ssize = 4;
            this.Frames = frames;
            this.Sque = sque;
            this.Rque = rque;
            
            //Console.WriteLine(ack.Id);
        }
        public void Send()
        {
            if (Sn < Sf + Ssize && Sn < 16)
            {

                Console.WriteLine("Sender:发送帧{0}", Sn);
                Frames[Sn].Timer = new Timer(new TimerCallback(Timeout), Frames[Sn], 3000, 0);
                if (!Frame_Will_Lose())
                {
                    Console.WriteLine("发送成功", Sn);
                    Sque.Enqueue(Frames[Sn]);
                    //Frames[Sn].Timer = new Timer(new TimerCallback(Timeout), Frames[Sn], 5000, 0);
                }
                else
                {
                    Console.WriteLine("帧{0}丢失", Sn);
                    //Frames[Sn].Timer = new Timer(new TimerCallback(Timeout), Frames[Sn], 5000, 0);
                }
                Sn++;

            }
            //Console.WriteLine(ack.Id);
            //Draw(Sf, Sn, this.ack.Id, Ssize, 16);
            Thread.Sleep(500);
        }

        public void Recv()
        {
            if (Rque.TryDequeue(out Frame ack))
            {

                if (ack.Id <= Sn && ack.Id > Sf)
                {
                    Console.WriteLine("收到帧{0}的ACK", ack.Id - 1);
                    Draw(Sf, Sn, ack.Id , Ssize, 16);
                    for (int i = Sf; i < ack.Id; i++)
                    {
                        //Console.WriteLine("帧{0}的定时器取消", i);
                        Frames[i].Timer.Dispose();
                    }
                    Sf = ack.Id;
                    if (ack.Id == 16)
                    {
                        Console.WriteLine("收到ACK16，传输完成！");
                        Draw(16, Sn, ack.Id, Ssize, 16);
                        Console.WriteLine("按任意键退出...");
                        Console.Read();
                        System.Environment.Exit(0);
                    }
                }
            }
            Thread.Sleep(500);
        }

        public void Timeout(Object state)
        {
            Frame frame = (Frame)state;
            Console.WriteLine("帧{0}的定时器超时,重发", frame.Id);
            for (int i = frame.Id; i < Sn; i++)
            {
                Frames[i].Timer.Dispose();
            }
            Sn = frame.Id;
            //Send();
        }

        public void Run()
        {
            while (true)
            {
                //Console.WriteLine("Sf:{0} Sn:{1} Ssize:{2}", Sf, Sn, Ssize);

                Recv();
                Send();

            }

        }

        public bool Frame_Will_Lose()
        {
            Random rand = new Random();
            return rand.Next(1, 10) > 8;
        }

        
    }
}
