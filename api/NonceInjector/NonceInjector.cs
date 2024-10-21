using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;

public static class NonceInjector
{
    [FunctionName("NonceInjector")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
        // Generate a new nonce
        var nonce = Guid.NewGuid().ToString("N");

        // Path to the index.html file (assuming it's in the 'wwwroot' folder)
        var indexPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html");

        if (!File.Exists(indexPath))
        {
            log.LogError("Index.html not found at {0}", indexPath);
            return new NotFoundResult();
        }

        // Read the index.html content
        string indexContent = await File.ReadAllTextAsync(indexPath, Encoding.UTF8);

        // Replace 'DYNAMIC_NONCE_VALUE' with the generated nonce
        string updatedContent = indexContent.Replace("DYNAMIC_NONCE_VALUE", nonce);

        // Return the modified HTML content
        return new ContentResult
        {
            Content = updatedContent,
            ContentType = "text/html",
            StatusCode = 200
        };
    }
}
