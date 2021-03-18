using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrViewer
{

    public class TRLine
    {
        public TRLine()
        {

        }
        public TRLine(Point start, Point end, Color color)
        {
            SX = start.X;
            SY = start.Y;
            EX = end.X;
            EY = end.Y;
            R = color.R;
            G = color.G;
            B = color.B;
        }
        public TRLine(BinaryReader br)
        {
            this.SX = br.ReadInt32();
            this.SY = br.ReadInt32();
            this.EX = br.ReadInt32();
            this.EY = br.ReadInt32();
            this.R = br.ReadByte();
            this.G = br.ReadByte();
            this.B = br.ReadByte();
            this.T = br.ReadByte();
            this.last_4_bytes = br.ReadInt32();
        }
        public void Write(BinaryWriter bw)
        {
            bw.Write(SX);
            bw.Write(SY);
            bw.Write(EX);
            bw.Write(EY);
            bw.Write(R);
            bw.Write(G);
            bw.Write(B);
            bw.Write(T);
            bw.Write(last_4_bytes);
        }
        public int SX, SY, EX, EY, last_4_bytes;
        public byte R, G, B, T;
        public override string ToString()
        {
            return string.Format("[TRLine SX={0}, SY={1}, EX={2}, EY={3}, R={4}, G={5}, B={6}, T={7}]", SX, SY, EX, EY, R, G, B, T);
        }
        public virtual TRLine Convert()
        {
            return this;
        }

        static public TRLine Recognize(TRLine input)
        {
            switch (input.T)
            {
                case 0:
                    {
                        return new Border(new Point(input.SX, input.SY), new Point(input.EX, input.EY), Color.FromArgb(input.R, input.G, input.B));
                    }
                case 1:
                    {
                        return new LowWall(new Point(input.SX, input.SY), new Point(input.EX, input.EY), Color.FromArgb(input.R, input.G, input.B));
                    }
                case 2:
                    {
                        return new MidWall(new Point(input.SX, input.SY), new Point(input.EX, input.EY), Color.FromArgb(input.R, input.G, input.B));
                    }
                case 34:
                    {
                        return new TallWall(new Point(input.SX, input.SY), new Point(input.EX, input.EY), Color.FromArgb(input.R, input.G, input.B));
                    }
                case 3:
                    {
                        return new Arc(new Point(input.SX, input.SY), new Point(input.EX, input.EY), Color.FromArgb(input.R, input.G, input.B));
                    }
                case 4:
                    {
                        switch (input.G)
                        {
                            case 0:
                                {

                                    return new Rail(new Point(input.SX, input.SY), new Point(input.EX, input.EY), Color.FromArgb(input.R, input.G, input.B));
                                }
                            case 128:
                                {
                                    return new StopRail(new Point(input.SX, input.SY), new Point(input.EX, input.EY), Color.FromArgb(input.R, input.G, input.B));
                                }
                            case 196:
                                {
                                    return new LargeStopRail(new Point(input.SX, input.SY), new Point(input.EX, input.EY), Color.FromArgb(input.R, input.G, input.B));
                                }
                            default: return new Rail(new Point(input.SX, input.SY), new Point(input.EX, input.EY), Color.FromArgb(input.R, input.G, input.B));
                        };

                    }
                case 36:
                    {
                        switch (input.G)
                        {
                            case 0:
                                {

                                    return new InterchangeRail(new Point(input.SX, input.SY), new Point(input.EX, input.EY), Color.FromArgb(input.R, input.G, input.B));
                                }
                            case 128:
                                {
                                    return new InterchangeStopRail(new Point(input.SX, input.SY), new Point(input.EX, input.EY), Color.FromArgb(input.R, input.G, input.B));
                                }
                            case 196:
                                {
                                    return new InterchangeLargeStopRail(new Point(input.SX, input.SY), new Point(input.EX, input.EY), Color.FromArgb(input.R, input.G, input.B));
                                }
                            default: return new InterchangeRail(new Point(input.SX, input.SY), new Point(input.EX, input.EY), Color.FromArgb(input.R, input.G, input.B));
                        };
                    }
                case 5:
                    {
                        switch (input.R)
                        {
                            case 0:
                                {

                                    return new Trees(new Point(input.SX, input.SY), new Point(input.EX, input.EY));
                                }
                            case 1:
                                {
                                    return new BushyTrees(new Point(input.SX, input.SY), new Point(input.EX, input.EY));
                                }
                            case 2:
                                {
                                    return new DenseTrees(new Point(input.SX, input.SY), new Point(input.EX, input.EY));
                                }
                            default: return input;
                        };
                    }
                case 6:
                    {
                        return new DoubleLine(new Point(input.SX, input.SY), new Point(input.EX, input.EY));
                    }
                case 7:
                    {
                        return new DoubleLinePosts(new Point(input.SX, input.SY), new Point(input.EX, input.EY));
                    }
                case 8:
                    {
                        return new DoubleLinePostsroad(new Point(input.SX, input.SY), new Point(input.EX, input.EY));
                    }
                case 9:
                    {
                        return new DoubleLinePostsroadTrees(new Point(input.SX, input.SY), new Point(input.EX, input.EY));
                    }
                case 10:
                    {
                        return new Ground(new Point(input.SX, input.SY), new Point(input.EX, input.EY), Color.FromArgb(input.R, input.G, input.B));
                    }
                case 11:
                    {
                        return new Bridge(new Point(input.SX, input.SY), new Point(input.EX, input.EY), Color.FromArgb(input.R, input.G, input.B));
                    }
                case 12:
                    {
                        return new RoadBridge(new Point(input.SX, input.SY), new Point(input.EX, input.EY), Color.FromArgb(input.R, input.G, input.B));
                    }
                case 13:
                    {
                        return new TrafficLight(new Point(input.SX, input.SY), new Point(input.EX, input.EY), Color.FromArgb(input.R, input.G, input.B));
                    }
                case 14:
                    {
                        return new Semaphore(new Point(input.SX, input.SY), new Point(input.EX, input.EY), Color.FromArgb(input.R, input.G, input.B));
                    }
                case 15:
                    {
                        return new StaticObject(new Point(input.SX, input.SY), new Point(input.EX, input.EY), Color.FromArgb(input.R, input.G, input.B));
                    }
                default:
                    {
                        return input;
                    }
            }
        }
    }
    #region separate_classes
    public class Border : TRLine
    {
        public Border(Point start, Point end, Color color) : base(start, end, color)
        {
            T = 0;
        }
        public override TRLine Convert()
        {
            T++;
            return Recognize(this);
        }
    }
    public class LowWall : TRLine
    {
        public LowWall(Point start, Point end, Color color) : base(start, end, color)
        {
            T = 1;
        }
        public override TRLine Convert()
        {
            T++;
            return Recognize(this);
        }
    }
    public class MidWall : TRLine
    {
        public MidWall(Point start, Point end, Color color) : base(start, end, color)
        {
            T = 2;
        }
        public override TRLine Convert()
        {
            T += 32;
            return Recognize(this);
        }
    }
    public class TallWall : TRLine
    {
        public TallWall(Point start, Point end, Color color) : base(start, end, color)
        {
            T = 34;
        }
        public override TRLine Convert()
        {
            T = 3;
            return Recognize(this);
        }
    }
    public class Arc : TRLine
    {
        public Arc(Point start, Point end, Color color) : base(start, end, color)
        {
            T = 3;
        }
        public override TRLine Convert()
        {
            T=0;
            return Recognize(this);
        }
    }
    public class Rail : TRLine
    {
        public Rail(Point start, Point end, Color color) : base(start, end, color)
        {
            color = Color.FromArgb(color.R, 0, 0);
            T = 4;
        }
        public override TRLine Convert()
        {
            G = 128;
            return Recognize(this);
        }
    }
    public class StopRail : Rail
    {
        public StopRail(Point start, Point end, Color color) : base(start, end, color)
        {
            color = Color.FromArgb(color.R, 128, 0);
        }
        public override TRLine Convert()
        {
            G = 192;
            return Recognize(this);
        }
    }
    public class LargeStopRail : Rail
    {
        public LargeStopRail(Point start, Point end, Color color) : base(start, end, color)
        {
            color = Color.FromArgb(color.R, 192, 0);
        }
        public override TRLine Convert()
        {
            G = 0; T = 36;
            return Recognize(this);
        }
    }
    public class InterchangeRail : Rail
    {
        public InterchangeRail(Point start, Point end, Color color) : base(start, end, color)
        {
            color = Color.FromArgb(color.R, 0, 0);
            T = 36;
        }
        public override TRLine Convert()
        {
            G = 128;
            return Recognize(this);
        }
    }
    public class InterchangeStopRail : InterchangeRail
    {
        public InterchangeStopRail(Point start, Point end, Color color) : base(start, end, color)
        {
            color = Color.FromArgb(color.R, 128, 0);
        }
        public override TRLine Convert()
        {
            G = 192;
            return Recognize(this);
        }
    }
    public class InterchangeLargeStopRail : InterchangeRail
    {
        public InterchangeLargeStopRail(Point start, Point end, Color color) : base(start, end, color)
        {
            color = Color.FromArgb(color.R, 192, 0);
        }
        public override TRLine Convert()
        {
            G = 0;T = 5;
            return Recognize(this);
        }
    }
    public class Trees : TRLine
    {
        public Trees(Point start, Point end) : base(start, end, Color.FromArgb(0, 0, 0))
        {
            //TODO in original game files, G and B components are random seed for tree generation
            T = 5;
        }
        public override TRLine Convert()
        {
            R = 1;
            return Recognize(this);
        }
    }
    public class BushyTrees : Trees
    {
        public BushyTrees(Point start, Point end) : base(start, end)
        {
            //TODO in original game files, G and B components are random seed for tree generation
            T = 5;
            R = 1; G = 0; B = 0;
        }
        public override TRLine Convert()
        {
            R = 2;
            return Recognize(this);
        }
    }
    public class DenseTrees : Trees
    {
        public DenseTrees(Point start, Point end) : base(start, end)
        {
            //TODO in original game files, G and B components are random seed for tree generation
            T = 5;
            R = 2; G = 0; B = 0;
        }
        public override TRLine Convert()
        {
            R = 0;
            return Recognize(this);
        }
    }
    public class Ground : TRLine
    {
        public Ground(Point start, Point end, Color color) : base(start, end, color)
        {
            T = 10;
        }
    }
    public class DoubleLine : TRLine
    {
        public DoubleLine(Point start, Point end) : base(start, end, Color.FromArgb(0, 0, 0))
        {
            T = 6;
        }
        public override TRLine Convert()
        {
            T++;
            return Recognize(this);
        }
    }
    public class DoubleLinePosts : TRLine
    {
        public DoubleLinePosts(Point start, Point end) : base(start, end, Color.FromArgb(2, 0, 0))
        {
            T = 7;
        }
        public override TRLine Convert()
        {
            T++;
            return Recognize(this);
        }
    }
    public class DoubleLinePostsroad : TRLine
    {
        public DoubleLinePostsroad(Point start, Point end) : base(start, end, Color.FromArgb(0, 0, 0))
        {
            T = 8;
        }
        public override TRLine Convert()
        {
            T++;
            return Recognize(this);
        }
    }
    public class DoubleLinePostsroadTrees : TRLine
    {
        public DoubleLinePostsroadTrees(Point start, Point end) : base(start, end, Color.FromArgb(2, 0, 0))
        {
            T = 9;
        }
        public override TRLine Convert()
        {
            T=6;
            return Recognize(this);
        }
    }
    public class Bridge : Rail
    {
        public Bridge(Point start, Point end, Color color) : base(start, end, color)
        {
            T = 11;
        }
        public override TRLine Convert()
        {
            T++;
            return Recognize(this);
        }
    }
    public class RoadBridge : TRLine
    {
        public RoadBridge(Point start, Point end, Color color) : base(start, end, color)
        {
            T = 12;
            R = color.R; G = 0; B = 0;
        }
             public override TRLine Convert()
        {
            T--;
            return Recognize(this);
        }
    
    }
    public class TrafficLight : TRLine
    {
        public TrafficLight(Point start, Point end, Color color) : base(start, end, color)
        {
            T = 13;
        }
    }
    public class Semaphore : TRLine
    {
        public Semaphore(Point start, Point end, Color color) : base(start, end, color)
        {
            T = 14;
            R = 0; G = 0; B = 0;
        }
    }
    public class StaticObject : TRLine
    {
        public StaticObject(Point start, Point end, Color color) : base(start, end, color)
        {
            T = 15;
        }
        //конвертация должна происходить из карты путем присвоения следующего номера. Управлять номерами может толкьо тот, у кого они есть.
        public int Number
        {
            get
            {
                return last_4_bytes;
            }
        }
    }
    #endregion
}
