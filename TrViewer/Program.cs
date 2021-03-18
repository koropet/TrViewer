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
			/*TRLine[] additional = Circle(500,545,15200,14300,256);
			TRLine[] additional_rails = RailCircle(520,15200,14300,20,true);
			TRLine[] additional_rails2 = RailCircle(525,15200,14300,20,false);
			
			map.AddLines(additional,map.lines.GetLength(0)-1);
			map.AddLines(additional_rails,0);
			map.AddLines(additional_rails2,0);*/
			string image_file = "test.png";
			var bmp = new Bitmap(image_file);
			
			map.AddLines(MakeGround(bmp),0);
			
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
		static TRLine[] RailCircle(int r,int x,int y, int div_length, bool ClockWise=false)
		{
			double circle_length = r*Math.PI*2;
			int division = Convert.ToInt32(circle_length/div_length);
			
			double angle=0,next_angle;
			double angle_division = Math.PI*2/division;
			var output = new TRLine[division];
			
			TRLine rail;
			for(int i=0;i<division;i++)
			{
				angle+=angle_division;
				next_angle=angle+angle_division;
				if(ClockWise)
				{
					rail= new TRLine()
					{
						SX=x+Convert.ToInt32(r*Math.Cos(angle)),
						SY=y+Convert.ToInt32(r*Math.Sin(angle)),
						
						EX=x+Convert.ToInt32(r*Math.Cos(next_angle)),
						EY=y+Convert.ToInt32(r*Math.Sin(next_angle)),
						
						R=255,G=0,B=0,T=4
					};
				}
				else
				{
					rail= new TRLine()
					{
						SX=x+Convert.ToInt32(r*Math.Cos(next_angle)),
						SY=y+Convert.ToInt32(r*Math.Sin(next_angle)),
						
						EX=x+Convert.ToInt32(r*Math.Cos(angle)),
						EY=y+Convert.ToInt32(r*Math.Sin(angle)),
						
						R=255,G=0,B=0,T=4
					};
				}
				output[i]=rail;
			}
			return output;
		}
		static TRLine[] MakeGround(Bitmap bmp)
		{
			int quad_count = bmp.Width*bmp.Height;
			if(quad_count>500000) throw new ArgumentException("Too large image");
			
			TRLine[] output = new TRLine[quad_count*2];
			
			int increment_X = 30000/bmp.Width, increment_Y = 30000/bmp.Height;
			int line_counter=0;
			
			for(int i=0;i<bmp.Width;i++)
			{
				for(int j=0;j<bmp.Height;j++)
				{
					var color = bmp.GetPixel(i,j);
					TRLine border = new TRLine()
					{
						SX=i*increment_X,
						SY=j*increment_Y,
						
						EX=(i+1)*increment_X,
						EY=j*increment_Y,
						R=color.R,G=color.G,B=color.B,T=0
					};
					
					TRLine ground = new TRLine()
					{
						SX=i*increment_X,
						SY=(j+1)*increment_Y,
						
						EX=(i+1)*increment_X,
						EY=(j+1)*increment_Y,
						R=color.R,G=color.G,B=color.B,T=10
					};
					output[line_counter++]=border;
					output[line_counter++]=ground;
				}
			}
			return output;
		}
	}

}