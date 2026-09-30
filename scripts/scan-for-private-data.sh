#!/bin/zsh
set -eu

repo_root="${0:A:h:h}"
pattern='(/Users/[^/[:space:]]+|[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}|BEGIN (RSA|OPENSSH|EC) PRIVATE KEY|api[_-]?key|access[_-]?token|client[_-]?secret|password[[:space:]]*[:=])'

if rg -n -i --hidden \
  --glob '!.git/**' \
  --glob '!**/obj/**' \
  --glob '!**/Release*/**' \
  --glob '!**/__pycache__/**' \
  --glob '!scripts/scan-for-private-data.sh' \
  --glob '!docs/privacy.md' \
  "$pattern" "$repo_root"; then
  printf '\nPotential private data found. Review every match before publishing.\n' >&2
  exit 1
fi

printf 'No obvious PII, credentials, or private database paths found.\n'
