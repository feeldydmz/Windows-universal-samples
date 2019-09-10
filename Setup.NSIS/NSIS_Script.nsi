; Script generated by the HM NIS Edit Script Wizard.

; HM NIS Edit Wizard helper defines
!define VersionHeader ".\Assembly_Version.nsh"
!define PROCESS_NAME "HyperSubtitleEditor.exe"
!define PRODUCT_NAME "Hyper Subtitle Editor"
#!define PRODUCT_VERSION "0.5.0.4973
!define PRODUCT_PUBLISHER "Megazone"
!define PRODUCT_WEB_SITE "http://www.megazone.com"
!define MCM_WEB_SITE "https://console.media.megazone.io"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\${PROCESS_NAME}"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"
!define COMPANY_NAME "Megazone"

!addplugindir "Plugins"

!include "LogicLib.nsh"
!include "WinVer.nsh"
!include "LangFile.nsh"
; Add extension plugin-header --------
!include "DotNetCheckerForceInstallVer.nsh"
!include "NTProfiles.nsh"
!include /NONFATAL "${VersionHeader}"

; MUI 1.67 compatible ------
!include "MUI2.nsh"

; MUI Settings
!define MUI_ABORTWARNING
!define MUI_ICON "Resources\Logo.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\win-uninstall.ico"

; Language Selection Dialog Settings
!define MUI_LANGDLL_REGISTRY_ROOT "${PRODUCT_UNINST_ROOT_KEY}"
!define MUI_LANGDLL_REGISTRY_KEY "${PRODUCT_UNINST_KEY}"
!define MUI_LANGDLL_REGISTRY_VALUENAME "NSIS:Language"
#!define MUI_LANGDLL_ALLLANGUAGES ; ��� ��� ǥ��.

; Welcome page
!define MUI_PAGE_CUSTOMFUNCTION_SHOW "WelcomeShowCallback"
!define MUI_PAGE_CUSTOMFUNCTION_LEAVE "WelcomeLeaveCallback"
!insertmacro MUI_PAGE_WELCOME
; License page
!define MUI_LICENSEPAGE_CHECKBOX
;!insertmacro MUI_PAGE_LICENSE "Resources\License.rtf"
!insertmacro MUI_PAGE_LICENSE $(license)
; Components page
#!insertmacro MUI_PAGE_COMPONENTS
; Directory page
!define MUI_PAGE_CUSTOMFUNCTION_PRE "CanDirectoryPageSkip"
!insertmacro MUI_PAGE_DIRECTORY
; Instfiles page
!insertmacro MUI_PAGE_INSTFILES
; Finish page
!define MUI_FINISHPAGE_RUN "$INSTDIR\${PROCESS_NAME}"
!define MUI_PAGE_CUSTOMFUNCTION_SHOW "FinishShowCallback"
!insertmacro MUI_PAGE_FINISH

; Uninstaller pages
!insertmacro MUI_UNPAGE_WELCOME
!define MUI_PAGE_CUSTOMFUNCTION_SHOW "un.UnPageConfirmShow"
!define MUI_PAGE_CUSTOMFUNCTION_LEAVE "un.UnPageConfirmLeave"
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH

; Language files
!insertmacro MUI_LANGUAGE "English"
#!insertmacro MUI_LANGUAGE "Japanese"
!insertmacro MUI_LANGUAGE "Korean"

!include "Resources.nsh"

; MUI end ------

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
;OutFile "Hyper-SubtitleEditor-Setup-${PRODUCT_VERSION}.exe"
OutFile "Setup.exe"
InstallDir "$PROGRAMFILES64\${COMPANY_NAME}\${PRODUCT_NAME}"
InstallDirRegKey HKLM "${PRODUCT_DIR_REGKEY}" ""
ShowInstDetails show
ShowUnInstDetails show
BrandingText "${PRODUCT_PUBLISHER}."

LicenseLangString license ${LANG_ENGLISH} "Resources\License_en.rtf"
;LicenseLangString license ${LANG_JAPANESE} "Resources\License_jp.rtf"
LicenseLangString license ${LANG_KOREAN} "Resources\License_ko.rtf"
LicenseData $(license)

;--------------------------------
#Setup File Version Information
  VIProductVersion ${PRODUCT_VERSION}"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "ProductVersion" "${PRODUCT_VERSION}"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "ProductName" "${PRODUCT_NAME}"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "Comments" " ${PRODUCT_NAME} "
  VIAddVersionKey /LANG=${LANG_ENGLISH} "CompanyName" "Megazone Corp."
#  VIAddVersionKey /LANG=${LANG_ENGLISH} "LegalTrademarks" "${PRODUCT_NAME} is a trademark of ${PRODUCT_PUBLISHER}."
  VIAddVersionKey /LANG=${LANG_ENGLISH} "LegalCopyright" "Copyright �� 2017. Megazone Corp."
  VIAddVersionKey /LANG=${LANG_ENGLISH} "FileDescription" "${PRODUCT_NAME} Setup Lanuncher"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "FileVersion" "${PRODUCT_VERSION}"
;--------------------------------

; Utility Function Start---------------------------------------------------

