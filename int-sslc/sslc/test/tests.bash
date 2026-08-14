set -e
set -u

. utils.bash

#
# This file runs sslc on test files in this folder
# and returns error code if something is not as expected.
#
# To reset snapshots call this file with `--reset` argument.


if [[ -z "${SSLC}" ]]; then
  echo "FAIL: No compiler env variable"
  exit 1
fi

SSLC=$(realpath "$SSLC")

cd embedded

set +u
if [[ "$1" == "--reset" ]]; then
  echo "== Resetting snapshots =="
  clear_test_snapshots
  if [[ -z "$2" ]]; then
    echo "ERROR: No path to snapshots provided, usage: $0 --reset <path_to_exe>"
    exit 1
  fi
  make_test_snapshots "$2" "../include"
  exit 0
fi
set -u

run_tests "$SSLC" "../include"