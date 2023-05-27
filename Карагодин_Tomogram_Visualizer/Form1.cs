using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Карагодин_Tomogram_Visualizer
{
    public partial class Form1 : Form
    {
        Bin bin = new Bin();
        View view = new View();
        bool loaded = false;
        private int currentLayer;
        int FrameCount;
        DateTime NextFPSUpdate = DateTime.Now.AddSeconds(1);
        bool needReload = false;
        int renderType = 1;
        void displayFPS()
        {
            if(DateTime.Now >= NextFPSUpdate)
            {
                this.Text = String.Format("CT Visualizer (fps={0})", FrameCount);
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
                FrameCount = 0;
            }
            FrameCount++;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                string str = dialog.FileName;
                bin.readBIN(str);
                view.SetupView(glControl1.Width, glControl1.Height);
                loaded = true;
                glControl1.Invalidate();
            }
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (loaded)
            {
                if (needReload)
                {
                    switch (renderType)
                    {
                        case 1:
                            view.DrawQuads(currentLayer, trackBar2.Value, trackBar3.Value);
                            glControl1.SwapBuffers();
                            break;
                        case 2:
                            view.generateTextureImage(currentLayer, trackBar2.Value, trackBar3.Value);
                            view.Load2dTexture();
                            view.DrawTexture();
                            glControl1.SwapBuffers();
                            break;
                        case 3:
                            view.DrawQuadStrip(currentLayer, trackBar2.Value, trackBar3.Value);
                            glControl1.SwapBuffers();
                            break;
                    }
                    needReload = false;
                }
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;
            needReload = true;
        }
        
        void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                displayFPS();
                glControl1.Invalidate();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                renderType = 1;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                renderType = 2;
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
                renderType = 3;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            needReload = true;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            needReload = true;
        }
    }
}
