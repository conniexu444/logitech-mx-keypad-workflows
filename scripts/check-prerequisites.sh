#!/bin/zsh
set -eu

failures=0
check() {
  if command -v "$1" >/dev/null 2>&1; then
    printf 'OK   %s\n' "$1"
  else
    printf 'MISS %s\n' "$1"
    failures=$((failures + 1))
  fi
}

check dotnet
check python3
check swiftc
check osascript

plugin_api='/Applications/Utilities/LogiPluginService.app/Contents/MonoBundle/PluginApi.dll'
if [[ -f "$plugin_api" ]]; then
  printf 'OK   Logi Plugin API\n'
else
  printf 'MISS Logi Plugin API: %s\n' "$plugin_api"
  failures=$((failures + 1))
fi

exit "$failures"
