# Troubleshooting

## A key appears but does nothing

Confirm the action exists in Logi Options+, inspect the plugin log, and restart Logi Plugin Service after rebuilding.

## The wrong app profile appears

Verify the app bundle identifier in the application profile. The Home action intentionally opens Finder to restore the default launcher.

## Artwork is small or inset

Return `null` from `GetCommandDisplayName` and use `SetWidget(true)` for full-key decorative artwork.

## Voice produces blank audio

Check System Settings → Privacy & Security → Microphone, make sure the recorder helper is listed and enabled, and verify your `CODEX_KEYPAD_WHISPER_MODEL` path. Record for more than a brief tap before stopping.

## Automation cannot click menus or type

Enable Accessibility for the relevant Logitech service. Menu labels may differ by app version or locale; update the AppleScript accordingly.
