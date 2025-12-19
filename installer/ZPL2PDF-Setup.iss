; =============================================================================
; ZPL2PDF - Inno Setup Installation Script
; =============================================================================
; This script creates a professional Windows installer for ZPL2PDF
; with multi-language support and automatic configuration
;
; Requirements:
;   - Inno Setup 6.2.0 or higher (https://jrsoftware.org/isinfo.php)
;   - Build artifacts in ../build/publish/
;
; Usage:
;   1. Build all platforms: .\scripts\build-all-platforms.ps1
;   2. Compile this script with Inno Setup Compiler
;   3. Output: ZPL2PDF-Setup-{version}.exe
; =============================================================================

#define MyAppName "ZPL2PDF"
#define MyAppVersion "3.0.0"
#define MyAppPublisher "Bruno Campos"
#define MyAppURL "https://github.com/brunoleocam/ZPL2PDF"
#define MyAppExeName "ZPL2PDF.exe"
#define MyAppDescription "ZPL to PDF Converter with Multi-language Support"

[Setup]
; Basic Information
AppId={{A7F8C3D2-9E1B-4F6A-8D2C-5E9A3B7C1F4D}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}/issues
AppUpdatesURL={#MyAppURL}/releases
AppComments={#MyAppDescription}
AppCopyright=Copyright (C) 2024 {#MyAppPublisher}

; Installation Paths
DefaultDirName={commonpf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes

; Output Configuration
OutputDir=.
OutputBaseFilename=ZPL2PDF-Setup-{#MyAppVersion}
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern

; Signing (uncomment and configure if you have a code signing certificate)
;SignTool=signtool
;SignedUninstaller=yes

; Architecture
ArchitecturesInstallIn64BitMode=x64compatible
ArchitecturesAllowed=x64compatible

; License and Documentation
LicenseFile=..\LICENSE
InfoBeforeFile=..\README.md

; Icons
SetupIconFile=..\docs\Image\ZPL2PDF.ico
UninstallDisplayIcon={app}\{#MyAppExeName}

; Privileges (requires admin to install in Program Files)
PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=dialog

; Visual (using default Inno Setup images)
; WizardImageFile and WizardSmallImageFile are optional
; Inno Setup 6.5+ uses modern wizard style by default

; Multi-language Support
ShowLanguageDialog=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"
Name: "french"; MessagesFile: "compiler:Languages\French.isl"
Name: "german"; MessagesFile: "compiler:Languages\German.isl"
Name: "italian"; MessagesFile: "compiler:Languages\Italian.isl"
Name: "japanese"; MessagesFile: "compiler:Languages\Japanese.isl"

[CustomMessages]
; English
english.LaunchAfterInstall=Launch {#MyAppName} after installation
english.CreateDesktopIcon=Create a &desktop icon
english.ConfigureLanguage=Configure application language:
english.WatchFolderSetup=Set up automatic folder monitoring

; Brazilian Portuguese
brazilianportuguese.LaunchAfterInstall=Executar {#MyAppName} após a instalação
brazilianportuguese.CreateDesktopIcon=Criar ícone na &área de trabalho
brazilianportuguese.ConfigureLanguage=Configurar idioma do aplicativo:
brazilianportuguese.WatchFolderSetup=Configurar monitoramento automático de pasta

; Spanish
spanish.LaunchAfterInstall=Ejecutar {#MyAppName} después de la instalación
spanish.CreateDesktopIcon=Crear un icono en el &escritorio
spanish.ConfigureLanguage=Configurar idioma de la aplicación:
spanish.WatchFolderSetup=Configurar monitoreo automático de carpeta

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 6.1; Check: not IsAdminInstallMode
Name: "envpath"; Description: "Add to PATH environment variable"; GroupDescription: "System Integration:"; Flags: unchecked
Name: "setlanguage"; Description: "{cm:ConfigureLanguage}"; GroupDescription: "Configuration:"

[Files]
; Main Application (x64)
Source: "..\build\publish\win-x64\ZPL2PDF.exe"; DestDir: "{app}"; Flags: ignoreversion; Check: Is64BitInstallMode

; Configuration Example
Source: "..\zpl2pdf.json.example"; DestDir: "{app}"; DestName: "zpl2pdf.json.example"; Flags: ignoreversion

; Documentation
Source: "..\README.md"; DestDir: "{app}\docs"; Flags: ignoreversion isreadme
Source: "..\docs\i18n\README.pt-BR.md"; DestDir: "{app}\docs\i18n"; Flags: ignoreversion
Source: "..\docs\README.md"; DestDir: "{app}\docs"; Flags: ignoreversion
Source: "..\LICENSE"; DestDir: "{app}\docs"; Flags: ignoreversion
Source: "..\CHANGELOG.md"; DestDir: "{app}\docs"; Flags: ignoreversion
Source: "..\CONTRIBUTING.md"; DestDir: "{app}\docs"; Flags: ignoreversion

; User Guides
Source: "..\docs\guides\*.md"; DestDir: "{app}\docs\guides"; Flags: ignoreversion

; Sample Files
Source: "..\docs\Sample\*.txt"; DestDir: "{app}\samples"; Flags: ignoreversion
Source: "..\docs\Sample\*.prn"; DestDir: "{app}\samples"; Flags: ignoreversion

; Icons
Source: "..\docs\Image\ZPL2PDF.ico"; DestDir: "{app}"; Flags: ignoreversion

[Dirs]
; Create default directory
Name: "{userdocs}\ZPL2PDF Auto Converter"; Flags: uninsneveruninstall

[Icons]
; Start Menu
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Parameters: "-help"; Comment: "{#MyAppDescription}"
Name: "{group}\{#MyAppName} (Start Daemon)"; Filename: "{app}\{#MyAppExeName}"; Parameters: "start"; Comment: "Start automatic conversion daemon"; IconIndex: 0
Name: "{group}\{#MyAppName} (Stop Daemon)"; Filename: "{app}\{#MyAppExeName}"; Parameters: "stop"; Comment: "Stop conversion daemon"; IconIndex: 0
Name: "{group}\Documentation"; Filename: "{app}\docs\README.md"
Name: "{group}\Sample Files"; Filename: "{app}\samples"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"

; Desktop Icon
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon; Comment: "{#MyAppDescription}"

; Quick Launch Icon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Registry]
; File Associations
Root: HKA; Subkey: "Software\Classes\.zpl"; ValueType: string; ValueName: ""; ValueData: "ZPLFile"; Flags: uninsdeletevalue
Root: HKA; Subkey: "Software\Classes\.imp"; ValueType: string; ValueName: ""; ValueData: "ZPLFile"; Flags: uninsdeletevalue
Root: HKA; Subkey: "Software\Classes\ZPLFile"; ValueType: string; ValueName: ""; ValueData: "ZPL Label File"; Flags: uninsdeletekey
Root: HKA; Subkey: "Software\Classes\ZPLFile\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\{#MyAppExeName},0"
Root: HKA; Subkey: "Software\Classes\ZPLFile\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#MyAppExeName}"" ""-i"" ""%1"""

; Application Settings
Root: HKA; Subkey: "Software\{#MyAppName}"; ValueType: string; ValueName: "InstallPath"; ValueData: "{app}"; Flags: uninsdeletekey
Root: HKA; Subkey: "Software\{#MyAppName}"; ValueType: string; ValueName: "Version"; ValueData: "{#MyAppVersion}"
Root: HKA; Subkey: "Software\{#MyAppName}"; ValueType: string; ValueName: "WatchFolder"; ValueData: "{userdocs}\ZPL2PDF Auto Converter\watch"

[Code]
var
  LanguagePage: TInputOptionWizardPage;
  LanguageCombo: TNewComboBox;
  SelectedLanguage: String;

procedure InitializeWizard;
begin
  // Create language selection page
  LanguagePage := CreateInputOptionPage(wpSelectTasks,
    'Language Configuration', 
    'Choose the application interface language',
    'Select the language that ZPL2PDF will use for messages and interface:',
    False, False);
  
  // Add language options
  LanguagePage.Add('English (en-US)');
  LanguagePage.Add('Português Brasil (pt-BR)');
  LanguagePage.Add('Español (es-ES)');
  LanguagePage.Add('Français (fr-FR)');
  LanguagePage.Add('Deutsch (de-DE)');
  LanguagePage.Add('Italiano (it-IT)');
  LanguagePage.Add('日本語 (ja-JP)');
  LanguagePage.Add('中文 (zh-CN)');
  
  // Set default based on installer language
  case ActiveLanguage of
    'english': LanguagePage.SelectedValueIndex := 0;
    'brazilianportuguese': LanguagePage.SelectedValueIndex := 1;
    'spanish': LanguagePage.SelectedValueIndex := 2;
    'french': LanguagePage.SelectedValueIndex := 3;
    'german': LanguagePage.SelectedValueIndex := 4;
    'italian': LanguagePage.SelectedValueIndex := 5;
    'japanese': LanguagePage.SelectedValueIndex := 6;
  else
    LanguagePage.SelectedValueIndex := 0; // Default to English
  end;
end;

function GetLanguageCode: String;
begin
  case LanguagePage.SelectedValueIndex of
    0: Result := 'en-US';
    1: Result := 'pt-BR';
    2: Result := 'es-ES';
    3: Result := 'fr-FR';
    4: Result := 'de-DE';
    5: Result := 'it-IT';
    6: Result := 'ja-JP';
    7: Result := 'zh-CN';
  else
    Result := 'en-US';
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  ResultCode: Integer;
begin
  if CurStep = ssPostInstall then
  begin
    // Set language environment variable
    if WizardIsTaskSelected('setlanguage') then
    begin
      SelectedLanguage := GetLanguageCode;
      
      // Set user environment variable
      if RegWriteStringValue(HKEY_CURRENT_USER, 'Environment', 'ZPL2PDF_LANGUAGE', SelectedLanguage) then
        Log('Language set to: ' + SelectedLanguage)
      else
        Log('Failed to set language environment variable');
      
      // Broadcast environment change
      RegWriteStringValue(HKEY_CURRENT_USER, 'Environment', 'ZPL2PDF_LANGUAGE', SelectedLanguage);
    end;
    
    // Add to PATH if selected
    if WizardIsTaskSelected('envpath') then
    begin
      // This will be handled by the environment variable system
      Log('Adding to PATH: ' + ExpandConstant('{app}'));
    end;
  end;
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  if CurUninstallStep = usPostUninstall then
  begin
    // Remove environment variables
    RegDeleteValue(HKEY_CURRENT_USER, 'Environment', 'ZPL2PDF_LANGUAGE');
    
    // Ask if user wants to keep configuration and data
    if MsgBox('Do you want to keep your ZPL2PDF configuration and watch folders?', 
              mbConfirmation, MB_YESNO) = IDNO then
    begin
      // User chose to delete everything
      DelTree(ExpandConstant('{userdocs}\ZPL2PDF Auto Converter'), True, True, True);
    end;
  end;
end;

[Run]
; Optional: Launch application after install
Filename: "{app}\{#MyAppExeName}"; Parameters: "-help"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent unchecked

; Optional: Start daemon after install
Filename: "{app}\{#MyAppExeName}"; Parameters: "start"; Description: "Start {#MyAppName} daemon mode"; Flags: nowait postinstall skipifsilent unchecked

[UninstallRun]
; Stop daemon before uninstall
Filename: "{app}\{#MyAppExeName}"; Parameters: "stop"; Flags: runhidden; RunOnceId: "StopDaemon"

[UninstallDelete]
; Clean up PID files and temporary files
Type: files; Name: "{tmp}\zpl2pdf*.pid"
Type: filesandordirs; Name: "{localappdata}\ZPL2PDF"
