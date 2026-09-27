PHONY: build

build:
	dotnet build ./FontCreeper/FontCreeper.csproj --configuration Release

PHONY: pkg

pkg:
	dotnet msbuild ./FontCreeper/FontCreeper.csproj /t:CreateDeb /p:TargetFramework=net10.0 /p:RuntimeIdentifier=linux-x64 /p:Configuration=Release

PHONY: clean

clean:
	dotnet clean ./FontCreeper.slnx

PHONY: deps

deps:
	dotnet restore ./FontCreeper.slnx

all: build pkg
