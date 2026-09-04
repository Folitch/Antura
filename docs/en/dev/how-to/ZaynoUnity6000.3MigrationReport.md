# Zayno Antura: Unity 6000.3 LTS migration report

Status: analysis only. No Unity migration and no build have been performed.

## Baseline

- Source branch preserved: `zayno-webgl` at commit `87a0359c91b72018255f98757421c6525fe5fe57`.
- Compatibility branch: `compat/unity-6000.3-lts`, created from the exact same commit.
- Current declared editor: `6000.5.10f1` revision `3bd4f66ad299`.
- Verified official Antura reference for Unity 6000.3: commit `c7e60a1dbecd26246700259211f83d8d5792a761`, editor `6000.3.13f1`.
- Migration target proposed for validation: `6000.3.13f1`, subject to that exact patch being available in Unity Build Automation.

Do not reset the project to the April reference commit: that would discard later code, assets and content. Preserve current gameplay and apply only compatibility changes.

## Files expected to change in a controlled migration

### Mandatory editor/package files

1. `ProjectSettings/ProjectVersion.txt`: change only after approval from 6000.5.10f1 to 6000.3.13f1.
2. `Packages/manifest.json`: align incompatible package versions with the last official 6000.3 Antura baseline.
3. `Packages/packages-lock.json`: regenerate with Unity 6000.3; never edit dependency hashes manually.
4. `ProjectSettings/ProjectSettings.asset`: inspect Unity's migration diff. Current serializedVersion is 29 versus 28 in the 6000.3 baseline. Do not replace the whole file because it also contains newer product, Android and WebGL settings.

### Potentially generated/settings files

5. `Assets/_discover/Settings/NewInput/AnturaInputActions.cs`: regenerate or compare if Input System is moved from 1.20.0 to 1.19.0.
6. `ProjectSettings/Packages/dev.yarnspinner/YarnSpinnerProjectSettings.json`: compare if Yarn Spinner is pinned back from the moving `current` branch to v3.0.3.
7. `ProjectSettings/PackageManagerSettings.asset`: inspect only if Unity 6000.3 rewrites registry/package settings.
8. `ProjectSettings/PhysicsCoreProjectSettings2D.asset`: Unity 6000.5-added settings; first test whether 6000.3 safely ignores them. Do not delete pre-emptively.
9. `ProjectSettings/Packages/com.unity.ai.assistant/Settings.json`: relevant only if the pre-release AI Assistant package is excluded from the 6000.3 manifest. Do not delete pre-emptively.
10. `Assets/Editor/ZaynoWebGLBuild.cs`: compile-check only. Its Addressables and WebGL APIs are expected to exist, but no edit should be made unless Unity 6000.3 reports a concrete API incompatibility.

### Conditional WebGL adaptations, separate from the editor downgrade

11. `Assets/_core/_scripts/Database/DBService.cs`: SQLiteConnection is used without a UNITY_WEBGL guard.
12. `Assets/_core/_scripts/_Core/AppConfig.cs`: contains the sqlite3 library naming/configuration path.
13. `Assets/_core/_scripts/_Core/Services/Gallery/GalleryService.cs`: calls NativeGallery directly without a UNITY_WEBGL guard.

No scene, prefab, model, animation, audio file, illustration or pedagogical database should be rewritten by the migration plan.

## Package alignment against the official Unity 6000.3 Antura baseline

