on run argv
    if (count of argv) is not 2 then error "Expected an action type and value."

    set actionType to item 1 of argv
    set actionValue to item 2 of argv
    tell application "Figma" to activate
    delay 0.25

    tell application "System Events"
        if actionType is "key" then
            keystroke actionValue
            return
        end if

        if actionType is "shape" then
            keystroke "/" using command down
            delay 0.35
            keystroke actionValue
            delay 0.25
            key code 36
            return
        end if
    end tell

    error "Unknown Figma workflow type: " & actionType
end run
