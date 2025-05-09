# Parameters to control versioning
param (
    [string]$type = "patch"  # Options: "major", "minor", "patch"
)

# Step 1: Manage Versioning
$versionFile = "./configs/version.txt"

if (!(Test-Path $versionFile)) {
    "0.0.1" | Out-File $versionFile  # Initialize if missing
}

$version = Get-Content $versionFile
$versionParts = $version -split "\."

if ($type -eq "major") {
    $versionParts[0] = [int]$versionParts[0] + 1
    $versionParts[1] = 0
    $versionParts[2] = 0
} elseif ($type -eq "minor") {
    $versionParts[1] = [int]$versionParts[1] + 1
    $versionParts[2] = 0
} else {
    $versionParts[2] = [int]$versionParts[2] + 1
}

$newVersion = "$($versionParts[0]).$($versionParts[1]).$($versionParts[2])"
$newVersion | Out-File $versionFile
Write-Output "New version: $newVersion"

# Step 2: Define Output Paths
$basePublishFolder = "bin\Release\net9.0\win-x64"
$publishFolder = "$basePublishFolder\publish"  # Default publish path
$versionedPublishFolder = "$basePublishFolder\SDA_v$newVersion"  # Versioned folder
$zipName = "SDA_v$newVersion.zip"

# Step 3: Clean Old Publish Folder
if (Test-Path $publishFolder) {
    Write-Output "Cleaning old publish folder: $publishFolder..."
    Remove-Item -Recurse -Force $publishFolder
}

# Step 4: Build the .NET Executable
Write-Output "Building the project..."
dotnet publish -c Release -r win-x64 --self-contained true

# Step 5: Move Published Files to Versioned Folder
if (!(Test-Path $publishFolder)) {
    Write-Output "ERROR: Publish folder not found! Check the .NET build process."
    exit 1
}

Write-Output "Moving publish files to $versionedPublishFolder..."
Move-Item -Path $publishFolder -Destination $versionedPublishFolder -Force

# Step 6: Copy Additional Files (configs, dataStore, etc.)
Write-Output "Copying additional files..."
$extraFiles = @("configs", "dataStore", "output")
foreach ($folder in $extraFiles) {
    $sourcePath = ".\$folder"
    $destinationPath = Join-Path -Path $versionedPublishFolder -ChildPath $folder

    if (Test-Path $sourcePath) {
        Write-Output "Copying contents of $sourcePath to $destinationPath..."

        # Ensure destination directory exists (Create if missing)
        if (!(Test-Path $destinationPath)) {
            New-Item -ItemType Directory -Path $destinationPath -Force | Out-Null
        }

        # Get all files and directories inside the source directory
        Get-ChildItem -Path $sourcePath -Force | ForEach-Object {
            $targetPath = Join-Path -Path $destinationPath -ChildPath $_.Name
            Copy-Item -Path $_.FullName -Destination $targetPath -Recurse -Force -ErrorAction Stop
        }
    } else {
        Write-Output "Warning: Source folder $sourcePath not found, skipping..."
    }
}

# Step 7: Package Everything into a ZIP
Write-Output "Packaging to $zipName..."
Compress-Archive -Path "$versionedPublishFolder\*" -DestinationPath $zipName -Force

Write-Output "Build and packaging complete: $zipName (Version: $newVersion)"
