using System;
using System.IO;
using System.Web.Mvc;
using System.Data;
using BarcodeStandard;
using SkiaSharp;
using NCKH.Models;

namespace NCKH.Controllers
{
    public class DeviceController : Controller
    {
        private DatabaseHelper dbHelper = new DatabaseHelper();

        public ActionResult DeviceCode(int id)
        {
            DataTable device = dbHelper.GetDeviceCode(id);
            if (device == null || device.Rows.Count == 0)
                return HttpNotFound();

            // Tạo mã vạch
            Barcode barcode = new Barcode
            {
                IncludeLabel = true,
                Alignment = AlignmentPositions.Center
            };

            string code = device.Rows[0]["device_code"].ToString();

            using (SKImage image = barcode.Encode(
                BarcodeStandard.Type.Code128,
                code,
                SKColors.Black,
                SKColors.White,
                300,
                100))
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            using (var ms = new MemoryStream())
            {
                data.SaveTo(ms);
                string base64 = Convert.ToBase64String(ms.ToArray());
                ViewBag.BarcodeImage = "data:image/png;base64," + base64;
            }

            return View(device);
        }

    }
}
