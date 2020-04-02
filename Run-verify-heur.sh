for i in {1..199..2}
do
	number=""
	if [ $i -lt 10 ];
		then
			number="00$i"
	elif [ $i -lt 100 ];
		then number="0$i"
	else number="$i"
	fi
	input="./Testcases/heur_$number.gr"
	output="./Results/heur_$number.tree"
	echo "Verifying heur_$number..."
	./verify $input $output
	echo ""
done 
read 