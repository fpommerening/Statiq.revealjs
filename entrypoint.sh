#!/bin/bash
set -e
if [[ $1 = --* ]]
then
  /app/Statiq.RevealJS $1 $2 $3
else
  /app/Statiq.RevealJS --input $1 --setting baseurl=$2 --output $3
fi
