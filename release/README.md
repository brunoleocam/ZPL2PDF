# Release artifacts

This folder is **generated** by the release process. It contains the files that are uploaded to the [GitHub Release](https://github.com/brunoleocam/ZPL2PDF/releases).

**Do not edit manually.** Run the scripts in `release-scripts/` to (re)generate contents.

## Contents (after a full release)

- `ZPL2PDF-v{Version}-win-x64.zip`, `win-x86.zip`, `win-arm64.zip`
- `ZPL2PDF-v{Version}-linux-x64.tar.gz`, `linux-arm64.tar.gz`, `linux-arm.tar.gz`
- `ZPL2PDF-v{Version}-linux-amd64.deb`, `linux-x64-rpm.tar.gz`
- `ZPL2PDF-v{Version}-osx-x64.tar.gz`, `osx-arm64.tar.gz`
- `ZPL2PDF-Setup-{Version}.exe`
- `SHA256SUMS.txt`
- `ZPL2PDF-{Version}-source.zip`, `-source.tar.gz` (created by step 13)

The folder is in `.gitignore` and is not committed. See `release-scripts/README.md` for the full release process.
