# 파워쉘 환경변수에 패스 추가.
# msbuild.exe 를 사용하기 위해 아래 패스를 추가한다.
$env:Path += ";C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin"
#$env:Path += ";C:\Windows\Microsoft.NET\Framework64\v4.0.30319"

# 빌드버전 구하기.
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

    # 빌드번호 구하기.
    if (Test-Path $buildNumberFilePath) {
        $buildNumber = [int][IO.File]::ReadAllText($buildNumberFilePath, [System.Text.UTF8Encoding]::UTF8);
    } else {
        $buildNumber = 0;
    }

    $buildNumber = $buildNumber + 1;
    [IO.File]::WriteAllText($buildNumberFilePath, $buildNumber, [System.Text.UTF8Encoding]::UTF8);

    $versionbuildString = "$version.$buildNumber"
    #설치정보를 nsh 파일에 적용한다.
    [IO.File]::WriteAllText(".\Assembly_Version.nsh", ('!define PRODUCT_VERSION "' + $versionbuildString + '"'), [System.Text.ASCIIEncoding]::ASCII);

    return $versionbuildString;
}

Function RunCodeSign([string[]] $fileList) {
    #전자서명 수행.
    Try {
        #전자서명 정보
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
        #[System.Windows.MessageBox]::Show('Setup script 빌드에서 오류가 발생하였습니다.');
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


# 빌드 조건 설정.
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
        # 빌드
        msbuild.exe "..\Megazone.HyperSubtitleEditor.sln" /property:Configuration=$buildMode /property:Version=$buildversion
    } Catch {
        echo "[ERROR] 빌드도구로 빌드중에 오류발생."
        Break;
    }
#}

# 빌드 파일들을 셋업 작업 폴더에 복사한다.
if (Test-Path $installJoblDir) {
    rm -r $installJoblDir
} 
mkdir $installJoblDir

# 복사
XCopy "$releaseBuildDir\*.dll" $installJoblDir
XCopy "$releaseBuildDir\*.exe" $installJoblDir
XCopy "$releaseBuildDir\*.txt" $installJoblDir
XCopy "$releaseBuildDir\PreferedLanguageInfo.json" $installJoblDir
Copy-Item -Path $releaseBuildDir\ko-kr -Destination $installJoblDir -Recurse -Container 


# Code sign part...
if ($canCodesign.Equals("True")) {
    Try {
        #전자서명 대상 파일 구하기.
        $dllList = Get-Item -Path ".\Install_Files\Megazone.*.*"
        $exeList = Get-Item -Path ".\Install_Files\*.exe"
        #$lang_jp = Get-Item -Path ".\Install_Files\ja-jp\Megazone.*.dll"
        $lang_ko = Get-Item -Path ".\Install_Files\ko-kr\Megazone.*.dll"
        
        #$fileList = $dllList + $exeList + $lang_jp + $lang_ko
        $fileList = $dllList + $exeList + $lang_ko

        #전자서명 정보
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
        echo "[ERROR] Setup script 빌드에서 오류가 발생하였습니다. 전자서명중에 오류 발생."
        #[System.Windows.MessageBox]::Show('Setup script 빌드에서 오류가 발생하였습니다.');
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
        echo "[ERROR] NSIS_Script.nsi 빌드 오류, Setup 파일 생성중 발생했습니다. 스크립트를 확인하세요."
        #[System.Windows.MessageBox]::Show('Setup script 빌드에서 오류가 발생하였습니다.');
        #Break;
    }

    Try {
        #0.5초 대기.
        # https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.utility/start-sleep?view=powershell-6
        Start-Sleep -m 800

        #Setup File 전자서명.
        .\CodeSignTools\signtool.exe sign /f $cerfFile /p $password /t $timestempUrl /v ".\Setup.exe"

        Start-Sleep -m 500
    } Catch {
        echo "[ERROR] Setup 파일 전자서명 중 오류가 발생했습니다. 스크립트를 확인하세요."
        #[System.Windows.MessageBox]::Show('Setup 파일 전자서명 중 오류가 발생했습니다. 스크립트를 확인하세요.');
        #Break;
    }
#}

# Rename setup file name...
#{
    #$stageName = "Staging"; // Staging이면 'Staging'으로 설정, 릴리즈배포버전에서는 공백으로 처리.
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

    #[System.Windows.MessageBox]::Show('작업 완료. 셋업파일을 확인하세요.');
    echo $buildMode + " Setup Build Finish..."
#}
