using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Server.Controllers
{
    public class UploadImage
    {
        public string Photo { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class GalleriesController : ControllerBase
    {
        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadImage([FromBody] UploadImage model)
        {
            try
            {
                string fileName = $"{Guid.NewGuid()}.jpg";
                if (model.Photo.Contains(','))
                {
                    model.Photo = model.Photo.Split(',')[1];
                }
                byte[] byteArray = Convert.FromBase64String(model.Photo);

                using (Image image = Image.Load(byteArray))
                {
                    image.Mutate(x => x.Resize(x.GetCurrentSize().Width / 2, x.GetCurrentSize().Height / 2));

                    string folderName = "uploads";
                    string pathFolder = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                    string outPath = Path.Combine(pathFolder, fileName);

                    image.Save(outPath);
                    return Ok(new { Image = $"/images/{fileName}" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
