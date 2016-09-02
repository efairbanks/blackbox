#ifndef _BLACKBOX_
#define _BLACKBOX_

double mtof(double midi);
int dtom(int degree, int mode);
double dtof(int degree, int mode);

class BlackBox {
	public:
		virtual double Process(int sampleRate) = 0;
};

class Metronome {
	private:
		double delta;
	public:
		double tempo;
		Metronome();
		double Process(int sampleRate);
};

class Line {
	private:
		double delta;
	public:
		double duration;
		double from;
		double to;
		Line();
		void Reset(); 
		double Process(int sampleRate);
};

class Saw : public BlackBox {
	private:
		double delta;
	public:
		double freq;
		Saw(); 
		double Process(int sampleRate); 
};

class SawOsc : public Saw {
	public:
		double amp;
		SawOsc(); 
		double Process(int sampleRate);
};

class SinOsc : public Saw {
	public:
		double amp;
		SinOsc();
		double Process(int sampleRate);
};

class PulseOsc : public Saw {
	public:
		double amp;
		double width;
		PulseOsc();
		double Process(int sampleRate); 
};

class Chime : public BlackBox {
	private:
		PulseOsc* osc;
		Line* env;
	public:
		Chime(); 
		double Process(int sampleRate);
		void PlayNote(double midi, double duration);
};

class Chimes : public BlackBox {
	private:
		Chime* chimes[10];
		int chimesIndex;
	public:
		Chimes();
		double Process(int sampleRate);
		void PlayNote(double midi, double duration);
};

class Delay : public BlackBox {
	private:
		double* buffer;
		int bufferSize;
		int index;
		double input;
	public:
		Delay(double delay, int sampleRate);
		void In(double input); 
		double Process(int sampleRate); 
};

class FeedbackDelay : public BlackBox {
	private:
		double* buffer;
		int bufferSize;
		int index;
		double input;
	public:
		double feedback;
		FeedbackDelay(double delay, int sampleRate);
		void In(double input);
		double Process(int sampleRate);
};

class Laser : public BlackBox {
	private:
		SawOsc* sawOsc;
		SinOsc* sinOsc;
		Line* env;
		FeedbackDelay* delay;
	public:
		double frequency;
		Laser();
		void Fire();
		void Fire(double frequency, double duration);
		double Process(int sampleRate);
};
#endif