; Version Compare Macro ---------------------------------------------------
Function VersionCompare
	!define VersionCompare `!insertmacro VersionCompareCall`

	!macro VersionCompareCall _VER1 _VER2 _RESULT
		Push `${_VER1}`
		Push `${_VER2}`
		Call VersionCompare
		Pop ${_RESULT}
	!macroend

	Exch $1
	Exch
	Exch $0
	Exch
	Push $2
	Push $3
	Push $4
	Push $5
	Push $6
	Push $7

	begin:
	StrCpy $2 -1
	IntOp $2 $2 + 1
	StrCpy $3 $0 1 $2
	StrCmp $3 '' +2
	StrCmp $3 '.' 0 -3
	StrCpy $4 $0 $2
	IntOp $2 $2 + 1
	StrCpy $0 $0 '' $2

	StrCpy $2 -1
	IntOp $2 $2 + 1
	StrCpy $3 $1 1 $2
	StrCmp $3 '' +2
	StrCmp $3 '.' 0 -3
	StrCpy $5 $1 $2
	IntOp $2 $2 + 1
	StrCpy $1 $1 '' $2

	StrCmp $4$5 '' equal

	StrCpy $6 -1
	IntOp $6 $6 + 1
	StrCpy $3 $4 1 $6
	StrCmp $3 '0' -2
	StrCmp $3 '' 0 +2
	StrCpy $4 0

	StrCpy $7 -1
	IntOp $7 $7 + 1
	StrCpy $3 $5 1 $7
	StrCmp $3 '0' -2
	StrCmp $3 '' 0 +2
	StrCpy $5 0

	StrCmp $4 0 0 +2
	StrCmp $5 0 begin newer2
	StrCmp $5 0 newer1
	IntCmp $6 $7 0 newer1 newer2

	StrCpy $4 '1$4'
	StrCpy $5 '1$5'
	IntCmp $4 $5 begin newer2 newer1

	equal:
	StrCpy $0 0
	goto end
	newer1:
	StrCpy $0 1
	goto end
	newer2:
	StrCpy $0 2

	end:
	Pop $7
	Pop $6
	Pop $5
	Pop $4
	Pop $3
	Pop $2
	Pop $1
	Exch $0
FunctionEnd
;--------------------------------------------------------------------------

;----------------------------------------------------------------------
; 64bit Environment
Function CheckOS64
  Var /GLOBAL IsWow64
  StrCpy $IsWow64 "False"

  ; 64��Ʈ ���� üũ�ϱ�
  System::Call "kernel32::GetCurrentProcess() i .s"
  System::Call "kernel32::IsWow64Process(i s, *i .r0)"
  StrCmp $0 '0' Win32 Win64

  Win32:
;    MessageBox MB_OK 'Windows x32'
    Goto End
  Win64:
;    MessageBox MB_OK 'Windows x64'
    StrCpy $IsWow64 "True"
    SetRegView 64 ;64��Ʈ os��, 32��Ʈ�� ���α׷��� ���� �ϱ� ���ؼ� ���
  End:
FunctionEnd
;----------------------------------------------------------------------
; 64bit Environment
Function un.CheckOS64
  ; 64��Ʈ ���� üũ�ϱ�
  System::Call "kernel32::GetCurrentProcess() i .s"
  System::Call "kernel32::IsWow64Process(i s, *i .r0)"
  StrCmp $0 '0' Win32 Win64

  Win32:
;    MessageBox MB_OK 'Windows x32'
    Goto End
  Win64:
;    MessageBox MB_OK 'Windows x64'
    SetRegView 64 ;64��Ʈ os��, 32��Ʈ�� ���α׷��� ���� �ϱ� ���ؼ� ���
  End:
FunctionEnd
;----------------------------------------------------------------------
; Windows Version
; .Net 4.6.1 ��ġ ȯ�� Ȯ��.
Function CheckWindowsVersion
  ; MS official : Windows Server 2008 R2 SP1, Windows7
  ; (�����δ�, Vista������ .Net 4.6.1�� ��ġ������.)
  ; Window vista �̻󿡼��� ���� ����.
  ; Window XP, Window Server 2003���ϴ� �ȵ�.
  ; Windows OS Version Info.
  ; https://msdn.microsoft.com/ko-kr/library/windows/desktop/ms724832(v=vs.85).aspx
  ; Windows vista : 6.0
  ; Windows 7 : 6.1
  Var /GLOBAL Major
  Var /GLOBAL Minor
  File /oname=$TEMP\nsisos.dll nsisos.dll
  CallInstDLL $TEMP\nsisos.dll osversion
  StrCpy $Major $0
  StrCpy $Minor $1

  StrCpy $R0 "FALSE"
  ${If} $Major > 6
    StrCpy $R0 "TRUE"
  ${ElseIf} $Major == 6
    ${If} $Minor >= 1
      StrCpy $R0 "TRUE"
    ${EndIf}
  ${EndIf}
  Delete $TEMP\nsisos.dll
FunctionEnd

;----------------------------------------------------------------------

!define FindProc_NOT_FOUND 1
!define FindProc_FOUND 0
!macro FindProc result processName
    ExecCmd::exec "%SystemRoot%\System32\tasklist /NH /FI $\"IMAGENAME eq ${processName}$\" | %SystemRoot%\System32\find /I $\"${processName}$\""
    Pop $0 ; The handle for the process
    ExecCmd::wait $0
    Pop ${result} ; The exit code
!macroend

