using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendeeAllocatorTest
{
    class Proguram
    {
        [STAThread]
        static void Main()
        {
            ////不要 
            //Application.EnableVisualStyles(); 
            //Application.SetCompatibleTextRenderingDefault(false); 
            //Application.Run(new Form1()); 
            ////追加 
            NUnit.Gui.AppEntry.Main(new string[] { System.Reflection.Assembly.GetExecutingAssembly().Location, "/run" });
        }
    }
}
