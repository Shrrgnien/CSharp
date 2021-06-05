using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnalogClock
{
    public partial class AnalogClockForm : Form
    {
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public void RotateImage(Graphics gr, Bitmap imageToRotate, float angle)
        {
            gr.TranslateTransform(imageToRotate.Width / 2, imageToRotate.Height / 2);
            gr.RotateTransform(angle);
            gr.TranslateTransform(-imageToRotate.Width / 2, -imageToRotate.Height / 2);
            gr.DrawImage(imageToRotate, 0, 0, imageToRotate.Width, imageToRotate.Height);
        }

        Bitmap ClockSource { get; set; } = new Bitmap("clock.png");
        Bitmap SecondArrowSource { get; set; } = new Bitmap("second arrow.png");
        Bitmap HourArrowSource { get; set; } = new Bitmap("hour arrow.png");
        Bitmap MinuteArrowSource { get; set; } = new Bitmap("minute arrow.png");

        public AnalogClockForm()
        {
            Size = new Size(ClockSource.Width + 150, ClockSource.Height + 50);

            var clockPictureBox = new PictureBox()
            {
                Size = ClockSource.Size,
                Location = new Point(0,0),
                BackColor = Color.Transparent
            };

            var utcLabel = new Label()
            {
                Text = "UTC",
                Font = new Font("Arial", 12),
                Location = new Point(clockPictureBox.Width + 5, 5),
                AutoSize = true
            };

            var utcListBox = new ComboBox()
            {
                Location = new Point(utcLabel.Location.X, utcLabel.Location.Y + utcLabel.Height),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            var timer = new Timer()
            {
                Interval = 1000
            };

            var utcNow = DateTime.UtcNow;
            var now = DateTime.Now;
            var currentUtc = now.Hour - utcNow.Hour;

            var cl = new Bitmap(ClockSource.Width, ClockSource.Height);
            var hrHand = new Bitmap(HourArrowSource.Width, HourArrowSource.Height);
            var minHand = new Bitmap(MinuteArrowSource.Width, MinuteArrowSource.Height);
            var secHand = new Bitmap(SecondArrowSource.Width, SecondArrowSource.Height);
            Graphics clGr = Graphics.FromImage(cl);
            Graphics hrGr = Graphics.FromImage(hrHand);
            Graphics minGr = Graphics.FromImage(minHand);
            Graphics secGr = Graphics.FromImage(secHand);

            for (int i = 12; i >= -12; i--)
            {
                utcListBox.Items.Add(i.ToString());
            }
            utcListBox.SelectedItem = currentUtc.ToString();

            Controls.Add(utcListBox);
            Controls.Add(clockPictureBox);
            Controls.Add(utcLabel);

            timer.Start();

            timer.Tick += (sender,args)=>
            {
                int ss = DateTime.UtcNow.Second;
                int mm = DateTime.UtcNow.Minute;
                int hh = DateTime.UtcNow.Hour + int.Parse(utcListBox.SelectedItem.ToString());
                var secAngle = ss * 6;
                var minAngle = mm * 6 + secAngle / 60;
                var hrAngle = hh * 30 + minAngle / 12;

                hrHand.Dispose();
                minHand.Dispose();
                secHand.Dispose();
                hrGr.Dispose();
                minGr.Dispose();
                secGr.Dispose();

                hrHand = new Bitmap(HourArrowSource.Width, HourArrowSource.Height);
                minHand = new Bitmap(MinuteArrowSource.Width, MinuteArrowSource.Height);
                secHand = new Bitmap(SecondArrowSource.Width, SecondArrowSource.Height);
                hrGr = Graphics.FromImage(hrHand);
                minGr = Graphics.FromImage(minHand);
                secGr = Graphics.FromImage(secHand);

                clGr.Clear(Color.Transparent);
                hrGr.Clear(Color.Transparent);
                minGr.Clear(Color.Transparent);
                secGr.Clear(Color.Transparent);
                RotateImage(hrGr, HourArrowSource, hrAngle);
                RotateImage(minGr, MinuteArrowSource, minAngle);
                RotateImage(secGr, SecondArrowSource, secAngle);

                clGr.DrawImage(ClockSource, 0,0, ClockSource.Width, ClockSource.Height);
                clGr.DrawImage(hrHand, 0, 0, ClockSource.Width, ClockSource.Height);
                clGr.DrawImage(secHand, 0, 0, ClockSource.Width, ClockSource.Height);
                clGr.DrawImage(minHand, 0, 0, ClockSource.Width, ClockSource.Height);

                clockPictureBox.Image = cl;
            };

            Activated += (sender, args) =>
            {
                ActiveControl = null;
            };

            utcListBox.SelectionChangeCommitted += (sender, args) =>
            {
                ActiveControl = null;
            };

            utcListBox.MouseLeave += (sender, args) =>
            {
                ActiveControl = null;
            };

        }
    }
}
