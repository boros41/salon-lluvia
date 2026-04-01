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
    private readonly GalleryData<Image, HairProfile, HairStyle, HairColor> _galleryData;

    public GalleryController(GalleryData<Image, HairProfile, HairStyle, HairColor> galleryData)
    {
        _galleryData = galleryData;
    }
    public ActionResult Delete(int id)
    {
        Image? image = _galleryData.ImageRepo.List(new QueryOptions<Image>()
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
        Image? imageToDelete = _galleryData.ImageRepo.List(new QueryOptions<Image>()
        {
            Where = currentImage => currentImage.Id == image.Id,
            Includes = "HairProfile",
            ThenIncludes = "HairStyles, HairColors"
        }).FirstOrDefault();

        if (imageToDelete is null)
        {
            return RedirectToAction("Gallery", "Home", new { area = "" });
        }

        // this is the principal table to Image so deleting this will also delete the Image & its related entities (HairProfile, HairStyle, HairColor)
        // this is fine since I have not implemented multiple Image uploads for the true one-to-many relationship of one HairProfile can have many Images
        // once multiple upload images are implemented, there should be logic to determine if we are deleting one Image or all Images related to HairProfile
        HairProfile hairProfileToDelete = imageToDelete.HairProfile;
        _galleryData.HairProfileRepo.Delete(hairProfileToDelete);
        _galleryData.HairProfileRepo.Save();

        return RedirectToAction("Gallery", "Home", new { area = "" });
    }
}
