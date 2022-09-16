#!/bin/sh
rm -f adam-closest-pair-out.txt
for FILE in *-tsp.txt

do
	base=${FILE%-tsp.txt}
    res=$(dotnet run ../ClosestPoints/Program.cs --project ../ClosestPoints < $FILE)
    echo ../data/$base.tsp: $res >> adam-closest-pair-out.txt
done

diff adam-closest-pair-out.txt closest-pair-out.txt