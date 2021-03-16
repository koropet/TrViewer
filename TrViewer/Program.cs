﻿/*
 * Создано в SharpDevelop.
 * Пользователь: PKorobkin
 * Дата: 15.03.2021
 * Время: 15:47
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

namespace TrViewer
{
	class Program
	{
		public static void Main(string[] args)
		{
			Stopwatch sw = new Stopwatch();

			Console.WriteLine(System.Reflection.Assembly.GetAssembly(typeof(Program)).FullName);

			string file = "1.tr";
			
			sw.Start();
			var map = new MapSource(file);
			sw.Stop();
			
			Console.WriteLine("Time elapsed {0} ms", sw.ElapsedMilliseconds);
			Console.WriteLine("Lines: {0}", map.lines.GetLength(0));
			Console.WriteLine("Models: {0}", map.models.GetLength(0));
			Console.WriteLine("Routes: {0}", map.routes.GetLength(0));

			Console.WriteLine("Doing some magic");
			sw.Start();
			//doing some magic
			TRLine[] additional = Circle(5000,10000,15000,15000,256);
			map.AddLines(additional,0);
			
			//stop magic
			sw.Stop();
			
			Console.WriteLine("Time elapsed {0} ms", sw.ElapsedMilliseconds);
			
			string out_file = "out.tr";
			map.Write(out_file);


			Console.WriteLine("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
		static TRLine[] Circle(int r1, int r2, int x, int y, int division)
		{
			double angle=0,next_angle;
			double angle_division = Math.PI*2/division;
			var output = new TRLine[division*2];
			
			double red_offset = 0;
			double green_offset = Math.PI*2/3;
			double blue_offset = Math.PI*4/3;
			
			for(int i=0;i<division;i++)
			{
				angle+=angle_division;
				next_angle=angle+angle_division;
				
				byte red = Convert.ToByte(128+127*Math.Cos((red_offset-angle)));
				byte green = Convert.ToByte(128+127*Math.Cos((green_offset-angle)));
				byte blue = Convert.ToByte(128+127*Math.Cos((blue_offset-angle)));
				
				TRLine border = new TRLine()
				{
					SX=x+Convert.ToInt32(r1*Math.Cos(angle)),
					SY=y+Convert.ToInt32(r1*Math.Sin(angle)),
					
					EX=x+Convert.ToInt32(r2*Math.Cos(angle)),
					EY=y+Convert.ToInt32(r2*Math.Sin(angle)),
					R=red,G=green,B=blue,T=0
				};
				
				TRLine ground = new TRLine()
				{
					SX=x+Convert.ToInt32(r1*Math.Cos(next_angle)),
					SY=y+Convert.ToInt32(r1*Math.Sin(next_angle)),
					
					EX=x+Convert.ToInt32(r2*Math.Cos(next_angle)),
					EY=y+Convert.ToInt32(r2*Math.Sin(next_angle)),
					R=red,G=green,B=blue,T=10
				};
				output[2*i]=border;
				output[2*i+1]=ground;
			}
			return output;
		}
	}
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
	public class TRmodelreference
	{
		public string name;
		public int position = 0;
		public TRmodelreference()
		{
			name = "";
		}
		public TRmodelreference(BinaryReader br)
		{
			position = br.ReadInt32();
			byte count = br.ReadByte();
			name = System.Text.Encoding.Default.GetString(br.ReadBytes(count));
		}
		public void Write(BinaryWriter bw)
		{
			bw.Write(position);

			if (name.Length > 255) throw new ArgumentException("too long name of model: " + name);
			bw.Write(Convert.ToByte(name.Length));

			bw.Write(System.Text.Encoding.Default.GetBytes(name));
		}
		public override string ToString()
		{
			return string.Format("[TRmodelreference Name={0}, Position={1}]", name, position);
		}
	}
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
	public class MapSource
	{
		const int in_game_route_count = 126;
		string file;
		byte[] header;
		public TRLine[] lines;
		public TRmodelreference[] models;
		public TrRoute[] routes;
		int line_count;
		int models_count;
		public MapSource(string file)
		{
			this.file=file;
			try
			{
				var br = new BinaryReader(File.Open(file, FileMode.Open));
				header = br.ReadBytes(10);

				line_count = br.ReadInt32();
				lines = new TRLine[line_count];
				for (int i = 0; i < line_count; i++)
				{
					lines[i] = new TRLine(br);
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
			if(start>=line_count) throw new ArgumentException("Start position more than array has");
			if(start<0) throw new ArgumentException("Start position less than 0");
			
			line_count=additional.GetLength(0)+line_count;
			TRLine[] newer = new TRLine[line_count];
			int counter=0;
			for (counter=0;counter<start;counter++)
			{
				newer[counter]=lines[counter];
			}
			for(counter=start;counter<additional.GetLength(0)+start;counter++)
			{
				newer[counter]=additional[counter-start];
			}
			for (counter=additional.GetLength(0)+start;counter<line_count;counter++)
			{
				newer[counter]=lines[counter-additional.GetLength(0)];
			}
			lines=newer;
			MappingRouteReferences(start,additional.GetLength(0));
		}
		void MappingRouteReferences(int start, int offset)
		{
			foreach(var r in routes)
			{
				for(int i=0;i<r.points.GetLength(0);i++)
				{
					if(r.points[i]>start) r.points[i]+=offset;
				}
			}
		}
	}

}