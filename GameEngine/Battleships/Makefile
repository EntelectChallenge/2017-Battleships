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
	  rm -fr $(SOURCEDIR)/Battleships/Battleships/bin/
	  rm -fr $(SOURCEDIR)/Battleships/Battleships/obj/
	  rm -fr $(SOURCEDIR)/Battleships/Domain/bin/
	  rm -fr $(SOURCEDIR)/Battleships/Domain/obj/
	  rm -fr $(SOURCEDIR)/Battleships/GameEngine/bin/
	  rm -fr $(SOURCEDIR)/Battleships/GameEngine/obj/
	  rm -fr $(SOURCEDIR)/Battleships/Tests.Domain/bin/
	  rm -fr $(SOURCEDIR)/Battleships/Tests.Domain/obj/
	  rm -fr $(SOURCEDIR)/Battleships/BotRunner/bin/
	  rm -fr $(SOURCEDIR)/Battleships/BotRunner/obj/


clean-all: clean-objs
	  rm -fr $(PACKAGEDIR)
	  find $(SOURCEDIR)/ -iname "*.exe" -iname "*.dll" -iname "*.obj" -delete

clean: clean-objs

$(TARGET):
	  @echo "building"
	  $(XBUILD) $(SOURCEDIR)/Battleships.sln


deps: $(DEPS)

all: $(TARGET)

REFBOTHOME=$(HOME)/source/entelect-2017/dls/Version\ 1.0.0/refbots
SAMPLEBOTHOME=$(HOME)/source/entelect-2017/2017-Battleships/Sample\ Bots
runbots:
	  $(MONO) $(TARGET) -b $(REFBOTHOME)/C# $(SAMPLEBOTHOME)/Golang  --clog --pretty --debug 


runbots-recompile:
	  rm -fr $(SAMPLEBOTHOME)/Golang/main
	  rm -fr $(SAMPLEBOTHOME)/C#/SampleBot/bin
	  $(MONO) $(TARGET) -b $(SAMPLEBOTHOME)/C#/SampleBot $(SAMPLEBOTHOME)/Golang  --clog --pretty --debug --forceRebuild

