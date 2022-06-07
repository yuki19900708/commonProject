@echo off & cd /d %~DP0 & TITLE %~DP0
cls
if "%1"x =="-sue"x (
	echo skip unity export
	call gradlew -Pconfig_mode=releaseLog -Pskip_unity_export=true genAPK
) else (
	call gradlew -Pconfig_mode=releaseLog -Pskip_unity_export=false genAPK
)
pause