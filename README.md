# DartSassBuilder

> A dart-compiled version of [LibSassBuilder](https://github.com/johan-v-r/LibSassBuilder), using [DartSassHost](https://github.com/Taritsyn/DartSassHost)

| Build                                                                              | NuGet Package                                                                                                  | .NET Global Tool                                                                                                       |
| ---------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------- |
| ![Build](https://github.com/deanwiseman/DartSassBuilder/workflows/Build/badge.svg) | [![Nuget](https://img.shields.io/nuget/vpre/DartSassBuilder)](https://www.nuget.org/packages/DartSassBuilder/) | [![.NET Tool](https://img.shields.io/nuget/vpre/DartSassBuilder)](https://www.nuget.org/packages/DartSassBuilder-Tool) |

## [Nuget Package](https://www.nuget.org/packages/DartSassBuilder)

`DartSassBuilder` NuGet package adds a build task to compile Sass files to `.css`. It's compatible with both MSBuild (VS) and `dotnet build`.

No configuration is required, it will compile the files implicitly on project build.

- ### Optionally provide arguments (see _Options_ below):

```xml
<PropertyGroup>
  <!-- outputstyle option -->
  <DartSassOutputStyle>compressed</DartSassOutputStyle>
  <DartSassOutputStyle Condition="'$(Configuration)' == 'Debug'">expanded</DartSassOutputStyle>
  <!-- level option -->
  <DartSassOutputLevel>verbose</DartSassOutputLevel>
  <!-- msbuild output level -->
  <DartSassMessageLevel>High</DartSassMessageLevel>
  <!-- include paths for imports -->
  <DartSassIncludePaths>node_modules</DartSassIncludePaths>
</PropertyGroup>
```

- ### Or take control of what files to process

```xml
<PropertyGroup>
  <!-- take full-control -->
  <EnableDefaultSassItems>false</EnableDefaultSassItems>
</PropertyGroup>

<ItemGroup>
  <!-- add files manually -->
  <SassFile Include="Vendor/**/*.scss" />
  <SassFile Include="Styles/**/*.scss" Exclude="Styles/unused/**" />
</ItemGroup>
```

- ### Or ignore all previous options (except for `<DartSassMessageLevel>`) and determine the arguments to the tool yourself

```xml
<PropertyGroup>
  <!-- Take even more full-control -->
  <DartSassBuilderArgs>directory "$(MSBuildProjectDirectory)"</DartSassBuilderArgs>
  <!-- msbuild output level -->
  <DartSassMessageLevel>High</DartSassMessageLevel>
</PropertyGroup>
```

---

## [.NET Global Tool](https://www.nuget.org/packages/DartSassBuilder)

Install:

```
dotnet tool install --global DartSassBuilder
```

Use:

```
dsb [optional-path] [options]
dsb help
dsb help directory
dsb help files
```

## Generic options

```
-l, --level      Specify the level of output (silent, default, verbose)

--outputstyle    Specify the style of output (compressed, condensed, nested, expanded)
```

## Directory command (default)

Scans a directory recursively to generate .css files

```
-e, --exclude          (Default: bin obj logs node_modules) Specify explicit directories to exclude. Overrides the default.

-i, --include-paths    (Default: node_modules) List of paths that library can look in to attempt to resolve @import declarations. Overrides the default.

--help                 Display this help screen.

--version              Display version information.

value pos. 0           Directory in which to run. Defaults to current directory.
```

Example:

```
dsb directory
dsb directory sources/styles -e node_modules
dsb directory sources/styles -e node_modules -l verbose
```

Files in the following directories are excluded by default:

- `bin`
- `obj`
- `logs`
- `node_modules`

## Files command (default)

Processes the files given on the commandline

```
-i, --include-paths    (Default: node_modules) List of paths that library can look in to attempt to resolve @import declarations. Overrides the default.

--help                 Display this help screen.

--version              Display version information.

value pos. 0           File(s) to process.
```

Example:

```
dsb files sources/style/a.scss sources/vendor/b.scss
dsb files sources/style/a.scss sources/vendor/b.scss -l verbose
```

---

## Requirements

`DartSassBuilder` can be installed on any project, however the underlying build tool requires [.NET 6](https://dotnet.microsoft.com/download/dotnet/6.0) installed on the machine. (.NET 5 required with `0.1.x-beta`)

## Support

The support is largely dependant on [DartSassHost](https://github.com/Taritsyn/DartSassHost)

This tool contains the following supporting packages:

- Microsoft.ClearScript.V8.Native.win-x64
- Microsoft.ClearScript.V8.Native.linux-x64
- Microsoft.ClearScript.V8.Native.osx-x64
- Microsoft.ClearScript.V8.Native.osx-arm64

## Package as nuget package

```powershell
./package.ps1 -PackageDir 'C:/LocalPackages' -Version '1.4.0.1'
```
