// ReSharper disable AllUnderscoreLocalParameterName

using System.IO;
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
using static Nuke.Common.Tools.DotNet.DotNetTasks;

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
								  });

	Target Compile => _ => _
						  .DependsOn(Restore)
						  .Executes(() =>
									{
										DotNetBuildSettings settings = new DotNetBuildSettings()
																	  .SetProjectFile(Solution)
																	  .SetConfiguration(Configuration)
																	  .EnableNoLogo()
																	  .EnableNoRestore();

										DotNetBuild(settings);
									});

	AbsolutePath CoverageResultsDirectory => TemporaryDirectory / "CoverageResults";

	Target Restore => _ => _
						  .DependsOn(Clean)
						  .Executes(() =>
									{
										DotNetRestoreSettings settings = new DotNetRestoreSettings()
																		.SetProjectFile(Solution)
																		.EnableNoCache()
																		.SetConfigFile(RootDirectory / "nuget.config");

										DotNetRestore(settings);
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

											   SonarScannerBeginSettings settings = new SonarScannerBeginSettings()
																				   .SetOrganization(SonarOrganization)
																				   .SetOpenCoverPaths(openCoverReportPath)
																				   .SetVSTestReports(vsTestReportPath)
																				   .SetProjectKey(SonarProjectKey)
																				   .SetToken(SonarToken);

											   SonarScannerTasks.SonarScannerBegin(settings);
										   });

	Target SonarScanEnd => _ => _
							   .After(Test)
							   .OnlyWhenDynamic(() => IsServerBuild)
							   .Executes(() =>
										 {
											 SonarScannerTasks.SonarScannerEnd(_ => _.SetToken(SonarToken));
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

									 DotNetTestSettings settings = new DotNetTestSettings()
																  .EnableCollectCoverage()
																  .EnableNoRestore()
																  .SetConfiguration(Configuration)
																  .SetCoverletOutput($"{CoverageResultsDirectory}/")
																  .SetCoverletOutputFormat(MergedCoverletOutputFormat)
																  .SetLoggers("trx")
																  .SetProcessAdditionalArguments(arguments)
																  .SetProjectFile(Solution)
																  .SetResultsDirectory(TestResultsDirectory);

									 DotNetTest(settings);
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

	static bool IsDocumentation(string s) =>
		s.StartsWith("docs") ||
		s.StartsWith("LICENSE") ||
		s.StartsWith("README.md");
}
