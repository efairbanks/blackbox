
#include "blackbox.h"
#include <math.h>

SinOsc::SinOsc( int samplerate ){

  sr = samplerate;

  this->phase = 0;

  this->delta = 0;

}

float SinOsc::Process(){
  
  float sample = 0;

  sample = sin( this->phase * 2 * M_PI );

  this->phase += this->delta;

  while( this->phase >= 1.0 )

    this->phase -= 1.0;

  while( this->phase < 0.0 )

    this->phase += 1.0;

  return sample;
  
}

void SinOsc::SetFrequency( float frequency ){

  this->delta = 1.0 / (((double)this->sr) / ((double)frequency));

}


Phasor::Phasor( int samplerate ){

  sr = samplerate;

  this->phase = 0;

  this->delta = 0;

}

float Phasor::Process(){
  
  float sample = 0;

  sample = this->phase;

  this->phase += this->delta;

  while( this->phase >= 1.0 )

    this->phase -= 1.0;

  while( this->phase < 0.0 )

    this->phase += 1.0;

  return sample;
  
}

void Phasor::SetFrequency( float frequency ){

  this->delta = 1.0 / (((double)this->sr) / ((double)frequency));

}

void Phasor::SetMS( float ms ){

  this->delta = 1.0 / ( ( ((double)this->sr) * ((double)ms) ) / 1000.0 );

}

void Phasor::SetPhase( float phase ){

  this->phase = ((double)phase);

}
