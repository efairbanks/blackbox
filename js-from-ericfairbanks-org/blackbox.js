function SinOsc (frequency,amplitude)
{
	
	this.freq = frequency;
	this.amp =  amplitude;
	this.delta = 0;
	
	this.process = function(samplerate)
	{
		
		var out = Math.sin(this.delta*2*Math.PI)
		
		this.delta = this.delta + (this.freq/samplerate);
		
		return out;
		
	};
	
	this.set = function(frequency,amplitude)
	{
		
		this.freq	= frequency;
		this.amp	= amplitude;
		
	};
	
}

function Line (from,to,duration)
{
	
	this.delta = -1;
	
	this.f = from;
	
	this.t = to;
	
	this.dur = duration;
	
	this.process = function(samplerate)
	{
		
		var out = 0;
		
		if(this.delta > -1)
		{
		
			out = ((this.t*this.delta)/this.dur) + ((this.f*(this.dur-this.delta))/this.dur);
		
			this.delta  = this.delta + (1/samplerate);
		
			if(this.delta>this.dur + (1/samplerate)) this.delta = -1;
		
		} else {
			
			out = 0;
			
		}
		
		return out;
		
	}
	
	this.reset = function(duration)
	{
		
		this.dur = duration;
		
		this.delta = 0;
		
	}
	
}

function Pulse (frequency,amplitude,pulsewidth)
{
	
	this.freq = frequency;
	
	this.amp = amplitude;
	
	this.pw = pulsewidth;
	
	this.delta = 0;
	
	this.process = function(samplerate)
	{
		
		var out = 0;
		
		if(this.delta+(this.freq/samplerate) < this.pw){
			
			out = this.amp;
			
		} else {
			
			out = -this.amp;
			
		}
		
		this.delta = this.delta + (this.freq/samplerate);
		
		while(this.delta>1) this.delta = this.delta - 1;
		
		return out;
		
	};
	
	this.set = function(frequency,amplitude,pulsewidth)
	{
		
		this.freq	= frequency;
		this.amp	= amplitude;
		this.pw		= pulsewidth;
		
	};
	
}

function Saw(frequency,amplitude)
{
	
	this.freq = frequency;
	
	this.amp = amplitude;
	
	this.delta = 0;
	
	this.process = function(samplerate)
	{
		
		var out = ((this.delta*2)-1)*this.amp;
		
		this.delta = this.delta + (samplerate/this.freq);
		
		while(this.delta>1) this.delta = this.delta - 1;
		
		return out;
		
	};
	
	this.set = function(frequency,amplitude)
	{
		
		this.freq	= frequency;
		this.amp	= amplitude;
		
	};
	
}

function Tri(frequency,amplitude)
{
	
	this.freq = frequency;
	
	this.amp = amplitude;
	
	this.delta = 0;
	
	this.process = function(samplerate)
	{
		
		var out = 0;
		
		if( this.delta < 0.25 ){
			
			out = delta*4;
			
		} else if( this.delta < 0.5 ){
			
			out = 1-((delta-0.25)*4);
			
		} else if( this.delta < 0.75 ){
			
			-((delta-0.5)*4);
			
		} else if( this.delta < 1 ){
			
			-(1-((delta-0.75)*4));
			
		} else {
			
			return 0;
			
		}
		
		out = out*this.amp;
		
		this.delta = this.delta + (samplerate/this.freq);
		
		while(this.delta>1) this.delta = this.delta - 1;
		
		return out;
		
	};
	
	this.set = function(frequency,amplitude)
	{
		
		this.freq	= frequency;
		this.amp	= amplitude;
		
	};
	
}

function Delay (delayinseconds,feedbackcoefficient)
{
	
	this.buffer = [];
	
	this.delaysec = delayinseconds;
	
	this.head = 0;
	
	this.fcoef = feedbackcoefficient;
	
	this.process = function (signalinput,samplerate)
	{
		
		var out = 0;
		
		if(this.buffer[this.head]){
		
			out = (this.buffer[this.head])+signalinput;
			
		} else {
			
			out = signalinput;
			
		}
		
		this.buffer[this.head] = out*this.fcoef;
		
		this.head = this.head+1;
		
		while( this.head > samplerate*this.delaysec ) this.head -= samplerate*this.delaysec;
		
		return out;
		
	}
	
}

