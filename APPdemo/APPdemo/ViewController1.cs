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
            data.Add("name", "木子屋");
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
            btn.SetTitle("我要图", UIControlState.Normal);
            btn.SetTitleColor(UIColor.Black, UIControlState.Normal);
            btn.Frame = new CGRect(30, 200, 200, 40);
            btn.AutoresizingMask = UIViewAutoresizing.All;
            btn.TouchUpInside += avatarTap;
            this.View.AddSubview(btn);

            // Perform any additional setup after loading the view, typically from a nib.
        }
        //获取图片
        public void avatarTap(object sender, EventArgs args)
        {
            UIAlertController alertController = UIAlertController.Create("设置头像", "", UIAlertControllerStyle.Alert);

            //取消
            UIAlertAction cancelAction = UIAlertAction.Create("取消", UIAlertActionStyle.Cancel, action);

            UIImagePickerController imagePicker = new UIImagePickerController();
            imagePicker.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            imagePicker.AllowsEditing = true;
            imagePicker.Delegate = new imageDe();
            
            //从相册中选择
            UIAlertAction fromPhotosAlbumAction = UIAlertAction.Create("从相册选择", UIAlertActionStyle.Default, (UIAlertAction a) => {
                imagePicker.SourceType = UIImagePickerControllerSourceType.SavedPhotosAlbum;
                this.PresentViewController(imagePicker, true, null);
            });

            //从图库选择
            UIAlertAction fromPhotoAction = UIAlertAction.Create("从图库选择", UIAlertActionStyle.Default, (UIAlertAction a) => {
                imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
                this.PresentViewController(imagePicker, true, null);
            });

            //相机拍摄
            UIAlertAction fromCameraAction = UIAlertAction.Create("相机拍摄", UIAlertActionStyle.Default, (UIAlertAction a) => {
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

        //如何进行压缩
        //将图片尺寸改为240x240
        //UIImage smallImage = scaleFromImage(image, new CGSize(240.0f, 240.0f));
        ////写入jpg文件
        //UIImageJPEGRepresentation(smallImage, 1.0f) writeToFile:imageFilePath atomically:YES];

        //+ (UIImage *)imageWithCGImage:(CGImageRef)cgImage scale:(CGFloat)scale orientation:(UIImageOrientation)orientation NS_AVAILABLE_IOS(4_0);**
        //该方法使用一个CGImageRef创建UIImage，在创建时还可以指定放大倍数以及旋转方向。当scale设置为1的时候，新创建的图像将和原图像尺寸一摸一样，而orientaion则可以指定新的图像的绘制方向。
        //也可以用这个方法进行压缩你会发现得到的大小会小了很多倍，如果想测试的话可以转换成 NSData 打印一下。

        //获取压缩后的图片

        public class imageDe : UIImagePickerControllerDelegate
        {
            public override void FinishedPickingMedia(UIImagePickerController picker, NSDictionary info)
            {
                picker.DismissViewController(true, null);
                string mediaType = info["UIImagePickerControllerMediaType"].ToString();
                if (mediaType == "public.image")
                {
                    //初始化imageNew为从相机中获得的--
                    UIImage imageView = (UIImage)info["UIImagePickerControllerOriginalImage"];
                    imageView.SaveToPhotosAlbum(null);
                    //对图片大小进行压缩--将图片尺寸改为240x240
                    imageView = scaleFromImage(imageView, new CGSize(200.0f, 200.0f));
                    //将图片转换成NSdata二进制存储
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
                    ////保存图片到Documents目录为PNG格式

                    //bool s_png = imageData.Save(filePath.AppendPathComponent((NSString)"Documents/image.png"), true);
                    //Console.WriteLine(s_png);
                    ////保存图片到Documents目录为JPG格式

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
        /// 上传文件到服务器
        /// </summary>
        /// <param name="url"></param>服务器的网址
        /// <param name="files"></param>文件路径名
        /// <param name="data"></param>数据源
        /// <param name="encoding"></param>确定中文编码方式
        /// <returns></returns> 
        public String HttpUploadFile(string url, string[] files, NameValueCollection data, Encoding encoding)
        {
            //分解线形式
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            //分界线
            byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            //结束符
            byte[] endbytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

            //1.HttpWebRequest建立web请求
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //首先声明数据类型为multipart/form-data, 然后定义边界字符串AaB03x，这个边界字符串就是用来在下面来区分各个数据的
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            //设置web请求的方式，是发还是收
            request.Method = "POST";
            //web请求的生命周期
            request.KeepAlive = true;
            // 获取或设置请求的身份验证信息
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

                //1.2 file请求头部信息
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
