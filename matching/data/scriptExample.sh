#!/bin/sh
for FILE in *-in.txt

do
	echo $FILE
    cd ../StableMatching
	base="../data/"${FILE%-in.txt}
    dotnet run < $base-in.txt > $base.yourname.out.txt
    diff $base.yourname.out.txt $base-out.txt
done
