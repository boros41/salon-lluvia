using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mvc.Data.Repository;
using Mvc.Models.Gallery;
using Mvc.Utilities;

namespace Mvc.Areas.Admin.Controllers;

[Authorize(Roles = "Admin")]
[Area("Admin")]
public class GalleryController : Controller
{
    private readonly IRepository<Image> _imageRepo;

    public GalleryController(IRepository<Image> imageRpo)
    {
        _imageRepo = imageRpo;
    }
    public ActionResult Delete(int id)
    {
        Image? image = _imageRepo.List(new QueryOptions<Image>()
        {
            Where = image => image.Id == id,
            Includes = "HairProfile",
            ThenIncludes = "HairStyles, HairColors"
        }).FirstOrDefault();

        if (image is null)
        {
            Tags.ToastMessage(TempData, new Tags.ToastValues("Image", $"No Image with ID \"{id}\" to delete", false));
            return RedirectToAction("Gallery", "Home", new { area = "" });
        }

        return View(image);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(Image image)
    {
        Image? imageToDelete = _imageRepo.Get(image.Id);

        if (imageToDelete is null)
        {
            return RedirectToAction("Gallery", "Home", new { area = "" });
        }

        _imageRepo.Delete(imageToDelete);
        _imageRepo.Save();

        return RedirectToAction("Gallery", "Home", new { area = "" });
    }
}
