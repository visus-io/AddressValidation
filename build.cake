#addin nuget:?package=Cake.Coverlet&version=4.0.1
#tool dotnet:?package=GitVersion.Tool&version=6.1.0

var configuration = Argument("configuration", "Release");
var target = Argument("target", "Test");
var testProjectFiles = GetFiles("./tests/**/*.csproj");

var defaultBuildSettings = new DotNetBuildSettings
{
    Configuration = configuration
};

var defaultTestSettings = new DotNetTestSettings
{
	ArgumentCustomization = args => args.Append("--logger:trx"),
    Configuration = configuration,
    NoBuild = true,
    NoRestore = true    
};

var coverletSettings = new CoverletSettings
{
    CollectCoverage = true,
    CoverletOutputFormat = CoverletOutputFormat.opencover,
    CoverletOutputName = "coverage.opencover.xml"
};

Task("Clean")
    .Does(() => 
{
    CleanDirectories("./src/**/bin/{configuration}");
    CleanDirectories("./src/**/obj");    
    CleanDirectories("./tests/**/bin/{configuration}");
    CleanDirectories("./tests/**/obj");
    CleanDirectories("./tests/**/TestResults");
    
    DeleteFiles("./tests/**/coverage.opencover.xml");    
});

Task("Build")
    .IsDependentOn("Clean")
    .Does(() => 
{
    DotNetBuild("./AddressValidation.sln", defaultBuildSettings);
});

Task("Test")
    .IsDependentOn("Clean")
    .IsDependentOn("Build")
    .Does(() => 
{
    foreach (var projectFile in testProjectFiles)
    {
        DotNetTest(projectFile.FullPath, defaultTestSettings, coverletSettings);
    }  
});

RunTarget(target);
