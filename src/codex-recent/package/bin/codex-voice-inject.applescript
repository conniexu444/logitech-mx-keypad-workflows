on run argv
    if (count of argv) is not 1 then error "Expected transcribed prompt text."
    set promptText to item 1 of argv
    tell application "ChatGPT" to activate
    delay 0.25
    tell application "System Events" to tell process "ChatGPT"
        keystroke promptText
        delay 0.15
        key code 36
    end tell
end run
