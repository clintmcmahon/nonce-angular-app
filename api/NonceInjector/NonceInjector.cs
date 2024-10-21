using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.IO;

public static class NonceInjector
{
    [FunctionName("NonceInjector")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("Nonce Injector function processed a request.");

        // Generate a nonce
        var nonce = GenerateNonce();

        // Construct the path to the wwwroot folder in Azure Functions
        var basePath = Path.Combine(Environment.GetEnvironmentVariable("HOME"), "site", "wwwroot");
        var filePath = Path.Combine(basePath, "index.html");

        // Log the file path for debugging purposes
        log.LogInformation($"Attempting to read the index.html file from: {filePath}");

        string htmlContent;

        if (File.Exists(filePath))
        {
            htmlContent = await File.ReadAllTextAsync(filePath);
            log.LogInformation("Successfully read the index.html file.");
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

    // Updated method to generate a secure nonce using RandomNumberGenerator
    private static string GenerateNonce()
    {
        byte[] nonceBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(nonceBytes);
        }

        // Convert to Base64 for a URL-safe nonce
        return Convert.ToBase64String(nonceBytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }
}
