const fs = require("fs");
const path = require("path");

module.exports = async function (context, req) {
  // Path to the built index.html file after the Angular build process
  const indexFilePath = path.join(
    __dirname,
    "../",
    "dist",
    "nonce-angular-app", // Adjust this to match your actual app name
    "browser", // Angular Universal structure, adjust if necessary
    "index.html"
  );

  try {
    // Read the current contents of index.html
    let indexContent = fs.readFileSync(indexFilePath, "utf8");

    // Generate a new nonce (use a secure random generator for production)
    const nonce = Math.random().toString(36).substr(2, 12);

    // Replace the old nonce in the index.html file with the new one
    indexContent = indexContent.replace(/nonce="[^"]*"/, `nonce="${nonce}"`);

    // Serve the updated index.html file
    context.res = {
      status: 200,
      headers: {
        "Content-Type": "text/html", // Ensure the correct content type for HTML
      },
      body: indexContent, // Return the updated index.html file content
    };
  } catch (error) {
    // If an error occurs, return an error message
    context.res = {
      status: 500,
      body: `Error serving index.html: ${error.message}`,
    };
  }
};
