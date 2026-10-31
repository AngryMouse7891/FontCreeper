ARCH=x64
CONFIG=Release

PHONY: build

build:
	dotnet build ./FontCreeper/FontCreeper.csproj -c $(CONFIG) -a $(ARCH)

PHONY: pkg

pkg:
	dotnet deb install
	dotnet restore -r linux-$(ARCH)
	dotnet msbuild ./FontCreeper/FontCreeper.csproj /t:CreateDeb /p:TargetFramework=net10.0 /p:RuntimeIdentifier=linux-$(ARCH) /p:Configuration=Release

PHONY: clean

clean:
	dotnet clean ./FontCreeper.slnx

PHONY: deps

deps:
	dotnet restore ./FontCreeper.slnx

all: build pkg
