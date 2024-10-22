const fs = require("fs");
const path = require("path");

module.exports = async function (context, req) {
  try {
    // List files in the current directory
    const currentDir = __dirname;
    const currentDirFiles = fs.readdirSync(currentDir);

    context.log("Current Directory:", currentDir);
    context.log("Files and folders in current directory:", currentDirFiles);

    // List files in the parent directory
    const parentDir = path.join(currentDir, "..");
    const parentDirFiles = fs.readdirSync(parentDir);

    context.log("Parent Directory:", parentDir);
    context.log("Files and folders in parent directory:", parentDirFiles);

    // Return the results as a response
    context.res = {
      status: 200,
      body: {
        currentDir: currentDir,
        currentDirFiles: currentDirFiles,
        parentDir: parentDir,
        parentDirFiles: parentDirFiles,
      },
    };
  } catch (err) {
    context.log("Error listing files:", err);
    context.res = {
      status: 500,
      body: "An error occurred while listing files",
    };
  }
};