| Package | Current | Official 6000.3 baseline | Proposed handling |
|---|---:|---:|---|
| com.unity.2d.spriteshape | 15.0.3 | 13.0.0 | Pin to baseline before import |
| com.unity.ai.assistant | 2.18.0-pre.2 | absent | Exclude from migration manifest unless proven compatible; editor-only |
| com.unity.ai.navigation | 2.0.14 | 2.0.12 | Pin to baseline |
| com.unity.cinemachine | 3.1.7 | 3.1.6 | Pin to baseline |
| com.unity.collections | 6.5.0 | 2.6.5 | High-risk downgrade; inspect compile errors and serialized data |
| com.unity.editorcoroutines | 1.1.0 | 1.0.1 | Pin to baseline |
| com.unity.ide.visualstudio | 2.0.28 | 2.0.27 | Editor-only; pin or omit in cloud |
| com.unity.inputsystem | 1.20.0 | 1.19.0 | Pin and verify generated input class |
| com.unity.localization | 1.5.12 | 1.5.11 | Pin and rebuild/validate tables |
| com.unity.mathematics | 1.4.0 | 1.3.3 | Pin to baseline |
| com.unity.mobile.android-logcat | 1.4.7 | absent | Editor-only; omit from migration manifest |
| com.unity.modules.physicscore2d | 1.0.0 | absent | Let Unity 6000.3 resolve; do not force |
| com.unity.probuilder | 6.1.2 | 6.0.9 | Pin to baseline; editor tooling |
| com.unity.recorder | 5.1.7 | 5.1.6 | Editor-only; pin or omit |
| com.unity.services.cloud-build | 2.0.8 | 2.0.7 | Pin to known baseline |
| com.unity.services.core | 1.18.0 | 1.16.0 | Pin and verify transitive services |
| com.unity.shadergraph | 17.5.0 | 17.3.0 | Pin; reimport shaders without rewriting materials |
| com.unity.splines | 2.9.0 | 2.8.4 | Pin; inspect serialization warnings |
| com.unity.test-framework | 1.7.0 | 1.6.0 | Pin; editor/test only |
| com.unity.test-framework.performance | 3.5.0 | 3.4.0 | Pin; test only |
| com.unity.timeline | 1.8.13 | 1.8.12 | Pin; validate timelines and animation events |
| com.unity.ugui | 2.5.0 | 2.0.0 | High-risk downgrade; validate every UI scene |
| dev.yarnspinner.unity | Git `current` | Git tag v3.0.3 | Pin to immutable tag; validate dialogue imports |

Keep packages not listed above unchanged initially, including Addressables 2.9.1, NativeGallery, Mobile Notifications 2.4.3 and UniTask, because the official 6000.3 baseline already used the same declarations. Their WebGL suitability is a separate gate.

## WebGL-specific impact

### SQLite

Native libraries exist for Android, Windows and Linux, but no WebGL sqlite3 plugin was identified. `DBService` opens SQLite databases through `Application.persistentDataPath` with no WebGL compile guard. Expected risks: IL2CPP/WebAssembly link failure, missing native symbol, or runtime persistence failure. Plan a WebGL storage adapter or a proven WebGL SQLite implementation; do not remove the desktop/mobile implementation.

### NativeGallery

`GalleryService` calls NativeGallery directly and no UNITY_WEBGL guard exists in the current repository. Browser download/export requires a separate WebGL implementation or a disabled gallery capability with explicit UI behavior. Preserve mobile behavior.

### Mobile notifications

The package is present but no direct AndroidNotificationCenter/MobileNotifications call was found. Keep it during the first compatibility import because it was already present in the official 6000.3 baseline. Confirm that its assemblies exclude WebGL before the first build.

### Git LFS

FBX, WAV, MP3, OGG, Terrain assets and GLB files are LFS-managed. Migration must use a full LFS checkout. Never accept a build where these files remain pointer text. Compare LFS object counts and representative hashes before and after migration.

### Git packages

Current Git dependencies are not all immutable: bgTools uses `#upm`, Yarn uses `#current`, and some URLs have no commit SHA. The controlled migration should pin every runtime-relevant Git package to the exact revision resolved by the official 6000.3 lockfile where possible. Moving refs create non-reproducible builds and can make package resolution appear hung.

## Risks

1. Collections 6.5.0 to 2.6.5 and UGUI 2.5.0 to 2.0.0 are the largest API/serialization risks.
2. Opening 6000.5-serialized project settings with 6000.3 may cause automatic rewrites; review every generated diff before commit.
3. Shader Graph, Splines, Timeline, Localization and Input System downgrades may trigger reimports or generated-file changes.
4. SQLite and NativeGallery can block WebGL independently of the Unity version migration.
5. LFS or moving Git package refs can cause a long import, missing media or non-reproducible results.
6. Reverting the whole repository to the old official commit would lose newer assets, content and mechanics and is prohibited.

## Controlled sequence proposed after approval

1. Confirm that Unity Build Automation offers exactly 6000.3.13f1; otherwise select no substitute without a new review.
2. Modify only ProjectVersion and manifest on the compatibility branch.
3. Let Unity 6000.3 resolve a fresh lockfile and import without saving scenes.
4. Review all generated diffs; reject any scene, prefab or asset rewrite.
5. Resolve compile errors package by package.
6. Add WebGL platform adapters for SQLite and NativeGallery only after the editor import is stable.
7. Run source/LFS/legal inventories before any Build Automation execution.
8. Launch no build until this migration diff is approved.

## Current verdict

A controlled Unity 6000.3 migration is plausible because the official project used 6000.3.13f1 recently. It is not a one-line ProjectVersion change: 23 direct package declarations differ, two major packages have large version gaps, and WebGL still needs explicit persistence/gallery handling. The compatibility branch is ready for a reviewed migration, but remains byte-for-byte identical to zayno-webgl except for this report.