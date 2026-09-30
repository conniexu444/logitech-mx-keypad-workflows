#!/bin/zsh
set -eu

source_dir="$HOME/Library/Application Support/Logi/LogiPluginService"
timestamp="$(date -u +%Y%m%dT%H%M%SZ)"
destination="${1:-$PWD/backups/logi-plugin-service-$timestamp}"

if [[ ! -d "$source_dir" ]]; then
  printf 'Logi Plugin Service configuration not found: %s\n' "$source_dir" >&2
  exit 1
fi

if [[ -e "$destination" ]]; then
  printf 'Refusing to overwrite existing backup: %s\n' "$destination" >&2
  exit 1
fi

mkdir -p "${destination:h}"
cp -R "$source_dir" "$destination"
printf 'Backup created: %s\n' "$destination"
