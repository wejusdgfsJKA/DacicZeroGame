# Workflow

How we move a task from Trello to `main`.

## Task lifecycle

1. Pick a task from **To Do** on Trello and move it to **In Progress**.
   - **Backlog** tasks are out of scope until a team meeting decides moving them into To Do.
2. Check out `main` and pull the latest changes.
3. Create a new branch off `main` for the task.
   - **If you wish,** prefix it with `feature/`, `fix/`, `chore/`, or `docs/`, or anything you deem fit given the case, depending on the type of change. This makes it easy to scan the branch/PR list and tell at a glance what a change is for.
   - Give it a descriptive name, e.g. `feature/bow`, `fix/npc-x-bug`.
   - See "Branch naming" below for the naming format.
4. Commit as you go, one commit per logically-separate change, with a message that clearly explains what the commit does.
5. When the task is done, open a Pull Request into `main`.
6. If your PR has conflicts with `main`, merge `main` into your branch and resolve them there.
7. If you need a feature that only exists on another branch (not yet in `main`), merge that branch into yours.
8. Once there are no conflicts (or they're resolved), move the Trello task to **In Review**.
9. Wait for at least one review from a teammate.
10. Merge the PR, then mark the Trello task **Done**.
    - See "Merge strategy" below for whether to squash or merge normally.
11. Delete the branch (locally and on the remote).

## Branch naming

Use only lowercase letters, digits, and `-` to split words.

- Good: `feature/bow-alt-fire`, `fix/ai-crash`
- Avoid: `Feature/BowAltFire`, `fix/AI_crash`

## Merge strategy

Default to a **regular merge** (keep all commits) when your commits are already logically separated and each one is meaningful on its own — this preserves history that's useful later for understanding why a change was made.

Use **squash merge** when the branch's commit history is messy and not worth keeping as-is, for example:
- Small task, e.g. a one-line fix or a tiny tweak, where multiple commits aren't meaningful.
- The history is full of "wip", "fix typo", "undo", or back-and-forth fixup commits.
- The branch was rebased/merged with `main` several times, leaving noisy merge commits.

## Team etiquette

- Before picking up a new task, check if any teammates have open PRs and review them first.
- Time is short: prioritize getting features complete over fully polished. Use placeholders (temporary art, sounds, numbers) rather than blocking a task on polish.

## See also

- [CodeGuidelines.md](CodeGuidelines.md) — code style conventions.