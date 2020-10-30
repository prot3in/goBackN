using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;


namespace goBackN
{
    public class Recver
    {
        public List<Frame> Frames { get; set; }
        public ConcurrentQueue<Frame> Sque { get; set; }
        public ConcurrentQueue<Frame> Rque { get; set; }
        public int Rn { get; set; }
        public Sender Sender { get; set; }
        //public bool recv_Rn;

        public Recver(List<Frame> frames, ConcurrentQueue<Frame> sque, ConcurrentQueue<Frame> rque,Sender sender)
        {
            this.Frames = frames;
            this.Sque = sque;
            this.Rque = rque;
            this.Sender = sender;
            Rn = 0;
        }

        public void SendACK(int i)
        {
            if (!Frame_Will_Lose())
            {

                Rque.Enqueue(Frames[i]);
            }
            else
            {
                Console.WriteLine("ACK{0}丢失", i);
            }

        }

        public void Recv()
        {

            if (Sque.TryDequeue(out Frame frame))
            {
                Sender.Draw(Sender.Sf, Sender.Sn, Rn, Sender.Ssize, 16);

                Console.WriteLine("Recver:收到帧{0}", frame.Id);
                if (frame.Id == Rn)
                {
                    Console.WriteLine("与Rn相同，发送ACK{0}", Rn + 1);
                    Rn++;
                    if (Rn <= 16)
                        SendACK(Rn);
                }
                else
                {
                    Console.WriteLine("与Rn不同，发送ACK{0}", Rn);

                    SendACK(Rn);
                }
                

            }
        }
        public void Run()
        {
            while (true)
            {
                Recv();
                Thread.Sleep(500);
            }
        }

        public bool Frame_Will_Lose()
        {
            Random rand = new Random();
            return rand.Next(1, 10) > 8;
        }
    }

}
