# Known Issues

## Repo / infrastructure

- **Git history is bloated (~200MB) by accidentally committed `Library/` files.** Early commits (project creation and a few after) committed Unity's `Library/ArtifactDB` and `Library/SourceAssetDB` cache files, some individual blobs as large as 67MB. `.gitignore` correctly excludes `Library/` now, but the old blobs are permanently in history, so every fresh clone still pays for it. Fixing this requires rewriting git history (e.g. with `git filter-repo`), which forces everyone to re-clone — should be planned and done deliberately, not casually.
- **No Git LFS.** Binary assets (audio, textures, etc.) are committed directly instead of through Git LFS. Recommended to set up before adding more/larger binary assets, to avoid repeating the bloat described above.
- **Unity version (6000.2.6f2) might have a problem** — remains to be investigated.
