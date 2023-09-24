using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsPlayerSample
{
	static class Program
	{
		/// <summary>
		/// This is base address for hosting, you can change it if you nead.
		/// </summary>
		public static string BaseAddress { get { return "http://localhost:9000/"; } }
		[STAThread]
		public static void Main()
		{
			using (WebApp.Start<Startup>(url: BaseAddress))
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new Form1());
			}
		}
	}
}
