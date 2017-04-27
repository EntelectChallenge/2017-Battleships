# Installation
## For Windows:
1. Download [golang compiler](https://storage.googleapis.com/golang/go1.8.1.windows-amd64.msi) OR the [zip](https://golang.org/dl/) if you wish to configure it yourself.
2. Instructions provided [here](https://golang.org/doc/install#windows)
3. Please ensure the Go/bin folder is available in the system PATH.

# Notes on alternative compilation

## GameEngine rebuild for those who don't like guis.
In case you're a mono/C# newcomer here is a small Makefile that may assist in rebuilding the GameEngine from scratch and running test matches.

Create a Makefile at GameEngine/Battleships/Makefile with the following:
~~~~
MONO=mono
XBUILD=xbuild
SOURCEDIR=.
NUGET=nuget
TARGET=$(SOURCEDIR)/Battleships/bin/Debug/Battleships.exe
PACKAGEDIR=./packages/
NUGETCMD=$(NUGET) install -OutputDirectory $(PACKAGEDIR)
DEPS=$(shell find $(SOURCEDIR) -name 'packages.config')

$(PACKAGEDIR):
	mkdir $(PACKAGEDIR)

$(DEPS): $(PACKAGEDIR)
	@echo "finding dependencies and installing"
	@$(foreach dep, $(DEPS), $(NUGETCMD) $(dep);)

clean-objs:
	rm -fr $(SOURCEDIR)/Battleships/bin/
	rm -fr $(SOURCEDIR)/Battleships/obj/

clean-all: clean-objs
	rm -fr $(PACKAGEDIR)
	find $(SOURCEDIR)/ -iname "*.exe" -iname "*.dll" -iname "*.obj" -delete

$(TARGET):
	@echo "building"
	$(XBUILD) $(SOURCEDIR)/Battleships.sln


deps: $(DEPS)

all: $(TARGET)

REFBOTHOME=/home/jdoe/source/entelect-2017/dls/Version\ 1.0.0/refbots
SAMPLEBOTHOME=/home/jdoe/source/entelect-2017/2017-Battleships/Sample\ Bots

runbots:
	$(MONO) $(TARGET) -b $(REFBOTHOME)/C# $(SAMPLEBOTHOME)/Golang  --clog --pretty --debug
~~~~

which is used as follows:

~~~~
make deps  # call Nuget for all package configs in the build tree.
make clean-objs all runbots  # run, modify, repeat
~~~~

## Cross compilation of Windows Golang target from Linux

~~~
GOOS=windows GOARCH=amd64 go build -o /some_refbot_dir/go/gobot.exe main.go
~~~


