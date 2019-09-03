# �Ŀ��� ȯ�溯���� �н� �߰�.
# msbuild.exe �� ����ϱ� ���� �Ʒ� �н��� �߰��Ѵ�.
$env:Path += ";C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin"
#$env:Path += ";C:\Windows\Microsoft.NET\Framework64\v4.0.30319"

# ������� ���ϱ�.
Function GetVersion() {
    $version = "1.0.0";
    $buildNumber = 1111;
    
    $buildNumberFilePath = ".\BuildNo.txt";
    $versionFilePath = ".\Version.txt";
    
    if (Test-Path $versionFilePath) {
        $version = [IO.File]::ReadAllText($versionFilePath, [System.Text.UTF8Encoding]::UTF8);
    } else {
        [IO.File]::WriteAllText($versionFilePath, $version, [System.Text.UTF8Encoding]::UTF8);
    }

    # �����ȣ ���ϱ�.
    if (Test-Path $buildNumberFilePath) {
        $buildNumber = [int][IO.File]::ReadAllText($buildNumberFilePath, [System.Text.UTF8Encoding]::UTF8);
    } else {
        $buildNumber = 0;
    }

    $buildNumber = $buildNumber + 1;
    [IO.File]::WriteAllText($buildNumberFilePath, $buildNumber, [System.Text.UTF8Encoding]::UTF8);

    $versionbuildString = "$version.$buildNumber"
    #��ġ������ nsh ���Ͽ� �����Ѵ�.
    [IO.File]::WriteAllText(".\Assembly_Version.nsh", ('!define PRODUCT_VERSION "' + $versionbuildString + '"'), [System.Text.ASCIIEncoding]::ASCII);

    return $versionbuildString;
}

Function RunCodeSign([string[]] $fileList) {
    #���ڼ��� ����.
    Try {
        #���ڼ��� ����
        $password = "347V5X";
        $timestempUrl = "http://timestamp.comodoca.com/authenticode";
        $cerfFile = ".\CodeSignTools\20170314-qbj-MEGAZONE.pfx";

        cls
        echo "Code sign start... "
        $startTime = Get-Date
        $fileCount = 0
        foreach($filePath in $fileList)
        {
            $fileCount++
            .\CodeSignTools\signtool.exe sign /f $cerfFile /p $password /t $timestempUrl /v $filePath
        }
        $endTime = Get-Date
        echo "Code sign finish... "
        echo "Signed File Count : $fileCount"
        echo "Start Time : $startTime ~ End Time : $endTime"
        echo "Elapsed Time : $($endTime - $startTime)"

        return true;
    } Catch {
        #[System.Windows.MessageBox]::Show('Setup script ���忡�� ������ �߻��Ͽ����ϴ�.');
        Break;
    }
    return false;
}

Function RunBuild([string] $BuildPath, [string] $BuildMode, [string] $Version) {
    $isSuccess = false;
    try {
        msbuild.exe $BuildPath /property:Configuration=$BuildMode /property:Version=$Version
        $isSuccess = true;
    } catch {
        break;
    }
    return $isSuccess;
}


# ���� ���� ����.
#$buildMode = "RELEASE"
#$releaseBuildDir = "..\Megazone.HyperSubtitleEditor\bin\Release";

$buildMode = "STAGING"
$releaseBuildDir = "..\Megazone.HyperSubtitleEditor\bin\Staging";

$buildFilePath = "..\Megazone.HyperSubtitleEditor.sln";
$buildversion = GetVersion;
$installJoblDir = ".\Install_Files";
echo "build version : $buildversion";
$canCodesign = "False";


# Build Part...
#{
    if (Test-Path $releaseBuildDir) {
        rm -r $releaseBuildDir
    } 

    Try {
        # ����
        msbuild.exe "..\Megazone.HyperSubtitleEditor.sln" /property:Configuration=$buildMode /property:Version=$buildversion
    } Catch {
        echo "[ERROR] ���嵵���� �����߿� �����߻�."
        Break;
    }
#}

# ���� ���ϵ��� �¾� �۾� ������ �����Ѵ�.
if (Test-Path $installJoblDir) {
    rm -r $installJoblDir
} 
mkdir $installJoblDir

