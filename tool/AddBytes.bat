echo off
setlocal enabledelayedexpansion

set DIR="%cd%"
for /R %DIR% %%i in (*.skel) do (
    SET FILE_NAME_NOT_PATH=%%~nxi
    echo !FILE_NAME_NOT_PATH!
    echo %%i
    copy "%%i" "%%i.bytes"
)