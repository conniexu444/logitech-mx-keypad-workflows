# Setup guide

## Automatic plugin installation

The fastest path is:

```bash
./scripts/check-prerequisites.sh
./scripts/build.sh
./scripts/install-plugins.sh
```

This installs the custom action plugins but deliberately leaves your current button assignments alone. Restart Logi Options+ and follow [map-actions.md](map-actions.md).

## Manual installation

### 1. Back up Logi Options+

Quit Logi Options+, run `scripts/backup-logi-config.sh`, and keep the printed backup path.

### 2. Create the launcher in Logi Options+

Use the default profile and assign **Open Application** actions following `config/workflows.example.json`. Use your own icons or generated neutral icons. Avoid committing exported profiles because they can contain local paths and identifiers.

### 3. Generate plugin artwork

```bash
python3 -m pip install Pillow
python3 src/codex-recent/generate_workflow_icons.py
python3 src/cursor-workflows/generate_icons.py
python3 src/figma-workflows/generate_icons.py
python3 src/starlight/generate_starlight_frames.py
```

### 4. Build plugins

Each `.csproj` references the `PluginApi.dll` shipped inside Logi Plugin Service:

```bash
dotnet build src/codex-recent/CodexRecentPlugin.csproj -c Release
dotnet build src/cursor-workflows/CursorWorkflowsPlugin.csproj -c Release
dotnet build src/figma-workflows/FigmaWorkflowsPlugin.csproj -c Release
dotnet build src/starlight/MatrixRainPlugin.csproj -c Release
```

Install the generated plugin directory under:

```text
~/Library/Application Support/Logi/LogiPluginService/Plugins/
```

Restart Logi Options+ after installing. Add the newly available actions to app-specific profiles through the UI.

### 5. Enable macOS permissions

System Settings → Privacy & Security may need:

- Accessibility for Logi Plugin Service automation
- Microphone for the optional voice recorder

Grant only permissions required by the workflows you install.

### 6. Optional voice support

Install `whisper.cpp`, download a model yourself, and set:

```bash
export CODEX_KEYPAD_WHISPER_MODEL="$HOME/.cache/whisper/ggml-base.en.bin"
```

`scripts/build.sh` compiles and locally signs the supplied Swift recorder. The first launch should produce the macOS microphone prompt.
