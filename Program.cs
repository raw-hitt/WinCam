using System.Threading;

namespace WinCam
{
    internal static class Program
    {
        private static Mutex m_Mutex;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            bool IsNew;
            m_Mutex = new Mutex(true, Application.ProductName, out IsNew);
            if (IsNew)
            {
                ApplicationConfiguration.Initialize();
                Application.Run(new Form1());
            }

        }
    }
}