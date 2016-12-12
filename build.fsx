#r "./src/packages/FAKE.4.1.3/tools/FakeLib.dll"

open Fake
open System.IO;

RestorePackages()
 
// Properties
let deployDir = "./deploy/"
 
// version info
let version = environVarOrDefault "PackageVersion" "3.0.0.0"  // or retrieve from CI server
let summary = "Open source portable .NET library providing sockets functionality to a variety of platforms."
let copyright = "Ian Bebbington, 2014"
let tags = "tcp udp WinRT UAP socket portable"
let description = "Open source portable .NET library providing sockets functionality to a variety of platforms."

let allAssemblies = [ "OneCog.Net.Common.dll"; "OneCog.Net.Common.pdb"; "OneCog.Net.Instrumentation.dll"; "OneCog.Net.Instrumentation.pdb"; "OneCog.Net.dll"; "OneCog.Net.pdb" ]
let sourcePath = "src"
let commonPath  = "OneCog.Net.Common"
let universalPath = "OneCog.Net.Uwp"
let desktopPath = "OneCog.Net.Desktop"
let configuration = "Release"
let binPath = "bin"

let libDir = "lib"
let srcDir = "src"
let win8Target = "portable-win81+wpa81"
let wp8Target = "wp8"
let net45Target = "net45"
let sl5Target = "sl5"
let pclTarget = "portable-net45+wp8+win81+wpa81"
let uapTarget = "uap10.0"
let netStandardTarget = "netstandard1.4"
let srcTarget = "src"
 
// Targets
Target "Clean" (fun _ ->
    CleanDirs [ deployDir ]
)
 
Target "Build" (fun _ ->
   !! "./src/**/*.csproj"
     |> MSBuildRelease "" "Build"
     |> Log "AppBuild-Output: "
)

Target "Test" (fun _ ->
    !! ("./src/**/*.Test/"  @@ binPath @@ configuration @@ "*.Test.dll")
    ++ ("./src/**/*.Tests/"  @@ binPath @@ configuration @@ "*.Tests.dll")
      |> NUnit (fun p ->
          {p with
             DisableShadowCopy = true;
             OutputFile = deployDir @@ "TestResults.xml" })
)

Target "Package" (fun _ ->

    CopyWithSubfoldersTo deployDir [ !! "./src/**/*.cs" ]
    allAssemblies |> List.map(fun a -> sourcePath @@ universalPath @@ binPath @@ configuration @@ a) |> Copy (deployDir @@ universalPath)
    allAssemblies |> List.map(fun a -> sourcePath @@ desktopPath @@ binPath @@ configuration @@ a) |> Copy (deployDir @@ desktopPath)

    let win8Files = allAssemblies |> List.map(fun a -> (universalPath @@ a, Some(Path.Combine(libDir, win8Target)), None))
    let wp8Files = allAssemblies  |> List.map(fun a -> (universalPath @@ a, Some(Path.Combine(libDir, wp8Target)), None))
    let net45Files = allAssemblies |> List.map(fun a -> (desktopPath @@ a, Some(Path.Combine(libDir, net45Target)), None))
    let sl5Files = allAssemblies |> List.map(fun a -> (universalPath @@ a, Some(Path.Combine(libDir, sl5Target)), None))
    let pclFiles = allAssemblies |> List.map(fun a -> (universalPath @@ a, Some(Path.Combine(libDir, pclTarget)), None))
    let uapFiles = allAssemblies |> List.map(fun a -> (universalPath @@ a, Some(Path.Combine(libDir, uapTarget)), None))
    let netStandardFiles = allAssemblies |> List.map(fun a -> (universalPath @@ a, Some(Path.Combine(libDir, netStandardTarget)), None))
    let srcFiles = [ (@"src\**\*.*", Some "src", None) ]

    NuGet (fun p -> 
        {p with
            Authors = [ "Ian Bebbington" ]
            Project = "OneCog.Net"
            Description = description
            Summary = summary
            Copyright = copyright
            Tags = tags
            OutputPath = deployDir
            WorkingDir = deployDir
            SymbolPackage = NugetSymbolPackage.Nuspec
            Version = version
            Files = win8Files @ wp8Files @ net45Files @ sl5Files @ pclFiles @ uapFiles @ netStandardFiles @ srcFiles
            Publish = false }) 
            "./src/OneCog.Net.nuspec"
)

Target "Run" (fun _ -> 
    trace "FAKE build complete"
)
  
// Dependencies
"Clean"
  ==> "Build"
  ==> "Test"
  ==> "Package"
  ==> "Run"
 
// start build
RunTargetOrDefault "Run"