all:
	dotnet publish -o build_linux -r linux-x64
	dotnet publish -o build_win -r win-x64