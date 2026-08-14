set -e
set -u

. utils.bash

mkdir -p gamescripts
cd gamescripts

# rm -rf *

if [ ! -d 'Fallout2_Restoration_Project' ]; then
  echo "== Fallout2_Restoration_Project scripts =="
  ### Checkout fallout2-rpu
  if true ; then
    echo "== Downloading Fallout2_Restoration_Project scripts =="
    git clone --depth=1 -n --filter=tree:0 https://github.com/BGforgeNet/Fallout2_Restoration_Project.git
    cd Fallout2_Restoration_Project
    git sparse-checkout init --cone
    git sparse-checkout set scripts_src
    git checkout

    cd ..
    echo "Done"
  else
    mkdir -p Fallout2_Restoration_Project/scripts_src/democity
    echo '
    #include <sfall.h>
    procedure start begin
    
    end ' > Fallout2_Restoration_Project/scripts_src/democity/script1.ssl
  fi

  echo "== Fallout2_Restoration_Project scripts removing \r =="
  find . -type f -iname '*.ssl' -exec sed -i 's/\r$//' {} \;
  find . -type f -iname '*.h' -exec sed -i 's/\r$//' {} \;

  echo "Done"
else 
  echo "== Fallout2_Restoration_Project scripts already downloaded =="
fi


MODDERPACK_DIR=$(pwd)/modderspack

## Download modderspack
if [ ! -d 'modderspack' ]; then
  echo "== modderpack =="

  if ! which 7z >/dev/null; then
    sudo apt update
    sudo apt install -y p7zip-full
  fi


  curl -L https://master.dl.sourceforge.net/project/sfall/Modders%20pack/modderspack_4.4.6.7z?viasf=1 > modderspack_4.4.6.7z
  7z x modderspack_4.4.6.7z -omodderspack
  
  echo "Done, removing \r"

  find modderspack -type f -iname '*.h' -exec sed -i 's/\r$//' {} \;
  echo "Done"
else 
  echo "== modderpack already downloaded =="
fi

cd Fallout2_Restoration_Project/scripts_src
  if [ ! -L 'sfall' ] && [ ! -d 'sfall' ]; then

    # echo "== Creating relative symlink to modderspack headers =="
    # ln -s ../../modderspack/scripting_docs/headers sfall

    echo "== Creating absolute symlink to modderspack headers =="
    ln -s "$MODDERPACK_DIR/scripting_docs/headers" sfall
  else
    echo "== Symlink to modderspack headers already exists =="
  fi
cd ../..



cd Fallout2_Restoration_Project/scripts_src


if false; then # Some debugging
  clear_test_snapshots
fi


make_test_snapshots "$MODDERPACK_DIR/ScriptEditor/resources/compile.exe" "$MODDERPACK_DIR/scripting_docs/headers"
