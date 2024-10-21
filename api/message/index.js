const fs = require("fs");
const path = require("path");

module.exports = async function (context, req) {
  context.log("HTTP trigger function processing request to get index.html");

  const filePath = path.join("/home/site/wwwroot", "index.html");

  // Check if file exists
  if (fs.existsSync(filePath)) {
    // Read the file content
    const htmlContent = fs.readFileSync(filePath, "utf8");

    // Return the HTML content with correct content type
    context.res = {
      status: 200,
      headers: {
        "Content-Type": "text/html",
      },
      body: htmlContent,
    };
  } else {
    // Return 404 if the file is not found
    context.res = {
      status: 404,
      body: "index.html not found",
    };
  }
};
