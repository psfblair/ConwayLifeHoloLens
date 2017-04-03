REM First command line argument is the path to the Unity folder, e.g.,
REM        C:\Program Files\Unity 5.6.0b9
REM Remaining command line arguments represent scenes to build, e.g.,
REM        Assets/Intro.unity Assets/ConwayLife.unity 

SET OUTPUT_DIR=App

cd %~dp0
if not exist "%OUTPUT_DIR%" mkdir "%OUTPUT_DIR%"

REM First comnand line argument
SET UNITY_EXECUTABLE=%1"\Editor\Unity.exe"

REM Get all but first command line argument 
@echo off
set SCENES_TO_BUILD=
shift
:loop1
if "%1"=="" goto after_loop
set SCENES_TO_BUILD=%SCENES_TO_BUILD% %1
shift
goto loop1
:after_loop

"%UNITY_EXECUTABLE%" -batchmode -quit -projectPath %~dp0 -executeMethod Autobuild.Build %SCENES_TO_BUILD%