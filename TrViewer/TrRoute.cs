using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrViewer
{

    public class TrRoute
    {
        public int[] models;
        public int[] points;
        int LRT = 0;
        int vagon_count = 1;
        int interval = 10;
        int point_count;
        int model_count;
        public TrRoute(BinaryReader br)
        {
            point_count = Convert.ToInt32(br.ReadInt16());
            model_count = br.ReadInt32();

            points = new int[point_count];
            for (int i = 0; i < point_count; i++)
            {
                points[i] = br.ReadInt32();
            }

            models = new int[model_count];
            for (int i = 0; i < model_count; i++)
            {
                models[i] = br.ReadInt32();
            }
            int LRT_Vag = Convert.ToInt32(br.ReadByte());
            LRT = (LRT_Vag & 16) / 16;
            vagon_count = LRT_Vag - LRT * 16;
            interval = Convert.ToInt32(br.ReadByte());
        }
        public void Write(BinaryWriter bw)
        {
            if (point_count > short.MaxValue) throw new ArgumentException("Point count over 65535");
            bw.Write(Convert.ToInt16(point_count));
            bw.Write(model_count);
            for (int i = 0; i < point_count; i++)
            {
                bw.Write(points[i]);
            }

            models = new int[model_count];
            for (int i = 0; i < model_count; i++)
            {
                bw.Write(models[i]);
            }
            bw.Write(Convert.ToByte(LRT * 16 + vagon_count));
            bw.Write(Convert.ToByte(interval));
        }
        public override string ToString()
        {
            if (points.GetLength(0) == 0) return "Empty route";

            return string.Format("[TrRoute LRT={0}, Vagon_count={1}, Interval={2}]", LRT, vagon_count, interval);
        }

    }
}
