# Way of Working

Prefer native tool calling for reading and writing files over writing scripts to do that for you. If you do need to write custom scripts to do some complicated things, that's ok, but place them on your scratchpad, not in the project.

# GitHub Issues

You have access to the GitHub MCP, use it to read and write issues in the repository.

## Git

When writing commit messages, use Conventional Commits. Use a short, imperative summary, a concise body, and an optional footer. To reference GitHub issues in the footer, use `Refs: #<issue-number>`. If you want the commit to automatically close the issue, use `Closes: #<issue-number>`.

```
feat: add player movement

Added player movement and jumping mechanics.

Refs: #1
```

You don't need to make feature branches, we can work directly on the `main` branch.