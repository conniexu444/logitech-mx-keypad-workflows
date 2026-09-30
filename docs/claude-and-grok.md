# Claude Code and Grok recipes

## Claude Code

The launcher opens Terminal through a small `.app` wrapper. Its executable can be a shell script like:

```zsh
#!/bin/zsh
exec /usr/bin/open -a Terminal "$HOME/Applications/Claude.command"
```

The referenced command should `cd` to the desired project and start `claude`. Keep project paths in a local file that is not committed. A voice-project action can transcribe “Go to my ___ project,” match it against a local project index, and start Claude in that folder.

Do not copy API keys or Claude configuration into a launcher bundle.

## Grok Bot

The simplest workflow maps a key to **Open Application** for Grok Bot. An app-specific page can reuse the same nine-key pattern as Claude or Cursor: navigation across the middle row, common prompts on another page, and Home at bottom-left.

The custom Grok agent definitions from the original private setup are intentionally not included because they can reveal project names and personal working context.
