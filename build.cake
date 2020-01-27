#addin "Cake.Npm"
#addin "Cake.Docker"
#addin "Cake.FileHelpers"
#addin nuget:?package=SharpZipLib&version=0.86.0
#addin nuget:?package=Cake.Compression&version=0.1.0

var target = Argument("target", "build");
var configuration = Argument<string>("configuration", "Release");
var runtime = "linux-x64";

var name = "import-coordinator";
var version = "0.11";

var projectFile = "./src/ImportCoordinator/ImportCoordinator.csproj";
var outputDir = "./build/bin";

Task("clean")
    .Does(ctx => {
        CleanDirectories("./build");
    });



Task("build-server")
    .IsDependentOn("clean")
    .Does(() =>
    {
        var settings = new DotNetCorePublishSettings
        {
            Configuration = configuration,
            OutputDirectory = outputDir,
            Runtime = runtime
        };

        DotNetCorePublish(projectFile, settings);

        MoveFiles("./build/bin/config*.json", "./build");
		CreateDirectory("./build/data/");
        
		DeleteFiles("./build/config.*.json");
    });


Task("build")
    .IsDependentOn("build-server");


Task("pack")
    .IsDependentOn("build")
    .Does(() => {
        var date = DateTime.Now.Date.ToString("yyyy-MM-dd");
        var zipName = $"{date}_{name}-{version}-{runtime}.tar.gz";
        GZipCompress("./build", zipName);
    });


Task("docker")
    .IsDependentOn("build")
	.Does(() => {
		Environment.SetEnvironmentVariable("IMPORT_COORDINATOR_VERSION", version);
		DockerComposeBuild(new DockerComposeBuildSettings
		{
			Files = new[] { "./docker/docker-compose.yml" },
			ForceRm = true
		}, new [] { "import-coordinator" });
		
	});


Task("publish")
	.Does(() => {
		DockerLogin("app", "Zxp107qg6VN+asf!Q324fGteQtxDmeU3o6", "app.azurecr.io");
		DockerPush($"app.azurecr.io/import-coordinator:{version}");
		
	});
	
RunTarget(target);
