
#include "soundengine.h"
#include <math.h>
#include <stdio.h>
#include <stdlib.h>


using std::vector;


Track::Track( int samplerate,
	      int tempo,
	      int numdivs,
	      int laststep,
	      int grainsizems,
	      int sfsamplerate,
	      int sflength,
	      float *sfdata ){

  this->sr = samplerate;

  this->tempo = tempo;

  this->Divisions = numdivs;

  this->GrainSize = grainsizems;

  this->TempoPercent = 100;

  this->StartStep = 0;

  this->LastStep = laststep;

  this->StepSkip = 0;

  this->SkipCount = 0;

  this->datasr = sfsamplerate;

  this->datalen = sflength;

  this->data = sfdata;

  this->PlayerPhase = new Phasor( this->sr );

  this->PlayerPhase->SetFrequency( ( ( 2.0 / 60.0 ) * ((float)( ( tempo * this->TempoPercent ) / 100)) ) / ((float)this->Divisions) );

  this->Player = new GrainEngine( this->sr,
				  this->datasr,
				  this->datalen,
				  this->GrainSize,
				  this->data );

  this->CurrentStep = 0;

  this->NextStep = 0;

}


int Track::Step( int inc ){


  if( this->SkipCount > this->StepSkip )

    this->SkipCount = 0;

  if( this->SkipCount < 0 )

    this->SkipCount = 0;


  if( this->SkipCount == 0 ){

  this->CurrentStep = this->NextStep;

  this->PlayerPhase->SetPhase( (((float)((this->CurrentStep + this->StartStep) % this->Divisions))) / ((float)this->Divisions) );

  this->NextStep = this->CurrentStep + inc;


  if( this->NextStep > this->LastStep )

	  this->NextStep = 0;


  while( this->NextStep >= this->Divisions )

	  this->NextStep -= this->Divisions;

  }


  this->SkipCount++;

  if( this->SkipCount > this->StepSkip )

    this->SkipCount = 0;


}

int Track::SetStep( int step ){

  if( step <= this->LastStep + this->StartStep && step >= this->StartStep )

	  this->NextStep = step - this->StartStep;


  return this->NextStep;

}

int Track::SetNumDivs( int numdivs ){

  this->Divisions = numdivs;

}

int Track::SetLastStep( int laststep ){

  this->LastStep = laststep;

  while( this->CurrentStep > this->LastStep )

    this->CurrentStep -= this->LastStep + 1;

  while( this->CurrentStep < 0 )

    this->CurrentStep += this->LastStep + 1;

}

int Track::SetStartStep( int startstep ){

	this->StartStep = startstep;

	while( this->StartStep > this->Divisions )

		this->StartStep -= this->Divisions;


	return this->StartStep;

}

int Track::SetTempo( int tempo ){

  this->tempo = tempo;

  this->PlayerPhase->SetFrequency( ( ( 2.0 / 60.0 ) * ((float)( ( tempo * this->TempoPercent ) / 100)) ) / ((float)this->Divisions) );

}

int Track::SetGrainSize( int grainsizems ){


	this->GrainSize = grainsizems;

	this->Player->SetGrainSize( this->GrainSize );


	return this->GrainSize;


}

int Track::SetTempoPercent( int percent ){


	this->TempoPercent = percent;

	this->PlayerPhase->SetFrequency( ( ( 2.0 / 60.0 ) * ((float)( ( tempo * this->TempoPercent ) / 100)) ) / ((float)this->Divisions) );


}


SoundEngine::SoundEngine( int samplerate,
			  int tempo,
			  int maxsteps,
			  int maxrecbuflenseconds,
			  int defaultgrainsizems ){

  this->sr = samplerate;

  this->tempo = tempo;

  this->maxsteps = maxsteps;

  this->trackstepphase = 1.0;

  this->recbuflen = maxrecbuflenseconds;

  this->recbuf = 

    (float*)malloc( this->recbuflen * this->sr * sizeof( float ) );

  this->recbuf = 

    (float*)malloc( this->recbuflen * this->sr * sizeof( float ) );

    this->defaultgrainsize = defaultgrainsizems;


}

float SoundEngine::Process(){


  float sample = 0;

  double trackstepdelta = 2.0 / ( ( ((double)this->sr) * 60.0 ) / ((double)this->tempo) );


  this->trackstepphase += trackstepdelta;


  if( this->trackstepphase >= 1.0 ){

    while( this->trackstepphase >= 1.0 ) this->trackstepphase -= 1.0;

    for( int i = 0; i < this->Tracks.size(); i++ ){

      this->Tracks[i]->Step( 1 );

    }

  }


  for( int i = 0; i < this->Tracks.size(); i++ ){

    float phase = this->Tracks[i]->PlayerPhase->Process();

    this->Tracks[i]->Player->SetPlaybackPos( phase );

    sample += this->Tracks[i]->Player->Process() / ((float)this->Tracks.size());

  }


  return sample;


}

Track* SoundEngine::NewTrack( char *soundfile ){


  Track* newtrack = NULL;

  SNDFILE* sndfile = NULL;

  float* mysound = NULL;

  float* frame = NULL;

  SF_INFO sinfo;

  sinfo.format = 0;


  sndfile = sf_open( soundfile, SFM_READ, &sinfo );


  if( sndfile == NULL ){

    return NULL;

  }

  mysound = (float*)malloc( sizeof( float ) * sinfo.frames );

  frame = (float*)malloc( sizeof( float ) * sinfo.channels );


  for( int i = 0; i < sinfo.frames; i++ ){

    sf_read_float( sndfile, frame, sinfo.channels );

    mysound[i] = frame[0];

    for( int j = 1; j < sinfo.channels; j++ )

      mysound[i] += frame[j] / 2;

  }


  newtrack = new Track( this->sr,
			this->tempo,
			this->maxsteps,
			this->maxsteps - 1,
			this->defaultgrainsize,
			sinfo.samplerate,
			sinfo.frames,
			mysound );

  this->Tracks.push_back( newtrack );

  return newtrack;

}


int SoundEngine::SetTempo( int bpm ){

  
  this->tempo = bpm;


  for( int i = 0; i < this->Tracks.size(); i++ ){

    this->Tracks[i]->SetTempo( bpm );

  }


  return this->tempo;


}

