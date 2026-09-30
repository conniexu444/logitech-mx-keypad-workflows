#!/bin/zsh
set -eu

repo_root="${0:A:h:h}"
cd "$repo_root"

if ! python3 -c 'import PIL' >/dev/null 2>&1; then
  printf 'Pillow is required. Install it with: python3 -m pip install Pillow\n' >&2
  exit 1
fi

if ! command -v dotnet >/dev/null 2>&1; then
  printf 'The .NET 10 SDK is required and dotnet is not on PATH.\n' >&2
  exit 1
fi

python3 src/codex-recent/generate_workflow_icons.py
python3 src/cursor-workflows/generate_icons.py
python3 src/figma-workflows/generate_icons.py
python3 src/starlight/generate_starlight_frames.py

recorder_bundle="$repo_root/src/codex-recent/package/bin/CodexVoiceRecorder.app"
recorder_binary="$recorder_bundle/Contents/MacOS/CodexVoiceRecorder"
swift_cache="$repo_root/.build-cache/swift"
rm -rf "$recorder_bundle"
mkdir -p "${recorder_binary:h}" "$swift_cache"
cp src/codex-recent/voice/Info.plist "$recorder_bundle/Contents/Info.plist"
CLANG_MODULE_CACHE_PATH="$swift_cache" SWIFT_MODULE_CACHE_PATH="$swift_cache" \
  swiftc -O -framework AVFoundation \
  src/codex-recent/voice/CodexVoiceRecorder.swift \
  -o "$recorder_binary"
xattr -cr "$recorder_bundle"
codesign --force --sign - "$recorder_bundle"

projects=(
  src/codex-recent/CodexRecentPlugin.csproj
  src/cursor-workflows/CursorWorkflowsPlugin.csproj
  src/figma-workflows/FigmaWorkflowsPlugin.csproj
  src/starlight/MatrixRainPlugin.csproj
)
for project in "${projects[@]}"; do
  dotnet build "$project" -c Release
done

printf '\nBuild complete. Run ./scripts/install-plugins.sh next.\n'
