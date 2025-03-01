# Define the array of branches you want to iterate over
$Branches = @("master", "1.39.1", "1.40.0")  # Add your branches here

# Define variables
$BuildCommand = "dotnet build -c Release"  # Change this to your actual build command
$BuildDir = "bin\Release\zip"   # Path to build output
$DestDir = "../Config-Share-Deployment"  # Destination folder for the build files

# Stop script on error
$ErrorActionPreference = "Stop"

foreach ($Branch in $Branches) {
    Write-Host "🚀 Switching to branch: $Branch" -ForegroundColor Cyan
    git checkout $Branch
    git pull origin $Branch

    Write-Host "🛠️ Running build for branch: $Branch" -ForegroundColor Green
    Invoke-Expression $BuildCommand

    # Create a subdirectory for each branch in the destination folder
    $BranchDestDir = Join-Path -Path $DestDir -ChildPath $Branch

    Write-Host "📂 Copying build files to $BranchDestDir for branch: $Branch" -ForegroundColor Yellow
    if (!(Test-Path $BranchDestDir)) {
        New-Item -ItemType Directory -Path $BranchDestDir | Out-Null
    }

    # Copy build files to the unique folder for the branch
    Copy-Item -Path "$BuildDir\*" -Destination $BranchDestDir -Recurse -Force

    Write-Host "✅ Build and deployment completed for branch: $Branch!" -ForegroundColor Green
    Write-Host "-----------------------------------------------" -ForegroundColor DarkGray
}
Read-Host "Press any key to exit ..."
