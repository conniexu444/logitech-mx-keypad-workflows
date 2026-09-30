on run argv
    if (count of argv) < 2 then error "Expected a workflow type and arguments."

    set workflowType to item 1 of argv
    tell application "ChatGPT" to activate
    delay 0.2

    if workflowType is "menu" then
        if (count of argv) is not 3 then error "A menu workflow needs a menu and item name."
        set menuName to item 2 of argv
        set itemName to item 3 of argv
        tell application "System Events" to tell process "ChatGPT"
            click menu item itemName of menu menuName of menu bar item menuName of menu bar 1
        end tell
        return
    end if

    if workflowType is "prompt" then
        if (count of argv) is not 2 then error "A prompt workflow needs prompt text."
        set promptText to item 2 of argv
        tell application "System Events" to tell process "ChatGPT"
            keystroke "n" using command down
            delay 0.35
            keystroke promptText
            delay 0.15
            key code 36
        end tell
        return
    end if

    error "Unknown Codex workflow type: " & workflowType
end run
