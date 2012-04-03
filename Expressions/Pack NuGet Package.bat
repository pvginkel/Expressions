@echo off

rem We need to replace Expressions.dll in the release folder with the merged
rem one.

echo Merging

cd ..
call Merge.bat
cd Expressions

echo Copying files

del bin\Release\Expressions.dll
del bin\Release\Antlr3.Runtime.dll
copy ..\Expressions.dll bin\Release

echo Packing

..\Libraries\NuGet\nuget.exe pack -prop configuration=release
