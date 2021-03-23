using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrViewer
{
    public class MapSource
    {
        const int in_game_route_count = 126;
        const int MaxBounds=30000;
        string file;
        byte[] header;
        public TRLine[] lines;
        public TRmodelreference[] models;
        public TrRoute[] routes;
        int line_count;
        int models_count;
        public MapSource(string file)
        {
            this.file = file;
            try
            {
                var br = new BinaryReader(File.Open(file, FileMode.Open));
                header = br.ReadBytes(10);

                line_count = br.ReadInt32();
                lines = new TRLine[line_count];
                for (int i = 0; i < line_count; i++)
                {
                	lines[i] = TRLine.Recognize(new TRLine(br));
                }

                models_count = br.ReadInt32();
                models = new TRmodelreference[models_count];

                for (int i = 0; i < models_count; i++)
                {
                    models[i] = new TRmodelreference(br);
                }
                routes = new TrRoute[in_game_route_count];

                for (int i = 0; i < in_game_route_count; i++)
                {
                    routes[i] = new TrRoute(br);
                }
                br.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public void Write(string out_file)
        {
            var bw = new BinaryWriter(File.Open(out_file, FileMode.OpenOrCreate));
            bw.Write(header);
            bw.Write(line_count);
            for (int i = 0; i < line_count; i++)
            {
                lines[i].Write(bw);
            }
            bw.Write(models_count);
            for (int i = 0; i < models_count; i++)
            {
                models[i].Write(bw);
            }
            for (int i = 0; i < in_game_route_count; i++)
            {
                routes[i].Write(bw);
            }
            bw.Close();
        }
        public void AddLines(TRLine[] additional, int start)
        {
            if (start >= line_count) throw new ArgumentException("Start position more than array has");
            if (start < 0) throw new ArgumentException("Start position less than 0");

            line_count = additional.GetLength(0) + line_count;
            TRLine[] newer = new TRLine[line_count];
            int counter = 0;
            for (counter = 0; counter < start; counter++)
            {
                newer[counter] = lines[counter];
            }
            for (counter = start; counter < additional.GetLength(0) + start; counter++)
            {
                newer[counter] = additional[counter - start];
            }
            for (counter = additional.GetLength(0) + start; counter < line_count; counter++)
            {
                newer[counter] = lines[counter - additional.GetLength(0)];
            }
            lines = newer;
            MappingRouteReferences(start, additional.GetLength(0));
        }
        public void Move(int X, int Y)
        {
        	foreach(var l in lines)
        	{
        		if(l.SX+X>MaxBounds||l.SY+Y>MaxBounds||l.EX+X>MaxBounds||l.EY+Y>MaxBounds||
        		   l.SX+X<0||l.SY+Y<0||l.EX+X<0||l.EY+Y<0) throw new ArgumentException("Moving over bounds");
        		l.SX+=X;
        		l.SY+=Y;
        		l.EX+=X;
        		l.EY+=Y;
        	}
        }
        void MappingRouteReferences(int start, int offset)
        {
            foreach (var r in routes)
            {
                for (int i = 0; i < r.points.GetLength(0); i++)
                {
                    if (r.points[i] > start) r.points[i] += offset;
                }
            }
        }
    }
}
