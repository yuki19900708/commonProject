printf '\33c\e[3J'
if [ "$1"x = "-sue"x ]; then
	bundle exec fastlane ios genIPA config_mode:release skip_unity_export:true force_dev:true
else
	bundle exec fastlane ios genIPA config_mode:release skip_unity_export:false force_dev:true
fi