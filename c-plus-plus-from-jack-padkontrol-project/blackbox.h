
#ifndef BLACKBOX
#define BLACKBOX


class BlackBox{

  
 public:
  
  virtual float Process() = 0;
  

};


class SinOsc : public BlackBox{

 public:

  SinOsc( int samplerate );

  float Process();

  void SetFrequency( float frequency );


 private:

  int sr;

  double phase;

  double delta;
  

};


class Phasor : public BlackBox{

 public:

  Phasor( int samplerate );

  float Process();

  void SetFrequency( float frequency );

  void SetMS( float ms );

  void SetPhase( float phase );


 private:

  int sr;

  double phase;

  double delta;
  

};


#endif
