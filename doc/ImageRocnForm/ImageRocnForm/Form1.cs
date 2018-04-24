using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using FeatureMatchingExample;

namespace ImageRocnForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            long matchTime;
            using (Mat modelImage = CvInvoke.Imread(@"C:\Users\toky\Desktop\Source\LeSoir_LeSoirBruxellesBrabant_20180202_001 - Copie.jpg", ImreadModes.AnyColor))
            using (Mat observedImage = CvInvoke.Imread(@"C:\Users\toky\Desktop\Source\LeSoir_LeSoirBruxellesBrabant_20180202_001.jpg", ImreadModes.AnyColor))
            {
                Mat result = DrawMatches.Draw(modelImage, observedImage, out matchTime);
                ImageViewer.Show(result, String.Format("Matched in {0} milliseconds", matchTime));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Image<Bgr, byte> ImgInput = new Image<Bgr, byte>(@"C:\Users\toky\Desktop\Source\LeSoir_LeSoirBruxellesBrabant_20180202_001 - Copie.jpg");
            imageBox1.Image = ImgInput;
        }
    }
}
