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
        [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req,
        ILogger log)
    {
        // Get the current directory
        var currentDir = Directory.GetCurrentDirectory();
        log.LogInformation($"Current Directory: {currentDir}");

        // Get all files and directories in the current directory
        string[] filesAndDirs = Directory.GetFileSystemEntries(currentDir, "*", SearchOption.AllDirectories);

        // Format the results into a readable string
        var resultContent = "Files and directories in the current working directory:\n\n";
        foreach (var entry in filesAndDirs)
        {
            resultContent += entry + "\n";
        }

        // Return the list as plain text
        return new ContentResult
        {
            Content = resultContent,
            ContentType = "text/plain",
            StatusCode = 200
        };
    }

    // Helper method to generate a secure nonce
    private static string GenerateNonce()
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            var byteArray = new byte[16];
            rng.GetBytes(byteArray);
            return Convert.ToBase64String(byteArray);
        }
    }
}
