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

    }
}
