@echo off & cd /d %~DP0 & TITLE %~DP0
cls
call gradlew -Pconfig_mode=release prepareForGenAPK
pause