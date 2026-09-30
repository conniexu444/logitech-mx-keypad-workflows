# Cursor workflow

Create a Cursor application profile and assign actions from **Cursor Workflows**. The included plugin controls Cursor with menu actions, shortcuts, and reusable Agent prompts.

## Page 1 — Cursor Commands

| Top row | Middle row | Bottom row |
|---|---|---|
| New Agent · Create PR · Command Palette | Previous Tab · New Agent · Next Tab | Home · Review Code · Fix Bug |

The Create PR action sends:

> Review and create a PR for this branch. Inspect the branch diff and recent commits, identify and fix any blocking correctness issues, run the relevant checks, then push the branch if needed and create the pull request with a concise title and a clear summary of the changes and verification performed. Preserve unrelated user changes and do not use destructive git commands.

## Page 2 — Status & Controls

| Top row | Middle row | Bottom row |
|---|---|---|
| Changes · Model Picker · Compact Context | Up · Enter · Down | Home · Scroll Down · Empty |

This mirrors Claude's status/answer page, preserving the same muscle memory across both tools.

## Page 3 — Cursor Prompts

| Top row | Middle row | Bottom row |
|---|---|---|
| Explore · Explain · Review Code | Optimize · Refactor · Write Tests | Home · Fix Bug · Security |

## Page 4 — Cursor Git

| Top row | Middle row | Bottom row |
|---|---|---|
| Commit · Create PR · Git Diff | Git Log · Push · Git Status | Home · Empty · Empty |

## Customize it

- Edit prompts in [`src/cursor-workflows/WorkflowCommands.cs`](../src/cursor-workflows/WorkflowCommands.cs).
- Edit application shortcuts in [`cursor-workflow.applescript`](../src/cursor-workflows/package/bin/cursor-workflow.applescript).
- Regenerate neutral icons with `src/cursor-workflows/generate_icons.py`.
- If a Cursor update renames a menu item, update the corresponding AppleScript menu label.
