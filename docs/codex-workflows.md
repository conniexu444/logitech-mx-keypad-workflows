# Codex workflow

Create an application profile for the ChatGPT desktop app and assign actions from **Codex Workflows**. The included `CodexRecent` plugin supplies the commands.

## Page 1 — Recent & Navigate

| Top row | Middle row | Bottom row |
|---|---|---|
| Recent 1 · Recent 2 · Recent 3 | Previous Task · New Task · Next Task | Home · Mic Voice · Starlight Right |

- **Recent 1–3** query the three most recently used, non-archived Codex tasks from the local Codex state database and open the selected task through its `codex://` URL.
- **Previous/Next Task** select the neighboring task through the ChatGPT desktop menu.
- **New Task** opens a fresh Codex task.
- **Mic Voice** starts recording on the first press and stops, transcribes, and sends on the second press.
- **Home** activates Finder so the keypad returns to the default launcher profile.

## Page 2 — Codex Prompts

| Top row | Middle row | Bottom row |
|---|---|---|
| Explore · Explain · Review Code | Optimize · Refactor · Write Tests | Home · Fix Bug · Security |

These buttons open a new task with a complete, editable prompt. The prompts live in [`src/codex-recent/WorkflowCommands.cs`](../src/codex-recent/WorkflowCommands.cs).

## Page 3 — Git & Review

| Top row | Middle row | Bottom row |
|---|---|---|
| Git Status · Git Diff · Git Log | Commit · Create PR · Push | Home · Open Terminal · New Task |

The Git buttons ask Codex to inspect or perform the operation in the current project. Mutating prompts explicitly preserve unrelated work and avoid destructive Git commands.

## Voice setup

The optional voice workflow is fully local:

1. `CodexVoiceRecorder` records a temporary 16 kHz mono WAV.
2. `whisper-cli` transcribes it using the model in `CODEX_KEYPAD_WHISPER_MODEL`.
3. AppleScript inserts the transcript into the active Codex composer.
4. Temporary audio and transcript files are deleted.

See [setup.md](setup.md#6-optional-voice-support) for requirements and macOS microphone permissions.

## Customize it

- Change prompt text in `WorkflowCommands.cs`.
- Change the number of recent tasks by adding another `RecentCommand` subclass and icon.
- Replace Starlight with any other action if you prefer all nine keys to be functional.
- Generate neutral icons with `generate_workflow_icons.py`.