; HyperBrowser.exe Process Runtime check.
Function CheckProcessRuntime
  Var /GLOBAL processFound
  StrCpy $processFound "1"

  !insertmacro FindProc $processFound "${PROCESS_NAME}"

  IntCmp $processFound ${FindProc_NOT_FOUND} Equal Less More
  Equal:
    Goto End
  Less:
    Goto IntallStop
  More:
    Goto IntallStop
  IntallStop:
    ; MSG_PROCESS_CLOSE: "${PRODUCT_NAME}�� �������Դϴ�. ${PRODUCT_NAME}�� �����Ͻʽÿ�."
    MessageBox MB_OK|MB_ICONEXCLAMATION "$(MSG_PROCESS_CLOSE)" /SD IDOK
    Abort
  End:
FunctionEnd

Function un.CheckProcessRuntime
  !insertmacro FindProc $processFound "${PROCESS_NAME}"

  IntCmp $processFound ${FindProc_NOT_FOUND} Equal Less More
  Equal:
    Goto End
  Less:
    Goto IntallStop
  More:
    Goto IntallStop
  IntallStop:
    MessageBox MB_OK|MB_ICONEXCLAMATION "$(MSG_PROCESS_CLOSE)" /SD IDOK
    Abort
  End:
FunctionEnd

;----------------------------------------------------------------------
; Language Code
Function GetWindowLocalLanguageCode
  # $LANGUAGE : �ν�Ʈ�ѷ����� ������ Language code.

  System::Call 'kernel32::GetUserDefaultUILanguage() i.r10'
  ${Switch} $R0
  ${Case} 1033
    StrCpy $R0 "en-US"
    ${Break}
  ${Case} 1041
    StrCpy $R0 "ja-JP"
    ${Break}
  ${Case} 1042
    StrCpy $R0 "ko-KR"
    ${Break}
  ${Default}
    StrCpy $R0 "en-US"
    ${Break}
  ${EndSwitch}
FunctionEnd

;----------------------------------------------------------------------
/*
[Root Key]
HKCR or HKEY_CLASSES_ROOT
HKLM or HKEY_LOCAL_MACHINE
HKCU or HKEY_CURRENT_USER
HKU or HKEY_USERS
HKCC or HKEY_CURRENT_CONFIG
HKDD or HKEY_DYN_DATA
HKPD or HKEY_PERFORMANCE_DATA
SHCTX or SHELL_CONTEXT
*/
  
; Registry SubKey
!define PRODUCT_REGISTRY_SUBKEY "SOFTWARE\${COMPANY_NAME}\${PRODUCT_NAME}"

!define REGISTRY_REGKEY_Culture "Culture"                  #��� Language
!define REGISTRY_REGKEY_InstallPath "InstallPath"          #��ġ ���
!define REGISTRY_REGKEY_InstalledVersion "Version"         #��ġ���� ����

; Custom Url Scheme
!define CUSTOM_URL_SCHEME_REGISTRY_SUBKEY "Megazone.HyperSubtitleEditor.v1"

; ������Ʈ�� ���.
Function WriteRegistry
  Var /GLOBAL RegValue_Culture

  Call GetWindowLocalLanguageCode
  Pop $R0
  # default value setting.
  StrCpy $RegValue_Culture $R0

  # install default infomation.
  WriteRegStr "HKLM" "${PRODUCT_REGISTRY_SUBKEY}" "${REGISTRY_REGKEY_Culture}" "$RegValue_Culture"
  WriteRegStr "HKLM" "${PRODUCT_REGISTRY_SUBKEY}" "${REGISTRY_REGKEY_InstallPath}" $INSTDIR
  WriteRegStr "HKLM" "${PRODUCT_REGISTRY_SUBKEY}" "${REGISTRY_REGKEY_InstalledVersion}" "${PRODUCT_VERSION}"
  
  # register custom url scheme.
  WriteRegStr "HKCR" "${CUSTOM_URL_SCHEME_REGISTRY_SUBKEY}" "URL protocol" ""
  WriteRegStr "HKCR" "${CUSTOM_URL_SCHEME_REGISTRY_SUBKEY}\shell" "" "open"
  WriteRegStr "HKCR" "${CUSTOM_URL_SCHEME_REGISTRY_SUBKEY}\shell\open" "" "command"
  WriteRegStr "HKCR" "${CUSTOM_URL_SCHEME_REGISTRY_SUBKEY}\shell\open\command" "" '"$INSTDIR\${PROCESS_NAME}" "%1"'
FunctionEnd

;----------------------------------------------------------------------
; ��ġ ���� üũ. Install Sheild�� ��ġ�� ������ ���� �ʿ�.
Function CheckInstalledVersion
  Var /GLOBAL InstalledVersion
  Var /GLOBAL CanUpdate

  ; ��ġ�� ������ ��ġ�Ϸ��� �������� ���� �������� Ȯ���Ѵ�.
  ReadRegStr $InstalledVersion HKLM "${PRODUCT_REGISTRY_SUBKEY}" "${REGISTRY_REGKEY_InstalledVersion}"

  StrCpy $CanUpdate "False"

  ${If} $InstalledVersion == ""
    Goto Version_Check_Finish
  ${EndIf}

  ${VersionCompare} $InstalledVersion "${PRODUCT_VERSION}" $R0
