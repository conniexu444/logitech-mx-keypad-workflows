# Architecture

The setup has three layers:

1. **Logi profile:** maps each physical key to a macro or plugin command and switches by foreground application.
2. **Plugin command:** a small C# class supplies the image, name, and action. Decorative commands mark themselves as widgets so artwork fills the key.
3. **macOS automation:** AppleScript invokes menu items or keyboard shortcuts in the target application. The voice action launches a tiny signed Swift recorder and runs local Whisper.

The code uses Logitech's bundled `PluginApi.dll` from the installed Logi Plugin Service. That API and the on-disk profile schema are implementation details, not a compatibility guarantee.

## Recent Codex tasks

The three recent-task commands query IDs from `~/.codex/state_5.sqlite` and open `codex://threads/<id>`. No task text is exported or embedded.

## Home navigation

App-specific profiles use Finder as a neutral Home target. Bringing Finder to the foreground makes the keypad return to its default launcher profile.

## Voice dictation

The Swift helper records a temporary 16 kHz mono WAV. `whisper-cli` transcribes it locally, an AppleScript inserts the result into the active Codex composer, and temporary audio is deleted. Set `CODEX_KEYPAD_WHISPER_MODEL` to your local model path.
