#!/bin/bash

function exit_with_usage {
	echo "USAGE: $0 <major>.<minor>.<patch>"
	exit $1
}


if [ $# -lt 1 ]; then
	echo "Invalid number of arguments!"
	exit_with_usage 1
fi

IFS='.' read -a revisions <<< $1

major="${revisions[0]}"
minor="${revisions[1]}"
patch="${revisions[2]}"

number_regex='^[0-9]+$'
if ! [[ $major =~ $number_regex ]]; then
	echo "Invalid major version number"
	exit_with_usage 2
fi
if ! [[ $minor =~ $number_regex ]]; then
	echo "Invalid minor version number"
	exit_with_usage 3
fi
if ! [[ $patch =~ $number_regex ]]; then
	echo "Invalid patch level number"
	exit_with_usage 4
fi

sed -i "s/Major = \"[0-9]\+\"/Major = \"$major\"/" GlobalAssemblyInfo.cs
sed -i "s/Minor = \"[0-9]\+\"/Minor = \"$minor\"/" GlobalAssemblyInfo.cs
sed -i "s/Patch = \"[0-9]\+\"/Patch = \"$patch\"/" GlobalAssemblyInfo.cs
unix2dos GlobalAssemblyInfo.cs
sed -i "s#<ApplicationVersion>.*</ApplicationVersion>#<ApplicationVersion>$major.$minor.$patch.0</ApplicationVersion>#" */*.csproj
unix2dos */*.csproj
