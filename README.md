# SpineViewerWPF
A tool that can view spine files with different spine-runtime versions and export to gif or png.

中文說明請看[這裡](README_zhTW.md)

## Usage
Start by loading an Atlas file for the specific spine you want to view, and the program will then try to import the skeleton data in JSON format and the sprite-atlas in PNG format. All 3 files must be named according to the atlas data (eg: Spine1.atlas, Spine1.json, Spine1.png). Make sure to select the correct spine version according to your skeleton data, or else there may be visual bugs, or the spine will not load/animate correctly. 

## Hot Key
* Ctrl+Mousewheel  Canvas Scaling
* Alt+Mousewheel  Spine Scaling
* Ctrl+Mousedown+Mousemove  Canvas Moving
* Alt+Mousedown+Mousemove  Spine Moving

## Features
* Support Spine Runtime Versions 
  * **2.1.08**
  * **2.1.25**
  * **3.1.07**
  * **3.2.xx**
  * **3.4.02**
  * **3.5.51**
  * **3.6.32**
  * **3.6.39**
  * **3.6.53**
  * **3.7.94**
  * **3.8.95**
  * **4.0.31**
  * **4.0.64**
  * **4.1.00**
  * **4.2.33**
* Export animation to gif or png file.
* Can view Animation with different options.

## Uses
Library:
- [ImageSharp](https://github.com/SixLabors/ImageSharp)
- [WpfXnaControl](https://github.com/erickeek/WpfXnaControl)
- [spine-runtimes](https://github.com/EsotericSoftware/spine-runtimes)

Requirements:
- [.NET 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Microsoft XNA Framework Redistributable 4.0](https://www.microsoft.com/en-us/download/details.aspx?id=20914)

## Changelog
* 31/01/2025 (Eleiyas)
  * Upgraded program to .NET 8.0
  * Updated packages to latest versions
  * Imported MonoGame packages to retain usage of Microsoft.Xna.Framework
  * Custom import of WpfXnaControl.dll file as original NuGet package is massively outdated
  * Cleaned code
  * Updated ReadMe

## Issue:
*  \_(:3」∠)\_
