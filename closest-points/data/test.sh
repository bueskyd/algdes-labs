#!/bin/sh
rm -f adam-closest-pair-out.txt
for FILE in *-tsp.txt

do
    echo "bruh"
	base=${FILE%-tsp.txt}
    res=$(dotnet run ../ClosestPoints/Program.cs --project ../ClosestPoint < $FILE)
    echo ../data/$base.tsp: $res >> tjoms-closest-pair-out.txt
done

diff tjoms-closest-pair-out.txt closest-pair-out.txt