using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aray
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            BaseArea baseAreaForm = new BaseArea();
            UpperGround upperGroundForm = new UpperGround();

            baseAreaForm.SetUpperGroundForm(upperGroundForm);
            upperGroundForm.SetBaseAreaForm(baseAreaForm);

            Application.Run(baseAreaForm);
        }
    }
}
