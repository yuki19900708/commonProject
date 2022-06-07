echo off & color 0A

setlocal enabledelayedexpansion

set DIR="%cd%/../proto/"
for /R %DIR% %%i in (*.proto) do (
    SET FILE_NAME_NOT_PATH=%%~nxi
    echo !FILE_NAME_NOT_PATH!
    %cd%/../tool/protoc/bin/protoc.exe !FILE_NAME_NOT_PATH! --csharp_out=../project/unity_project/Assets/Scripts/Game/Protobuf --proto_path=%DIR%
)
pause