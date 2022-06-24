# NetworkDriveLauncher

A plugin for [Flow launcher](https://github.com/Flow-Launcher/Flow.Launcher).

# Motivation

On my day-to-day work, there are many project folders on different network paths that I need to frequently access. This plugin helps access these folders by typing some of the words of the folders.

Given a list of paths, builds an index of the inner folders.
The folders can be then opened from the Flow Plugin by searching keywords.

## Setup

### Configuration File

First, create a `NetworkDriveLauncher.Core\appsettings.json`. A copy of the `appsettins.Development.json` will work.

The file should look something like this:
``` json
{
  "Depth": 4,
  "OverwriteIndex": true,
  "RootDirectories": [ 
    "\\Path\To\Network\Drive\Folder\",
    "\\Path\To\Another\Network\Drive\Folder\"
  ],
  "OutputFilename":  "C:\Index.txt" 
}
```

### Install the plugin

After installing Flow Launcher, execute the `NetworkDriveLauncher.FlowPlugin\Flow.Launcher.Plugin.NetworkDriveLauncher\install.ps1` powershell script. This will build the solution and copy the files the the Plugins folder.

### Build the index

The current implementation creates a plain text file. The destination filename of the index can be set in the `appsettings.json`.

After opening the Flow Launcher, type `n build`, an option is available to build the index. This needs to be executed periodically, depending on how much the folders contents change.

The `Depth` setting can be used to set the desired length of indexing:
`\Depth1\Depth2\Depth3\Depth4`. (Maybe, I do not remember if it is 0-based)

### Change settings after installation

Typing `n settings` will display an option to open the `appsettings.json` configuration file with your default file editor.

## Usage

Call your flow launcher and type `n <keywords>`. The results will be displayed in the list. Pressing `Enter` opens the windows explorer in that folder.