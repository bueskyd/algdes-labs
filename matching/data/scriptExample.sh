#!/bin/sh
for FILE in *-in.txt

do
	echo $FILE
	base=${FILE%-in.txt}
    dotnet run ../StableMatching/Program.cs --project ../StableMatching < $FILE > $base.adam.out.txt
    diff $base.adam.out.txt $base-out.txt
done
