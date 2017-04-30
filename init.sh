#!/bin/bash
cp pre-commit.sh ./.git/hooks/pre-commit
chmod +x .git/hooks/pre-commit
echo done!