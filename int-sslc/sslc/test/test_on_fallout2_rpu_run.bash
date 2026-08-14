set -e
set -u

if [[ -z "$SSLC" ]]; then
  echo "FAIL: No compiler env variable"
  exit 1
fi

SSLC=$(realpath "$SSLC")
# echo "Debug: fullpath=$SSLC"

. utils.bash

cd gamescripts

MODDERPACK_DIR=$(pwd)/modderspack


cd Fallout2_Restoration_Project/scripts_src

run_tests "$SSLC" "$MODDERPACK_DIR/scripting_docs/headers"