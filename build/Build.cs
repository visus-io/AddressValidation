// ReSharper disable AllUnderscoreLocalParameterName

using System.IO;
using Amazon.S3;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.SonarScanner;
using Serilog;
using Spectre.Console;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.SonarScanner.SonarScannerTasks;

[UnsetVisualStudioEnvironmentVariables]
[DotNetVerbosityMapping]
class Build : NukeBuild
{
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Required]
    [GitVersion(Framework = "net8.0", NoCache = true, NoFetch = true)]
    readonly GitVersion GitVersion;

    readonly string MergedCoverletOutputFormat = $"\\\"{CoverletOutputFormat.opencover},{CoverletOutputFormat.json}\\\"";

    [Solution(GenerateProjects = true)]
    readonly Solution Solution;

    Target Clean => _ => _
                        .Before(Restore)
                        .Executes(() =>
                                  {
                                      DotNetClean(c => c.SetProject(Solution));

                                      if ( Directory.Exists(CoverageResultsDirectory) )
                                      {
                                          Directory.Delete(CoverageResultsDirectory, true);
                                      }

                                      if ( Directory.Exists(TestResultsDirectory) )
                                      {
                                          Directory.Delete(TestResultsDirectory, true);
                                      }

                                      if ( Directory.Exists(DocsArtifactsDirectory) )
                                      {
                                          Directory.Delete(DocsArtifactsDirectory, true);
                                      }
                                  });

    Target Compile => _ => _
                          .DependsOn(Restore)
                          .Executes(() =>
                                    {
                                        DotNetBuild(_ => _
                                                        .SetProjectFile(Solution)
                                                        .SetConfiguration(Configuration)
                                                        .EnableNoLogo()
                                                        .EnableNoRestore());
                                    });

    Target CompileDocs => _ => _
                              .DependsOn(Compile)
                              .Executes(() =>
                                        {
                                            AbsolutePath docFxConfig = DocsDirectory / "docfx.json";
                                            DotNet($"docfx {docFxConfig} -o {DocsArtifactsDirectory}");
                                        });

    AbsolutePath CoverageResultsDirectory => TemporaryDirectory / "CoverageResults";

    Target DeployDocs => _ => _
                             .DependsOn(CompileDocs)
                             .Executes(async () =>
                                       {
                                           AwsS3Tasks awsS3Tasks = new(new AmazonS3Client());
                                           string bucketName = EnvironmentInfo.GetVariable<string>("AWS_S3_BUCKET_NAME");

                                           await AnsiConsole.Status()
                                                            .StartAsync("Starting...",
                                                                        async ctx =>
                                                                        {
                                                                            ctx.Status("Emptying bucket...");
                                                                            await awsS3Tasks.EmptyAsync(bucketName)
                                                                                            .ConfigureAwait(false);

                                                                            ctx.Status("Uploading documentation...");
                                                                            await awsS3Tasks.UploadAsync(DocsArtifactsDirectory,
                                                                                                         bucketName)
                                                                                            .ConfigureAwait(false);
                                                                        })
                                                            .ConfigureAwait(false);
                                       });

    AbsolutePath DocsArtifactsDirectory => TemporaryDirectory / "docs";

    AbsolutePath DocsDirectory => RootDirectory / "docs";

    Target Restore => _ => _
                          .DependsOn(Clean)
                          .Executes(() =>
                                    {
                                        DotNetToolRestore();
                                        DotNetRestore(_ => _
                                                          .SetProjectFile(Solution)
                                                          .EnableNoCache()
                                                          .SetConfigFile(RootDirectory / "nuget.config"));
                                    });

    string SonarOrganization => EnvironmentInfo.GetVariable<string>("SONAR_ORGANIZATION");

    string SonarProjectKey => EnvironmentInfo.GetVariable<string>("SONAR_PROJECT_KEY");

    Target SonarScanBegin => _ => _
                                 .After(Restore)
                                 .OnlyWhenDynamic(() => IsServerBuild)
                                 .Executes(() =>
                                           {
                                               AbsolutePath openCoverReportPath = CoverageResultsDirectory / "coverage.opencover.xml";
                                               AbsolutePath vsTestReportPath = TestResultsDirectory / "*.trx";

                                               SonarScannerBegin(_ => _
                                                                     .SetOrganization(SonarOrganization)
                                                                     .SetOpenCoverPaths(openCoverReportPath)
                                                                     .SetVSTestReports(vsTestReportPath)
                                                                     .SetProjectKey(SonarProjectKey)
                                                                     .SetToken(SonarToken));
                                           });

    Target SonarScanEnd => _ => _
                               .After(Test)
                               .OnlyWhenDynamic(() => IsServerBuild)
                               .Executes(() =>
                                         {
                                             SonarScannerEnd(_ => _.SetToken(SonarToken));
                                         });

    string SonarToken => EnvironmentInfo.GetVariable<string>("SONAR_TOKEN");

    Target Test => _ => _
                       .DependsOn(Restore, SonarScanBegin)
                       .Executes(() =>
                                 {
                                     string[] arguments =
                                     [
                                         $"-p:MergeWith={CoverageResultsDirectory}/coverage.json",
                                         "-m:1"
                                     ];

                                     DotNetTest(_ => _
                                                    .EnableCollectCoverage()
                                                    .EnableNoRestore()
                                                    .SetConfiguration(Configuration)
                                                    .SetCoverletOutput($"{CoverageResultsDirectory}/")
                                                    .SetCoverletOutputFormat(MergedCoverletOutputFormat)
                                                    .SetLoggers("trx")
                                                    .SetProcessAdditionalArguments(arguments)
                                                    .SetProjectFile(Solution)
                                                    .SetResultsDirectory(TestResultsDirectory));
                                 })
                       .Triggers(SonarScanEnd);

    AbsolutePath TestResultsDirectory => TemporaryDirectory / "TestResults";

    public static int Main() => Execute<Build>(e => e.Compile);

    protected override void OnBuildInitialized()
    {
        base.OnBuildInitialized();

        if ( IsServerBuild )
        {
            Log.Logger = new LoggerConfiguration()
                        .Enrich.FromLogContext()
                        .MinimumLevel.Information()
                        .WriteTo.Logger(Log.Logger)
                        .CreateLogger();
        }
    }
}