/*
0	Versions are equal
1	Version 1 is newer
2	Version 2 is newer
*/
  ${If} $R0 == 1
    Goto Version_IntallStop
  ${Else}
    Goto Version_InstallContinue
  ${EndIf}

  Version_IntallStop:
    ;MSG_LAST_VERSION_CHECK_TEXT: "${PRODUCT_VERSION}���� �ֽŹ����� ��ġ�Ǿ� �ֽ��ϴ�. "
    MessageBox MB_OK|MB_ICONEXCLAMATION "$(MSG_LAST_VERSION_CHECK_TEXT) " /SD IDOK
    Abort

  Version_InstallContinue:
    StrCpy $CanUpdate "True"
    Goto Version_Check_Finish

  Version_Check_Finish:
    # Nothing...
FunctionEnd

;-----------------------------------------------------------------------------------------------------
; Welcom Text ����.
Function WelcomeShowCallback
  Call CheckInstalledVersion

  ${If} $CanUpdate == "True"
    SendMessage $mui.WelcomePage.Title ${WM_SETTEXT} 0 "STR:$(MSG_UPDATE_WELCOME_PAGE_TITLE)"
    SendMessage $mui.WelcomePage.Text ${WM_SETTEXT} 0 "STR:$(MSG_UPDATE_WELCOME_PAGE_TEXT)"
  ${EndIf}
FunctionEnd

;-----------------------------------------------------------------------------------------------------
; Welcom Text ����.
Function WelcomeLeaveCallback
  Call CheckProcessRuntime
FunctionEnd

;-----------------------------------------------------------------------------------------------------
; Directory Page Skip Check
Function CanDirectoryPageSkip
  ReadRegStr $0 "HKLM" "${PRODUCT_REGISTRY_SUBKEY}" "${REGISTRY_REGKEY_InstallPath}"
  ${IF} $0 != ''
    Abort
  ${EndIf}
FunctionEnd

;-----------------------------------------------------------------------------------------------------
Function FinishShowCallback
  ${If} $CanUpdate == "True"
    SendMessage $mui.FinishPage.Title ${WM_SETTEXT} 0 "STR:$(MSG_UPDATE_FINISH_PAGE_TITLE)"
    SendMessage $mui.FinishPage.Text ${WM_SETTEXT} 0 "STR:$(MSG_UPDATE_FINISH_PAGE_TEXT)"
  ${EndIf}
FunctionEnd
;-----------------------------------------------------------------------------------------------------

Var Checkbox
Var CheckState ; Stored globally so we remember the choice if the user presses the back button and goes back to our page
!define CheckHeight 28

!ifmacrondef _Z=
!error "NSIS 2.51 or 3.0rc1 or later required!"
!endif
!macro CreateNativeControl hParent cls style exstyle x y w h text ; Note: Only supports pixel coordinates
System::Call 'USER32::CreateWindowEx(i ${exstyle}, t "${cls}", ts, i ${style}, i ${x}, i ${y}, i ${w}, i ${h}, p ${hParent}, i0, i0, i0)p.s' "${text}"
!macroend

; Uninstaller Ȯ�� ������ ���� ����.
Function un.UnPageConfirmShow
    ;FindWindow $1 "#32770" "" $HWNDPARENT ; Find inner dialog
    ;System::Call 'USER32::CreateWindowEx(i${__NSD_CheckBox_EXSTYLE},t"${__NSD_CheckBox_CLASS}",t "$(MSG_REMOVE_USER_DATA)",i${__NSD_CheckBox_STYLE}, i0, i100, i450, i25, i$1, i0, i0, i0)i.s'
    ;Pop $0
    ;SendMessage $HWNDPARENT ${WM_GETFONT} 0 0 $1
    ;SendMessage $0 ${WM_SETFONT} $1 1
    ;SetCtlColors $0 "" ${MUI_BGCOLOR} ; This is the wrong color to use...

  ; Create some extra space by reducing the height of the top text:
  System::Call *(i,i,i,i)p.r0 ; NSIS 2.51+
  System::Call 'USER32::GetWindowRect(p$mui.DirectoryPage.Text, pr0)'
  System::Call 'USER32::MapWindowPoints(i0,p$mui.DirectoryPage,p$0,i2)'
  System::Call '*$0(i.r2,i.r3,i.r4,i.r5)'
  System::Free $0
  IntOp $5 $5 - ${CheckHeight}
  System::Call 'USER32::SetWindowPos(i$mui.DirectoryPage.Text,i,i,i,i$4,i$5,i0x6)'

  ; Create and initialize the checkbox
  IntOp $5 $3 + $5 ; y = TextTop + TextHeight

  !insertmacro CreateNativeControl $mui.UnConfirmPage ${__NSD_CheckBox_CLASS} "${__NSD_CheckBox_STYLE}" "${__NSD_CheckBox_EXSTYLE}" 0 100 450 ${CheckHeight} "$(MSG_REMOVE_USER_DATA)"
  Pop $Checkbox
  SendMessage $mui.UnConfirmPage ${WM_GETFONT} 0 0 $0 ; Get the dialog font
  SendMessage $Checkbox ${WM_SETFONT} $0 1 ; and apply it to the new control
  System::Call 'USER32::SetWindowPos(i$Checkbox,i0,i,i,i,i,i0x33)' ; Make sure it is on top of the tab order
  ${IfThen} $CheckState == "" ${|} StrCpy $CheckState 0 ${|} ; Set the default if this is our first time
  ${NSD_SetState} $Checkbox $CheckState

