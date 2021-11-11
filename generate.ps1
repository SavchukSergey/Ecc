function ProcessFile([string]$filePath, [int]$bits) {
    $content = Get-Content -Raw $filePath
    $content = $content.Replace("512", ($bits * 2).ToString())
    $content = $content.Replace("256", $bits.ToString())
    $content = $content.Replace("128", ($bits / 2).ToString())
    $newFileName = $filePath.Replace("256", $bits.ToString())
    Set-Content -NoNewLine -Path $newFileName -Value $content
}

Get-ChildItem **/*256* -recurse | Foreach-Object {
    ProcessFile $_.FullName 512;
}