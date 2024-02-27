using CSharp_NoyauModules.Noyau;

namespace CSharp_NoyauModules
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Console.OutputEncoding = System.Text.Encoding.UTF8;
			Console.Title = typeof(Program).Namespace;

			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine("==================");
			Console.WriteLine("Début du programme");
			Console.WriteLine("==================");
			Console.WriteLine();
			Console.ResetColor();



			new Procedure().Executer(args);



			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine();
			Console.WriteLine("================");
			Console.WriteLine("Fin du programme");
			Console.WriteLine("================");
			Console.ResetColor();
			Console.Read();
			Environment.Exit(0);
		}
	}
}
