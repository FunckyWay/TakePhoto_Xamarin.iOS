using CoreGraphics;
using System;

using UIKit;
using Foundation;
using System.Collections.Specialized;
using System.Text;
using System.Net;
using System.IO;

namespace APPdemo
{
    public partial class ViewController1 : UIViewController
    {
        NameValueCollection data;
        public ViewController1() : base("ViewController1", null)
        {
             data = new NameValueCollection();
            data.Add("name", "ľ����");
            data.Add("url", "http://www.mzwu.com/");
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.View.Frame = new CGRect(0, 20, 300, 300);
            this.View.AutoresizingMask = UIViewAutoresizing.All;
            
           
            UIButton btn = new UIButton();
            btn.SetTitle("��Ҫͼ", UIControlState.Normal);
            btn.SetTitleColor(UIColor.Black, UIControlState.Normal);
            btn.Frame = new CGRect(30, 200, 200, 40);
            btn.AutoresizingMask = UIViewAutoresizing.All;
            btn.TouchUpInside += avatarTap;
            this.View.AddSubview(btn);

            // Perform any additional setup after loading the view, typically from a nib.
        }
        //��ȡͼƬ
        public void avatarTap(object sender, EventArgs args)
        {
            UIAlertController alertController = UIAlertController.Create("����ͷ��", "", UIAlertControllerStyle.Alert);

            //ȡ��
            UIAlertAction cancelAction = UIAlertAction.Create("ȡ��", UIAlertActionStyle.Cancel, action);

            UIImagePickerController imagePicker = new UIImagePickerController();
            imagePicker.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            imagePicker.AllowsEditing = true;
            imagePicker.Delegate = new imageDe();
            
            //�������ѡ��
            UIAlertAction fromPhotosAlbumAction = UIAlertAction.Create("�����ѡ��", UIAlertActionStyle.Default, (UIAlertAction a) => {
                imagePicker.SourceType = UIImagePickerControllerSourceType.SavedPhotosAlbum;
                this.PresentViewController(imagePicker, true, null);
            });

            //��ͼ��ѡ��
            UIAlertAction fromPhotoAction = UIAlertAction.Create("��ͼ��ѡ��", UIAlertActionStyle.Default, (UIAlertAction a) => {
                imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
                this.PresentViewController(imagePicker, true, null);
            });

            //�������
            UIAlertAction fromCameraAction = UIAlertAction.Create("�������", UIAlertActionStyle.Default, (UIAlertAction a) => {
                if (UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera))
                {
                    imagePicker = new UIImagePickerController();
                    imagePicker.SourceType = UIImagePickerControllerSourceType.Camera;
                    imagePicker.Delegate = new imageDe();
                    this.PresentViewController(imagePicker, true, null);
                }
            });

            alertController.AddAction(cancelAction);
            alertController.AddAction(fromCameraAction);
            alertController.AddAction(fromPhotoAction);
            alertController.AddAction(fromPhotosAlbumAction);

            this.PresentViewController(alertController, true, null);
        }

        //��ν���ѹ��
        //��ͼƬ�ߴ��Ϊ240x240
        //UIImage smallImage = scaleFromImage(image, new CGSize(240.0f, 240.0f));
        ////д��jpg�ļ�
        //UIImageJPEGRepresentation(smallImage, 1.0f) writeToFile:imageFilePath atomically:YES];

        //+ (UIImage *)imageWithCGImage:(CGImageRef)cgImage scale:(CGFloat)scale orientation:(UIImageOrientation)orientation NS_AVAILABLE_IOS(4_0);**
        //�÷���ʹ��һ��CGImageRef����UIImage���ڴ���ʱ������ָ���Ŵ����Լ���ת���򡣵�scale����Ϊ1��ʱ���´�����ͼ�񽫺�ԭͼ��ߴ�һ��һ������orientaion�����ָ���µ�ͼ��Ļ��Ʒ���
        //Ҳ�����������������ѹ����ᷢ�ֵõ��Ĵ�С��С�˺ܶ౶���������ԵĻ�����ת���� NSData ��ӡһ�¡�

        //��ȡѹ�����ͼƬ

