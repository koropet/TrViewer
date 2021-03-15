/*
 * Создано в SharpDevelop.
 * Пользователь: PKorobkin
 * Дата: 15.03.2021
 * Время: 15:47
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using System;
using System.IO;

namespace TrViewer
{
	class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine(System.Reflection.Assembly.GetCallingAssembly().FullName);
			
			// TODO: Implement Functionality Here
			//string file = Console.ReadLine();
			string file = "ЕУЫЕ.tr";
			try
			{
				var br = new BinaryReader(File.Open(file,FileMode.Open));
				var header = br.ReadBytes(10);
				
				int line_count = br.ReadInt32();
				Console.WriteLine(line_count);
				for(int i=0;i<line_count;i++)
				{
					Console.WriteLine("Reading line number {0}",i);
					var line = new TRLine(br);
					Console.WriteLine(line);
				}
				
				int models_count = br.ReadInt32();
				for(int i=0;i<models_count;i++)
				{
					Console.WriteLine("Reading model number {0}",i);
					var model = new TRmodelreference(br);
					Console.WriteLine(model);
				}
				for(int i=0;i<126;i++)
				{
					Console.WriteLine("Reading route number {0}",i);
					var route = new TrRoute(br);
					Console.WriteLine(route);
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
				
			}
			
			Console.WriteLine("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
	}
	public class TRLine
	{
		public TRLine()
		{
			
		}
		public TRLine(BinaryReader br)
		{
			this.SX=br.ReadInt32();
			this.SY=br.ReadInt32();
			this.EX=br.ReadInt32();
			this.EY=br.ReadInt32();
			this.R=br.ReadByte();
			this.G=br.ReadByte();
			this.B=br.ReadByte();
			this.T=br.ReadByte();
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
		public int SX,SY,EX,EY,last_4_bytes;
		public byte R,G,B,T;
		public override string ToString()
		{
			return string.Format("[TRLine SX={0}, SY={1}, EX={2}, EY={3}, R={4}, G={5}, B={6}, T={7}]", SX, SY, EX, EY, R, G, B, T);
		}

	}
	public class TRmodelreference
	{
		public string name;
		public int position=0;
		public TRmodelreference()
		{
			name="";
		}
		public TRmodelreference(BinaryReader br)
		{
			position=br.ReadInt32();
			byte count = br.ReadByte();
			name=System.Text.Encoding.Default.GetString(br.ReadBytes(count));
		}
		public void Write(BinaryWriter bw)
		{
			bw.Write(position);
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
		int LRT=0;
		int vagon_count=1;
		int interval=10;
		public TrRoute(BinaryReader br)
		{
			int point_count = Convert.ToInt32(br.ReadInt16());
			int model_count = br.ReadInt32();
			
			points=new int[point_count];
			for(int i=0;i<point_count;i++)
			{
				points[i]=br.ReadInt32();
			}
			
			models=new int[model_count];
			for(int i=0;i<model_count;i++)
			{
				models[i]=br.ReadInt32();
			}
			int LRT_Vag = Convert.ToInt32(br.ReadByte());
			LRT=(LRT_Vag&16)/16;
			vagon_count=LRT_Vag-LRT*16;
			interval = Convert.ToInt32(br.ReadByte());
		}
		public override string ToString()
		{
			if(points.GetLength(0)==0) return "Empty route";
			
			return string.Format("[TrRoute LRT={0}, Vagon_count={1}, Interval={2}]", LRT, vagon_count, interval);
		}

	}
}