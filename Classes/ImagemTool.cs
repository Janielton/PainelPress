using ImageProcessor.Imaging.Formats;
using ImageProcessor;
using StoryLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static Google.Rpc.Context.AttributeContext.Types;
using PainelPress.Elementos;

namespace PainelPress.Classes
{
    public class ImagemTool
    {
        public static BitmapFrame ByteArrayToBitmap(byte[] byteArrayIn, bool resize = false)
        {
            try
            {
                if (byteArrayIn == null)
                {
                    MessageBox.Show("Byte array é nulo", "Erro em ByteArrayToBitmap");
                    return null;
                }
                MemoryStream ms = new MemoryStream(byteArrayIn);
                BitmapFrame bit = BitmapFrame.Create(ms,BitmapCreateOptions.None,BitmapCacheOption.Default);
                if (resize)
                {
                    var bitmap = new TransformedBitmap(bit, new ScaleTransform(0.6,0.6));
                    bit = (BitmapFrame)bitmap.Source;
                }
                return bit;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro em ByteArrayToBitmap");
            }

            return null;
        }

        public static TransformedBitmap ToBitmapScale(byte[] byteArrayIn)
        {
            try
            {
                if (byteArrayIn == null)
                {
                    MessageBox.Show("Byte array é nulo", "Erro em ByteArrayToBitmap");
                    return null;
                }
                MemoryStream ms = new MemoryStream(byteArrayIn);
                var bit = BitmapFrame.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.Default);
                TransformedBitmap bitmap = new TransformedBitmap();
                bitmap.BeginInit();
                if (bit.Width > 1400)
                {
               
                    bitmap.Source = new TransformedBitmap(bit, new ScaleTransform(
                           0.7, 0.7));
                }
                else
                {
                    bitmap.Source = bit;
                }
                bitmap.EndInit();
                return bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro em ByteArrayToBitmap");
            }

            return null;
        }
        public static string SaveImagem(byte[] b_image, string nome = "")
        {
            try
            {

                if (nome == "")
                {
                  nome = "temp_file_save_" + DateTime.Now.ToString("HHmmss");
                }
                if (Directory.Exists(Constants.PASTA_TEMP))
                {
                   Directory.CreateDirectory(Constants.PASTA_TEMP);
                }
                using (MemoryStream inStream = new MemoryStream(b_image))
                {
                 using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                  {
                    ISupportedImageFormat pngFormat = new PngFormat { Quality = 100 };

                    string savePath = Constants.PASTA_TEMP + $"\\{nome}.png";
                    if(File.Exists(savePath))
                    {
                        File.Delete(savePath);
                    }
                    imageFactory.Load(inStream).Format(pngFormat).Save(savePath);
                    return savePath;
                   

                 }
                }

            }
            catch (Exception e)
            {
                AlertMensagem.instance.Show(e.Message, "Erro ao salver imagem temporaria");
            }
            return null;
        }

        public static async Task<string> BaixarImage(string url)
        {

            try
            {
                RestAPI restAPI = new RestAPI();
                var img = await restAPI.BaixarDados(url);
                if (img != null)
                {
                    string nome = getNome(url);
                    return SaveImagem(img, nome);
                }

            }
            catch (Exception ex)
            {
                AlertMensagem.instance.Show(ex.Message, "Erro ao baixar imagem");
            }
            return null;
        }

        public static async Task<byte[]> UrlToImage(string url)
        {

            try
            {
                RestAPI restAPI = new RestAPI();
                var img = await restAPI.BaixarDados(url);
                if (img != null)
                {
                    return img;
                }

            }
            catch (Exception ex)
            {
                AlertMensagem.instance.Show(ex.Message, "Erro ao baixar imagem");
            }
            return null;

        }

        public static string getExtensao(string caminho)
        {
            try
            {
                var split = caminho.Split(".");
                int index = split.Length - 1;
                return split[index];
            }catch(Exception ex)
            {
                AlertMensagem.instance.Show(ex.Message, "Erro ao definir extesão da imagem");
            }
            return "png";     
        }

        public static string getNome(string caminho)
        {
            string serarador = "\\";
            if (caminho.StartsWith("http")) serarador = "/";
            var split = caminho.Split(serarador);
            int index = split.Length - 1;
            var split2 = split[index].Split(".");
            return Ferramentas.ToUrlSlug(split2[0]);
        }
    }
}
