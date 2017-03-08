﻿namespace saaaaaa
{
	using SourceAFIS.Simple;
	using System;
	using System.Drawing;
	using System.IO;
	using System.Linq;
	using System.Threading;
	using System.Diagnostics;

	class MainClass
	{
		private static String DIR = "C:\\Users\\sean\\Downloads\\Twilio\\StealthApp";
		private static String DATABASE_DIR = DIR + "\\Database\\";

		private static String ACCOUNT_SID = "ACa520c27feb8d014ad7c0645989152fc7";
		private static String AUTH_TOKEN = "acd6380c8687a036873255f3aa66d42e";
		private static String APP_ID = "AP7adefab30e2165ca3a7764d977658a8c";
		private static String HARDWARE_URL = "https://788befff.ngrok.io";
		private static String FROM_NUMBER = "+16262262433";
		private static String TO_NUMBER = "+15106849508";
		private static String EMERGENCY = "+15106314655";

		static AfisEngine Afis = new AfisEngine();
		public static void Main(string[] args)
		{
			String param = ACCOUNT_SID + " " + AUTH_TOKEN + " " + APP_ID + " " + DIR + " " + HARDWARE_URL + " " + FROM_NUMBER + " " + TO_NUMBER + " " + EMERGENCY;

			//Database Start
			Fingerprint fp1 = new Fingerprint();
			fp1.AsBitmap = new Bitmap(Bitmap.FromFile(DATABASE_DIR + "2L.bmp"));
			Person person1 = new Person();
			person1.Fingerprints.Add(fp1);
			Afis.Extract(person1);

			Fingerprint fp2 = new Fingerprint();
			fp2.AsBitmap = new Bitmap(Bitmap.FromFile(DATABASE_DIR + "3L.bmp"));
			Person person2 = new Person();
			person2.Fingerprints.Add(fp2);
			Afis.Extract(person2);

			Fingerprint fp3 = new Fingerprint();
			fp3.AsBitmap = new Bitmap(Bitmap.FromFile(DATABASE_DIR + "4L.bmp"));
			Person person3 = new Person();
			person3.Fingerprints.Add(fp3);
			Afis.Extract(person3);

			//Database End

			//Testing
			//awaiting finger input
			int i = 0;

			while (true)
			{
				if (Directory.GetFiles("C:\\Images", "*.bmp").ToArray().Length != i)
				{
					//Directory.EnumerateFileSystemEntries("C:\\Images").Any()
					i = i + 1;

					Fingerprint fp4 = new Fingerprint();

					string[] bmpFiles = Directory.GetFiles("C:\\Images", "*.bmp").ToArray();
					string filename = bmpFiles[i - 1];
					fp4.AsBitmap = new Bitmap(Bitmap.FromFile(filename));

					Person test = new Person();
					test.Fingerprints.Add(fp4);
					Afis.Extract(test);

					float score1 = Afis.Verify(test, person1);
					float score2 = Afis.Verify(test, person2);
					float score3 = Afis.Verify(test, person3);

					bool match1 = (score1 > 0);
					bool match2 = (score2 > 0);
					bool match3 = (score3 > 0);

					//Console.WriteLine(match);
					Console.WriteLine("Awaiting...");

					System.Diagnostics.Process p = new Process();
					p.StartInfo.FileName = "java";
					if (match1)
					{
						p.StartInfo.Arguments = @" -jar " + DIR + "\\TwilioTest.jar conf " + param;
						p.Start();
						Console.WriteLine("911");
					}
					else if (match2)
					{
						p.StartInfo.Arguments = @" -jar " + DIR + "\\TwilioTest.jar on " + param;
						p.Start();
						Console.WriteLine("IoT");
					}
					else if (match3)
					{
						ProcessStartInfo pythonInfo = new ProcessStartInfo("python");
						pythonInfo.UseShellExecute = false;
						pythonInfo.RedirectStandardOutput = true;
					//	pythonInfo.Arguments = DIR + "\\gui.py " + DIR;
						pythonInfo.Arguments = DIR + "\\geolocator.py";
						Process py = new Process();
						py.StartInfo = pythonInfo;
						py.Start();
						StreamReader sr = py.StandardOutput;
						string output = sr.ReadLine();
						py.WaitForExit();
						py.Close();

						p.StartInfo.Arguments = @" -jar " + DIR + "\\TwilioTest.jar msg " + param + " " + output;
						p.Start();
						Console.WriteLine("msg");
					}

					Directory.Delete("C:\\Images\\bvs");
				}
			}
		}
	}
}
