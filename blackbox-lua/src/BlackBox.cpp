#include "BlackBox.hpp"
#include <stdio.h>
#include <stdlib.h>
#include <math.h> 

double mtof(double midi) {
	return pow(2,(midi+3)/12.0)*27.5;
}

int dtom(int degree, int mode) {
	int steps[] = {2,2,1,2,2,2,1};
	int midi = (degree/7)*12;
	for(int i = 0; i < degree%7; i++) midi += steps[(i+mode)%7];
	return midi;
}

double dtof(int degree, int mode) {
	int steps[] = {2,2,1,2,2,2,1};
	int midi = (degree/7)*12;
	for(int i = 0; i < degree%7; i++) midi += steps[(i+mode)%7];
	return mtof(midi);
}

Metronome::Metronome() {
	delta = 1;
	tempo = 120;
}

double Metronome::Process(int sampleRate) {
	double out = 0;
	if(this->delta >= 1) {
		this->delta = 0;
		out = 1;
	}
	this->delta += 1.0/(sampleRate*(60.0/this->tempo));
	return out;
}

Line::Line() {
	this->delta = 1;
	this->duration = 1;
	this->from = 1;
	this->to = 0;
}

void Line::Reset() {this->delta = 0;}

double Line::Process(int sampleRate) {
	double out = (this->from*(1-this->delta)) + (this->to*this->delta);
	if(this->delta < 1) this->delta += 1/(this->duration*sampleRate);
	return out;
}

Saw::Saw() {
	this->freq = 440;
	this->delta = 0;
}

double Saw::Process(int sampleRate) {
	double out = this->delta;
	this->delta += this->freq/sampleRate;
	while(this->delta > 1) this->delta -= 1;
	return out;
}

SawOsc::SawOsc() {this->amp = 1;}

double SawOsc::Process(int sampleRate) {return ((Saw::Process(sampleRate)*2)-1)*this->amp;}

SinOsc::SinOsc() {this->amp = 1;}

double SinOsc::Process(int sampleRate) {return sin(Saw::Process(sampleRate)*2*M_PI)*this->amp;}

PulseOsc::PulseOsc() {this->amp = 1; this->width = 0.5;}

double PulseOsc::Process(int sampleRate) {return (Saw::Process(sampleRate)<this->width?-1:1)*this->amp;}

Chime::Chime() {
	osc = new PulseOsc();
	env = new Line();
	env->duration = 0.5;
}

double Chime::Process(int sampleRate) {
	double envValue = env->Process(sampleRate);
	osc->width = 0.1 + (envValue * 0.4);
	return osc->Process(sampleRate) * envValue;
}

void Chime::PlayNote(double midi, double duration) {
	env->duration = duration;
	env->Reset();
	osc->freq = mtof(midi);
}

Chimes::Chimes() {
	for(int i = 0; i < 10; i++) chimes[i] = new Chime();
	chimesIndex = 0;
}

double Chimes::Process(int sampleRate) {
	double out = 0;
	for(int i = 0; i < 10; i++) out += chimes[i]->Process(sampleRate) / 10;
	return out;
}

void Chimes::PlayNote(double midi, double duration) {
	chimes[chimesIndex]->PlayNote(midi, duration);
	chimesIndex = (chimesIndex + 1) % 10;
}

Delay::Delay(double delay, int sampleRate) {
	this->bufferSize = (int)(delay*sampleRate);
	this->buffer = (double*)malloc(this->bufferSize*sizeof(double));
	this->index = 0;
}

void Delay::In(double input) { this->input = input; }

double Delay::Process(int sampleRate) {
	double out = 0;
	this->buffer[this->index] = this->input;
	this->index = (this->index + 1) % this->bufferSize;
	out = this->buffer[this->index];
	this->input = 0;
	return out;
}

FeedbackDelay::FeedbackDelay(double delay, int sampleRate) {
	this->bufferSize = (int)(delay*sampleRate);
	this->buffer = (double*)malloc(this->bufferSize*sizeof(double));
	this->index = 0;
	this->feedback = 0.8;
}

void FeedbackDelay::In(double input) { this->input = input; }

double FeedbackDelay::Process(int sampleRate) {
	double out = 0;
	out = this->buffer[this->index] + this->input;
	this->buffer[this->index] = out*this->feedback;
	this->index = (this->index + 1) % this->bufferSize;
	this->input = 0;
	return out;
}

Laser::Laser() {
	this->sawOsc = new SawOsc();
	this->sinOsc = new SinOsc();
	this->env = new Line();
	this->delay = new FeedbackDelay(0.07, 44100);
	this->delay->feedback = 0.7;
	this->env->duration = 0.4;
	this->frequency = 3000;
}

void Laser::Fire() {
	this->env->Reset();	
}

void Laser::Fire(double frequency, double duration) {
	this->env->Reset();
	this->frequency = frequency;
	this->env->duration = duration;
}

double Laser::Process(int sampleRate) {
	double env =  this->env->Process(sampleRate);
	double upperFreq = this->frequency;
	double lowerFreq = 100;
	double out = 0;
	double saw = 0;
	this->sinOsc->freq = (pow(env,7)*upperFreq+lowerFreq)/2;
	this->sawOsc->freq = pow(env,3)*upperFreq+lowerFreq;
	//delay->In((sawOsc->Process(sampleRate)+sinOsc->Process(sampleRate))*env);
	//return delay->Process(sampleRate);
	saw = this->sawOsc->Process(sampleRate);
	saw = (saw + 1) / 2 < env + 0.1 ? saw : 0;
	out = (this->sinOsc->Process(sampleRate)+saw)*env;
	this->delay->In(out);
	return out+(this->delay->Process(sampleRate)/5);
}
