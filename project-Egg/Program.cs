using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace project_Egg
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Form1 mainForm = new Form1();
            Application.Exit();
        }
    }
}
