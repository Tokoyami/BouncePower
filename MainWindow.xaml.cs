using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Threading;
using System.Runtime.InteropServices;
using System.Drawing;

namespace BouncePower
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int ballWidth = 10;
        private int ballHeight = 10;
        private int ballPosX = 0;
        private int ballPosY = 0;
        private int moveStepX = 4;
        private int moveStepY = 4;
        private static readonly Random _random = new Random();
        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("User32.dll")]
        public static extern void ReleaseDC(IntPtr hwnd, IntPtr dc);
        public MainWindow()
        {
            InitializeComponent();
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(100*10_000);
            dispatcherTimer.Start();
            new Thread(() => {
                Thread.Sleep(2000);
                dispatcherTimer.Interval = new TimeSpan(50);
            }).Start();
            element.MediaEnded += OnEnd;
        }
        private void OnEnd(object sender, EventArgs e)
        {
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            IntPtr desktopPtr = GetDC(IntPtr.Zero);
            Graphics g = Graphics.FromHdc(desktopPtr);
            SolidBrush b = new SolidBrush(Color.White);
            g.FillRectangle(b, new Rectangle(0, 0, screen.Size.Width, screen.Size.Height));
            g.Dispose();
            ReleaseDC(IntPtr.Zero, desktopPtr);
            Close();
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            int x = _random.Next(screen.Width - 10);
            int y = _random.Next(screen.Height - 10);
            IntPtr desktopPtr = GetDC(IntPtr.Zero);
            Graphics g = Graphics.FromHdc(desktopPtr);
            SolidBrush b = new SolidBrush(Color.FromArgb(_random.Next(256), _random.Next(256), _random.Next(256)));
            g.FillRectangle(b, new Rectangle(x, y, _random.Next(10,100), _random.Next(10, 100)));
            g.Dispose();
            ReleaseDC(IntPtr.Zero, desktopPtr);
            GC.Collect();
            // update coordinates
            ballPosX += moveStepX;
            if (
                ballPosX < 0 ||
                ballPosX + ballWidth > Screen.PrimaryScreen.WorkingArea.Width
                )
            {
                moveStepX = -moveStepX;
            }

            ballPosY += moveStepY;
            if (
                ballPosY < 0 ||
                ballPosY + ballHeight > Screen.PrimaryScreen.WorkingArea.Bottom
                )
            {
                moveStepY = -moveStepY;
            }
            var location = new System.Windows.Point(ballPosX, ballPosY);
            Left = location.X;
            Top = location.Y - Height;
        }
    }
}
