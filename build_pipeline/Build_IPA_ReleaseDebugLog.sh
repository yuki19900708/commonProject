printf '\33c\e[3J'
if [ "$1"x = "-sue"x ]; then
	bundle exec fastlane ios genIPA config_mode:releaseDebugLog skip_unity_export:true
else
	bundle exec fastlane ios genIPA config_mode:releaseDebugLog skip_unity_export:false
fi