FunctionEnd

Function un.UnPageConfirmLeave
  ${NSD_GetState} $Checkbox $CheckState
  ${If} $CheckState <> 0
    ;MessageBox mb_ok "Checkbox checked."
  ${EndIf}
  ;MessageBox mb_ok "Checkbox CheckState: $CheckState"
FunctionEnd

; Utility Function End-----------------------------------------------------

; ��ġ�Ϸ���.
Function .onInstSuccess
  Call WriteRegistry
FunctionEnd

Function .onInit
  !insertmacro MUI_LANGDLL_DISPLAY

  Var /GLOBAL ProgramDataPath
  ; Retrieving CommonApplicationData path (in $1)
  System::Call "shfolder::SHGetFolderPath(i $HWNDPARENT, i 0x0023, i 0, i 0, t.r1)"
  StrCpy $ProgramDataPath "$1\$0" -1

!define STARTMENU_PROGRAM "$ProgramDataPath\Microsoft\Windows\Start Menu\Programs"

  Call CheckOS64
  Call CheckWindowsVersion
  Pop $R0

  ${If} $R0 == 'FALSE'
    MessageBox MB_OK|MB_ICONSTOP "$(MSG_REQUIRED_OS_VERSION)" /SD IDOK
    Abort
  ${EndIf}
  
  ;WelcomPage�� Next ��ư�� Ŭ���Ҷ�, Ȯ���ϵ��� ����.
  ;���α׷��� ������Ʈ �� ��, ���α׷��� �����ϸ鼭, �¾��� �����ϸ�, ���α׷��� ������ ����Ǳ����� �¾��� ����Ǵ� ��찡 �߻�.
  #Call CheckProcessRuntime
  Call CheckInstalledVersion
FunctionEnd

Section "MainSection" SEC01
  # Chcek .Net Framework version and install.
  !insertmacro CheckNetFramework 461

  Var /GLOBAL InstalledPath
  ReadRegStr $InstalledPath HKLM "${PRODUCT_REGISTRY_SUBKEY}" "${REGISTRY_REGKEY_InstallPath}"

  ${If} $CanUpdate == "True"
    # ���� ��ġ��ġ�� �ִ�  ��ġ������ �����Ѵ�.
    IfFileExists "$INSTDIR" DeleteCurrentInstallPath InstallContinue
    DeleteCurrentInstallPath:
      Delete "$INSTDIR\ko-kr\Megazone.SubtitleEditor.Resources.resources.dll"
      RMDir "$INSTDIR\ko-kr"
      Delete "$INSTDIR\*.*"
      Delete "${STARTMENU_PROGRAM}\${COMPANY_NAME}\${PRODUCT_NAME}\*.*"
      RMDir "${STARTMENU_PROGRAM}\${COMPANY_NAME}\${PRODUCT_NAME}"
    InstallContinue:

  ${EndIf}

  SetOutPath "$INSTDIR"
  ;SetOverwrite try
  SetOverwrite on

  File "Install_Files\CommonServiceLocator.dll"
  File "Install_Files\DocumentFormat.OpenXml.dll"
  File "Install_Files\HyperSubtitleEditor.exe"
  File "Install_Files\LGPL_21.txt"
  File "Install_Files\log4net.dll"
  File "Install_Files\Megazone.Api.Transcoder.Domain.dll"
  File "Install_Files\Megazone.Api.Transcoder.Repository.dll"
  File "Install_Files\Megazone.Api.Transcoder.Service.dll"
  File "Install_Files\Megazone.Api.Transcoder.ServiceInterface.dll"
  File "Install_Files\Megazone.Cloud.Aws.Domain.dll"
  File "Install_Files\Megazone.Cloud.Common.Domain.dll"
  File "Install_Files\Megazone.Cloud.Media.Domain.dll"
  File "Install_Files\Megazone.Cloud.Media.Repository.dll"
  File "Install_Files\Megazone.Cloud.Media.Service.dll"
  File "Install_Files\Megazone.Cloud.Media.ServiceInterface.dll"
  File "Install_Files\Megazone.Cloud.Storage.Domain.dll"
  File "Install_Files\Megazone.Cloud.Storage.Domain.S3.dll"
  File "Install_Files\Megazone.Cloud.Storage.Repository.S3.dll"
  File "Install_Files\Megazone.Cloud.Storage.Service.S3.dll"
  File "Install_Files\Megazone.Cloud.Storage.ServiceInterface.S3.dll"
  File "Install_Files\Megazone.Cloud.Transcoder.Domain.dll"
  File "Install_Files\Megazone.Cloud.Transcoder.Domain.ElasticTranscoder.dll"
  File "Install_Files\Megazone.Cloud.Transcoder.Repository.ElasticTranscoder.dll"
  File "Install_Files\Megazone.Core.AWS.dll"
  File "Install_Files\Megazone.Core.Client.dll"
  File "Install_Files\Megazone.Core.Debug.dll"
  File "Install_Files\Megazone.Core.dll"
  File "Install_Files\Megazone.Core.IoC.Unity.dll"
  File "Install_Files\Megazone.Core.Log.Log4Net.dll"
  File "Install_Files\Megazone.Core.Security.dll"
  File "Install_Files\Megazone.Core.VideoTrack.dll"
  File "Install_Files\Megazone.Core.VideoTrack.WebVtt.dll"
  File "Install_Files\Megazone.Core.VideoTrack.Xaml.dll"
  File "Install_Files\Megazone.Core.Windows.Control.Buttons.dll"
  File "Install_Files\Megazone.Core.Windows.Control.RichTextBox.dll"
  File "Install_Files\Megazone.Core.Windows.Control.Spinner.dll"
  File "Install_Files\Megazone.Core.Windows.Control.TimeSpinner.dll"
  File "Install_Files\Megazone.Core.Windows.Control.VideoPlayer.dll"
  File "Install_Files\Megazone.Core.Windows.dll"
  File "Install_Files\Megazone.Core.Windows.Pinvoke.dll"
  File "Install_Files\Megazone.Core.Windows.Wpf.Debug.dll"
  File "Install_Files\Megazone.Core.Windows.Xaml.dll"
  File "Install_Files\Megazone.HyperSubtitleEditor.Presentation.dll"
  File "Install_Files\Megazone.HyperSubtitleEditor.Presentation.Infrastructure.dll"
  File "Install_Files\Megazone.HyperSubtitleEditor.Presentation.Resource.dll"
  File "Install_Files\Megazone.SubtitleEditor.Resources.dll"
  File "Install_Files\Megazone.VideoStudio.Presentation.Common.Infrastructure.dll"
  File "Install_Files\Microsoft.Win32.Primitives.dll"
  File "Install_Files\Ms-PL.txt"
  File "Install_Files\NAudio.dll"
  File "Install_Files\Newtonsoft.Json.dll"
  File "Install_Files\PreferedLanguageInfo.json"
  File "Install_Files\RestSharp.dll"
  File "Install_Files\System.AppContext.dll"
  File "Install_Files\System.Console.dll"
  File "Install_Files\System.Diagnostics.DiagnosticSource.dll"
  File "Install_Files\System.Globalization.Calendars.dll"
  File "Install_Files\System.IO.Compression.dll"
  File "Install_Files\System.IO.Compression.ZipFile.dll"
  File "Install_Files\System.IO.FileSystem.dll"
  File "Install_Files\System.IO.FileSystem.Primitives.dll"
  File "Install_Files\System.IO.Packaging.dll"
  File "Install_Files\System.Net.Http.dll"
  File "Install_Files\System.Net.Sockets.dll"
  File "Install_Files\System.Runtime.InteropServices.RuntimeInformation.dll"
  File "Install_Files\System.Security.Cryptography.Algorithms.dll"
  File "Install_Files\System.Security.Cryptography.Encoding.dll"
  File "Install_Files\System.Security.Cryptography.Primitives.dll"
  File "Install_Files\System.Security.Cryptography.X509Certificates.dll"
  File "Install_Files\System.Windows.Interactivity.dll"
  File "Install_Files\System.Xml.ReaderWriter.dll"
  File "Install_Files\Unity.Abstractions.dll"
  File "Install_Files\Unity.Configuration.dll"
  File "Install_Files\Unity.Container.dll"
  File "Install_Files\Unity.Interception.Configuration.dll"
  File "Install_Files\Unity.Interception.dll"
  File "Install_Files\Unity.RegistrationByConvention.dll"
  File "Install_Files\Unity.ServiceLocation.dll"

  SetOutPath "$INSTDIR\ko-kr"
  File "Install_Files\ko-kr\Megazone.SubtitleEditor.Resources.resources.dll"

