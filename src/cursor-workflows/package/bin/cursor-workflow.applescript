on run argv
    if (count of argv) < 2 then error "Expected a workflow type and arguments."

    set workflowType to item 1 of argv
    tell application "Cursor" to activate
    delay 0.25

    if workflowType is "menu" then
        if (count of argv) is not 3 then error "A menu workflow needs a menu and item name."
        set menuName to item 2 of argv
        set itemName to item 3 of argv
        tell application "System Events" to tell process "Cursor"
            click menu item itemName of menu menuName of menu bar item menuName of menu bar 1
        end tell
        return
    end if

    if workflowType is "shortcut" then
        if (count of argv) is not 2 then error "A shortcut workflow needs a shortcut name."
        set shortcutName to item 2 of argv
        tell application "System Events"
            if shortcutName is "new_agent" then
                keystroke "n" using command down
            else if shortcutName is "open_folder" then
                keystroke "o" using command down
            else if shortcutName is "open_ide" then
                keystroke "n" using {command down, shift down}
            else if shortcutName is "changes" then
                keystroke "e" using command down
            else if shortcutName is "browser" then
                keystroke "b" using {command down, shift down}
            else if shortcutName is "files" then
                keystroke "g" using command down
            else if shortcutName is "terminal" then
                keystroke "j" using command down
            else if shortcutName is "command_palette" then
                keystroke "k" using command down
            else if shortcutName is "previous_tab" then
                key code 48 using {control down, shift down}
            else if shortcutName is "next_tab" then
                key code 48 using control down
            else if shortcutName is "up" then
                key code 126
            else if shortcutName is "enter" then
                key code 36
            else if shortcutName is "down" then
                key code 125
            else if shortcutName is "scroll_down" then
                key code 121
            else
                error "Unknown shortcut: " & shortcutName
            end if
        end tell
        return
    end if

    if workflowType is "prompt" then
        if (count of argv) is not 2 then error "A prompt workflow needs prompt text."
        set promptText to item 2 of argv
        tell application "System Events" to tell process "Cursor"
            click menu item "New Agent" of menu "File" of menu bar item "File" of menu bar 1
            delay 0.5
            keystroke promptText
            delay 0.15
            key code 36
        end tell
        return
    end if

    error "Unknown Cursor workflow type: " & workflowType
end run
