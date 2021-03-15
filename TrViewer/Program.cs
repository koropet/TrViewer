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
			Console.WriteLine("Hello World!");
			
			// TODO: Implement Functionality Here
			string file = Console.ReadLine();
			try
			{
				var br = new BinaryReader(File.Open(file,FileMode.Open));
			}
			catch(Exception e)
			{
				Console.Write("Not found");
			}
			
			
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
	}
}