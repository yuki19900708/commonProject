printf '\33c\e[3J'
if [ "$1"x = "-sue"x ]; then
	./gradlew -Pconfig_mode=releaseDebugLog -Pskip_unity_export=true genAPK
else
	./gradlew -Pconfig_mode=releaseDebugLog -Pskip_unity_export=false genAPK
fi