# Claude Code workflow

This layout is designed for a Claude Code terminal session. It keeps the most common voice, navigation, review, answer, and Git actions in predictable positions.

The repository documents the reusable layout and launcher. It intentionally does not include private project names, directory indexes, terminal history, credentials, or custom agents.

## Page 1 — Voice & Commands

| Top row | Middle row | Bottom row |
|---|---|---|
| Voice · Go to Project · Voice Draft | Previous Tab · New Claude · Next Tab | Home · Review · Fix Bug |

- **Voice** records a spoken prompt, transcribes it locally, and submits it to Claude.
- **Go to Project** accepts a phrase such as “Go to my website project,” searches a local project index, opens that directory, and starts a Claude session.
- **Voice Draft** transcribes into the terminal without immediately submitting, allowing review first.
- **Previous/New/Next** navigate terminal tabs or start another Claude session.
- **Review/Fix Bug** submit reusable engineering prompts.

## Page 2 — Status & Answers

| Top row | Middle row | Bottom row |
|---|---|---|
| Cost · Model · Compact | Up · Enter · Down | Home · Scroll Down · Empty |

This page is useful while Claude is presenting a choice or waiting for confirmation. It also exposes session cost, model selection, and context compaction.

## Page 3 — Claude Prompts

| Top row | Middle row | Bottom row |
|---|---|---|
| Explore · Explain · Review | Optimize · Refactor · Write Tests | Home · Fix Bug · Security |

Keep prompts generic so they work in any repository. Project-specific prompts belong in a local, uncommitted configuration.

## Page 4 — Claude Git

| Top row | Middle row | Bottom row |
|---|---|---|
| Commit · Create PR · Diff | Log · Push · Status | Home · Empty · Empty |

## Reproduce the project voice action

1. Maintain a local list of project names and absolute paths outside the Git repository.
2. Record audio and transcribe it locally with `whisper.cpp`.
3. Remove a phrase prefix such as “go to my” and normalize the remaining project name.
4. Match the name against the local project index.
5. Open Terminal in the matched directory and start `claude`.
6. If no match is confident, show the likely matches instead of choosing silently.

See [claude-launcher.md](claude-launcher.md) for a minimal terminal launcher. Grok remains a homepage launcher only; no custom Grok workflows are published.
