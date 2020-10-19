//Create using directives for easier access of AForge library's methods
using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace OpenBots.UI.Forms.Supplement_Forms
{
    
    public partial class frmWebcam : Form
    {
        private string _imageOutputPath;
        private Stopwatch _stopwatch;
        //Create webcam object
        VideoCaptureDevice videoSource;

        public frmWebcam(string imageOutputPath = "")
        {
            InitializeComponent();
            _imageOutputPath = imageOutputPath;
            _stopwatch = new Stopwatch();
        }

        private void frmWebcam_Load(object sender, EventArgs e)
        {
            //List all available video sources. (That can be webcams as well as tv cards, etc)
            FilterInfoCollection videosources = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            //Check if atleast one video source is available
            if (videosources != null)
            {
                //For example use first video device. You may check if this is your webcam.
                videoSource = new VideoCaptureDevice(videosources[0].MonikerString);

                try
                {
                    //Check if the video device provides a list of supported resolutions
                    if (videoSource.VideoCapabilities.Length > 0)
                    {
                        string highestSolution = "0;0";
                        //Search for the highest resolution
                        for (int i = 0; i < videoSource.VideoCapabilities.Length; i++)
                        {
                            if (videoSource.VideoCapabilities[i].FrameSize.Width > Convert.ToInt32(highestSolution.Split(';')[0]))
                                highestSolution = videoSource.VideoCapabilities[i].FrameSize.Width.ToString() + ";" + i.ToString();
                        }
                        //Set the highest resolution as active
                        videoSource.VideoResolution = videoSource.VideoCapabilities[Convert.ToInt32(highestSolution.Split(';')[1])];
                    }
                }
                catch { }

                //Create NewFrame event handler
                //(This one triggers every time a new frame/image is captured
                videoSource.NewFrame += new NewFrameEventHandler(videoSource_NewFrame);

                //Start recording
                videoSource.Start();
                _stopwatch.Start();
            }

        }

        private void videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            //Cast the frame as Bitmap object and don't forget to use ".Clone()" otherwise
            //you'll probably get access violation exceptions
            Bitmap camImage = (Bitmap)eventArgs.Frame.Clone();
            Bitmap myImage = (Bitmap)eventArgs.Frame.Clone();

            pbWebcam.BackgroundImage = camImage;

            if (!string.IsNullOrEmpty(_imageOutputPath) && _stopwatch.ElapsedMilliseconds >= 1000)
            {
                string strGrabFileName = Path.Combine(_imageOutputPath, string.Format("Frame_{0:yyyyMMdd_hhmmss.fff}.bmp", DateTime.Now));
                myImage.Save(strGrabFileName, ImageFormat.Bmp);
                _stopwatch.Restart();
            }           
        }

        private void frmWebcam_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Stop and free the webcam object if application is closing
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource = null;
            }
            _stopwatch.Stop();
        }
    }
}