PHONY: build

build:
	dotnet build ./FontCreeper/FontCreeper.csproj --configuration Release

PHONY: pkg

pkg:
	dotnet build ./FontCreeper.Setup/FontCreeper.Setup.wixproj --configuration Release

PHONY: clean

clean:
	dotnet clean ./FontCreeper.slnx

PHONY: deps

deps:
	dotnet restore ./FontCreeper.slnx

all: 
	dotnet build ./FontCreeper.slnx --configuration Release