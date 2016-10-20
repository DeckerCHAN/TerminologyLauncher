$assemblyFile = "Core\Engine.cs"

$RegularExpression = [regex] '(?<=public\sint\sBuildVersion\s\=\>\s)(.*)(?=\;)'
    

# Get the Content of the file and store it in the  variable 
$fileContent = Get-Content $assemblyFile
#$fileContent

foreach($content in $fileContent)
{
    $match = [System.Text.RegularExpressions.Regex]::Match($content, $RegularExpression)
    if($match.Success) {
      $match.groups[1].value 
    }
}