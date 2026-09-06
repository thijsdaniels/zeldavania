---
trigger: model_decision
description: When writing commit messages, making commits, managing branches, or performing git operations.
---

# Git Workflow & Commit Guidelines

## 1. Branch Strategy
- Work directly on the `main` branch unless explicitly asked otherwise; feature branches are not required.

## 2. Conventional Commits
When writing commit messages, use the Conventional Commits format:
- Short, imperative summary line (e.g. `feat(combat): implement player bow and arrow item system`).
- Concise body explaining non-obvious details and rationale.
- Optional footer referencing issues:
  - Reference without closing: `Refs: #<issue-number>`
  - Automatically close on merge: `Closes: #<issue-number>`

### Example:
```
feat(combat): add player movement

Added player movement and jumping mechanics.

Refs: #1
```

## 3. Amending Commits for Minor Follow-ups
- When making small corrections, minor tweaks, spec syncing, or formatting adjustments directly related to the immediately preceding commit, **amend the previous commit** (`git commit --amend` and `git push --force-with-lease`) instead of creating separate micro-commits that pollute history.

## 4. Permission Required Before Committing and Pushing
- **Always ask the user for permission** before making any commit (`git commit`, `git commit --amend`) or pushing to the remote repository (`git push`, `git push --force-with-lease`).
- Present the proposed commit message, staged changes summary, and target branch for user review and confirmation before executing the command.