        public class imageDe : UIImagePickerControllerDelegate
        {
            public override void FinishedPickingMedia(UIImagePickerController picker, NSDictionary info)
            {
                picker.DismissViewController(true, null);
                string mediaType = info["UIImagePickerControllerMediaType"].ToString();
                if (mediaType == "public.image")
                {
                    //��ʼ��imageNewΪ������л�õ�--
                    UIImage imageView = (UIImage)info["UIImagePickerControllerOriginalImage"];
                    imageView.SaveToPhotosAlbum(null);
                    //��ͼƬ��С����ѹ��--��ͼƬ�ߴ��Ϊ240x240
                    imageView = scaleFromImage(imageView, new CGSize(200.0f, 200.0f));
                    //��ͼƬת����NSdata�����ƴ洢
                    NSData imageData;
                    if (imageView.AsPNG() == null)
                    {
                        imageData = imageView.AsJPEG(1.0f);
                    }
                    else
                    {
                        imageData = imageView.AsPNG();
                    }
                    NSFileManager fileManager = NSFileManager.DefaultManager;
                    var filePath =Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
                    string jpgFilename = System.IO.Path.Combine(filePath, "Photo.png");
                    NSError err = null;
                    if (imageData.Save(jpgFilename, false, out err))
                    {
                        UIImage res = UIImage.FromFile(jpgFilename);
                        res.SaveToPhotosAlbum(null);
                        Console.WriteLine("saved as " + jpgFilename);
                    }
                    else
                    {
                        Console.WriteLine("NOT saved as " + jpgFilename + " because" + err.LocalizedDescription);
                    }

                    //string[] path =  NSSearchPath.GetDirectories(NSSearchPathDirectory.DocumentDirectory,NSSearchPathDomain.User,true);
                    //string docpath = path[0];
                    //NSString finalPath = ((NSString)docpath).AppendPathComponent((NSString)"Documents/image.png");
                    //fileManager.CreateDirectory(finalPat  h.DeleteLastPathComponent(),true,null, );
                    //NSFileAttributes ss = null;
                    //bool ssss = fileManager.CreateFile(filePath.AppendPathComponent((NSString)"/var/mobile/Containers/Data/Application/08DCAFDC-75E4-4168-81FD-9B3E7E50A0FA/Documents/image.png"), imageData, ss);
                    ////����ͼƬ��DocumentsĿ¼ΪPNG��ʽ

                    //bool s_png = imageData.Save(filePath.AppendPathComponent((NSString)"Documents/image.png"), true);
                    //Console.WriteLine(s_png);
                    ////����ͼƬ��DocumentsĿ¼ΪJPG��ʽ

                    //bool s_jpg = imageData.Save(filePath.AppendPathComponent((NSString)"Documents/image.jpg"), true);
                    //Console.WriteLine(s_jpg);
                }
                else
                {
                    Console.WriteLine("MEdia");
                }
            }
            public UIImage scaleFromImage(UIImage image, CGSize size)
            {
                UIGraphics.BeginImageContext(size);
                image.Draw(new CGRect(0, 0, size.Width, size.Height));
                UIImage newImage = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();
                return newImage;
            }
        }

        /// <summary>
        /// �ϴ��ļ���������
        /// </summary>
        /// <param name="url"></param>����������ַ
        /// <param name="files"></param>�ļ�·����
        /// <param name="data"></param>����Դ
        /// <param name="encoding"></param>ȷ�����ı��뷽ʽ
        /// <returns></returns> 
        public String HttpUploadFile(string url, string[] files, NameValueCollection data, Encoding encoding)
        {
            //�ֽ�����ʽ
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            //�ֽ���
            byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            //������
            byte[] endbytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

            //1.HttpWebRequest����web����
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //����������������Ϊmultipart/form-data, Ȼ����߽��ַ���AaB03x������߽��ַ����������������������ָ������ݵ�
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            //����web����ķ�ʽ���Ƿ�������
            request.Method = "POST";
            //web�������������
            request.KeepAlive = true;
            // ��ȡ����������������֤��Ϣ
            request.Credentials = CredentialCache.DefaultCredentials;

            using (Stream stream = request.GetRequestStream())
            {
                //1.1 
                string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                if (data != null)
                {
                    foreach (string key in data.Keys)
                    {
                        stream.Write(boundarybytes, 0, boundarybytes.Length);
                        string formitem = string.Format(formdataTemplate, key, data[key]);
                        byte[] formitembytes = encoding.GetBytes(formitem);
                        stream.Write(formitembytes, 0, formitembytes.Length);
                    }
                }

                //1.2 file����ͷ����Ϣ
                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: application/octet-stream\r\n\r\n";
                byte[] buffer = new byte[4096];
                int bytesRead = 0;

                for (int i = 0; i < files.Length; i++)
                {
                    stream.Write(boundarybytes, 0, boundarybytes.Length);
                    string header = string.Format(headerTemplate, "file" + i, Path.GetFileName(files[i]));
                    byte[] headerbytes = encoding.GetBytes(header);
                    stream.Write(headerbytes, 0, headerbytes.Length);
                    using (FileStream fileStream = new FileStream(files[i], FileMode.Open, FileAccess.Read))
                    {
                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            stream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
                stream.Write(endbytes, 0, endbytes.Length);
                //
            }
            //2.WebResponse
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                return stream.ReadToEnd();
            }
        }

        private void action(UIAlertAction obj)
        {
            //Console.WriteLine(HttpUploadFile("http://localhost/Test", new string[] { @"E:\Index.htm", @"E:\test.rar" }, data, Encoding.UTF8));
            Console.WriteLine("cancel or submit");
        }

    }
}
