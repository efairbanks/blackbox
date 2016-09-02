
#include "grainengine.h"
#include <math.h>
#include <stdio.h>
#include <stdlib.h>

GrainEngine::GrainEngine( int samplerate,
			  int datasamplerate,
			  int datalength,
			  int grainsizems,
			  float* data ){

  this->sr = samplerate;

  this->dsr = datasamplerate;

  this->lso = 0;

  this->cso = 0;

  this->nso = 0.0;

  this->soupdate = false;

  this->gi = 0.0;

  this->ps = 1.0;

  this->ngs = grainsizems;

  this->gs = ( this->sr * this->ngs ) / 1000;

  this->sl = datalength;

  this->sd = data;

}

float GrainEngine::Process(){


  if( this->gi > 1.0 || this->gi < 0.0 ){

    
    int lastcoffset = 

      (int)( ( this->gi * this->gs ) + this->cso );

    this->lso = lastcoffset;


    this->gs = ( this->sr * this->ngs ) / 1000;
    

    while( this->gi > 1.0 ) this->gi -= 1.0;

    while( this->gi < 0.0 ) this->gi += 1.0;


    if( this->soupdate == true ){
    
      if( this->nso > 1.0 ) this->nso -= 1.0;
      
      if( this->nso < 0.0 ) this->nso += 1.0;
      
      
      this->cso = (int)( this->nso * ((double)(this->sl)) );
      
      this->soupdate = false;

    }


  }


  //sample calc

  int loffset =
    
    ( ( this->gi * this->gs ) + this->lso );

  int coffset = 

    ( ( this->gi * this->gs ) + this->cso );


  while( loffset >= this->sl ) loffset -= this->sl;

  while( loffset < 0 ) loffset += this->sl;


  while( coffset >= this->sl ) coffset -= this->sl;

  while( coffset < 0 ) coffset += this->sl;


  float li = cos( (M_PI/2.0) * this->gi );

  float ci = sin( (M_PI/2.0) * this->gi );


  li = li * li;

  ci = ci * ci;

  /*              Linear Interpolation
  li = this->gi;

  ci = 1.0 - this->gi;
  */

  float lsamp =

    this->sd[ loffset ] * li;

  float csamp =

    this->sd[ coffset ] * ci;


  //index calc

  this->gi += ( 1.0 / ((double)this->gs) ) * ( (((double)(this->dsr)) / ((double)(this->sr))) * this->ps );


  return lsamp + csamp;


}

void GrainEngine::SetPlaybackPos( double pos ){

  this->nso = pos;

  this->soupdate = true;

}

void GrainEngine::SetPlaybackSpeed( double speed ){

  this->ps = speed;

}

void GrainEngine::SetGrainSize( int ms ){

  this->ngs = ms;

}
