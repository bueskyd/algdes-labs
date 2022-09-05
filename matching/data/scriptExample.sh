#!/bin/bash

for FILE in *-in.txt; do echo $FILE; base=${FILE%-in.txt}; dotnet run $FILE > $base.emja.out.txt; diff $base.emja.out.txt $base-out.txt; done