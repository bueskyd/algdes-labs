#!/bin/sh

# clear file
true > answers.txt

for FILE in ../../data/*.txt
do
	echo $FILE >> answers.txt
  dotnet run GraphSolver.cs < $FILE >> answers.txt
done