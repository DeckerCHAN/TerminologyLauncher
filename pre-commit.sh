#!/bin/bash

oldnum=$(cat Core/Build.txt)
newnum=$((oldnum+1))
sed -i "s/$oldnum\$/$newnum/g" Core/Build.txt 
echo Vsersion promoted to $newnum
git add Core/Build.txt
exit 0
