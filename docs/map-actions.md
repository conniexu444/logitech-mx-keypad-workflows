# Map the actions in Logi Options+

After running `scripts/install-plugins.sh`, open Logi Options+ and select the MX Creative Console keypad. The right-hand action browser will contain these groups.

## Codex Workflows

Create an application profile for ChatGPT/Codex and map:

| Page | Top row | Middle row | Bottom row |
|---|---|---|---|
| Recent & Navigate | Recent 1 · Recent 2 · Recent 3 | Previous Task · New Task · Next Task | Home · Mic Voice · Starlight Right |
| Prompts | Explore · Explain · Review Code | Optimize · Refactor · Write Tests | Home · Fix Bug · Security |
| Git & Review | Git Status · Git Diff · Git Log | Commit · Create PR · Push | Home · Open Terminal · New Task |

“Home” can be a standard **Open Application → Finder** action. Bringing Finder forward returns the keypad to its default profile.

## Cursor Workflows

Create a Cursor application profile. Useful first-page actions are:

| Top row | Middle row | Bottom row |
|---|---|---|
| New Agent · Create PR · Command Palette | Previous Tab · New Agent · Next Tab | Home · Review Code · Fix Bug |

Add separate pages for the prompt and Git commands if desired. Every prompt is visible and editable in `src/cursor-workflows/WorkflowCommands.cs`.

## Figma Workflows

Create a Figma application profile:

| Top row | Middle row | Bottom row |
|---|---|---|
| Square · Ellipse · Diamond | Triangle · Star · Connector | Home · Sticky · Text |

## Starlight

Place **Starlight Left**, **Starlight Center**, and **Starlight Right** next to one another. These are decorative widget actions and intentionally do nothing when pressed.

## Launcher page

Use standard Logi Options+ **Open Application** or **Open Webpage** actions:

| Top row | Middle row | Bottom row |
|---|---|---|
| Decorative · Decorative · Figma | Claude Code · ChatGPT/Codex · Grok Bot | VS Code · Cursor · GitHub |

For Claude Code, see [claude-and-grok.md](claude-and-grok.md) for a terminal launcher.

The matching images are in [`../assets/homepage`](../assets/homepage). Select **Replace icon** for each launcher action and choose the corresponding PNG.
