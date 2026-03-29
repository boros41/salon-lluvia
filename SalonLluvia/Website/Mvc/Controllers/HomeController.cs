using Azure;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Mvc.Data.Repository;
using Mvc.Integrations.AzureBlobStorage.Interfaces;
using Mvc.Integrations.Calendly;
using Mvc.Models;
using Mvc.Models.Gallery;
using Mvc.Models.Gallery.ViewModels;
using Mvc.Models.ViewModels;
using Mvc.Utilities;
using Mvc.Utilities.Interfaces;
using System.Diagnostics;

namespace Mvc.Controllers;

[Route("[action]")]
public class HomeController : Controller
{
    private readonly IRepository<Appointment> _appointmentRepo;
    private readonly IRepository<Client> _clientRepo;
    private readonly GalleryData<Image, HairProfile, HairStyle, HairColor> _galleryData;
    private readonly ICalendlyAppointment _calendlyAppointment;
    private readonly IMemoryCache _memoryCache;

    public HomeController(IRepository<Appointment> appointmentRepo, IRepository<Client> clientRepo,
        GalleryData<Image, HairProfile, HairStyle, HairColor> galleryData, ICalendlyAppointment appointment, IMemoryCache memoryCache)
    {
        _appointmentRepo = appointmentRepo;
        _clientRepo = clientRepo;
        _galleryData = galleryData;
        _calendlyAppointment = appointment;
        _memoryCache = memoryCache;
    }

    [Route("/")]
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Appointment()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Appointment(AppointmentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (_memoryCache.TryGetValue("available-days", out HashSet<string>? availableDays))
        {
            if (availableDays is null)
            {
                Tags.ToastMessage(TempData, new Tags.ToastValues("Appointment", $"Unfortunately, An error occured when trying to book your appointment. If this continues, please contact support with this message: \"cache was empty\"", false));

                return RedirectToAction("Appointment");
            }

            DateTime date = (DateTime)model.Date!; // model.Date has a [Required] attribute which requires a nullable for value types
            if (!availableDays.Contains(date.ToString("yyyy-MM-dd")))
            {
                Tags.ToastMessage(TempData, new Tags.ToastValues("Appointment", "The date you tried to reserve is no longer available, please try again.", false));

                _memoryCache.Remove("available-days");

                return RedirectToAction("Appointment");
            }
        }
        else
        {
            Tags.ToastMessage(TempData, new Tags.ToastValues("Appointment", $"Unfortunately, An error occured when trying to book your appointment. If this continues, please contact support with this message: \"cache key did not exist\"", false));

            return RedirectToAction("Appointment");
        }

        Client? client = _clientRepo.List(new QueryOptions<Client>())
                                    .FirstOrDefault(c => c.PhoneNumber == model.PhoneNumber);
        if (client is null)
        {
            client = new Client()
            {
                Name = model.Name,
                PhoneNumber = model.PhoneNumber
            };

            // must save to increment client's id before appointment can relate to it
            _clientRepo.Insert(client);
            _clientRepo.Save();
        }

        Appointment appointment = new Appointment()
        {
            ClientId = client.Id,
            Client = client,
            Date = model.Date,
            DesiredService = model.DesiredService
        };

        try
        {
            await _calendlyAppointment.CreateAppointment(model);
            _memoryCache.Remove(Tags.AvailableDaysCacheKey); // the date the user just booked is no longer available
        }
        catch (HttpRequestException e)
        {
            if (e.StatusCode is null)
            {
                Tags.ToastMessage(TempData, new Tags.ToastValues("Appointment", $"Unfortunately, An error occured when trying to book your appointment. If this continues, please contact support with this message: {e.Message}", false));

                return RedirectToAction("Appointment");
            }

            int statusCode = (int)e.StatusCode.Value;
            string message;

            switch (statusCode)
            {
                case StatusCodes.Status403Forbidden:
                    // Access to the "/invitees" endpoint is limited to Calendly users on paid plans (Standard and above). Users on the Free plan will receive a 403 Forbidden response.
                    message = $"Unfortunately, the booking service rejected and did not create the appointment. If this continues, please contact support with this code: {statusCode}";
                    break;
                default:
                    message = $"Unfortunately, an error occured when trying to book your appointment. If this continues, please contact support with this code: {statusCode}";
                    break;
            }

            Tags.ToastMessage(TempData, new Tags.ToastValues("Appointment", message, false));

            return RedirectToAction("Appointment");
        }
        catch (Exception e)
        {
            Tags.ToastMessage(TempData, new Tags.ToastValues("Appointment", $"Unfortunately, An error occured when trying to book your appointment. If this continues, please contact support.", false));

            return RedirectToAction("Appointment");
        }

        _appointmentRepo.Insert(appointment);
        _appointmentRepo.Save();

        Tags.ToastMessage(TempData, new Tags.ToastValues("Appointment", "Thank you for your appointment! We will reach out to you soon to confirm.", true));

        return RedirectToAction("Appointment");
    }
    public IActionResult Service()
    {
        return View();
    }

