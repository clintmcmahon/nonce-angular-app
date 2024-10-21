using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public static class ListFilesFunction
{
    [FunctionName("ListFilesFunction")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
        // Get the current working directory
        string currentDirectory = Directory.GetCurrentDirectory();
        log.LogInformation($"Current Directory: {currentDirectory}");

        // Get all files and directories in the current directory
        StringBuilder result = new StringBuilder();
        result.AppendLine($"Files and directories in the current working directory: {currentDirectory}\n");

        try
        {
            // List all directories
            var directories = Directory.GetDirectories(currentDirectory);
            result.AppendLine("Directories:");
            foreach (var dir in directories)
            {
                result.AppendLine($" - {dir}");
            }

            // List all files
            var files = Directory.GetFiles(currentDirectory);
            result.AppendLine("\nFiles:");
            foreach (var file in files)
            {
                result.AppendLine($" - {file}");
            }
        }
        catch (System.Exception ex)
        {
            log.LogError($"Error accessing files or directories: {ex.Message}");
            return new BadRequestObjectResult("Error retrieving files and directories.");
        }

        // Return the list of files and directories as plain text
        return new ContentResult
        {
            Content = result.ToString(),
            ContentType = "text/plain",
            StatusCode = 200
        };
    }
}
