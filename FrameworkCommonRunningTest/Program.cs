using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using Framework.Common.Logger;
using Framework.Common.Config;
using Framework.Common.DTO;
using Framework.Common.Comm;

using Framework.Common.CoordinateSystem;
#if false
using Framework.Lib.Beacon.Logic.DTO;
#endif
using System.Diagnostics;

namespace FrameworkCommonRunningTest
{
    class Program
    {
        decimal SCALE = 1;
        decimal Value = 0;
        private readonly string ByteString = string.Empty;

        private readonly int[] ra = { 1, 2, 3, 4, 5 };


        static void Main(string[] args)
        {
            

            Program p = new Program();
            List<Thread> ThreadPoll = new List<Thread>();

            Stopwatch sp = new Stopwatch();
            sp.Start();

            try
            {
                Stopwatch ps = new Stopwatch();
                ps.Start();
                for (int i = 1; i <= 100000; i++)
                {
                    Thread t = new Thread(ThreadRun);
                    t.IsBackground = true;
                    t.Start(i);
                    ThreadPoll.Add(t);

                    if (i % 100 == 0)
                    {
                        ps.Stop();

                        TimeSpan pts = ps.Elapsed;
                        string pselapsedTime = String.Format("{4} {0:00}:{1:00}:{2:00}.{3:00}", pts.Hours, pts.Minutes, pts.Seconds, pts.Milliseconds / 10, i);
                        Console.WriteLine("RunTime  " + pselapsedTime);
                        // Console.WriteLine(i);

                        ps.Reset();
                        ps.Start();
                    }
                }
                ps.Stop();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            sp.Stop();

            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = sp.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);

            // DateTime End = DateTime.Now;

            // TimeSpan ts = End - Start;

            // Console.WriteLine(ts.TotalMilliseconds);
            Console.ReadKey();
        }

        private static void ThreadRun(object o)
        {
            int id = (int)o;
            int count = 1000;

            // 메모리 낭비용
            int[] array = new int[1048576*100];

            DateTime Start = DateTime.Now;

            while (count > 0)
            {
                // count--;
                // Console.WriteLine($"{id} - {count--}");
                Thread.Sleep(100);
            }

            DateTime End = DateTime.Now;

            TimeSpan ts = End - Start;

            Console.WriteLine($"Thread({id}) : {ts.TotalMilliseconds}ms");
        }

        public string Solution(string phonenumber)
        {
            string answer = phonenumber.Substring(phonenumber.Length - 4);

            Console.WriteLine($"Solution() {answer}");
            answer = answer.PadLeft(phonenumber.Length, '*');

            return answer;
        }

        public string DebugString()
        {
            return this.SCALE == 1
                ? $"{this.Value,8}            [{this.ByteString}]"
                : $"{this.Value,8} ({this.ScaledValue(),8}) [{this.ByteString}]";
        }

        private decimal ScaledValue()
        {
            return this.Value * this.SCALE;
        }

        static int DecimalPart(decimal real)
        {
            var decimalPart = real - (int)real;

            Console.WriteLine($"Real : {real}");

            int count = 0;
            while (decimalPart > 0)
            {
                decimalPart = decimalPart * 10;
                count++;
                decimalPart = decimalPart - (int)decimalPart;

                // Console.WriteLine($"DecimalParts : {decimalPart}");
            }
            

            Console.WriteLine($"{real} DecimalParts : {count}");

            return count;
        }

        static int ReadSimulationType(bool target, bool noise)
        {
            return (target && !noise) ? 0 : (target && noise) ? 1 : 3;
        }

        static void SortTestMain(string[] args)
        {
            List<int> src = new List<int>();

            for(int c = 1; c < 10000; c++ )
            {
                src.Add(c);
            }

            Random r = new Random();
            for (int c = 0; c < src.Count; c++)
            {
                int aa = r.Next(0, src.Count - 1);
                int bb = r.Next(0, src.Count - 1);

                int t = src[aa];
                src[aa] = src[bb];
                src[bb] = t;
            }

            int srcCount = src.Count;

            Thread.Sleep(100);

            long tick1 = 0, tick2 = 0;


            for (int c = 0; c < 10; c++ )
            {
                List<int> src1 = new List<int>();
                src1.AddRange(src);
                List<int> sorted1 = new List<int>();
                List<int> sorted2 = new List<int>();
                sorted2.AddRange(src);

                {
                    Console.WriteLine($"Insert({c + 1}) 시작");

                    DateTime start = DateTime.Now;
                    for (int i = 0; i < srcCount; i++)
                    {
                        // int min = src1.Min();
                        src1.RemoveAt(0);
                        // sorted1.Add(0);
                    }
                    DateTime finished = DateTime.Now;

                    TimeSpan s = finished - start;

                    Console.WriteLine($"Insert({c + 1}) 종료 {s.Ticks}");
                    tick2 += s.Ticks;

                    // Console.WriteLine("Min()->Insert() : " + string.Join(",", sorted1));
                }

                {
                    Console.WriteLine($"Sort({c + 1}) 시작");

                    DateTime start = DateTime.Now;

                    sorted2.Sort();

                    DateTime finished = DateTime.Now;

                    TimeSpan s = finished - start;

                    Console.WriteLine($"Sort({c + 1}) 종료 {s.Ticks}");
                    tick1 += s.Ticks;

                    // Console.WriteLine("Sort() : " + string.Join(",", sorted2));
                }
            }

            TimeSpan a = new TimeSpan(tick1);
            TimeSpan b = new TimeSpan(tick2);

            Console.WriteLine($"Sort()   Ticks : {a.TotalMilliseconds}");
            Console.WriteLine($"Insert() Ticks : {b.TotalMilliseconds}");



            // Console.ReadKey();
        }

