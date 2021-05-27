#!/bin/bash

echo "List modified files"
git diff --name-only HEAD^ HEAD
echo "Check paths of modified files"
git diff --name-only HEAD^ HEAD >files.txt

sssf=false
sssfp=false

while IFS= read -r file; do
  if [[ $file == src/SqlStreamStore.FSharp/* ]]; then
    sssf=true
  elif [[ $file == src/SqlStreamStore.FSharp.Postgres/* ]]; then
    sssfp=true
  fi
done <files.txt

rm -f files.txt

if [ $sssf = true ] && [ $sssfp = true ]; then
  echo "::set-output name=run_jobs:all"
elif [ $sssf = true ] && [ $sssfp = false ]; then
  echo "::set-output name=run_jobs:sssf"
elif [ $sssf = false ] && [ $sssfp = true ]; then
  echo "::set-output name=run_jobs:sssfp"
else
  echo "::set-output name=run_jobs:none"
fi
