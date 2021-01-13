IF NOT EXIST ".\Files" MKDIR ".\Files"
IF NOT EXIST ".\Files\x86" MKDIR ".\Files\x86"
IF NOT EXIST ".\Files\x64" MKDIR ".\Files\x64"

DEL /Q ".\Files\x86\*"
DEL /Q ".\Files\x64\*"

CD..
CD StatsDownload.FileDownload.Console

dotnet publish -c Release
XCOPY "bin\Release\netcoreapp3.1\publish" "..\StatsDownloadSetup\Files\x64" /e
dotnet publish -c Release
XCOPY "bin\Release\netcoreapp3.1\publish" "..\StatsDownloadSetup\Files\x86" /e

CD..
CD StatsDownloadSetup
DEL /Q ".\Files\x86\*.pdb"
DEL /Q ".\Files\x86\*.xml"
DEL /Q ".\Files\x64\*.pdb"
DEL /Q ".\Files\x64\*.xml"

"c:\Program Files (x86)\WiX Toolset v3.11\bin\heat.exe" dir ".\Files\x86" -dr INSTALLFOLDER -gg -sfrag -sreg -scom -cg FilesGroup -var var.FileSource -out Files-x86.wxs
"c:\Program Files (x86)\WiX Toolset v3.11\bin\heat.exe" dir ".\Files\x64" -dr INSTALLFOLDER -gg -sfrag -sreg -scom -cg FilesGroup -var var.FileSource -out Files-x64.wxs

WxsMerger.exe ".\Product.wxs.template" ".\Files-x86.wxs" ".\Files-x64.wxs" ".\Product.wxs"