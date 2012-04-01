@echo off

set ILMERGE=%ProgramFiles(x86)%\Microsoft\ILMerge\ILMerge.exe

if not exist "%ILMERGE%" set ILMERGE=%ProgramFiles%\Microsoft\ILMerge\ILMerge.exe

if not exist "%ILMERGE%" (
	echo Cannot find ILMerge
	goto exit
)

"%ILMERGE%" /keyfile:Expressions\Key.snk /out:Expressions.dll Expressions\bin\Release\Expressions.dll Libraries\Antlr\Antlr3.Runtime.dll

:exit
