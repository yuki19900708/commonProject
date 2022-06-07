printf '\33c\e[3J'
./gradlew publishAtlas
git add -A
git commit -m "更新图集"
git push --set-upstream origin dev