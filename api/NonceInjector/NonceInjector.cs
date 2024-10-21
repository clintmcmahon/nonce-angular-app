using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public static class NonceInjector
{
    [FunctionName("NonceInjector")]
    public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
    ILogger log)
    {
        log.LogInformation("Nonce Injector function processed a request.");

        // Generate a nonce using RNGCryptoServiceProvider for strong cryptography
        var nonce = GenerateNonce();

        // Path to the index.html file in the Azure Function's wwwroot folder
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html");
        string htmlContent;

        if (File.Exists(filePath))
        {
            htmlContent = await File.ReadAllTextAsync(filePath);
        }
        else
        {
            log.LogError($"index.html file not found at {filePath}");
            return new NotFoundObjectResult("Index file not found.");
        }

        // Replace nonce placeholder in the HTML file
        htmlContent = htmlContent.Replace("DYNAMIC_NONCE_VALUE", nonce);

        // Add the CSP header with the nonce value
        var result = new ContentResult
        {
            Content = htmlContent,
            ContentType = "text/html",
        };

        req.HttpContext.Response.Headers.Add("Content-Security-Policy", $"script-src 'self' 'nonce-{nonce}'");

        return result;
    }

    // Helper function to generate a secure nonce
    private static string GenerateNonce()
    {
        var nonceBytes = new byte[16];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(nonceBytes);
        }

        // Convert to Base64 for a URL-safe nonce
        return Convert.ToBase64String(nonceBytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }
}
