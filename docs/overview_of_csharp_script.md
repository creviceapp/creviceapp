
# Overview of C# Scripting

C# Scripting is a powerful and flexible scripting feature provided by [Microsoft Ryslyn](https://github.com/dotnet/roslyn). It designed to make scripts you cut and pasted from C# code work as-is. So, you can build your own gesture environment easily just only with a little of knowledge of C# language. But C# Scripting has only a few special feature C# language does not have. The following section is the introduction of the features very useful if you know it when you need to do it.

## #r directive

You can add reference to assemblies by `#r` directive. 

```cs
// Add a reference to dll file.
#r "path_to/Assembly.dll"

// Add a reference to an assembly.
#r "System.Speech"

// Error occurrs unless you have installed Visual Studio.
#r "Microsoft.VisualStudio" 
```

_Note 1: This directive should be placed on the top of your C# Scripting code._
_Note 2: This directive does not support to load NuGet package automatically. You can do it by download NuGet package by yourself, extract dll files, and add refereces to it by using `#r` directive._

## #load directive

You can load the content in another C# Scripting file by `#load` directive.

```cs
#load "path_to/another.csx"
```

_Note : This directive should be placed on the top of your C# Scripting code except for `#r` directive._
