printf '\33c\e[3J'
# 生成完成后要还原被修改的图集设置，要还原的路径和CommandBuild.BuildPVRTCAssetBundles里修改的路径保持一致
git checkout ../project/unity_project/Assets/ResourcesRaw/Atlas