function Chime ()
{
	
	this.delta	= -1;
	this.note	= 60;
	this.dur	= 0.5;
	this.amp	= 0.5;
	
	this.vib	= new SinOsc(10,1);
	this.osc	= new Pulse(440,0.5,0.5);
	this.env	= new Line(1,0,0.5);
	
	this.delay	= new Delay(0.2,0.8);
	
	this.playnote = function (midinote,amplitude,duration)
	{
		
		this.note 	= midinote;
		this.amp	= amplitude;
		this.dur	= duration;
		
		this.osc.set(Math.pow(2,(this.note-57)/12)*440,this.amp,0.5);
		
		this.env.reset(this.dur);
		
	};
	
	this.process = function (samplerate)
	{
		
		var out = 0;
		
		if(this.osc && this.env){
		
			var envn = this.env.process(samplerate);
			
			this.osc.pw = (1-envn)/2+0.5;
			
			this.osc.freq = (Math.pow(2,(this.note-57)/12)*440);
			
			out = this.osc.process(samplerate)*envn;
			
			out = out + (this.delay.process(out,samplerate)/8);
			
		}
		
		return [out,out];
		
	};
	
}

function Chimes ()
{
	
	this.chimes = [];
	
	this.voice = 0
	
	this.chimes[0] = new Chime();
	this.chimes[1] = new Chime();
	this.chimes[2] = new Chime();
	
	this.playnote = function (midinote,amplitude,duration)
	{
		
		this.chimes[this.voice].playnote(midinote,amplitude,duration);
		
		this.voice = (this.voice+1)%3;
		
	};
	
	this.process = function (samplerate)
	{
		
		var out;
		
		var outl = 0;
		var outr = 0;
		
		for(var i = 0; i < 3; i++){
			
			out = this.chimes[i].process(samplerate);
			
			outl = outl + out[0];
			
			outr = outr + out[1];
			
		}
		
		outl = outl/3;
		outr = outr/3;
		
		return [outl,outr];
		
	};
	
}

function NoisePerc ()
{

	this.dur	= 0.5;
	this.amp	= 0.5;

	this.env	= new Line(1,0,0.5);
	
	this.delay	= new Delay(0.005,0.5);
	
	this.delay2	= new Delay(0.06,0.7);
	
	this.playnote = function (amplitude,duration)
	{

		this.amp	= amplitude;

		this.dur	= duration;
		
		this.env.reset(this.dur);
		
	};
	
	this.process = function (samplerate)
	{
		
		var out = 0;
		
		if(this.env){
		
			var envn = this.env.process(samplerate);
			
			out = Math.random()*envn;
			
			out = this.delay.process(-out,samplerate);
			
			out = (out/2) + (this.delay2.process(out,samplerate)/4);
			
			out = out*this.amp;
			
		}
		
		return [out,out];
		
	};
	
}

function Kick ()
{

	this.dur	= 0.5;
	this.amp	= 0.5;

	this.env	= new Line(1,0,0.5);
	
	this.osc	= new SinOsc(250,0.2);
	
	this.playnote = function (amplitude,duration)
	{

		this.amp	= amplitude;

		this.dur	= duration;
		
		this.env.reset(this.dur);
		
		this.osc.delta = 0;
		
	};
	
	this.process = function (samplerate)
	{
		
		var out = 0;
		
		if(this.env){
		
			var envn = this.env.process(samplerate);
			
			this.osc.freq = envn*envn*envn*250;
			
			out = this.osc.process(samplerate)*envn;
			
			out = out*this.amp;
			
		}
		
		return [out,out];
		
	};
	
}

var audiogenerator = [];

audiogenerator['chimes'] = new Chimes();

audiogenerator['perc'] = new NoisePerc();

audiogenerator['kick'] = new Kick();
