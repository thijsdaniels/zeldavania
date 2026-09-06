# OpenSpec Workflow Rule

## Strict Workflow Guardrails

When executing OpenSpec commands or workflows (`/opsx-new`, `/opsx-continue`, `/opsx-propose`, `/opsx-update`, `/opsx-apply`, `/opsx-sync`, `/opsx-verify`, `/opsx-archive`):

1. **Do NOT Create Artifact Files During `/opsx-new`**:
   - `/opsx-new` ONLY initializes/scaffolds the change directory and shows the instructions/template for the first artifact (e.g. `proposal.md`).
   - You MUST NOT write content to `proposal.md`, `spec.md`, `design.md`, or `tasks.md` during `/opsx-new`.
   - STOP immediately after showing the first artifact instructions and wait for the user to trigger `/opsx-continue` or describe what to write.

2. **One Artifact At A Time**:
   - Never generate multiple artifacts in a single turn unless explicitly running `/opsx-propose` (which generates all artifacts at once).
   - Always await user direction or approval between artifact steps.

3. **Shell Environment**:
   - Follow [`git-bash-environment.md`](file:///c:/Users/thijs/Repositories/thijsdaniels/metroidvania/.agents/rules/git-bash-environment.md) for running `openspec` CLI commands.

## Requirement Granularity & Modularity

- **Keep Requirements Atomic**: Each `### Requirement: <Name>` MUST specify a single, cohesive capability or lifecycle behavior. Never lump multiple distinct behaviors (e.g. sleeping, chasing, falling, dying) into a single giant catch-all requirement.
- **Prevent Parallel Change Conflicts**: OpenSpec replaces the entire requirement block when merging a `MODIFIED` requirement at archive time. Keeping requirements fine-grained ensures concurrent changes can modify different aspects of the same system without merge collisions or overwriting unrelated scenarios.
- **Prefer `ADDED` for New Behaviors**: When adding new functionality to an existing capability, define a new atomic requirement under `## ADDED Requirements` rather than modifying and bloating existing broad requirements.

