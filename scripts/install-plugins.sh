#!/bin/zsh
set -eu

repo_root="${0:A:h:h}"
plugin_root="$HOME/Library/Application Support/Logi/LogiPluginService/Plugins"
timestamp="$(date -u +%Y%m%dT%H%M%SZ)"
backup_root="$repo_root/backups/plugins-$timestamp"
dry_run=false

if [[ "${1:-}" == "--dry-run" ]]; then
  dry_run=true
elif [[ $# -gt 0 ]]; then
  printf 'Usage: %s [--dry-run]\n' "$0" >&2
  exit 2
fi

typeset -A releases
releases[CodexRecent]="$repo_root/src/codex-recent/Release"
releases[CursorWorkflows]="$repo_root/src/cursor-workflows/Release"
releases[FigmaWorkflows]="$repo_root/src/figma-workflows/Release"
releases[Starlight]="$repo_root/src/starlight/ReleaseStarlight"

for plugin_name release_dir in ${(kv)releases}; do
  if [[ ! -d "$release_dir" ]]; then
    printf 'Missing build output: %s (run ./scripts/build.sh first)\n' "$release_dir" >&2
    exit 1
  fi
done

if $dry_run; then
  printf 'Dry run: plugins would be installed under %s\n' "$plugin_root"
  for plugin_name release_dir in ${(kv)releases}; do
    printf '  %-18s <- %s\n' "$plugin_name" "$release_dir"
  done
  exit 0
fi

mkdir -p "$plugin_root" "$backup_root"
for plugin_name release_dir in ${(kv)releases}; do
  destination="$plugin_root/$plugin_name"
  if [[ -d "$destination" ]]; then
    cp -R "$destination" "$backup_root/$plugin_name"
  fi
  mkdir -p "$destination"
  cp -R "$release_dir/." "$destination/"
  printf 'Installed %s\n' "$plugin_name"
done

printf '\nExisting plugins were backed up to: %s\n' "$backup_root"
printf 'Restart Logi Options+, then follow docs/map-actions.md.\n'
