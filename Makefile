ARCH=x64
CONFIG=Release

PHONY: build

build:
	dotnet build ./FontCreeper/FontCreeper.csproj -c $(CONFIG) -a $(ARCH)

PHONY: dpkg

dpkg:
	dotnet deb install
	dotnet restore -r linux-$(ARCH)
	dotnet msbuild ./FontCreeper/FontCreeper.csproj /t:CreateDeb /p:TargetFramework=net10.0 /p:RuntimeIdentifier=linux-$(ARCH) /p:Configuration=Release

PHONY: rpmbuild

dpkg:
	dotnet rpm install
	dotnet restore -r linux-$(ARCH)
	dotnet msbuild ./FontCreeper/FontCreeper.csproj /t:CreateRpm /p:TargetFramework=net10.0 /p:RuntimeIdentifier=linux-$(ARCH) /p:Configuration=Release

PHONY: tar

dpkg:
	dotnet tarball install
	dotnet restore -r linux-$(ARCH)
	dotnet msbuild ./FontCreeper/FontCreeper.csproj /t:CreateTarball /p:TargetFramework=net10.0 /p:RuntimeIdentifier=linux-$(ARCH) /p:Configuration=Release

PHONY: zip

dpkg:
	dotnet zip install
	dotnet restore -r linux-$(ARCH)
	dotnet msbuild ./FontCreeper/FontCreeper.csproj /t:CreateZip /p:TargetFramework=net10.0 /p:RuntimeIdentifier=linux-$(ARCH) /p:Configuration=Release

PHONY: clean

clean:
	dotnet clean ./FontCreeper.slnx

PHONY: deps

deps:
	dotnet restore ./FontCreeper.slnx

PHONY: packager-install:
	dotnet tool install --global dotnet-zip
	dotnet tool install --global dotnet-tarball
	dotnet tool install --global dotnet-rpm
	dotnet tool install --global dotnet-deb

all: build dpkg
