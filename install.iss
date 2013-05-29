; MARC-HI Visualizer Version 3.0 Installation

[Setup]
AppId={{3BFA5A48-F91F-4db7-8405-AE431861D19F}
AppName=MARC-HI Visualizer
AppVerName=MARC-HI Visualizer v3.3
OutputBaseFilename=visualizer-setup-3.3
LicenseFile=.\License.rtf
AppPublisher=Mohawk College of Applied Arts and Technology
AppPublisherURL=http://te.marc-hi.ca
AppSupportURL=http://te.marc-hi.ca/forums
AppUpdatesURL=http://te.marc-hi.ca
DefaultDirName={pf}\Mohawk College\Visualizer
DefaultGroupName=Mohawk College\Visualizer
AllowNoIcons=yes
OutputDir=.\dist
Compression=lzma
SolidCompression=yes
AppCopyright=Copyright (C) 2009-2013, Mohawk College of Applied Arts and Technology
RestartIfNeededByRun=yes
WizardImageFile=.\install-logo.bmp
[Files]
Source: .\bin\Debug\ClientAccessPolicy.xml; DestDir: {app}; Flags: ignoreversion; Components: srv
Source: .\bin\Debug\MARC.EHRS.Visualization.Core.dll; DestDir: {app}; Flags: ignoreversion; Components: srv
Source: .\bin\Debug\MARC.EHRS.VisualizationServer.exe; DestDir: {app}; Flags: restartreplace ignoreversion; Components: srv
Source: .\bin\Debug\MARC.EHRS.VisualizationServer.exe.config; DestDir: {app}; Flags: ignoreversion; Components: srv
Source: .\bin\Debug\MARC.EHRS.VisualizationServer.Notifier.dll; DestDir: {app}; Flags: ignoreversion; Components: srv
Source: .\bin\Debug\MARC.EHRS.VisualizationServer.Syslog.dll; DestDir: {app}; Flags: ignoreversion; Components: srv
Source: .\bin\Debug\MARC.HI.EHRS.SVC.Auditing.Atna.dll; DestDir: {app}; Flags: ignoreversion; Components: srv
Source: .\bin\Debug\MARC.HI.EHRS.SVC.Core.dll; DestDir: {app}; Flags: ignoreversion; Components: srv
Source: .\MARC.EHRS.VisualizationClient.Silverlight\Bin\Debug\MARC.EHRS.VisualizationClient.Silverlight.xap; DestDir: {app}\web\ClientBin; Components: web
Source: .\MARC.EHRS.VisualizationClient.Silverlight\Bin\Debug\MARC.EHRS.Silverlight.dll; DestDir: {app}\web\ClientBin; Components: web
Source: .\MARC.EHRS.VisualizationClient.Silverlight\Bin\Debug\MARC.EHRS.VisualizationClient.Silverlight.dll; DestDir: {app}\web\ClientBin; Components: web
Source: .\MARC.EHRS.VisualizationClient.Silverlight\Bin\Debug\System.ComponentModel.DataAnnotations.dll; DestDir: {app}\web\ClientBin; Components: web
Source: .\MARC.EHRS.VisualizationClient.Silverlight\Bin\Debug\System.Windows.Controls.Data.Input.dll; DestDir: {app}\web\ClientBin; Components: web
Source: .\MARC.EHRS.VisualizationClient.Silverlight\Bin\Debug\System.Xml.Linq.dll; DestDir: {app}\web\ClientBin; Components: web
Source: .\MARC.EHRS.VisualizationClient.Silverlight\Bin\Debug\System.Xml.Serialization.dll; DestDir: {app}\web\ClientBin; Components: web
Source: .\MARC.EHRS.VisualizationClient.Silverlight.Test\ClientBin\*.png; DestDir: {app}\web\ClientBin; Components: web
Source: .\MARC.EHRS.VisualizationClient.Silverlight.Test\default.html; DestDir: {app}\web; Components: web
Source: .\*.*; Excludes: "*.dll,*.xml,*.pdb,*.obj,*.exe,*.suo,*.iss"; Flags: recursesubdirs; DestDir: {app}\src; Components: src
Source: .\netfx\dotNetFx35setup.exe; DestDir: {tmp} ; Flags: dontcopy
Source: .\License.rtf; DestDir: {app} ; Flags: dontcopy

[Icons]
Name: {group}\Visualizer Source Code; FileName: {app}\src\visualizer.sln; Components:src
Name: {group}\Visualizer Client; FileName: {app}\web\default.html; Components: web
Name: {group}\{cm:UninstallProgram,Visualizer}; FileName: {uninstallexe}
Name: {group}\License; FileName: {app}\License.rtf
Name: {group}\Support Site; FileName: http://te.marc-hi.ca

[UninstallRun]
Filename: "{dotnet20}\installutil.exe"; Parameters: "/uninstall ""{app}\MARC.EHRS.VisualizationServer.exe"""; Components: srv; StatusMsg: "Uninstalling Visualization Service..."; Flags: runhidden
Filename: "{dotnet20}\ngen.exe"; Parameters: "uninstall ""{app}\MARC.EHRS.Visualization.Core.dll"" /nologo /silent" ; Components:srv; StatusMsg: "Removing Native Assemblies"; Flags:runhidden
Filename: "{dotnet20}\ngen.exe"; Parameters: "uninstall ""{app}\MARC.EHRS.VisualizationServer.exe"" /nologo /silent" ; Components:srv; StatusMsg: "Removing Native Assemblies"; Flags:runhidden
Filename: "{dotnet20}\ngen.exe"; Parameters: "uninstall ""{app}\MARC.EHRS.VisualizationServer.Notifier.dll"" /nologo /silent" ; Components:srv; StatusMsg: "Removing Native Assemblies"; Flags:runhidden
Filename: "{dotnet20}\ngen.exe"; Parameters: "uninstall ""{app}\MARC.EHRS.VisualizationServer.Syslog.dll"" /nologo /silent" ; Components:srv; StatusMsg: "Removing Native Assemblies"; Flags:runhidden
Filename: "{dotnet20}\ngen.exe"; Parameters: "uninstall ""{app}\MARC.HI.EHRS.SVC.Auditing.Atna.dll"" /nologo /silent" ; Components:srv; StatusMsg: "Removing Native Assemblies"; Flags:runhidden
Filename: "{dotnet20}\ngen.exe"; Parameters: "uninstall ""{app}\MARC.HI.EHRS.SVC.Core.dll"" /nologo /silent" ; Components:srv; StatusMsg: "Removing Native Assemblies"; Flags:runhidden

[Run]
Filename: "{dotnet20}\ngen.exe"; Parameters: "install ""{app}\MARC.EHRS.Visualization.Core.dll"" /nologo /silent" ; Components:srv; StatusMsg: "Generating Native Assemblies"; Flags:runhidden
Filename: "{dotnet20}\ngen.exe"; Parameters: "install ""{app}\MARC.EHRS.VisualizationServer.exe"" /nologo /silent" ; Components:srv; StatusMsg: "Generating Native Assemblies"; Flags:runhidden
Filename: "{dotnet20}\ngen.exe"; Parameters: "install ""{app}\MARC.EHRS.VisualizationServer.Notifier.dll"" /nologo /silent" ; Components:srv; StatusMsg: "Generating Native Assemblies"; Flags:runhidden
Filename: "{dotnet20}\ngen.exe"; Parameters: "install ""{app}\MARC.EHRS.VisualizationServer.Syslog.dll"" /nologo /silent" ; Components:srv; StatusMsg: "Generating Native Assemblies"; Flags:runhidden
Filename: "{dotnet20}\ngen.exe"; Parameters: "install ""{app}\MARC.HI.EHRS.SVC.Auditing.Atna.dll"" /nologo /silent" ; Components:srv; StatusMsg: "Generating Native Assemblies"; Flags:runhidden
Filename: "{dotnet20}\ngen.exe"; Parameters: "install ""{app}\MARC.HI.EHRS.SVC.Core.dll"" /nologo /silent" ; Components:srv; StatusMsg: "Generating Native Assemblies"; Flags:runhidden
Filename: "{dotnet20}\installutil.exe"; Parameters: """{app}\MARC.EHRS.VisualizationServer.exe"""; Components: srv; StatusMsg: "Installing Visualization Service..."; Flags: runhidden

[Types]
Name: full; Description: Complete Installation
Name: serveronly; Description: Server Only
Name: webonly; Description: Web Client Only

[Components]
Name: srv; Description: Server Components; Types: full serveronly; Flags: restart
Name: web; Description: Web Client; Types: full webonly
Name: src; Description: Source Code; Types: full

[Code]
var
  dotnetRedistPath: string;
  downloadNeeded, needsUninstall: boolean;
  dotNetNeeded: boolean;
  memoDependenciesNeeded: string;


const
  dotnetRedistURL = '{tmp}\dotNetFx35setup.exe';
  // local system for testing...
  // dotnetRedistURL = 'http://192.168.1.1/dotnetfx.exe';

function InitializeSetup(): Boolean;
begin
  Result := true;
  dotNetNeeded := false;

  if (not DirExists(ExpandConstant('{dotnet20}'))) then begin
    dotNetNeeded := true;
    if (not IsAdminLoggedOn()) then begin
      MsgBox('Visualizer needs the Microsoft .NET Framework 3.5 to be installed by an Administrator', mbInformation, MB_OK);
      Result := false;
    end else begin
      memoDependenciesNeeded := memoDependenciesNeeded + '      .NET Framework 3.5' #13;
      dotnetRedistPath := ExpandConstant('{tmp}\dotNetFx35setup.exe');

    end;
  end;

end;

function PrepareToInstall(var needsRestart:Boolean): String;
var
  hWnd: Integer;
  ResultCode : integer;
  uninstallString : string;
begin
     Result := '';

     hWnd := StrToInt(ExpandConstant('{wizardhwnd}'));

     EnableFsRedirection(true);

     // Should we uninstall the old?
    if (Result = '') and (dotNetNeeded = true) then begin

    ExtractTemporaryFile('dotNetFx35setup.exe')
    if Exec(ExpandConstant(dotnetRedistPath), '/passive /norestart', '', SW_SHOW, ewWaitUntilTerminated, ResultCode) then begin
        // handle success if necessary; ResultCode contains the exit code
        if not (ResultCode = 0) then begin
          Result := '.NET Framework 3.5 is Required';
        end;
      end else begin
        // handle failure if necessary; ResultCode contains the error code
          Result := '.NET Framework 3.5 is Required';
      end;
    end;
end;

function UpdateReadyMemo(Space, NewLine, MemoUserInfoInfo, MemoDirInfo, MemoTypeInfo, MemoComponentsInfo, MemoGroupInfo, MemoTasksInfo: String): String;
var
  s: string;

begin
  if memoDependenciesNeeded <> '' then s := s + 'Dependencies that will be automatically downloaded And installed:' + NewLine + memoDependenciesNeeded + NewLine;

  Result := s
end;


// Custom Form has become active
procedure CustomForm_Activate(Page: TWizardPage);
begin
end;

