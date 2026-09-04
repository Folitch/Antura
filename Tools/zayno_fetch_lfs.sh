#!/usr/bin/env bash
    set -euo pipefail

    REPOSITORY_ROOT="$(git rev-parse --show-toplevel)"
    cd "$REPOSITORY_ROOT"

    echo "ANTURA_LFS_FETCH_START"
    git lfs install --local
    git config --local lfs.concurrenttransfers 16
    git -c lfs.fetchexclude= -c lfs.fetchinclude= lfs pull

    missing_count="$(git lfs ls-files | awk 'substr($0, 12, 1) == "-" { missing++ } END { print missing + 0 }')"
    if [ "$missing_count" -ne 0 ]; then
      echo "ANTURA_LFS_ERROR missing_files=$missing_count" >&2
      exit 1
    fi

    echo "ANTURA_LFS_OK"
    