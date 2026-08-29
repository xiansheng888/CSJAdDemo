param(
    [Parameter(Mandatory = $true)]
    [string]$ProjectPath
)

$ErrorActionPreference = "Stop"

function Normalize-Path {
    param([string]$PathValue)

    try {
        return [System.IO.Path]::GetFullPath($PathValue).TrimEnd('\\').ToLowerInvariant()
    }
    catch {
        return $PathValue.TrimEnd('\\').ToLowerInvariant()
    }
}

$normalizedTarget = Normalize-Path -PathValue $ProjectPath

$unityProcesses = Get-CimInstance Win32_Process -Filter "Name = 'Unity.exe'" |
    Select-Object ProcessId, CommandLine

$matching = @()
$others = @()

foreach ($proc in $unityProcesses) {
    $cmd = $proc.CommandLine

    if ([string]::IsNullOrWhiteSpace($cmd)) {
        continue
    }

    $match = [regex]::Match(
        $cmd,
        '["'']?-projectpath["'']?\s+("([^"]+)"|(\S+))',
        [System.Text.RegularExpressions.RegexOptions]::IgnoreCase
    )

    if (-not $match.Success) {
        $others += [PSCustomObject]@{
            processId = $proc.ProcessId
            projectPath = $null
            commandLine = $cmd
        }
        continue
    }

    $projectPathInProcess = $match.Groups[2].Value
    if ([string]::IsNullOrWhiteSpace($projectPathInProcess)) {
        $projectPathInProcess = $match.Groups[3].Value
    }

    $normalizedProcessPath = Normalize-Path -PathValue $projectPathInProcess

    $record = [PSCustomObject]@{
        processId = $proc.ProcessId
        projectPath = $projectPathInProcess
        commandLine = $cmd
    }

    if ($normalizedProcessPath -eq $normalizedTarget) {
        $matching += $record
    }
    else {
        $others += $record
    }
}

$result = [PSCustomObject]@{
    inputProjectPath = $normalizedTarget
    editorRunningForProject = ($matching.Count -gt 0)
    matchingProcesses = $matching
    otherUnityProcesses = $others
    recommendation = if ($matching.Count -gt 0) { "run-tests-in-open-editor" } else { "can-use-batchmode" }
}

$result | ConvertTo-Json -Depth 8
