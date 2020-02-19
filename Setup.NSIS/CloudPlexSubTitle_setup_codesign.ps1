
# c:\signcode -spc comodossl.spc -v comodossl.pvk -n ComodosslCodesign -t http://timestamp.comodoca.com/authenticode 

#SignTool.exe(서명 도구) https://msdn.microsoft.com/ko-kr/library/8s9b9yaz(v=vs.110).aspx
# 0 : Execution was successful.
# signtool sign /f MyCert.pfx /p MyPassword MyFile.exe 
# signtool.exe sign /f [SHA-1인증서이름].pfx /p [인증서패스워드] /tr [타임스탬프주소] /v [서명할파일].cab

# 빌드 조건 설정.
$buildversion = "0.8.0.11";
$coustomName = ""; #Types: NEXON, ATOMY, GAMEVIL, GENERAL

#전자서명 대상 파일 구하기.
$dllList = Get-Item -Path ".\Install_Files\Megazone.*.*"
$exeList = Get-Item -Path ".\Install_Files\*.exe"
$lang_ko = Get-Item -Path ".\Install_Files\ko-kr\Megazone.*.dll"

#$dllList1 = Get-Item -Path ".\Install_Files\HyperSubtitleEditor\Megazone.*.*"
#$exeList1 = Get-Item -Path ".\Install_Files\HyperSubtitleEditor\*.exe"
#$lang_ko1 = Get-Item -Path ".\Install_Files\HyperSubtitleEditor\ko-kr\Megazone.*.dll"

$fileList = $dllList + $exeList + $lang_jp + $lang_ko + $lang_jp1 


#전자서명 정보
$password = "347V5X";
$timestempUrl = "http://timestamp.comodoca.com/authenticode";
$cerfFile = ".\CodeSignTools\20170314-qbj-MEGAZONE.pfx";

#전자서명 수행.
foreach($filePath in $fileList)
{
    .\CodeSignTools\signtool.exe sign /f $cerfFile /p $password /t $timestempUrl /v $filePath
}

# Setup File Build.
&'C:\Program Files (x86)\NSIS\makensis.exe' ".\NSIS_Script.nsi"

#Setup File 전자서명.
.\CodeSignTools\signtool.exe sign /f $cerfFile /p $password /t $timestempUrl /v ".\Setup.exe"

$outputSetup = "Megazone-CloudPlex-Media-Caption-Editor-Setup-" + $buildversion + ".exe";
echo $outputSetup;
Rename-Item -Path ".\Setup.exe" -NewName $outputSetup;