    public IActionResult Pricing()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Gallery([FromServices] IAzureBlobStorageImages azureBlobStorageImages)
    {
        #region Lookup data for hairstyle & hair color checkboxes in admin upload image button
        IEnumerable<HairStyle> hairstyles = _galleryData.HairstyleRepo.List(new QueryOptions<HairStyle>());
        IEnumerable<HairColor> hairColors = _galleryData.HairColorRepo.List(new QueryOptions<HairColor>());
        List<HairstyleCheckboxVm> hairStyleViewModels = [];
        List<HairColorCheckboxVm> hairColorViewModels = [];

        hairStyleViewModels.AddRange(hairstyles.Select(hairstyle => new HairstyleCheckboxVm() { Style = hairstyle.Style }));
        hairColorViewModels.AddRange(hairColors.Select(hairColor => new HairColorCheckboxVm() { Color = hairColor.Color }));

        ImageViewModel model = new ImageViewModel()
        {
            HairStyles = hairStyleViewModels,
            HairColors = hairColorViewModels
        };
        #endregion

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Gallery(ImageViewModel model, [FromServices] IImageHelper imageHelper, [FromServices] IAzureBlobStorageImages azureBlobStorageImages)
    {
        HashSet<string> acceptedMediaType = ["image/jpeg", "image/png", "image/webp"];

        bool isMediaTypeAccepted = acceptedMediaType.Contains(model.Image.ContentType);

        if (!isMediaTypeAccepted)
        {
            ModelState.AddModelError(nameof(model.Image), "Please upload a \".jpg\", \".png\", or \".webp\" image.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        string imageHash = await imageHelper.GetFileHashCodeAsync(model.Image);

        // hash first so we can easily search the image in Azure Blob Storage only by this prefix
        string imageName = $"{imageHash}-{Tags.BusinessName}-{Tags.ImagePurpose}-{Tags.ImageVariant}";

        Response<BlobContentInfo> response;

        try
        {
            response = await azureBlobStorageImages.PostImageAsync(imageName, model.Image);

            _memoryCache.Remove(Tags.GalleryImagesCacheKey); // refresh cache after uploading image so it shows after redirect
        }
        catch (RequestFailedException e)
        {
            const string errorMessage = "The specified image already exists in Azure Blob Storage.";
            ModelState.AddModelError(nameof(model.Image), errorMessage);

            Tags.ToastMessage(TempData, new Tags.ToastValues("Image Upload", errorMessage, false));

            return View(model);
        }

        HairProfile hairProfile = new HairProfile() { Gender = model.Gender };

        List<string> selectedHairstyles = model.HairStyles
                                               .Where(hairstyleVm => hairstyleVm.IsChecked)
                                               .Select(hairstyleVm => hairstyleVm.Style)
                                               .ToList();

        List<string> selectedHairColors = model.HairColors
                                               .Where(hairColorVm => hairColorVm.IsChecked)
                                               .Select(hairColorVm => hairColorVm.Color)
                                               .ToList();

        // This is the skip navigation property in HairProfile so adding these hairstyles from the DB should populate the HairProfileHairStyle junction table
        // E.g., if 3 hairstyles were selected, EF Core will add an entry to the HairProfile table and 3 new junction table entries linking that HairProfile.Id to those 3 HairStyle.Ids
        List<HairStyle> existingHairStyles = _galleryData.HairstyleRepo
                                                         .List(new QueryOptions<HairStyle>()
                                                         {
                                                             Where = hairstyle => selectedHairstyles.Contains(hairstyle.Style)
                                                         })
                                                         .ToList();

        // This is the skip navigation property in HairProfile so adding these hair colors from the DB should populate the HairProfileHairColor junction table
        // E.g., if 3 hair colors were selected, EF Core will add an entry to the HairProfile table and 3 new junction table entries linking that HairProfile.Id to those 3 HairColor.Ids
        List<HairColor> existingHairColors = _galleryData.HairColorRepo
                                                         .List(new QueryOptions<HairColor>()
                                                         {
                                                             Where = hairColor => selectedHairColors.Contains(hairColor.Color)
                                                         })
                                                         .ToList();

        hairProfile.HairStyles.AddRange(existingHairStyles);
        hairProfile.HairColors.AddRange(existingHairColors);

        Image uploadedImage = new Image()
        {
            Name = imageName,
            Description = model.Description,
            HairProfile = hairProfile
        };

        bool isImageInDatabase = _galleryData.ImageRepo
                                             .List(new QueryOptions<Image>())
                                             .FirstOrDefault(image => image.Name == imageName) is not null;

        if (isImageInDatabase)
        {
            const string message = "Image was already uploaded to the database, but did not exist in Azure. Now saved to both.";
            Tags.ToastMessage(TempData, new Tags.ToastValues("Image Upload", message, true));

            return RedirectToAction("Gallery");
        }

        _galleryData.ImageRepo.Insert(uploadedImage);
        _galleryData.ImageRepo.Save();

        Tags.ToastMessage(TempData, new Tags.ToastValues("Image Upload", "Successfully uploaded image!", true));

        // RDG pattern
        return RedirectToAction("Gallery");
    }

    public IActionResult Team()
    {
        return View();
    }

    public IActionResult Testimonial()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}