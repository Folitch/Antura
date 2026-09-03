# Antura WebGL build for Zayno

This branch prepares the official Antura Unity project for a browser build without porting or simplifying its games.

## Pinned source and engine

- Source branch: zayno-webgl, forked from the verified official commit 2ba9d7d9f0170d807ce4048f87821e47b16efa8b.
- Unity: 6000.5.10f1 with WebGL Build Support.
- Git LFS files must be fully downloaded before Unity imports or builds the project.

## Arabic content gate

The build stops unless the Arabic learning-block, play-session, letter and word databases exist. It also checks for stage 6, minigame sessions and assessment sessions in the Arabic schedule.

The current multiedition branch does not contain the same phrase/audio inventory as the historical Arabic edition. Keep vgwb/Antura_arabic as a comparison source; do not delete historical content merely because it is absent here.

## Unity Build Automation configuration

1. Create a Unity Cloud project and connect repository Folitch/Antura.
2. Select branch zayno-webgl and Unity 6000.5.10f1.
3. Enable Git LFS checkout and the WebGL Build Support module.
4. Set target platform to WebGL.
5. Invoke Zayno.Build.WebGLBuild.Perform as the custom build method or from a batchmode pre-build step.
6. Publish the complete Builds/ZaynoWebGL directory as the artifact.

Batchmode equivalent:

    Unity -batchmode -quit -projectPath . -executeMethod Zayno.Build.WebGLBuild.Perform -logFile -

## Required validation before Zayno integration

- Build log contains ANTURA_ARABIC_CONTENT_OK and ANTURA_WEBGL_BUILD_OK.
- Addressables are present and load from the final same-origin URL.
- Arabic contextual forms, diacritics, words and audio are tested in desktop Chrome, Android Chrome and iOS Safari.
- Every enabled minigame launches through Antura's Teacher and progression systems.
- NativeGallery, SQLite, analytics and mobile notifications are reviewed or conditioned for WebGL.
- Build download size, startup memory, loading time and browser audio unlock are measured.
- No Zayno game card is connected until this gate passes.

## Planned Zayno architecture after a successful build

Serve the immutable Unity output and Addressables on the same origin, open it from a protected full-screen child route, and exchange only identity/progression events through a narrow Unity-JavaScript bridge. Do not port the minigames to React.

## Licensing gate

Keep LICENSE.md and Credits files. Code is BSD-2-Clause and first-party digital assets are CC-BY-4.0 unless overridden. Fonts, third-party packages, learning data, branding and recorded voices need an itemized release review before public deployment.