/*
  # ������ ����.
  # LocalAppData\Megazone\HyperBrowser -> LocalAppData\Megazone\Hyper Browser
  IfFileExists "$LOCALAPPDATA\${COMPANY_NAME}\HyperBrowser" GotoExist GotoNotExist
  GotoExist:
    IfFileExists "$LOCALAPPDATA\${COMPANY_NAME}\Hyper Browser" CanNotRename CanRename
    CanNotRename:
      ; ���������� ��Ȳ.
      RMDir /r "$LOCALAPPDATA\${COMPANY_NAME}\HyperBrowser"
      goto IntallContinue
    CanRename:
      Rename "$LOCALAPPDATA\${COMPANY_NAME}\HyperBrowser" "$LOCALAPPDATA\${COMPANY_NAME}\Hyper Browser"
      goto IntallContinue
  GotoNotExist:
    goto IntallContinue
  IntallContinue:
    ; Nothing...

  # ICON Image File copy
  SetOutPath "$LOCALAPPDATA\${COMPANY_NAME}\${PRODUCT_NAME}"
  File "Resources\ICON_IMAGE\logo.png"
  File "Resources\ICON_IMAGE\logo_small.png"
*/
  # Create shortcut in start menu.
  CreateDirectory "$SMPROGRAMS\${COMPANY_NAME}\${PRODUCT_NAME}"
  CreateShortCut "$SMPROGRAMS\${COMPANY_NAME}\${PRODUCT_NAME}\${PRODUCT_NAME}.lnk" "$INSTDIR\${PROCESS_NAME}"
  CreateShortCut "$SMPROGRAMS\${COMPANY_NAME}\${PRODUCT_NAME}\Megazone Media Cloud.lnk" "$INSTDIR\Megazone Media Cloud.url"
  ;CreateShortCut "$DESKTOP\${PRODUCT_NAME}.lnk" "$INSTDIR\${PROCESS_NAME}"

  # Create shortcut on desktop screen.
  # �������� ����ϵ��� �Ѵ�.
  ${ProfilePathAllUsers} $0
  CreateShortCut "$0\Public\Desktop\${PRODUCT_NAME}.lnk" "$INSTDIR\${PROCESS_NAME}"
  

  # �������� ����� ���, ����ȭ�鿡 �ݿ����� ����.
  # ���� : �����쿡�� ����ϴ� ������ �̹��� ĳ�� ��å ����.
  # LocalAppData > IconCache.db�� ������ ������ϸ�, ����ȭ���� �������� ����� ���������� �ݿ�������, Window 10 > �� �˻������� ���� �������� ����.
