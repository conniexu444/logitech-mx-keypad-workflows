# Privacy checklist

Before publishing a profile or pull request:

- Search for `/Users/`, usernames, email addresses, phone numbers, and organization names.
- Exclude `.codex/state_*.sqlite`, Logitech logs, audio recordings, temporary transcripts, and exported task content.
- Exclude access tokens, API keys, `.env` files, cookies, and application databases.
- Replace absolute paths with `$HOME`, `Path.home()`, or `Environment.SpecialFolder.UserProfile`.
- Do not redistribute app logos unless their license permits it; generated neutral icons are safer.
- Inspect profile JSON for local application paths and opaque identifiers before sharing.

Run the included scan:

```bash
./scripts/scan-for-private-data.sh
```

The scan is a guardrail, not a guarantee. Review the Git diff manually before publishing.
