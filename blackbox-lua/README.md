This is some rudimentary sound synthesis with a musical example. It's 
pretty basic so far. Polyphony is coming soon, along with some simple 
feedback delay/reverb and some SFX examples like LASERS.

BUILD INSTRUCTIONS
------------------

Requires CMake 2.8 or greater.  Install in Ubuntu 14.04 with:

> sudo apt-get install cmake

You'll also need to download and build the SDL2 libraries from here:

http://www.libsdl.org/release/SDL2-2.0.3.tar.gz
http://www.libsdl.org/projects/SDL_mixer/release/SDL2_mixer-2.0.0.tar.gz
http://www.libsdl.org/projects/SDL_image/release/SDL2_image-2.0.0.tar.gz
http://www.libsdl.org/projects/SDL_net/release/SDL2_net-2.0.0.tar.gz
http://www.libsdl.org/projects/SDL_ttf/release/SDL2_ttf-2.0.12.tar.gz

Follow the SDL2 build and install instructions.  On Ubuntu 14.04 you'll need to install
some other packages first of all:

> sudo apt-get install build-essential xorg-dev libudev-dev libts-dev libgl1-mesa-dev libglu1-mesa-dev libasound2-dev libpulse-dev libopenal-dev libogg-dev libvorbis-dev libaudiofile-dev libpng12-dev libfreetype6-dev libusb-dev libdbus-1-dev zlib1g-dev libdirectfb-dev

Building the SDL libraries is done using autotools, and generally goes like this from each unpacked directory:

> ./configure
> make
> sudo make install

After building and installing SDL2 and associated libraries you are ready to build this project:

From the top project directory:

> mkdir build
> cd build
> cmake -G "Unix Makefiles" ../src
> make

That's it!  If something went wrong, the CMake will fail with a hopefully descriptive error.

RUNNING
-------

From top level project directory:

> ./build/bbal

Watch for errors!


