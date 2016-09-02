#include <stdio.h>
#include <math.h>
#include <SDL.h>
#include <SDL_mixer.h>

#include "BlackBox.hpp"

#include "LuaManager.hpp"
#include "LuaLibrary.hpp"

Metronome* metro;
Chimes* lead;
Chimes* bass;
FeedbackDelay* delay;
Laser *laser;

int tick = 0;
int seq[] = {28,24,20,16,12,8,4,6};
int arp[] = {4,7,2,7,6};
int mode = 0;

void stream(int chan, void *stream, int len, void *udata);

int main(int argc, char** argv)
{
	if(SDL_Init(SDL_INIT_AUDIO)==-1) printf("ERROR SDL INIT\n");
	if(Mix_Init(0)==-1) printf("ERROR MIX INIT");
	if(Mix_OpenAudio(44100, AUDIO_S16SYS, 2, 1024)==-1) printf("ERROR MIX OPENAUDIO");
	metro = new Metronome();
	lead = new Chimes();
	bass = new Chimes();
	delay = new FeedbackDelay(0.1, 44100);	
	laser = new Laser();
	if(!Mix_RegisterEffect(MIX_CHANNEL_POST, stream, NULL, NULL)) {
		printf("Mix_RegisterEffect: %s\n", Mix_GetError());
	}

	// -----------------------------------------------------
	// initialize a Lua interpreter for loading config files
	// -----------------------------------------------------
	LuaManager luaManager;
	luaManager.logVersion();

	// register our interface functions
	luaManager.registerLibrary("Audio", LuaLib::LibAudio);

	// load our Lua script
	if (!luaManager.runFile("test.lua"))
	{
		printf("Couldn't load and run test.lua\n");
	}

	luaManager.queueFunction("do_something_cool");
	luaManager.queueFunction("do_something_else");

	// this simulates the main game loop
	while(true)
	{
		// process and queued lua chunks (deltaTime is 10ms)
		luaManager.processAllScripts(0.01f);
		SDL_Delay(10);
	}

	Mix_Quit();
	SDL_Quit();
	return 0;
}

void stream(int chan, void *stream, int len, void *udata)
{
	int16_t* buffer = (int16_t*)stream;
	for(int i = 0; i < len/4; i++) {
		double out = 0;
		metro->tempo = 130*2;
		if(metro->Process(44100) > 0) {
			int degree = seq[(tick/8)%8]+arp[tick%5];
			double note = dtom(degree,mode)+36;
			mode = (((tick/64)*3)+5)%7;
			while(note > 55) note -= 12;
			lead->PlayNote(note, 0.7);
			if(tick%4==0) {
				double midi = dtom(seq[(tick/8)%8],mode)+36;
				while(midi > 20) midi -= 12;
				bass->PlayNote(midi, 2);
			}
			//if(tick%8==0) laser->Fire(rand()%6000+400, ((double)(rand()%1000))/1000.0 + 0.005 );
			tick++;
		}
		out += lead->Process(44100)/2;
		out += bass->Process(44100);
		delay->In(out);
		out += delay->Process(44100);

		// comment the line below out to hear music
		out = (laser->Process(44100)/2);

		buffer[i*2] = (int16_t)(out*16000);
		buffer[i*2+1] = (int16_t)(out*16000);
	}
}
