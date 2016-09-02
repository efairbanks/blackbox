
#ifndef GRAINENGINE
#define GRAINENGINE

#include "blackbox.h"


class GrainEngine : public BlackBox{


 public:

  GrainEngine( int samplerate,
	       int datasamplerate,
	       int datalength,
	       int grainsizems,
	       float* data );

  float Process();

  void SetPlaybackPos( double pos );

  void SetPlaybackSpeed( double speed );

  void SetGrainSize( int ms );


 private:

  int sr; //sample rate

  int dsr; //data sample rate


  int lso; //last sample offset

  int cso; //curr sample offset


  double nso; //new sample offset

  bool soupdate; //update sample offset?


  double gi; //grain index  

  double ps; //playback speed (1.0 is default)


  int gs; //grain size

  int ngs; //new grain size;


  int sl; //sound length

  float* sd; //sound data


};

#endif GRAINENGINE