        // These legacy simulations require a DLL that is not part of this repository.
#if false
        static void _Main(string[] args)
        {
            RunSimulation(120000, 48000, 4000, 4000, 1, 6);
            RunSimulation(65000, 120000, 3000, 3000, 2, 9);
            RunSimulation(50000, 5000, 2000, 3000, 10, 2.5M);
            RunSimulation(300000, 360000, 3000, 4000, 0, 6);
            RunSimulation(75000, 135000, 2500, 4000, 1, 6);

            RunSimulation1();
            RunSimulation2();

            RunSimulationms(75000, 56000, 1000, 1500, 2, 9);

            Console.ReadKey();
        }

        static void RunSimulation(decimal start, decimal finish, decimal startv, decimal finishv, decimal azimuth, decimal elevation)
        {
            MissileMovingSectionItem Item = new MissileMovingSectionItem(start, finish, startv, finishv, azimuth, elevation);

            for (double i = 0; i < (double)Item.SectionSecond + 1; i++)
            {
                int distance = Item.RunningDistance(i);
                int velocity = Item.RunningVelocity(i);
                var lo3D = Item.CurrentLocation3DmSecond((decimal)i * 1000);
                var vl3D = Item.CurrentVelocity3DmSecond((decimal)i * 1000);

                Console.WriteLine($"{i},{distance},{velocity},X:{lo3D.IntX},Y:{lo3D.IntY},Z:{lo3D.IntZ},Vx:{vl3D.IntVx},Vy:{vl3D.IntVy},Vz:{vl3D.IntVz}");
            }

            Console.WriteLine();
        }

        static void RunSimulationms(decimal start, decimal finish, decimal startv, decimal finishv, decimal azimuth, decimal elevation)
        {
            MissileMovingSectionItem Item = new MissileMovingSectionItem(start, finish, startv, finishv, azimuth, elevation);

            for (double i = 0; i < (double)Item.SectionSecond + 1; )
            {
                int distance = Item.RunningDistance(i);
                int velocity = Item.RunningVelocity(i);
                var lo3D = Item.CurrentLocation3DmSecond((decimal)i * 1000);
                var vl3D = Item.CurrentVelocity3DmSecond((decimal)i * 1000);

                Console.WriteLine($"{i},{distance},{velocity},X:{lo3D.IntX},Y:{lo3D.IntY},Z:{lo3D.IntZ},Vx:{vl3D.IntVx},Vy:{vl3D.IntVy},Vz:{vl3D.IntVz}");

                if (i < 1.2)
                {
                    i += 0.13;
                }
                else
                {
                    i += 1.5;
                }
            }

            Console.WriteLine();
        }

        static void RunSimulation1()
        {
            MissileSimulateDto dto = new MissileSimulateDto();

            dto.Add(new MissileMovingSectionItem(77000, 82000, 1000, 1000, 2, 9));
            dto.Add(new MissileMovingSectionItem(82000, 87000, 1000, 1500, 2, 9));
            dto.Add(new MissileMovingSectionItem(87000, 110000, 1500, 3100, 2, 9));

            // Console.WriteLine(333);

            for (double i = 0; i < dto.TotalSecond + 1; i++)
            {
                var Item = dto.Find((decimal)i);
                if (Item != null)
                {
                    int distance = Item.RunningDistance(i);
                    int velocity = Item.RunningVelocity(i);
                    var lo3D = Item.CurrentLocation3DmSecond((decimal)i * 1000);
                    var vl3D = Item.CurrentVelocity3DmSecond((decimal)i * 1000);

                    Console.WriteLine($"{i},{distance},{velocity},X:{lo3D.IntX},Y:{lo3D.IntY},Z:{lo3D.IntZ},Vx:{vl3D.IntVx},Vy:{vl3D.IntVy},Vz:{vl3D.IntVz}");
                }
            }

            Console.WriteLine();
        }

        static void RunSimulation2()
        {
            MissileSimulateDto dto = new MissileSimulateDto();

            dto.Add(new MissileMovingSectionItem(110000, 85000, 2000, 3000, 2, 9));
            dto.Add(new MissileMovingSectionItem(85000, 75000, 3000, 1000, 2, 9));
            dto.Add(new MissileMovingSectionItem(75000, 70000, 1000, 1500, 2, 9));

            for (double i = 0; i < dto.TotalSecond + 1; i++)
            {
                var Item = dto.Find((decimal)i);
                if (Item != null)
                {
                    decimal distance = Item.RunningDistance(i);
                    decimal velocity = Item.RunningVelocity(i);
                    var lo3D = Item.CurrentLocation3DmSecond((decimal)i * 1000);
                    var vl3D = Item.CurrentVelocity3DmSecond((decimal)i * 1000);

                    Console.WriteLine($"{i},{distance},{velocity},X:{lo3D.IntX},Y:{lo3D.IntY},Z:{lo3D.IntZ},Vx:{vl3D.IntVx},Vy:{vl3D.IntVy},Vz:{vl3D.IntVz}");
                }
            }

            Console.WriteLine();
        }
#endif
    }
}