# ����
XCopy "$releaseBuildDir\*.dll" $installJoblDir
XCopy "$releaseBuildDir\*.exe" $installJoblDir
XCopy "$releaseBuildDir\*.txt" $installJoblDir
XCopy "$releaseBuildDir\PreferedLanguageInfo.json" $installJoblDir
Copy-Item -Path $releaseBuildDir\ko-kr -Destination $installJoblDir -Recurse -Container 


# Code sign part...
if ($canCodesign.Equals("True")) {
    Try {
        #���ڼ��� ��� ���� ���ϱ�.
        $dllList = Get-Item -Path ".\Install_Files\Megazone.*.*"
        $exeList = Get-Item -Path ".\Install_Files\*.exe"
        #$lang_jp = Get-Item -Path ".\Install_Files\ja-jp\Megazone.*.dll"
        $lang_ko = Get-Item -Path ".\Install_Files\ko-kr\Megazone.*.dll"
        
        #$fileList = $dllList + $exeList + $lang_jp + $lang_ko
        $fileList = $dllList + $exeList + $lang_ko

        #���ڼ��� ����
        $password = "347V5X";
        $timestempUrl = "http://timestamp.comodoca.com/authenticode";
        $cerfFile = ".\CodeSignTools\20170314-qbj-MEGAZONE.pfx";

        cls
        echo "Code sign start... "
        $startTime = Get-Date
        $fileCount = 0
        foreach($filePath in $fileList)
        {
            $fileCount++
            &.\CodeSignTools\signtool.exe sign /f $cerfFile /p $password /t $timestempUrl /v $filePath
        }
        $endTime = Get-Date
        echo "Code sign finish... "
        echo "Signed File Count : $fileCount"
        echo "Start Time : $startTime ~ End Time : $endTime"
        echo "Elapsed Time : $($endTime - $startTime)"

    } Catch {
        echo "[ERROR] Setup script ���忡�� ������ �߻��Ͽ����ϴ�. ���ڼ����߿� ���� �߻�."
        #[System.Windows.MessageBox]::Show('Setup script ���忡�� ������ �߻��Ͽ����ϴ�.');
        #Break;
    }
}

#Make setup file... 
#{
    Try{
        if (Test-Path .\Setup.exe) {
            Remove-Item .\Setup.exe
        }
        Start-Sleep -m 500
        # Setup File Build.
        &'C:\Program Files (x86)\NSIS\makensis.exe' "NSIS_Script.nsi"

    } Catch {
        echo "[ERROR] NSIS_Script.nsi ���� ����, Setup ���� ������ �߻��߽��ϴ�. ��ũ��Ʈ�� Ȯ���ϼ���."
        #[System.Windows.MessageBox]::Show('Setup script ���忡�� ������ �߻��Ͽ����ϴ�.');
        #Break;
    }

    Try {
        #0.5�� ���.
        # https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.utility/start-sleep?view=powershell-6
        Start-Sleep -m 800

        #Setup File ���ڼ���.
        .\CodeSignTools\signtool.exe sign /f $cerfFile /p $password /t $timestempUrl /v ".\Setup.exe"

        Start-Sleep -m 500
    } Catch {
        echo "[ERROR] Setup ���� ���ڼ��� �� ������ �߻��߽��ϴ�. ��ũ��Ʈ�� Ȯ���ϼ���."
        #[System.Windows.MessageBox]::Show('Setup ���� ���ڼ��� �� ������ �߻��߽��ϴ�. ��ũ��Ʈ�� Ȯ���ϼ���.');
        #Break;
    }
#}

# Rename setup file name...
#{
    #$stageName = "Staging"; // Staging�̸� 'Staging'���� ����, ������������������� �������� ó��.
    $outputSetup = "Hyper-Subtitle-Editor-Setup-" + $buildversion + ".exe";

    if($buildMode.Equals("STAGING")) {
        $outputSetup = "Hyper-Subtitle-Editor-Setup-" + $buildversion + "-Staging.exe";
    }

    if (Test-Path .\$outputSetup) {
        Remove-Item .\$outputSetup
    }
    Rename-Item -Path ".\Setup.exe" -NewName $outputSetup
    
    if (!(Test-Path ".\SetupFiles")) {
        MKDIR "SetupFiles"
    }
    Move-Item -Path $outputSetup ".\SetupFiles"

    #[System.Windows.MessageBox]::Show('�۾� �Ϸ�. �¾������� Ȯ���ϼ���.');
    echo $buildMode + " Setup Build Finish..."
#}
