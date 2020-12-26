const path = require("path");
const fs = require("fs");
const { exec } = require("child_process");
const colors = require("colors");

const projectFile = path.join(
  __dirname,
  "./RestApiClient.NetStandard/RestApiClient.NetStandard.csproj"
);

const args = require("yargs").argv;

let command = "dotnet build -c Preview";
if (args.build == "release") {
  command =
    "dotnet build -c Release && dotnet publish -c Release ./RestApiClient.NetStandard";
} else if (args.build == "release-rc") {
  command = "dotnet build -c Release-rc";
}

const buildProcess = exec(command);
console.log(colors.green(command));

buildProcess.stdout.on("data", (data) =>
  console.log(colors.green(data.toString()))
);

buildProcess.stderr.on("data", (data) =>
  console.log(colors.cyan(`BUILD OUTPUT: ${data.toString()}`))
);

buildProcess.on("close", (code) => {
  if (code != 0) return console.log(colors.red("ERROR with the build!"));

  console.log(colors.blue("build successful\n"));
  // git tag the build with version
  const version = getVersion();
  let cmd = `git tag ${version} && git tag --list`;

  console.log(cmd);

  exec(cmd, (err, stdout, stderr) => {
    if (err) {
      console.log(colors.red("ERROR: " + err));
      return console.log(colors.red("ERROR: " + stderr));
    }

    console.log(colors.cyan(stdout));
    console.log(colors.blue("tagged commit\n"), colors.blue(`${version}`));
  });
});

function getVersion() {
  const xml = fs.readFileSync(projectFile, { encoding: "utf-8" });
  const match = /<Version>([\d\w-\.]+?)<\/Version>/gi.exec(xml);
  return match[1];
}
