set -e
set -u


SSLC_FLAGS="-q -p -l -O2 -d -s -n"


clear_test_snapshots() {
  echo "== Removing test snapshots in $(pwd) =="
  find . -type f -iname '*.expected' -delete -print
  echo "Done"
}

NEW_LINE='
'

make_test_snapshots() {
  REFERENCE_COMPILE_EXE=$1 # Note: it have to be full path to sslc executable
  INCLUDE_HEADERS_DIR=$2 # Note: it have to be full path or relative to .ssl file

  WINE_IS_INSTALLED=""
  if ! which apt >/dev/null 2>&1; then
    echo "Runnion on Windows, not using wine"
    WINE=""
  else
    echo "Running on Linux, using wine"
    WINE="wine"
  fi

  COMPILER_IS_CHECKED=""

  echo "== Building test snapshots in $(pwd) =="
  COMPILATION_FAILED_FILES=""

  for f in $(find . -type f -iname '*.ssl') ; do
    OLD_PWD=$(pwd)

    DIR=$(dirname $f)
    FBASE=$(basename -s .ssl $f)
    FNAME=$(basename $f)


    cd "$DIR"

    if [[ -f "$FBASE.stdout.expected" && -f "$FBASE.returncode.expected" ]]; then
      echo "$DIR/$FNAME: already built, skipping"
    else
      echo "$DIR/$FNAME: building reference snapshot"
      set +e

      if [[ ! -n "$WINE_IS_INSTALLED" && -n "$WINE" ]]; then
        if ! which wine >/dev/null; then
          echo "Installing wine"
          sudo dpkg --add-architecture i386
          sudo apt update
          sudo apt install -y wine32
        else
          echo "Wine is already installed"
        fi
        WINE_IS_INSTALLED="yes"
      fi

      if [[ ! -n "$COMPILER_IS_CHECKED" ]]; then
        echo "Checking compiler"
        if $WINE $REFERENCE_COMPILE_EXE | grep -q "Startrek Scripting Language compiler"; then
          echo "Compiler is ok"
        else
          echo "Compiler is not working, please check your modderspack installation"
          exit 1
        fi
        COMPILER_IS_CHECKED="yes"
      fi

      $WINE $REFERENCE_COMPILE_EXE $SSLC_FLAGS \
        "-I$INCLUDE_HEADERS_DIR" \
        "$FNAME" -o "$FBASE.int.expected" > "$FBASE.stdout.expected"
      RETURN_CODE_EXPECTED=$?
      echo -n "$RETURN_CODE_EXPECTED" > "$FBASE.returncode.expected"
      set -e
      sed -i 's/\r//g' $FBASE.stdout.expected
      # sed -i 's#[a-zA-Z0-9\/\:]*/test/gamescripts/Fallout2_Restoration_Project/scripts_src/#/scripts_src/#g' "$FBASE.stdout.expected" # On wine absolute paths can be different
      sed -i 's#[a-zA-Z0-9_./:\\-]*/test/gamescripts/#/#g' "$FBASE.stdout.expected" # On wine absolute paths can be different
      sed -i 's#[a-zA-Z0-9_./:\\-]*/test/embedded/#/#g' "$FBASE.stdout.expected" # On wine absolute paths can be different

      echo "  Done, return code $RETURN_CODE_EXPECTED"

      if [ "$RETURN_CODE_EXPECTED" -ne 0 ]; then
        COMPILATION_FAILED_FILES="$COMPILATION_FAILED_FILES$DIR/$FNAME$NEW_LINE"
      fi

    fi      




    cd "$OLD_PWD"
  done

  if [ -n "$COMPILATION_FAILED_FILES" ]; then
    echo "=== Compilation errors found in the following files: ==="
    echo "$COMPILATION_FAILED_FILES"
  fi    
}



