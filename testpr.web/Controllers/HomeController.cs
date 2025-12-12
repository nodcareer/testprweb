using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using testpr.web.Models;
using testpr.web.Services;

namespace testpr.web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IBlobStorageService _blobStorageService;
    private const string ContainerName = "uploads";

    public HomeController(ILogger<HomeController> logger, IBlobStorageService blobStorageService)
    {
        _logger = logger;
        _blobStorageService = blobStorageService;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var blobs = await _blobStorageService.ListBlobsAsync(ContainerName);
            return View(blobs);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error loading blobs: {ex.Message}");
            return View(new List<string>());
        }
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            ModelState.AddModelError("file", "Please select a file to upload");
            var blobs = await _blobStorageService.ListBlobsAsync(ContainerName);
            return View("Index", blobs);
        }

        try
        {
            await _blobStorageService.UploadFileAsync(file, ContainerName);
            TempData["SuccessMessage"] = $"File {file.FileName} uploaded successfully!";
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error uploading file: {ex.Message}");
            ModelState.AddModelError("file", "Error uploading file. Please try again.");
        }

        var uploadedBlobs = await _blobStorageService.ListBlobsAsync(ContainerName);
        return View("Index", uploadedBlobs);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteFile(string fileName)
    {
        try
        {
            await _blobStorageService.DeleteBlobAsync(fileName, ContainerName);
            TempData["SuccessMessage"] = $"File {fileName} deleted successfully!";
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting file: {ex.Message}");
            TempData["ErrorMessage"] = "Error deleting file. Please try again.";
        }

        var blobs = await _blobStorageService.ListBlobsAsync(ContainerName);
        return View("Index", blobs);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}
