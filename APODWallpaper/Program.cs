using APODWallpaper.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace APODWallpaper
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var mutex = new Mutex(true, $@"Global\f869dc45-2d7b-4b74-b427-3bb57cbaf655", out bool onlyInstance);

            try
            {
                if (!onlyInstance) {
		            return;
	            }
	            Application.Run(new MainForm(new APODImageProvider(), new Settings()));
	            GC.KeepAlive(mutex);
            }
            finally
            {
                if (mutex != null)
                {
                    mutex.Close();
                    mutex = null;
                }
            }
        }
    }
}