SectionEnd

Section -AdditionalIcons
  SetOutPath $INSTDIR
  WriteIniStr "$INSTDIR\${PRODUCT_NAME}.url" "InternetShortcut" "URL" "${PRODUCT_WEB_SITE}"
  WriteIniStr "$INSTDIR\Megazone Media Cloud.url" "InternetShortcut" "URL" "${MCM_WEB_SITE}"
  ;CreateShortCut "$SMPROGRAMS\${COMPANY_NAME}\${PRODUCT_NAME}\${PRODUCT_NAME}.lnk" "$INSTDIR\${PRODUCT_NAME}.url"
  ;CreateShortCut "$SMPROGRAMS\${COMPANY_NAME}\${PRODUCT_NAME}\Uninstall.lnk" "$INSTDIR\uninst.exe"
SectionEnd

Section -Post
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\${PROCESS_NAME}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\${PROCESS_NAME}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
  WriteRegDWORD ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "EstimatedSize" 0x2A5C
SectionEnd

; Section descriptions
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC01} "Main Component"
!insertmacro MUI_FUNCTION_DESCRIPTION_END


Function un.onUninstSuccess
  HideWindow
  #MessageBox MB_ICONINFORMATION|MB_OK "$(^Name)��(��) ������ ���ŵǾ����ϴ�."
  ;MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) was successfully removed from your computer."
FunctionEnd

Function un.onInit
!insertmacro MUI_UNGETLANGUAGE
  #MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "$(^Name)��(��) �����Ͻðڽ��ϱ�?" IDYES +2
  ;MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "Are you sure you want to completely remove $(^Name) and all of its components?" IDYES +2
  ;Abort

  Call un.CheckOS64
  Call un.CheckProcessRuntime
FunctionEnd

