using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Zoom3Web.Controllers
{
    public class SandBoxController : Controller
    {
        //
        // GET: /SandBox/

        public ActionResult SandBox()
        {
            return View();
        }

        public ActionResult waterMark()
        {
            waterMarkText();
            return View();
        }

        public void waterMarkText()
        {
            string WorkingDirectory = @"C:\Users\Marta\Pictures\Berlin\Con Filtrini";
            string Copyright = "Copyright © 2015 - AP Zoom3Web";

            Image imgPhoto = Image.FromFile(WorkingDirectory + "\\Foto 3-6-15 11 06 30.jpg");
            int phWidth = imgPhoto.Width; int phHeight = imgPhoto.Height;

            Bitmap bmPhoto = new Bitmap(phWidth, phHeight, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(72, 72);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            //CHECK
            //Image imgWatermark = new Bitmap(WorkingDirectory+ "\\watermark.bmp");
            //int wmWidth = imgWatermark.Width;
            //int wmHeight = imgWatermark.Height;

            //WATER MARK TEXT

            grPhoto.SmoothingMode = SmoothingMode.AntiAlias;
            grPhoto.DrawImage(
                imgPhoto,
                new Rectangle(0, 0, phWidth, phHeight),
                0,
                0,
                phWidth,
                phHeight,
                GraphicsUnit.Pixel);

            int[] sizes = new int[] { 16, 14, 12, 10, 8, 6, 4 };
            Font crFont = null;
            SizeF crSize = new SizeF();
            for (int i = 0; i < 7; i++)
            {
                crFont = new Font("arial", sizes[i], FontStyle.Bold);
                crSize = grPhoto.MeasureString(Copyright, crFont);

                if ((ushort)crSize.Width < (ushort)phWidth) break;

            }

            int yPixlesFromBottom = (int)(phHeight * .05);
            float yPosFromBottom = ((phHeight -
                       yPixlesFromBottom) - (crSize.Height / 2));
            float xCenterOfImg = (phWidth / 2);

            StringFormat StrFormat = new StringFormat();
            StrFormat.Alignment = StringAlignment.Center;

            SolidBrush semiTransBrush2 = new SolidBrush(Color.FromArgb(153, 0, 0, 0));

            grPhoto.DrawString(Copyright, crFont, semiTransBrush2, new PointF(xCenterOfImg + 1, yPosFromBottom + 1), StrFormat);

            SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(153, 255, 255, 255));

            grPhoto.DrawString(Copyright, crFont, semiTransBrush, new PointF(xCenterOfImg, yPosFromBottom),StrFormat);

        }


        public void waterMarkImage()
        {
            string WorkingDirectory = @"C:\Users\Marta\Pictures\Berlin\Con Filtrini";
            string Copyright = "Copyright © 2015 - AP Zoom3Web";

            Image imgPhoto = Image.FromFile(WorkingDirectory + "\\Foto 3-6-15 11 06 30.jpg");
            int phWidth = imgPhoto.Width; int phHeight = imgPhoto.Height;

            Bitmap bmPhoto = new Bitmap(phWidth, phHeight, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(72, 72);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);

            Image imgWatermark = new Bitmap(WorkingDirectory + "\\watermark.bmp");
            int wmWidth = imgWatermark.Width;
            int wmHeight = imgWatermark.Height;

            //WATERMARK IMAGE

            Bitmap bmWatermark = new Bitmap(bmPhoto);
            bmWatermark.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics grWatermark = Graphics.FromImage(bmWatermark);
            ImageAttributes imageAttributes = new ImageAttributes();
            ColorMap colorMap = new ColorMap();

            colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
            colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
            ColorMap[] remapTable = { colorMap };

            imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

            float[][] colorMatrixElements = { 
               new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
               new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
               new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
               new float[] {0.0f,  0.0f,  0.0f,  0.3f, 0.0f},
               new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
            };

            ColorMatrix wmColorMatrix = new ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(wmColorMatrix,  ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            int xPosOfWm = ((phWidth - wmWidth) - 10);
            int yPosOfWm = 10;

            grWatermark.DrawImage(imgWatermark,new Rectangle(xPosOfWm, yPosOfWm, wmWidth,wmHeight),0, 0,
                wmWidth, wmHeight, GraphicsUnit.Pixel, imageAttributes);


            //SAVE NEW
            imgPhoto = bmWatermark;
                grPhoto.Dispose();
                grWatermark.Dispose();

                imgPhoto.Save(WorkingDirectory + "\\watermark_final.jpg");

                imgPhoto.Dispose();
                imgWatermark.Dispose();
        }

        ZOOM3Entities db = new ZOOM3Entities();

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> SandBox(UserData model)
        {
            var user = db.UserData.Where(u => u.US_Email.Equals(model.US_Email));
            if (user.Count() == 0)
            {
                ModelState.AddModelError("", "El Usuario no Existe");
            }
            else
            {
                var aux = user.Single();
                var userid = aux.US_Id.ToString();
                string url = "http://localhost:51065/Account/RecoverPassword/" + userid;
                if (ModelState.IsValid)
                {
                    var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p><a href={3}>Recuperar Contraseña</a>";
                    var message = new System.Net.Mail.MailMessage();
                    message.To.Add(new MailAddress(model.US_Email));  // replace with valid value 
                    message.From = new MailAddress("zoomthreeweb@gmail.com");  // replace with valid value
                    message.Subject = "Su Compra";
                    string emailMessage = "Para recuperar su contraseña, pinche en el siguiente enlace";
                    message.Body = string.Format(body, "Zoom3 - Admin", message.From, emailMessage, url);
                    message.IsBodyHtml = true;
                    //Send Files

                    var database = db.Photo.Where(i => i.PH_Id == 1).Single();
                    var fileName = database.PH_FileName.ToString();
                    byte[] resumeBytes = database.PH_Image;
                    message.Attachments.Add(new Attachment(new MemoryStream(resumeBytes), fileName));
                    //message.Attachments.Add(new Attachment(HttpContext.Server.MapPath("~/App_Data/Test.docx")));

                    using (var smtp = new SmtpClient())
                    {
                        var credential = new NetworkCredential
                        {
                            UserName = "zoomthreeweb@gmail.com",  // replace with valid value
                            Password = "realmadrid.15"  // replace with valid value
                        };
                        smtp.Credentials = credential;
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        try
                        {

                            await smtp.SendMailAsync(message);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        return RedirectToAction("Index", "Home");
                    }
                }

            }
            return View();
        }

        public ActionResult funcyBox(){
            return View();
        }
        


 }//END
}

