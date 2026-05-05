# SugboGo

SugboGo is an ASP.NET Core MVC travel web app focused on helping people discover Cebu like a local. The current experience is designed as a polished landing page with supporting exploration pages for tours, hiking, gastronomy, and trending spots around Cebu.

## What the project is about

The app presents Cebu as a destination for:

- Local-guided tours and adventures
- Hiking trails and mountain destinations
- Food and gastronomy experiences
- Trending city and island spots

Right now, the main landing page is the most complete part of the project. It includes a hero section, featured tour cards, hiking highlights, food discovery content, and a newsletter call-to-action. The secondary pages are already routed and styled, but currently show simple "coming soon" placeholders:

- `/Explore/Tours`
- `/Explore/Hiking`
- `/Explore/Gastronomy`
- `/Explore/Spots`

The default route opens the Explore homepage:

- `/`
- `/Explore/Index`

## Tech stack

- ASP.NET Core MVC
- .NET 10 (`net10.0`)
- Razor views
- Custom CSS in `wwwroot/css/sugbogo.css`

## Prerequisites

Before running the project, make sure you have:

- [.NET 10 SDK](https://dotnet.microsoft.com/download) installed

## How to run

From the project root, run:

```powershell
dotnet restore
dotnet run
```

Then open one of the local URLs from `Properties/launchSettings.json`:

- `http://localhost:5115`
- `https://localhost:7225`

If you prefer, you can also run the project directly from Visual Studio using the `http` or `https` launch profile.

## Notes

- The app uses `ExploreController` as the default entry point instead of `HomeController`.
- During verification, package restore completed successfully, but a later build was blocked because the compiled DLL in `obj/Debug/net10.0/` was locked by another running `.NET Host` process. If that happens locally, stop the running app and build or run again.
- Restore currently reports low-severity NuGet vulnerability warnings for `NuGet.Packaging` and `NuGet.Protocol`, which come from the existing project dependency graph.

## Project structure

```text
SugboGo/
|-- Controllers/
|   |-- ExploreController.cs
|   `-- HomeController.cs
|-- Views/
|   |-- Explore/
|   `-- Shared/
|-- wwwroot/
|   |-- css/
|   |-- images/
|   `-- js/
|-- Properties/
|   `-- launchSettings.json
|-- Program.cs
`-- SugboGo.csproj
```
