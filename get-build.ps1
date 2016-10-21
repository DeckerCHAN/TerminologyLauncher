$assemblyFile = "Core\Build.txt"
    

# Get the Content of the file and store it in the  variable 
$fileContent = Get-Content $assemblyFile

Write-Host $fileContent