run_tests() {
  TESTING_SSLC=$1 # Note: it have to be full path to testing sslc executable
  INCLUDE_HEADERS_DIR=$2 # Note: it have to be full path or relative to .ssl file

  if [[ ! -f "$TESTING_SSLC" ]]; then
    echo "ERROR: No compiler executable found at $TESTING_SSLC"
    exit 1
  fi

  TEST_FAILED_FILES=""

  TESTS_FAILED_COUNT=0
  TESTS_SUCCESS_COUNT=0
  EXPECTED_SUCCESSFULL_COMPILED_FILES=0

  for f in $(find . -type f -iname '*.ssl') ; do
    if [ "$f" != "./epa/epac8.ssl" ]; then
      # continue # Debugging
      true
    fi

    DIR=$(dirname $f)
    FBASE=$(basename -s .ssl $f)
    FNAME=$(basename $f)

    OLD_PWD=$(pwd)
    echo "======================= $DIR/$FNAME ========================"
    cd "$DIR"

    # Expected build
    if [ ! -f "$FBASE.returncode.expected" ]; then
      echo "ERROR: NO EXPECTED RETURN CODE FILE $FBASE.returncode.expected"
      echo "Please run test/test_on_fallout2_rpu_setup.bash first"
      echo ""
      echo "If this error appears on CI then try to bump cache version in all actions/cache@v4"
      exit 1
    fi
    RETURN_CODE_EXPECTED=$(cat "$FBASE.returncode.expected")
    if [ "$RETURN_CODE_EXPECTED" -eq 0 ]; then
        EXPECTED_SUCCESSFULL_COMPILED_FILES=$((EXPECTED_SUCCESSFULL_COMPILED_FILES + 1))
    fi
    # On Windows it might checkout files and automatically use CLRF line endings
    sed -i 's/\r//g' "$FBASE.stdout.expected"

    # Obvserved build
    set +e
    $TESTING_SSLC $SSLC_FLAGS \
      "-I$INCLUDE_HEADERS_DIR" \
      "$FNAME" -o "$FBASE.int.observed" > "$FBASE.stdout.observed"
    RETURN_CODE_OBSERVED=$?
    set -e
    sed -i 's/\r//g' "$FBASE.stdout.observed"
    sed -i 's#[a-zA-Z0-9_./:\\-]*/test/gamescripts/#/#g' "$FBASE.stdout.observed" # On wine absolute paths can be different
    sed -i 's#[a-zA-Z0-9_./:\\-]*/test/embedded/#/#g' "$FBASE.stdout.observed" # On wine absolute paths can be different

    if [ "$RETURN_CODE_OBSERVED" -ne "$RETURN_CODE_EXPECTED" ]; then
        echo "  > FAIL: Return code mismatch, want $RETURN_CODE_EXPECTED got $RETURN_CODE_OBSERVED ==="
        TEST_FAILED_FILES="$TEST_FAILED_FILES$DIR/$FNAME=RETURNCODE$NEW_LINE"
        TESTS_FAILED_COUNT=$((TESTS_FAILED_COUNT + 1))
    else
      if [ "$RETURN_CODE_EXPECTED" -eq 0 ] && ! diff "$FBASE.int.expected" "$FBASE.int.observed" ; then
        echo "  > FAIL: .INT files mismatch"
        TEST_FAILED_FILES="$TEST_FAILED_FILES$DIR/$FNAME=INT$NEW_LINE"
        TESTS_FAILED_COUNT=$((TESTS_FAILED_COUNT + 1))
      elif ! diff -q $FBASE.stdout.expected $FBASE.stdout.observed ; then
        echo "  > FAIL: STDOUT mismatch"
        set +e
        diff "$FBASE.stdout.expected" "$FBASE.stdout.observed"
        set -e
        TEST_FAILED_FILES="$TEST_FAILED_FILES$DIR/$FNAME=STDOUT$NEW_LINE"
        TESTS_FAILED_COUNT=$((TESTS_FAILED_COUNT + 1))
      elif ls *.tmp 1> /dev/null 2>&1; then
        echo "  > FAIL: Temporary files found, please check the test"
        TEST_FAILED_FILES="$TEST_FAILED_FILES$DIR/$FNAME=TMPFILES$NEW_LINE"
        TESTS_FAILED_COUNT=$((TESTS_FAILED_COUNT + 1))
      else
        TESTS_SUCCESS_COUNT=$((TESTS_SUCCESS_COUNT + 1))
        echo "  > OK"
      fi
    fi

    cd "$OLD_PWD"
  done

  echo "=== Total tests: $((TESTS_SUCCESS_COUNT + TESTS_FAILED_COUNT)) ==="
  echo "=== Successful tests: $TESTS_SUCCESS_COUNT ==="
  echo "=== Failed tests: $TESTS_FAILED_COUNT ==="

  if [ "$EXPECTED_SUCCESSFULL_COMPILED_FILES" -eq 0 ]; then
    echo "ERROR: No files were expected to compile successfully, please check the setup."
    exit 1
  fi

  if [ -n "$TEST_FAILED_FILES" ]; then
    echo "=== Errors found in the following files: ==="
    echo "$TEST_FAILED_FILES"
    exit 1
  else
    echo "=== All tests passed successfully! ==="
  fi    
}