Section Uninstall

  #�ٷΰ��� ����.
  Delete "$SMPROGRAMS\${COMPANY_NAME}\${PRODUCT_NAME}\Uninstall.lnk"
  Delete "$SMPROGRAMS\${COMPANY_NAME}\${PRODUCT_NAME}\${PRODUCT_NAME}.lnk"
  Delete "$DESKTOP\${PRODUCT_NAME}.lnk"
  RMDir /r "$SMPROGRAMS\${COMPANY_NAME}\${PRODUCT_NAME}"

  #����ȭ���� �ٷΰ��� ����.
  ${ProfilePathAllUsers} $0
  Delete "$0\Public\Desktop\${PRODUCT_NAME}.lnk"

  #Delete install files.
  Delete "$INSTDIR\Megazone Media Cloud.url"
  Delete "$INSTDIR\${PRODUCT_NAME}.url"
  Delete "$INSTDIR\uninst.exe"
  Delete "$INSTDIR\ko-kr\Megazone.SubtitleEditor.Resources.resources.dll"
  Delete "$INSTDIR\CommonServiceLocator.dll"
  Delete "$INSTDIR\DocumentFormat.OpenXml.dll"
  Delete "$INSTDIR\HyperSubtitleEditor.exe"
  Delete "$INSTDIR\LGPL_21.txt"
  Delete "$INSTDIR\log4net.dll"
  Delete "$INSTDIR\Megazone.Api.Transcoder.Domain.dll"
  Delete "$INSTDIR\Megazone.Api.Transcoder.Repository.dll"
  Delete "$INSTDIR\Megazone.Api.Transcoder.Service.dll"
  Delete "$INSTDIR\Megazone.Api.Transcoder.ServiceInterface.dll"
  Delete "$INSTDIR\Megazone.Cloud.Aws.Domain.dll"
  Delete "$INSTDIR\Megazone.Cloud.Common.Domain.dll"
  Delete "$INSTDIR\Megazone.Cloud.Media.Domain.dll"
  Delete "$INSTDIR\Megazone.Cloud.Media.Repository.dll"
  Delete "$INSTDIR\Megazone.Cloud.Media.Service.dll"
  Delete "$INSTDIR\Megazone.Cloud.Media.ServiceInterface.dll"
  Delete "$INSTDIR\Megazone.Cloud.Storage.Domain.dll"
  Delete "$INSTDIR\Megazone.Cloud.Storage.Domain.S3.dll"
  Delete "$INSTDIR\Megazone.Cloud.Storage.Repository.S3.dll"
  Delete "$INSTDIR\Megazone.Cloud.Storage.Service.S3.dll"
  Delete "$INSTDIR\Megazone.Cloud.Storage.ServiceInterface.S3.dll"
  Delete "$INSTDIR\Megazone.Cloud.Transcoder.Domain.dll"
  Delete "$INSTDIR\Megazone.Cloud.Transcoder.Domain.ElasticTranscoder.dll"
  Delete "$INSTDIR\Megazone.Cloud.Transcoder.Repository.ElasticTranscoder.dll"
  Delete "$INSTDIR\Megazone.Core.AWS.dll"
  Delete "$INSTDIR\Megazone.Core.Client.dll"
  Delete "$INSTDIR\Megazone.Core.Debug.dll"
  Delete "$INSTDIR\Megazone.Core.dll"
  Delete "$INSTDIR\Megazone.Core.IoC.Unity.dll"
  Delete "$INSTDIR\Megazone.Core.Log.Log4Net.dll"
  Delete "$INSTDIR\Megazone.Core.Security.dll"
  Delete "$INSTDIR\Megazone.Core.VideoTrack.dll"
  Delete "$INSTDIR\Megazone.Core.VideoTrack.WebVtt.dll"
  Delete "$INSTDIR\Megazone.Core.VideoTrack.Xaml.dll"
  Delete "$INSTDIR\Megazone.Core.Windows.Control.Buttons.dll"
  Delete "$INSTDIR\Megazone.Core.Windows.Control.RichTextBox.dll"
  Delete "$INSTDIR\Megazone.Core.Windows.Control.Spinner.dll"
  Delete "$INSTDIR\Megazone.Core.Windows.Control.TimeSpinner.dll"
  Delete "$INSTDIR\Megazone.Core.Windows.Control.VideoPlayer.dll"
  Delete "$INSTDIR\Megazone.Core.Windows.dll"
  Delete "$INSTDIR\Megazone.Core.Windows.Pinvoke.dll"
  Delete "$INSTDIR\Megazone.Core.Windows.Wpf.Debug.dll"
  Delete "$INSTDIR\Megazone.Core.Windows.Xaml.dll"
  Delete "$INSTDIR\Megazone.HyperSubtitleEditor.Presentation.dll"
  Delete "$INSTDIR\Megazone.HyperSubtitleEditor.Presentation.Infrastructure.dll"
  Delete "$INSTDIR\Megazone.HyperSubtitleEditor.Presentation.Resource.dll"
  Delete "$INSTDIR\Megazone.SubtitleEditor.Resources.dll"
  Delete "$INSTDIR\Megazone.VideoStudio.Presentation.Common.Infrastructure.dll"
  Delete "$INSTDIR\Microsoft.Win32.Primitives.dll"
  Delete "$INSTDIR\Ms-PL.txt"
  Delete "$INSTDIR\NAudio.dll"
  Delete "$INSTDIR\Newtonsoft.Json.dll"
  Delete "$INSTDIR\PreferedLanguageInfo.json"
  Delete "$INSTDIR\RestSharp.dll"
  Delete "$INSTDIR\System.AppContext.dll"
  Delete "$INSTDIR\System.Console.dll"
  Delete "$INSTDIR\System.Diagnostics.DiagnosticSource.dll"
  Delete "$INSTDIR\System.Globalization.Calendars.dll"
  Delete "$INSTDIR\System.IO.Compression.dll"
  Delete "$INSTDIR\System.IO.Compression.ZipFile.dll"
  Delete "$INSTDIR\System.IO.FileSystem.dll"
  Delete "$INSTDIR\System.IO.FileSystem.Primitives.dll"
  Delete "$INSTDIR\System.IO.Packaging.dll"
  Delete "$INSTDIR\System.Net.Http.dll"
  Delete "$INSTDIR\System.Net.Sockets.dll"
  Delete "$INSTDIR\System.Runtime.InteropServices.RuntimeInformation.dll"
  Delete "$INSTDIR\System.Security.Cryptography.Algorithms.dll"
  Delete "$INSTDIR\System.Security.Cryptography.Encoding.dll"
  Delete "$INSTDIR\System.Security.Cryptography.Primitives.dll"
  Delete "$INSTDIR\System.Security.Cryptography.X509Certificates.dll"
  Delete "$INSTDIR\System.Windows.Interactivity.dll"
  Delete "$INSTDIR\System.Xml.ReaderWriter.dll"
  Delete "$INSTDIR\Unity.Abstractions.dll"
  Delete "$INSTDIR\Unity.Configuration.dll"
  Delete "$INSTDIR\Unity.Container.dll"
  Delete "$INSTDIR\Unity.Interception.Configuration.dll"
  Delete "$INSTDIR\Unity.Interception.dll"
  Delete "$INSTDIR\Unity.RegistrationByConvention.dll"
  Delete "$INSTDIR\Unity.ServiceLocation.dll"

  RMDir "$INSTDIR\ko-kr"
  RMDir /r "$INSTDIR"

  #Delete registry.
  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey "HKLM" "${PRODUCT_DIR_REGKEY}"
  DeleteRegKey "HKLM" "${PRODUCT_REGISTRY_SUBKEY}"
  DeleteRegKey "HKCR" "${CUSTOM_URL_SCHEME_REGISTRY_SUBKEY}"

  # Delete user file.
  ${If} $CheckState <> 0
    Delete "$LOCALAPPDATA\${COMPANY_NAME}\${PRODUCT_NAME}\*.*"
    RMDir /r "$LOCALAPPDATA\${COMPANY_NAME}\${PRODUCT_NAME}"
  ${EndIf}

  SetAutoClose true
SectionEnd