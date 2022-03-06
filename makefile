all:
	dotnet publish -o build_linux -r linux-x64 -c RELEASE
	dotnet publish -o build_win -r win-x64 -c RELEASE
debug:
	dotnet publish -o build_linux -r linux-x64 -c DEBUG
	dotnet publish -o build_win -r win-x64 -c DEBUG