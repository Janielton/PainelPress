using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PainelPress.Classes
{
    public class ScreenShot
    {
        public bool getAndSave(Grid source) {
            try
            {
                SaveFileDialog dlg = new SaveFileDialog()
                {
                    DefaultExt = ".jpg",
                    Filter = "JPG image (*.jpg)|*.jpg|All files (*.*)|*.*"
                };
                Nullable<bool> result = dlg.ShowDialog();
                if (result == false) return false;
                if (File.Exists(dlg.FileName) && new FileInfo(dlg.FileName).Length != 0)
                    File.Delete(dlg.FileName);

                double actualWidth = source.ActualWidth;
                source.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                source.Arrange(new Rect(0, 0, actualWidth, source.DesiredSize.Height));
                double actualHeight = source.ActualHeight;

                RenderTargetBitmap renderTarget = new RenderTargetBitmap((int)source.ActualWidth, (int)source.ActualHeight, 96, 96, PixelFormats.Pbgra32);

                renderTarget.Render(source);


                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTarget));

                using (FileStream stream = new FileStream(dlg.FileName, FileMode.Create, FileAccess.Write))
                {
                    encoder.Save(stream);

                    return true;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return false;
        }


        public byte[]  getAndTemp(Grid source)
        {
            try
            {
                string file = $"{Constants.PASTA}/imagem-temp.png";
                if (File.Exists(file) && new FileInfo(file).Length != 0)
                    File.Delete(file);

                double actualWidth = source.ActualWidth;
                source.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                source.Arrange(new Rect(0, 0, actualWidth, source.DesiredSize.Height));
                double actualHeight = source.ActualHeight;

                RenderTargetBitmap renderTarget = new RenderTargetBitmap((int)source.ActualWidth, (int)source.ActualHeight, 96, 96, PixelFormats.Pbgra32);

                renderTarget.Render(source);


                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTarget));

                using (FileStream stream = new FileStream(file, FileMode.Create, FileAccess.Write))
                {
                    encoder.Save(stream);
                    stream.Close();
                    return File.ReadAllBytes(file);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return null;
        }

        public static byte[] ReadFully(FileStream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }


    }
}
