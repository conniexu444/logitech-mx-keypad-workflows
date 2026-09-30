# Logitech MX Creative Console workflows

A reusable set of workflows for turning the nine-key Logitech MX Creative Console keypad into an app-aware launcher and workflow surface for Codex, Claude Code, Cursor, Figma/FigJam, GitHub, VS Code, and other tools.

These are the source files and layouts behind my daily keypad setup. You can build the plugins, install their actions, and map the same layouts onto your own Logitech keypad.

## Quick start

```bash
git clone https://github.com/conniexu444/logitech-mx-keypad-workflows.git
cd logitech-mx-keypad-workflows
./scripts/check-prerequisites.sh
./scripts/build.sh
./scripts/install-plugins.sh
```

Restart Logi Options+, then use [map-actions.md](docs/map-actions.md) to place the actions on your keypad. The installer adds the reusable actions; it does not overwrite your existing pages or button assignments.

## Homepage assets

The exact seven launcher images are included in [`assets/homepage`](assets/homepage):

<p>
  <img src="assets/homepage/claude.png" alt="Claude" width="72">
  <img src="assets/homepage/codex.png" alt="Codex" width="72">
  <img src="assets/homepage/grok-bot.png" alt="Grok Bot" width="72">
  <img src="assets/homepage/figma.png" alt="Figma" width="72">
  <img src="assets/homepage/vscode.png" alt="Visual Studio Code" width="72">
  <img src="assets/homepage/cursor.png" alt="Cursor" width="72">
  <img src="assets/homepage/github.png" alt="GitHub" width="72">
</p>

Use Logi Options+’s **Replace icon** control and select the matching PNG.

## Animated buttons

![Three-key pulsing star animation](assets/animated-stars-preview.gif)

The repository includes the full workflow behind the three synchronized pulsing-star buttons: a frame generator, a Logitech plugin that refreshes the key images at 10 FPS, and instructions for adapting it to your own animation.

Read **[Animated buttons: the three-key Starlight workflow](docs/animated-buttons.md)** for the step-by-step explanation.

## What the setup does

The default page is an app launcher. Opening an app activates its own keypad profile:

- **Codex:** three most recent tasks, task navigation, reusable prompts, Git/PR commands, and local Whisper dictation.
- **Claude Code:** launch a terminal session and organize voice/project commands.
- **Cursor:** navigation, Agent prompts, review, tests, Git, and PR creation.
- **Figma/FigJam:** create a FigJam board and select common shape tools.
- **Grok Bot:** homepage launcher only; custom Grok workflows are intentionally not included.
- **Starlight:** a three-key pulsing-star animation used as decorative filler.

See [the exact layouts](docs/layouts.md), [how the pieces fit together](docs/architecture.md), and the [Claude terminal launcher](docs/claude-launcher.md).

## Codex workflow

The Codex profile centers the three latest tasks on the top row and adds two pages for common prompts and Git work.

| Top row | Middle row | Bottom row |
|---|---|---|
| Recent 1 · Recent 2 · Recent 3 | Previous · New Task · Next | Home · Mic Voice · Starlight |

The mic records locally, transcribes with `whisper.cpp`, and sends the prompt to the current Codex task. Additional pages include Explore, Explain, Review, Tests, Fix Bug, Security, Git Status, Diff, Commit, Create PR, Push, and Terminal.

**[See the complete Codex workflow →](docs/codex-workflows.md)**

## Claude Code workflow

The Claude page combines offline dictation, spoken project navigation, terminal tabs, reusable prompts, answer controls, model/cost controls, and Git actions.

| Top row | Middle row | Bottom row |
|---|---|---|
| Voice · Go to Project · Voice Draft | Previous Tab · New Claude · Next Tab | Home · Review · Fix Bug |

The project voice action is designed for phrases such as “Go to my website project,” then matches the spoken name against a local project index and opens Claude in that folder. The public guide excludes the private project list.

**[See the complete Claude Code workflow →](docs/claude-workflows.md)**

## Cursor workflow

The Cursor profile follows the same muscle-memory layout as Claude, then adds dedicated prompt, status, and Git pages.

| Top row | Middle row | Bottom row |
|---|---|---|
| New Agent · Create PR · Command Palette | Previous Tab · New Agent · Next Tab | Home · Review Code · Fix Bug |

The Create PR action sends the full review-and-create prompt from `src/cursor-workflows/WorkflowCommands.cs`, while the other actions use Cursor menu items, shortcuts, or editable Agent prompts.

**[See the complete Cursor workflow →](docs/cursor-workflows.md)**

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

## Install manually or customize

Logitech does not currently expose these profile files as a stable public configuration API. Back up your current setup before experimenting:

```bash
./scripts/backup-logi-config.sh
```

Then follow [setup.md](docs/setup.md). The source under `src/` is separated by workflow so you can install one plugin or customize its prompts without adopting the entire setup.

## Personal data and security

The repository does not contain my personal paths, task database, recordings, or credentials because those would not work for anyone else. Read [privacy.md](docs/privacy.md) before publishing your own exported profile. Codex recent-task buttons query only task IDs from your local Codex database; voice transcription runs locally.

## Project status

This is a community reference project, not an official Logitech, OpenAI, Anthropic, Cursor, Figma, GitHub, or xAI product. Application menu names and Logitech internals can change after updates.

## License

MIT. Product names and trademarks belong to their respective owners. Bring your own icons or generate neutral ones with the included scripts.
