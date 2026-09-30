# Logitech MX Creative Console workflows

A privacy-safe reference implementation for turning the nine-key Logitech MX Creative Console keypad into an app-aware launcher and workflow surface for Codex, Claude Code, Cursor, Figma/FigJam, GitHub, VS Code, and other tools.

This repository documents a real daily setup, but contains no exported user profiles, task databases, recordings, tokens, usernames, or absolute home-directory paths.

## What the setup does

The default page is an app launcher. Opening an app activates its own keypad profile:

- **Codex:** three most recent tasks, task navigation, reusable prompts, Git/PR commands, and local Whisper dictation.
- **Claude Code:** launch a terminal session and organize voice/project commands.
- **Cursor:** navigation, Agent prompts, review, tests, Git, and PR creation.
- **Figma/FigJam:** create a FigJam board and select common shape tools.
- **Grok Bot:** app launcher or a custom command page.
- **Starlight:** a three-key pulsing-star animation used as decorative filler.

See [the exact layouts](docs/layouts.md), [how the pieces fit together](docs/architecture.md), and the [Claude/Grok recipes](docs/claude-and-grok.md).

## Requirements

- macOS on Apple silicon
- Logitech MX Creative Console with Logi Options+
- The desktop apps you choose to map
- .NET SDK 10 for custom Logitech plugins
- Python 3 and Pillow for generated icons
- Optional: `whisper.cpp` for local dictation

Run:

```bash
./scripts/check-prerequisites.sh
```

## Start safely

Logitech does not currently expose these profile files as a stable public configuration API. Back up your current setup before experimenting:

```bash
./scripts/backup-logi-config.sh
```

Then follow [setup.md](docs/setup.md). The source under `src/` is intentionally separated by workflow so you can borrow one idea without adopting the entire setup.

## Privacy and security

Read [privacy.md](docs/privacy.md) before publishing an exported profile. Codex recent-task buttons query only task IDs from your local Codex database; the database itself is never copied into this repository. Voice recordings are temporary and transcription runs locally.

## Project status

This is a community reference project, not an official Logitech, OpenAI, Anthropic, Cursor, Figma, GitHub, or xAI product. Application menu names and Logitech internals can change after updates.

## License

MIT. Product names and trademarks belong to their respective owners. Bring your own icons or generate neutral ones with the included scripts.
