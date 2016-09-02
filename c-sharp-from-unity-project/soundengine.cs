using UnityEngine;
using System;  // Needed for Math
using System.Collections.Generic;

using System.IO;

public class Sequence
{
    List<double> seq;
    public Sequence()
    {
        seq = new List<double> ();
    }
    public Sequence (params double[] input)
    {
        seq = new List<double> ();
        for (int i = 0; i < input.Length; i++)
            seq.Add (input [i]);
    }
    public static Sequence GenDup(double n, int size)
    {
        Sequence ret = new Sequence ();
        for (int i = 0; i < size; i++)
            ret.seq.Add (n);
        return ret;
    }
    public Sequence DegreesToChromatic()
    {
        double[] aeolianTemp = {0,2,3,5,7,8,10};
        Sequence ret = new Sequence ();
        Sequence aeolian = new Sequence (aeolianTemp);
        for (int i = 0; i < this.Size(); i++) {
            double degree = this[i];
            double note = aeolian[this[i]];
            while(degree >= 7) {
                note = note + 12;
                degree = degree - 7;
            }
            ret.seq.Add (note);
        }
        return ret;
    }
    public Sequence Append(double input)
    {
        Sequence ret = new Sequence ();
        for (int i = 0; i < this.Size (); i++)
            ret.seq.Add (this [i]);
        ret.seq.Add (input);
        return ret;
    }
    public Sequence Concat(Sequence input)
    {
        Sequence ret = new Sequence();
        for(int i = 0; i < this.seq.Count; i++) ret.seq.Add(this.seq[i]);
        for(int i = 0; i < input.seq.Count; i++) ret.seq.Add(input.seq[i]);
        return ret;
    }
    public int Size()
    {
        return seq.Count;
    }
    public static Sequence operator +(Sequence a, Sequence b)
    {
        Sequence ret = new Sequence();
        int maxSize = a.seq.Count;
        if(b.seq.Count > maxSize) maxSize = b.seq.Count;
        for(int i = 0; i < maxSize; i++) ret.seq.Add(a[i] + b[i]);
        return ret;
    }
    public static Sequence operator -(Sequence a, Sequence b)
    {
        Sequence ret = new Sequence();
        int maxSize = a.seq.Count;
        if(b.seq.Count > maxSize) maxSize = b.seq.Count;
        for(int i = 0; i < maxSize; i++) ret.seq.Add(a[i] - b[i]);
        return ret;
    }
    public static Sequence operator *(Sequence a, Sequence b)
    {
        Sequence ret = new Sequence();
        int maxSize = a.seq.Count;
        if(b.seq.Count > maxSize) maxSize = b.seq.Count;
        for(int i = 0; i < maxSize; i++) ret.seq.Add(a[i] * b[i]);
        return ret;
    }
    public static Sequence operator /(Sequence a, Sequence b)
    {
        Sequence ret = new Sequence();
        int maxSize = a.seq.Count;
        if(b.seq.Count > maxSize) maxSize = b.seq.Count;
        for(int i = 0; i < maxSize; i++) ret.seq.Add(a[i] / b[i]);
        return ret;
    }
    public static Sequence operator %(Sequence a, Sequence b)
    {
        Sequence ret = new Sequence();
        int maxSize = a.seq.Count;
        if(b.seq.Count > maxSize) maxSize = b.seq.Count;
        for(int i = 0; i < maxSize; i++) ret.seq.Add(a[i] % b[i]);
        return ret;
    }
    public static Sequence operator >>(Sequence a, int b)
    {
        Sequence ret = new Sequence();
        while (b > a.Size ())
            b += a.Size ();
        while (b < 0)
            b += a.Size ();    
        for(int i = 0; i < a.Size (); i++) ret.seq.Add(a[(i + b) % a.Size ()]);
        return ret;
    }
    public static Sequence operator <<(Sequence a, int b)
    {
        Sequence ret = new Sequence();
        while (b > a.Size ())
            b += a.Size ();
        while (b < 0)
            b += a.Size ();
        b = a.Size () - b;
        for(int i = 0; i < a.Size (); i++) ret.seq.Add(a[(i + b) % a.Size ()]);
        return ret;
    }
    public int GetInt(int index) {
        return (int)(Math.Floor (this [index]));
    }
    public int GetInt(double index) {
        return (int)(Math.Floor (this [(int)(Math.Floor(index))]));
    }
    public double this[int index]
    {   
        get 
        { 
            return seq[index % seq.Count];
        }
        set{
            seq[index % seq.Count] = value;
        }
    }
    public double this[double index] // perhaps interpolate in the future
    {   
        get 
        { 
            return seq[((int)Math.Floor(index)) % seq.Count];
        }
        set{
            seq[((int)Math.Floor(index)) % seq.Count] = value;
        }
    }
    public override string ToString()
    {
        String ret = "";
        for(int i = 0; i < seq.Count; i++) ret = ret + seq[i] + " ";
        return ret;
    }
    public Sequence SelectRange(int from, int to)
    {
        Sequence ret = new Sequence ();
        for (int i = from; i <= to; i++)
            ret=ret.Append (this [i]);
        return ret;
    }
    public double Sum()
    {
        double ret = 0;
        for (int i = 0; i < this.Size (); i++)
            ret = ret + this [i];
        return ret;
    }
    public Sequence Substitute(Sequence input)
    {
        Sequence ret = new Sequence ();
        for (int i = 0; i < this.Size (); i++)
            ret=ret.Append (input [this [i]]);
        return ret;
    }
    public Sequence Expand(int n)
    {
        Sequence ret = new Sequence ();
        if (n >= 1) n = n - 1;
        for (int i = 0; i < this.Size (); i++) {
            ret=ret.Append (this [i]);
            for(int j = 0; j < n; j++)
                ret=ret.Append (0);
        }
        return ret;
    }
    public Sequence ExpandRepeat(int n)
    {
        Sequence ret = new Sequence ();
        if (n >= 1) n = n - 1;
        for (int i = 0; i < this.Size (); i++) {
            ret=ret.Append (this [i]);
            for(int j = 0; j < n; j++)
                ret=ret.Append (this[i]);
        }
        return ret;
    }
    public bool Contains(double n) {
        bool doesContain = false;
        for (int i = 0; i < this.Size(); i++) {
            if(this[i] == n) {
                doesContain = true;
                break;
            }
        }
        return doesContain;
    }
    public Sequence Replace(int toReplace, int replaceWith)
    {
        Sequence ret = new Sequence ();
        for (int i = 0; i < this.Size (); i++) {
            if (this [i] == toReplace) {
                ret=ret.Append (replaceWith);
            } else {
                ret=ret.Append (this [i]);
            }
        }
        return ret;
    }
    public Sequence Transpose(int transposeTo)
    {
        Sequence ret = new Sequence ();
        for (int i = 0; i < this.Size (); i++) {
            double note = this [i] % 12;
            while (note < transposeTo)
                note = note + 12;
            ret=ret.Append (note);
        }
        return ret;
    }
    public Sequence Integrate()
    {
        Sequence ret = new Sequence ();
        for (int i = 0; i < this.Size (); i++) {
            ret = ret.Append (this [i]);
            if (i > 0)
                ret [i] = ret [i] + ret [i - 1];
        }
        return ret;
    }
    public Sequence Derive()
    {
        Sequence ret = new Sequence ();
        for (int i = 0; i < this.Size (); i++) {
            ret = ret.Append (this [i]);
            if (i > 0)
                ret [i] = ret [i] - this [i - 1];
        }
        return ret;
    }
    public double Max()
    {
        double ret = this[0];
        for (int i = 0; i < this.Size (); i++) {
            if (this [i] > ret)
                ret = this [i];
        }
        return ret;
    }
    public double Min()
    {
        double ret = this[0];
        for (int i = 0; i < this.Size (); i++) {
            if (this [i] < ret)
                ret = this [i];
        }
        return ret;
    }
    public Sequence Normalize()
    {
        Sequence ret = new Sequence ();
        for (int i = 0; i < this.Size (); i++) {
            double min = this.Min (), max = this.Max ();
            ret = ret.Append ((this [i] - min) / (max - min));
        }
        return ret;
    }
    public Sequence Reverse()
    {
        Sequence ret = new Sequence ();
        for (int i = this.Size () - 1; i >= 0; i--)
            ret = ret.Append (this [i]);
        return ret;
    }
    public Sequence RepeatIf(double n)
    {
        Sequence ret = new Sequence ();
        for (int i = 0; i < this.Size (); i++) {
            ret = ret.Append (this [i]);
            if (this [i] == n && i > 0) {
                ret [i] = ret [i - 1];
            } else {
                ret [i] = this [i];
            }
        }
        return ret;
    }
    public Sequence SequencedSubstitute(List<Sequence> values)
    {
        Sequence valuesIndices = new Sequence();
        Sequence ret = new Sequence ();
        for (int i = 0; i < values.Count; i++)
            valuesIndices = valuesIndices.Append (0);
        for (int i = 0; i < this.Size (); i++) {
            int indicesIndex = this.GetInt(i);
            ret = ret.Append (values [indicesIndex % values.Count] [valuesIndices [indicesIndex]]);
            valuesIndices [indicesIndex] += 1;
        }
        return ret;
    } 
    public static Sequence GenBjork(int beats, int rests)
    {
        Sequence ret = new Sequence();
        int nPegs, nHoles;
        List<Sequence> pegs = new List<Sequence>();
        List<Sequence> holes = new List<Sequence>();
        nPegs = beats;
        nHoles = rests - beats;
        for (int i = 0; i < nPegs; i++)
            pegs.Add (new Sequence(1));
        for (int i = 0; i < nHoles; i++)
            holes.Add (new Sequence(0));
        int lastPegIndex = -1;
        do {
            int inc;
            if(lastPegIndex != -1) {
                List<Sequence> temp = new List<Sequence>();
                for(int i = lastPegIndex; i < pegs.Count; i++)
                    temp.Add(pegs[i]);
                holes = temp;
                temp = new List<Sequence>();
                for(int i = 0; i < lastPegIndex; i++)
                    temp.Add(pegs[i]);
                pegs = temp;
            }
            nPegs = pegs.Count;
            nHoles = holes.Count;
            for(inc = 0; inc < nHoles; inc++)
                pegs[inc % nPegs] = pegs[inc % nPegs].Concat(holes[inc % nHoles]);
            lastPegIndex = inc % nPegs;
        } while (lastPegIndex != 0 && lastPegIndex != (nPegs - 1));
        for(int i = 0; i < pegs.Count; i++) ret = ret.Concat(pegs[i]);
        return ret;
    }
    public Sequence BjorkSelect(int select)
    {
        if (select < 1)
            return Sequence.GenDup (0, this.Size ());
        Sequence ret = new Sequence ();
        int selectIndex = 0;
        Sequence selectPattern = Sequence.GenBjork (select, (int)(Math.Floor(this.Sum ())));
        for (int i = 0; i < this.Size (); i++) {
            ret = ret.Append (this [i]);
            if (ret [i] > 0) {
                ret [i] *= selectPattern [selectIndex];
                selectIndex += 1;
            }
        }
        return ret;
    }
    public static Sequence GenBjorkStructure(int size, Sequence selects)
    {
        Sequence ret = Sequence.GenDup (0, size);
        Sequence currentLevel = Sequence.GenDup (1, size);
        for (int i = 0; i < selects.Size (); i++) {
            currentLevel = currentLevel.BjorkSelect (selects.GetInt(i));
            ret = ret + currentLevel;
        }
        return ret;
    }
}

public class AudioHelpers
{
    public static double dB2Linear(double dB)
    {
        return Math.Pow(10, dB / 10);
    }
    public static double Linear2dB(double linear)
    {
        return 10 * Math.Log10(linear);
    }
}

public class SinOsc
{
    public double frequency;
    public double amplitude;
    private double delta;
    
    public SinOsc(double frequency, double amplitude)
    {
        this.frequency = frequency;
        this.amplitude = amplitude;
    }
    
    public double Process()
    {
        double output = Math.Sin (this.delta*2*Math.PI);
        this.delta = this.delta + (this.frequency / 48000);
        return output * amplitude;
    }
    
    public double Process(double phase)
    {
        double output = Math.Sin (this.delta*2*Math.PI+phase);
        this.delta = this.delta + (this.frequency / 48000);
        return output * amplitude;
    }
}

public class Line
{
    public double from;
    public double to;
    public double duration;
    
    private double delta;
    
    public Line(double from, double to, double duration)
    {
        this.from = from;
        this.to = to;
        this.duration = duration;
        this.delta = -1;
    }
    
    public double Process()
    {
        double output = 0;
        if(this.delta > -1)
        {
            output = ((this.to*this.delta)/this.duration) + ((this.from*(this.duration-this.delta))/this.duration);
            this.delta  = this.delta + (1.0/48000.0);
            if(this.delta>this.duration + (1.0/48000.0)) this.delta = -1;
        } else {
            output = this.to;
        }
        return output;    
    }
    
    public void Reset()
    {
        this.delta = 0;
    }
}

public class Pulse
{
    public double frequency;
    public double amplitude;
    public double pulsewidth;
    
    private double delta;
    
    public Pulse(double frequency, double amplitude, double pulsewidth)
    {
        this.frequency = frequency;
        this.amplitude = amplitude;
        this.pulsewidth = pulsewidth;
        this.delta = 0;
    }
    
    public double Process()
    {
        double output = 0;
        
        if(this.delta+(this.frequency/48000) < this.pulsewidth){
            output = this.amplitude;
        } else {
            output = -this.amplitude;
        }
        
        this.delta = this.delta + (this.frequency/48000);
        while(this.delta>1) this.delta = this.delta - 1;
        
        return output;
    }
}

public class Saw
{
    public double amplitude = 1;
    
    private double phase = 0;
    private double delta = 0;
    
    public double Process()
    {
        this.phase = this.phase + this.delta;
        double output = ((this.phase*2)-1)*this.amplitude;
        while(this.phase>1) this.phase = this.phase - 1;
        
        return output;
    }
    
    public double frequency {
        get {
            return this.delta*48000;
        }
        set {
            this.delta = (value/48000);
        }
    } 
}

public class Tri
{
    public double frequency;
    public double amplitude;
    
    private double delta;
    
    public double Process()
    {
        double output = 0;
        
        if( this.delta < 0.25 ){
            output = delta*4;
        } else if( this.delta < 0.5 ){
            output = 1-((delta-0.25)*4);
        } else if( this.delta < 0.75 ){
            output = -((delta-0.5)*4);
        } else if( this.delta < 1 ){
            output = -(1-((delta-0.75)*4));
        } else {
            output = 0;
        }
        
        output = output*this.amplitude;
        this.delta = this.delta + (48000/this.frequency);
        while(this.delta>1) this.delta = this.delta - 1;
        
        return output;
    }
}

public class WhiteNoise
{
    private System.Random rand = new System.Random ();
    public double Process() {
        return rand.NextDouble () * 2 - 1;
    }
}

public class Lag
{
    bool initialized = false;
    double value;
    public double Process(double target, double rate) {
        if(initialized == false) {
            initialized = true;
            value = target;
        }
        if (rate <= 0) {
            value = target;
        } else {
            value += (target - value) / (rate * 48000d);
        }
        return value;
    }
    public double SetValue(double target) {
        value = target;
        return value;
    }
}

public class Compressor
{
    bool initialized = false;
    Lag gainFilter = new Lag();
    double lastControlValue;
    public double Process(double input, double control, double dBthreshhold, double ratio, double attack, double release) {
        double controlRMS = Math.Abs (control), output = 0, smoothedRMS, gain = 1;
        if (initialized == false) {
            initialized = true;
            lastControlValue = control;
        }
        if(controlRMS > lastControlValue) {
            smoothedRMS = gainFilter.Process(controlRMS, attack);
        } else {
            smoothedRMS = gainFilter.Process(controlRMS, release);
        }
        if (smoothedRMS > AudioHelpers.dB2Linear (dBthreshhold)) {
            double dBdifference = AudioHelpers.Linear2dB(smoothedRMS) - dBthreshhold;
            double dBreduction = dBdifference-(dBdifference/ratio);
            gain = AudioHelpers.dB2Linear(-dBreduction);
        }
        output = input * gain;
        // --- //
        lastControlValue = Math.Abs (control);
        return output;
    }
}

public class LaserSynth
{
    private Pulse pulse = new Pulse (440, 1, 0.4);
    private double frequency = 440;
    private Line pitchEnv = new Line(1,0,0.15);
    private SinOsc vibrato = new SinOsc(40,1);
    
    public double Process()
    {
        double output = 0;
        double pitchEnvValue = pitchEnv.Process();
        vibrato.frequency = frequency * 6    ;
        pulse.frequency = frequency * Math.Pow (pitchEnvValue, 10 ) * 7 + frequency + (vibrato.Process()*(frequency/2));
        pulse.pulsewidth = Math.Pow (pitchEnvValue, 2);
        output += pulse.Process ();
        //return lowpass2.Process(lowpass.Process(output) + (highpass.Process(output)/2));
        return output;
    }
    
    public void PlayNote(double midiNote)
    {
        frequency = Math.Pow(2, midiNote/12) * 25.5;
        pitchEnv.Reset ();
    }
}

public class PhaseModBass
{
    private Biquad lowpass = new Biquad (FilterType.LowPass, 440d / 48000d, 1, 0);
    private Biquad lowpass2 = new Biquad (FilterType.LowPass, 440d / 48000d, 1, 0);
    private Biquad highpass = new Biquad (FilterType.HighPass, 440d / 48000d, 1, 0);
    private SinOsc modulator = new SinOsc (440, 1);
    private SinOsc osc = new SinOsc (440, 1);
    private Pulse pulse = new Pulse (440, 1, 0.4);
    private double frequency = 440;
    
    public double Process()
    {
        double output = 0;
        double mod = Math.Tanh(modulator.Process ()*5);
        osc.frequency = mod * 500 + (frequency * 2);
        output = osc.Process (mod * 10) / 2d;
        output = Math.Tanh (output * 10);
        output += pulse.Process ();
        return lowpass2.Process(lowpass.Process(output) + (highpass.Process(output)/2));
    }
    
    public void PlayNote(double midiNote)
    {
        frequency = Math.Pow(2, midiNote/12) * 25.5;
        pulse.frequency = frequency;
        modulator.frequency = frequency * 8 + 1;
        lowpass.SetFc (frequency / 48000d * 4d);
        lowpass2.SetFc (frequency / 48000d * 110d);
        highpass.SetFc (frequency / 48000d * 96d);
    }
}

public class SuperSaw
{
    private List<Saw> oscs = new List<Saw> ();
    private List<double> freqOffsets = new List<double>();
    private Line outEnv = new Line (1, 0, 1);
    private Line inEnv = new Line (0, 1, 0.01);
    private double midiNote = 48;
    System.Random rand = new System.Random ();
    
    public SuperSaw()
    {
        for (int i = 0; i < 15; i++) {
            oscs.Add (new Saw ());
            freqOffsets.Add (rand.NextDouble() * 2 - 1);
        }
    }
    
    public double Process()
    {
        double output = 0;
        double outEnvValue = outEnv.Process ();
        double inEnvValue = inEnv.Process ();
        for (int i = 0; i < oscs.Count; i++) {
            output += oscs [i].Process();
        }
        output = output / oscs.Count;
        return output * outEnvValue * inEnvValue;
    }
    
    public void PlayNote(double midiNote)
    {
        this.midiNote = midiNote;
        for (int i = 0; i < freqOffsets.Count; i++)
            freqOffsets[i] = rand.NextDouble() * 2 - 1;
        inEnv.Reset ();
        outEnv.Reset ();
        double frequency = Math.Pow (2, midiNote / 12) * 25.5;
        for (int i = 0; i < oscs.Count; i++) {
            oscs [i].frequency = frequency + (freqOffsets [i] * frequency / 45);
        }
    }
}

public class PortamentoSuperPulse
{
    private List<Pulse> oscs = new List<Pulse> ();
    private List<double> freqOffsets = new List<double>();
    private SinOsc widthMod = new SinOsc(0.01,0.25);
    private Line outEnv = new Line (1, 0, 1);
    private Line inEnv = new Line (0, 1, 0.01);
    private Lag midiLag = new Lag ();
    private double midiNote = 48;
    private Biquad lowpass = new Biquad (FilterType.LowPass, 440d / 48000d, 0.1, 0);
    System.Random rand = new System.Random ();
    
    public PortamentoSuperPulse()
    {
        for (int i = 0; i < 30; i++) {
            oscs.Add (new Pulse (440, 1, 0.1));
            freqOffsets.Add (rand.NextDouble() * 2 - 1);
        }
    }
    
    public double Process()
    {
        double output = 0;
        double outEnvValue = outEnv.Process ();
        double inEnvValue = inEnv.Process ();
        double frequency = Math.Pow (2, midiLag.Process (midiNote, 0) / 12) * 25.5;
        widthMod.frequency = frequency;
        double widthModValue = widthMod.Process ();
        for (int i = 0; i < oscs.Count; i++) {
            oscs[i].frequency = frequency * Math.Pow (2, i % 2) + (freqOffsets[i] * frequency / 30);
            oscs[i].pulsewidth = widthModValue + 0.5;
            output += oscs [i].Process ();
        }
        output = output / oscs.Count;
        lowpass.SetFc (frequency / 48000 * 6);
        return output * outEnvValue * inEnvValue;
    }
    
    public void PlayNote(double midiNote)
    {
        this.midiNote = midiNote;
        for (int i = 0; i < freqOffsets.Count; i++)
            freqOffsets[i] = rand.NextDouble() * 2 - 1;
        inEnv.Reset ();
        outEnv.Reset ();
    }
}

public class Voice
{
    private Pulse osc = new Pulse (440, 1, 0.1);
    private SinOsc widthModulator = new SinOsc (7, 0.5);
    private SinOsc detuneModulator = new SinOsc (0, 2);
    private Line outEnv = new Line (1, 0, 4);
    private Line inEnv = new Line (0, 1, 1);
    private Biquad lowpass = new Biquad (FilterType.LowPass, 440d / 48000d, 0.1, 0);
    System.Random rand = new System.Random ();
    
    public double Process()
    {
        double outEnvValue = outEnv.Process ();
        double inEnvValue = inEnv.Process ();
        if (detuneModulator.frequency == 0) {
            detuneModulator.frequency = rand.NextDouble ()+0.1;
        }
        return osc.Process () * outEnvValue * inEnvValue;
    }
    
    public void PlayNote(double midiNote)
    {
        osc.frequency = Math.Pow(2, midiNote/12) * 25.5;
        lowpass.SetFc (osc.frequency / 48000 * 6);
        osc.pulsewidth = widthModulator.Process() + 0.5;
        detuneModulator.amplitude = osc.frequency / 1500000;
        inEnv.Reset ();
        outEnv.Reset ();
    }
}

public class ThreeOhThree
{
    private Pulse pulse = new Pulse (440, 1, 0.5);
    private Saw saw = new Saw();
    private Line outEnv = new Line (1, 0, 0.3);
    private Line inEnv = new Line (0, 1, 0.01);
    private Biquad lowpass = new Biquad (FilterType.LowPass, 440d / 48000d, 4, 0);
    private Lag portamento = new Lag();
    
    private double note = 0;
    private double lag = 0.01;
    
    public double Process()
    {
        double outEnvValue = outEnv.Process ();
        double inEnvValue = inEnv.Process ();
        double freq = Math.Pow (2, portamento.Process(note, lag) / 12) * 25.5;
        pulse.frequency = freq;
        saw.frequency = freq;
        lowpass.SetFc ((freq * 8 * outEnvValue * inEnvValue + 20d) / 48000d);
        return Math.Tanh(lowpass.Process((pulse.Process () + saw.Process()) * outEnvValue * inEnvValue) * 4);
    }
    
    public void PlayNote(double midiNote)
    {
        note = midiNote;
        inEnv.Reset ();
        outEnv.Reset ();
    }
    
    public void PlayNote(double midiNote, double duration, double lag)
    {
        outEnv.duration = duration;
        this.lag = lag;
        note = midiNote;
        inEnv.Reset ();
        outEnv.Reset ();
    }
}

public class PolyphonicInstrument
{
    private Voice[] voices = new Voice[3];
    int voiceIndex = 0;
    
    public PolyphonicInstrument()
    {
        for (int i = 0; i < voices.Length; i++)
            voices [i] = new Voice ();
    }
    
    public double Process()
    {
        double output = 0;
        foreach (Voice voice in voices) {
            output = output + voice.Process();
        }
        return output / voices.Length;
    }
    
    public void PlayNote(double midiNote)
    {
        voices [this.voiceIndex++ % voices.Length].PlayNote (midiNote);
    }
}

public class PolyphonicSuperSaw
{
    private SuperSaw[] voices = new SuperSaw[6];
    int voiceIndex = 0;
    
    public PolyphonicSuperSaw()
    {
        for (int i = 0; i < voices.Length; i++)
            voices [i] = new SuperSaw ();
    }
    
    public double Process()
    {
        double output = 0;
        foreach (SuperSaw voice in voices) {
            output = output + voice.Process();
        }
        return output / voices.Length;
    }
    
    public void PlayNote(double midiNote)
    {
        voices [this.voiceIndex++ % voices.Length].PlayNote (midiNote);
    }
}

public class Kick
{
    private SinOsc osc = new SinOsc (0, 1);
    private Line outEnv = new Line (1, 0, 0.4);
    private Line inEnv = new Line (0, 1, 0.0015);
    
    public double Process()
    {
        double inEnvValue, outEnvValue, ampEnv, freqEnv;
        inEnvValue = inEnv.Process ();
        outEnvValue = outEnv.Process ();
        freqEnv = Math.Pow (inEnvValue, 2) * Math.Pow (outEnvValue, 10) * 300;
        ampEnv = inEnvValue * outEnvValue;
        
        osc.frequency = freqEnv;
        return osc.Process () * ampEnv;
    }
    
    public void Play()
    {
        inEnv.Reset ();
        outEnv.Reset ();
    }
}

public class PolyphonicSample
{
    private double[] samples;
    List<Sample> voices = new List<Sample>();
    int voiceIndex = 0;
    
    private PolyphonicSample() {}
    
    public PolyphonicSample(string sampleName, double delta, int numVoices)
    {
        byte[] audioFileBytes = File.ReadAllBytes (Application.dataPath + "/" + sampleName + ".raw");
        samples = new double[audioFileBytes.Length];
        
        for (int i = 0; i < samples.Length; i++) {
            samples[i] = (audioFileBytes[i] / 255d) * 2 - 1;
        }
        
        for (int i = 0; i < numVoices; i++) {
            voices.Add (new Sample(samples, delta));
        }
    }
    
    public double Process() {
        double output = 0;
        for(int i = 0; i < voices.Count; i++) {
            output += voices[i].Process();
        }
        return output;
    }
    
    public void Play(double offset)
    {
        voices [this.voiceIndex++ % voices.Count].Play (offset);
    }
}

public class Sample
{
    private double sampleIndex;
    private double[] samples;
    private double delta = 1;
    
    private Sample() {}
    
    public Sample(string sampleName)
    {
        byte[] audioFileBytes = File.ReadAllBytes (Application.dataPath + "/" + sampleName + ".raw");
        samples = new double[audioFileBytes.Length];
        sampleIndex = samples.Length;
        for (int i = 0; i < samples.Length; i++) {
            samples[i] = (audioFileBytes[i] / 255d) * 2 - 1;
        }
        sampleIndex = samples.Length;
    }
    
    public Sample(double[] sampleData, double delta)
    {
        samples = sampleData;
        sampleIndex = samples.Length;
    }
    
    public Sample(string sampleName, double delta)
    {
        byte[] audioFileBytes = File.ReadAllBytes (Application.dataPath + "/" + sampleName + ".raw");
        samples = new double[audioFileBytes.Length];
        sampleIndex = samples.Length;
        for (int i = 0; i < samples.Length; i++) {
            samples[i] = (audioFileBytes[i] / 255d) * 2 - 1;
        }
        this.delta = delta;
        sampleIndex = samples.Length;
    }
    
    public double Process()
    {
        double output = 0;
        if (sampleIndex < samples.Length) {
            output = samples [(int)(sampleIndex)];
            sampleIndex += delta;
        }
        return output;
    }
    
    public void Play()
    {
        sampleIndex = 0;
    }
    
    public void Play(double offset)
    {
        sampleIndex = (int)((samples.Length-1) * offset);
    }
}

public class FallingToneBass
{
    private Pulse osc = new Pulse (440,1,0.5);
    private Biquad lp = new Biquad (FilterType.LowPass, 440d / 48000d, 1, 0);
    private Biquad hp = new Biquad (FilterType.HighPass, 220d / 48000d, 1, 0);
    private Line outEnv = new Line (1, 0, 0.075);
    private Line inEnv = new Line (0, 1, 0.001);
    private Line freqEnv = new Line (1, 0, 3);
    private double freq = 440;
    private bool stopped = true;
    
    public double Process()
    {
        double inEnvValue, outEnvValue, ampEnv, freqEnvValue, freqEnvPreValue;
        inEnvValue = inEnv.Process ();
        outEnvValue = outEnv.Process ();
        freqEnvPreValue = freqEnv.Process ();
        freqEnvValue = freq * 2 * freqEnvPreValue + freq;
        ampEnv = inEnvValue;
        if (stopped == true)
            ampEnv = ampEnv * outEnvValue;
        osc.frequency = freqEnvValue;
        osc.pulsewidth = freqEnvPreValue * 0.45 + 0.05;
        lp.SetFc ((freqEnvValue * 2 + 20) / 48000d);
        double oscVal = osc.Process ();
        return hp.Process (lp.Process (oscVal * ampEnv));
    }
    
    public void PlayNote(double midiNote)
    {
        freq = Math.Pow(2, midiNote/12) * 25.5 / 2;
        if(stopped) inEnv.Reset ();
        freqEnv.Reset ();
        stopped = false;
    }
    
    public void Stop()
    {
        outEnv.Reset ();
        stopped = true;
    }
}

public class ShriekBass {
    private Saw sawOsc = new Saw();
    private SinOsc sinOsc = new SinOsc(30,1);
    private Line inEnv = new Line(0,1,0.5);
    
    private double midiNote = 0;
    private double harmonic = 11;
    private double phaseMod = 1;
    private double distortion = 2;

    private bool playing = true;
    
    public double Process()
    {
        if (!playing)
            return 0;

        sinOsc.frequency = Math.Pow(2, midiNote/12) * 25.5 / 2;
        sawOsc.frequency = Math.Pow(2, midiNote/12) * 25.5 * harmonic;
        double inEnvVal = inEnv.Process ();
        double sawVal = sawOsc.Process ();
        double sinVal = sinOsc.Process (sawVal * phaseMod * inEnvVal);

        return Math.Tanh(sinVal * (1+distortion));
    }
    
    public void PlayNote(double midiNote)
    {
        this.midiNote = midiNote;
        inEnv.Reset ();
        playing = true;
    }
    
    public void PlayNote(double midiNote, double harmonic, double phaseMod, double distortion)
    {
        this.midiNote = midiNote;
        this.harmonic = harmonic;
        this.phaseMod = phaseMod;
        this.distortion = distortion;
        inEnv.Reset ();
        playing = true;
    }

    public void Stop()
    {
        playing = false;
    }
}

public class NoiseBuilder {
    private Line inEnv = new Line(0,1,4);
    private WhiteNoise noise = new WhiteNoise ();
    private Biquad highpass = new Biquad(FilterType.HighPass, 22000d/48000d, 1, 0);
    private Biquad lowpass = new Biquad(FilterType.LowPass, 7000d/48000d, 1, 0);
    private Saw saw = new Saw ();
    private bool playing = false;
    private bool started = false;
    
    public double Process()
    {
        if (!started || !playing)
            return 0;
        double env = inEnv.Process ();
        highpass.SetFc (10000d * (1-env) / 48000);
        return lowpass.Process(highpass.Process(Math.Pow (env,3) * noise.Process() + saw.Process() * Math.Pow (env,3)));
    }
    
    public void Play(double inDuration, double outDuration)
    {
        started = true;
        inEnv.Reset ();
        playing = true;
        saw.frequency = 60;
        saw.amplitude = 4;
    }
    
    public void Stop()
    {
        playing = false;
    }
}

public class Hats
{
    private Line outEnv = new Line (1, 0, 0.075);
    private Line inEnv = new Line (0, 1, 0.001);
    private Biquad low = new Biquad (FilterType.BandPass, 7000d/48000d, 2, 3);
    private Biquad mid = new Biquad (FilterType.BandPass, 9000d/48000d, 2, 2);
    private Biquad high = new Biquad (FilterType.BandPass, 11000d/48000d, 2, 1);
    private Biquad lowpass = new Biquad (FilterType.LowPass, 11000d/48000d, 2, 0);
    System.Random rand = new System.Random ();
    
    public double Process()
    {
        double inEnvValue, outEnvValue, ampEnv, output = 0;
        inEnvValue = inEnv.Process ();
        outEnvValue = outEnv.Process ();
        ampEnv = inEnvValue * outEnvValue;
        
        output = rand.NextDouble () * 2 - 1;
        output = low.Process (output) + mid.Process (output) + high.Process (output);
        output = lowpass.Process (output / 3);
        
        return output * ampEnv;
    }
    
    public void Play()
    {
        inEnv.Reset ();
        outEnv.Reset ();
    }
}

public class PortamentoBlip {
    private SinOsc osc = new SinOsc(440,1);
    private Pulse mod = new Pulse (440, 1.0, 0.5);
    private Line env = new Line(1,0,0.1);
    private Lag portamento = new Lag();
    private WhiteNoise noise = new WhiteNoise();
    private Pulse pulseOsc = new Pulse (440, 1, 0.3);
    private Biquad lowpass = new Biquad (FilterType.LowPass, 100d / 48000d, 1, 0);
    private double frequency = 440;
    
    public double Process()
    {
        double freq = portamento.Process(frequency, 0.01);
        double envValue = env.Process ();
        mod.frequency = freq + (noise.Process()*freq*0.1);
        double modValue = mod.Process ();
        osc.frequency = freq;
        pulseOsc.frequency = freq;
        lowpass.SetFc (freq * 7 / 48000d);
        pulseOsc.pulsewidth = 0.05 + ((envValue) * 0.45);
        return (lowpass.Process(osc.Process (modValue * 15) + pulseOsc.Process()/4)) * envValue;
    }
    
    public void PlayNote(double midiNote)
    {
        frequency = Math.Pow(2, midiNote/12) * 25.5;
        env.Reset ();
    }
}

public enum FilterType {
    LowPass,
    HighPass,
    BandPass,
    Notch,
    Peak,
    LowShelf,
    HighShelf
};

public class Comb {
    double[] buffer;
    double feedback = 0;
    int head = 0;
    private Comb() { }
    public Comb(double lengthInSeconds, double feedbackCoef) {
        buffer = new double[(int)(lengthInSeconds*48000)];
        feedback = feedbackCoef;
    }
    public double Process(double input) {
        double output = 0;
        output = buffer [head] + input;
        buffer[head] = output * feedback;
        head = head + 1;
        while (head >= buffer.Length)
            head -= buffer.Length;
        return output;
    }
}

public class Biquad {
    public Biquad()
    {
        type = FilterType.LowPass;
        a0 = 1.0;
        a1 = a2 = b1 = b2 = 0.0;
        Fc = 0.50;
        Q = 0.707;
        peakGain = 0.0;
        z1 = z2 = 0.0;
    }
    public Biquad(FilterType type, double Fc, double Q, double peakGainDB)
    {
        SetBiquad(type, Fc, Q, peakGainDB);
        z1 = z2 = 0.0;
    }
    public void SetType(FilterType type)
    {
        this.type = type;
        calcBiquad();
    }
    public void SetQ(double Q)
    {
        this.Q = Q;
        calcBiquad();
    }
    public void SetFc(double Fc)
    {
        this.Fc = Fc > 0.5 ? 0.5 : Fc;
        calcBiquad();
    }
    public void SetPeakGain(double peakGainDB)
    {
        this.peakGain = peakGainDB;
        calcBiquad();
    }
    public void SetBiquad(FilterType type, double Fc, double Q, double peakGainDB)
    {
        this.type = type;
        this.Q = Q;
        this.Fc = Fc;
        SetPeakGain(peakGainDB);
    }
    public double Process(double input) {
        double output = input * a0 + z1;
        z1 = input * a1 + z2 - b1 * output;
        z2 = input * a2 - b2 * output;
        return output;
    }
    
    protected void calcBiquad()
    {
        double norm;
        double V = Math.Pow(10, Math.Abs(peakGain) / 20.0);
        double K = Math.Tan(Math.PI * Fc);
        switch (this.type) {
        case FilterType.LowPass:
            norm = 1 / (1 + K / Q + K * K);
            a0 = K * K * norm;
            a1 = 2 * a0;
            a2 = a0;
            b1 = 2 * (K * K - 1) * norm;
            b2 = (1 - K / Q + K * K) * norm;
            break;
            
        case FilterType.HighPass:
            norm = 1 / (1 + K / Q + K * K);
            a0 = 1 * norm;
            a1 = -2 * a0;
            a2 = a0;
            b1 = 2 * (K * K - 1) * norm;
            b2 = (1 - K / Q + K * K) * norm;
            break;
            
        case FilterType.BandPass:
            norm = 1 / (1 + K / Q + K * K);
            a0 = K / Q * norm;
            a1 = 0;
            a2 = -a0;
            b1 = 2 * (K * K - 1) * norm;
            b2 = (1 - K / Q + K * K) * norm;
            break;
            
        case FilterType.Notch:
            norm = 1 / (1 + K / Q + K * K);
            a0 = (1 + K * K) * norm;
            a1 = 2 * (K * K - 1) * norm;
            a2 = a0;
            b1 = a1;
            b2 = (1 - K / Q + K * K) * norm;
            break;
            
        case FilterType.Peak:
            if (peakGain >= 0) {    // boost
                norm = 1 / (1 + 1/Q * K + K * K);
                a0 = (1 + V/Q * K + K * K) * norm;
                a1 = 2 * (K * K - 1) * norm;
                a2 = (1 - V/Q * K + K * K) * norm;
                b1 = a1;
                b2 = (1 - 1/Q * K + K * K) * norm;
            }
            else {    // cut
                norm = 1 / (1 + V/Q * K + K * K);
                a0 = (1 + 1/Q * K + K * K) * norm;
                a1 = 2 * (K * K - 1) * norm;
                a2 = (1 - 1/Q * K + K * K) * norm;
                b1 = a1;
                b2 = (1 - V/Q * K + K * K) * norm;
            }
            break;
        case FilterType.LowShelf:
            if (peakGain >= 0) {    // boost
                norm = 1 / (1 + Math.Sqrt(2) * K + K * K);
                a0 = (1 + Math.Sqrt(2*V) * K + V * K * K) * norm;
                a1 = 2 * (V * K * K - 1) * norm;
                a2 = (1 - Math.Sqrt(2*V) * K + V * K * K) * norm;
                b1 = 2 * (K * K - 1) * norm;
                b2 = (1 - Math.Sqrt(2) * K + K * K) * norm;
            }
            else {    // cut
                norm = 1 / (1 + Math.Sqrt(2*V) * K + V * K * K);
                a0 = (1 + Math.Sqrt(2) * K + K * K) * norm;
                a1 = 2 * (K * K - 1) * norm;
                a2 = (1 - Math.Sqrt(2) * K + K * K) * norm;
                b1 = 2 * (V * K * K - 1) * norm;
                b2 = (1 - Math.Sqrt(2*V) * K + V * K * K) * norm;
            }
            break;
        case FilterType.HighShelf:
            if (peakGain >= 0) {    // boost
                norm = 1 / (1 + Math.Sqrt(2) * K + K * K);
                a0 = (V + Math.Sqrt(2*V) * K + K * K) * norm;
                a1 = 2 * (K * K - V) * norm;
                a2 = (V - Math.Sqrt(2*V) * K + K * K) * norm;
                b1 = 2 * (K * K - 1) * norm;
                b2 = (1 - Math.Sqrt(2) * K + K * K) * norm;
            }
            else {    // cut
                norm = 1 / (V + Math.Sqrt(2*V) * K + K * K);
                a0 = (1 + Math.Sqrt(2) * K + K * K) * norm;
                a1 = 2 * (K * K - 1) * norm;
                a2 = (1 - Math.Sqrt(2) * K + K * K) * norm;
                b1 = 2 * (K * K - V) * norm;
                b2 = (V - Math.Sqrt(2*V) * K + K * K) * norm;
            }
            break;
        }
        return;
    }
    protected FilterType type;
    protected double a0, a1, a2, b1, b2;
    protected double Fc, Q, peakGain;
    protected double z1, z2;    
};

public class SoundEngine : MonoBehaviour
{
    public double gain = 0.2;
    
    public int intensityLevel = 0;
    
    public bool firing = false;
    
    public int kickRhythmDensity = 7;
    public int bassRhythmDensity = 5;
    public double[] highHatPattern = {16,7,4,2};
    public double[] leadsArp = {0,2,4,7,0,2,9,7};
    
    Line metronome = new Line(1,0,0.39/12);

    SuperSaw lead = new SuperSaw ();
    SuperSaw fifthsPad = new SuperSaw ();
    SuperSaw thirdsPad = new SuperSaw ();
    PolyphonicInstrument chimes = new PolyphonicInstrument();
    PolyphonicSuperSaw superSaw = new PolyphonicSuperSaw ();
    PortamentoBlip blip = new PortamentoBlip ();
    ThreeOhThree tb = new ThreeOhThree ();
    ShriekBass sBass = new ShriekBass ();
    Comb leadVerb = new Comb (0.15, 0.75);
    NoiseBuilder build = new NoiseBuilder();
    double[] samples;

    Sample kick;
    Sample clap;
    Sample snare;
    Sample crash;
    PolyphonicSample hats;

    double risingEnv = 0;
    double fallingEnv = 1;
    
    int metro = 0;
    bool started = false;
    
    float prox = 0;

    void Update () {
        //PlayerControl gameObj = GameObject.FindGameObjectWithTag ("Player1").GetComponent<PlayerControl> ();
        //gameObj.proxCalcRate = 2;
        //prox = gameObj.proximityCoeff;
        //gain = 0.2 * AudioHelpers.dB2Linear (-10 + (10m * prox));
        prox = (float)(prox * 4.1);
        //Debug.Log (prox);
        //intensityLevel = prox < 4 ? ((int)prox) : 4;
        
        if (Input.GetButton ("Fire1")) {
            firing = true;
        } else
            firing = false;
    }
    
    Sequence rhythm = Sequence.GenBjorkStructure(128, new Sequence(64,32,16,8,8,4));
    Sequence melody = new Sequence();
    Sequence progression = new Sequence ();
    
    List<Sequence> chordProgression;
    
    Dictionary<string, PolyphonicSample> soundEffects = new Dictionary<string, PolyphonicSample>();

    public void PlaySound(int sound, double amplitude, double time) {
        if (soundEffects.ContainsKey (sound.ToString())) soundEffects [sound.ToString()].Play (0);
    }

    public void PlaySound(string sound, double amplitude, double time) {
        if (soundEffects.ContainsKey (sound)) soundEffects [sound].Play (0);
    }
    
    void Start () {
        kick = new Sample ("kick", 1);
        clap = new Sample ("snare2");
        snare = new Sample ("snare");
        hats = new PolyphonicSample ("hats", 1, 10);
        crash = new Sample ("crash", 1);
        
        // --- //
        

        string[] filePaths = Directory.GetFiles (Application.dataPath);
        foreach (string filePath in filePaths) {
            if(filePath.EndsWith(".raw")) {
                string soundName = Path.GetFileName(filePath).Replace(".raw","");
                soundEffects.Add (soundName, new PolyphonicSample(soundName,0,2));
            }
        }
        
        // --- //
        
        for (int i = 0; i < 8; i++) progression = progression.Append (i * 4);
        progression = progression.Reverse ();
        
        melody = rhythm;
        for (int i = 0; i <= 6; i++) {
            melody = melody.Replace (i, i > 3 ? 1 : 0);
        }
        melody = melody.BjorkSelect (progression.Size ());
        List<Sequence> seqs = new List<Sequence> ();
        seqs.Add(new Sequence(-1));
        seqs.Add(progression);
        melody = melody.SequencedSubstitute (seqs);
        melody = melody.RepeatIf (-1);
        progression = melody;
        chordProgression = new List<Sequence>();
    }
    
    void BuildProgA() {
        chordProgression.Add (new Sequence (6,9,1));
        chordProgression.Add (new Sequence (6,9,1));
        chordProgression.Add (new Sequence (6,9,1));
        chordProgression.Add (new Sequence (5,9,0));
        chordProgression.Add (new Sequence (6,9,1));
        chordProgression.Add (new Sequence (6,9,1));
        chordProgression.Add (new Sequence (6,9,1));
        chordProgression.Add (new Sequence (4,8,11));
        chordProgression.Add (new Sequence (6,9,1));
        chordProgression.Add (new Sequence (6,9,1));
        chordProgression.Add (new Sequence (6,9,1));
        chordProgression.Add (new Sequence (5,9,0));
        chordProgression.Add (new Sequence (10,1,5));
        chordProgression.Add (new Sequence (3,6,10));
        chordProgression.Add (new Sequence (8,11,3));
        chordProgression.Add (new Sequence (11,2,6));
    }

    void BuildProgB() {
        chordProgression.Add (new Sequence (6,9,1));
        chordProgression.Add (new Sequence (11,2,6));
        chordProgression.Add (new Sequence (4,8,11));
        chordProgression.Add (new Sequence (1,5,8));
        chordProgression.Add (new Sequence (2,6,9));
        chordProgression.Add (new Sequence (11,2,5));
        chordProgression.Add (new Sequence (1,6,8));
        chordProgression.Add (new Sequence (1,5,8));
    }

    void InstrumentationA() {

        if(rhythm[metro] > 3) {
            kick.Play();
        }
        
        if(rhythm[metro] == 5) {
            snare.Play ();
            clap.Play ();
        }
        
        if((new Sequence(3,4,5,6)).Contains((double)rhythm[metro])) {
            fifthsPad.PlayNote(chord.Transpose(32)[1]);
            superSaw.PlayNote(chord.Transpose(20)[0]);
            superSaw.PlayNote(chord.Transpose(27)[1]);
            superSaw.PlayNote(chord.Transpose(34)[2]);
            superSaw.PlayNote(chord.Transpose(41)[0]);
            superSaw.PlayNote(chord.Transpose(48)[1]);
            superSaw.PlayNote(chord.Transpose(55)[2]);
            tb.PlayNote(chord.Transpose(14)[rhythm[metro]] + (12*(new Sequence(2,0,1,0))[rhythm[metro]]), Math.Pow (rhythm[metro]/6,1.5), rhythm[metro]/500);
        }
        
        if((new Sequence(5,6)).Contains((double)rhythm[metro])) {
            for(int n = 0; n < chord.Size(); n++) {
                double note = chord.Transpose(50)[n];
                chimes.PlayNote(note);
            }
        }
        
        if((new Sequence(4,6)).Contains((double)rhythm[metro])) {
            lead.PlayNote(chord.Transpose(60)[metro*7]);
        }
        
        if((new Sequence(3)).Contains((double)rhythm[metro])) {
            blip.PlayNote(chord.Transpose(48)[2]);
        }
        
        if(metro % 1 == 0) {
            Sequence hatSeq = rhythm.Normalize();
            hats.Play(1-(Math.Pow(hatSeq[metro],0.4)));
        }

        sBass.Stop ();
    }

    int instrumentationStepB = 0;
    void InstrumentationB() {

        if ((metro + Math.Pow (2,6)) % (Math.Pow (2,6)*8) == 0)
            build.Play (1, 1);

        if(rhythm[metro] > 3) {
            kick.Play();
        }
        
        if(rhythm[metro] == 5) {
            snare.Play ();
            clap.Play ();
        }

        sBass.Stop ();
        switch (instrumentationStepB) {
        case 0:
            if ((new Sequence (3, 4, 5, 6)).Contains ((double)rhythm [metro])) {
                fifthsPad.PlayNote (chord.Transpose (32) [1]);
                superSaw.PlayNote (chord.Transpose (20) [0]);
                superSaw.PlayNote (chord.Transpose (27) [1]);
                superSaw.PlayNote (chord.Transpose (34) [2]);
                superSaw.PlayNote (chord.Transpose (41) [0]);
                superSaw.PlayNote (chord.Transpose (48) [1]);
                superSaw.PlayNote (chord.Transpose (55) [2]);
            }
            break;
        case 1:
            if ((new Sequence (4, 6)).Contains ((double)rhythm [metro])) {
                lead.PlayNote (chord.Transpose (60) [metro * 7]);
            }
            
            if ((new Sequence (3)).Contains ((double)rhythm [metro])) {
                blip.PlayNote (chord.Transpose (48) [2]);
            }
            break;
        case 2:
            if ((new Sequence (3, 4, 5, 6)).Contains ((double)rhythm [metro])) {
                tb.PlayNote (chord.Transpose (14) [rhythm [metro]] + (12 * (new Sequence (2, 0, 1, 0)) [rhythm [metro]]), Math.Pow (rhythm [metro] / 9, 1.5), rhythm [metro] / 500);
            }
            break;
        case 3:
            if (metro % 1 == 0) {
                Sequence hatSeq = rhythm.Normalize ();
                hats.Play (1 - (Math.Pow (hatSeq [metro], 0.4)));
            }
            break;
        case 4:
            if ((new Sequence (2, 3, 4, 5, 6)).Contains ((double)rhythm [metro])) {
                sBass.PlayNote (chord.Transpose (22) [2] + (12 * (new Sequence (0, 0, 1)) [rhythm [metro]]), rhythm [metro], 1 - rhythm.Normalize () [metro], (1 - rhythm.Normalize () [metro]) * 10);
            }
            break;
        }

        if(metro % 1 == 0) instrumentationStepB = (instrumentationStepB + 1) % 5;
    }

    int instrumentationStepC = 0;
    void InstrumentationC() {

        if ((metro + Math.Pow (2,6)) % (Math.Pow (2,6)*8) == 0)
            build.Play (1, 1);

        if(rhythm[metro] > 3) {
            kick.Play();
        }
        
        if(rhythm[metro] == 5) {
            snare.Play ();
            clap.Play ();
        }
        
        sBass.Stop ();
        switch (instrumentationStepC) {
        case 0:

            if ((new Sequence (5)).Contains ((double)rhythm [metro])) {
                superSaw.PlayNote (chord.Transpose (20) [0]);
                superSaw.PlayNote (chord.Transpose (27) [1]);
                superSaw.PlayNote (chord.Transpose (34) [2]);
                superSaw.PlayNote (chord.Transpose (41) [0]);
                superSaw.PlayNote (chord.Transpose (48) [1]);
                superSaw.PlayNote (chord.Transpose (55) [2]);
            }

            if ((new Sequence (5, 6)).Contains ((double)rhythm [metro])) {
                lead.PlayNote (chord.Transpose (60) [metro * 7]);
                fifthsPad.PlayNote (chord.Transpose (65) [metro * 7 + 1]);
            }

            if ((new Sequence (3)).Contains ((double)rhythm [metro])) {
                blip.PlayNote (chord.Transpose (48) [2]);
            }

            if ((new Sequence (3)).Contains ((double)rhythm [metro])) {
                tb.PlayNote (chord.Transpose (14) [rhythm [metro]] + (12 * (new Sequence (2, 0, 1, 0)) [rhythm [metro]]), Math.Pow (rhythm [metro] / 9, 1.5), rhythm [metro] / 500);
            }

            if (metro % 1 == 0) {
                Sequence hatSeq = rhythm.Normalize ();
                hats.Play (1 - (Math.Pow (hatSeq [metro], 0.4)));
            }

            if ((new Sequence (3)).Contains ((double)rhythm [metro])) {
                sBass.PlayNote (chord.Transpose (22) [2] - 12, 7, (1 - rhythm.Normalize () [metro]) * 100, (1 - rhythm.Normalize () [metro]) * 10);
            }
            break;
        }
        
        //if(metro % 1 == 0) instrumentationStepC = (instrumentationStepC + 1) % 5;
    }

    int progIndex = -1;
    Sequence chord = new Sequence (0, 3, 7);

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (started == false) {
            if(true) started = true;
        }
        
        for (var i = 0; i < data.Length; i = i + channels) {
            double output = 0;
            
            if(metronome.Process() == 0)
            {
                risingEnv = ((double)metro)/512d;
                fallingEnv = 1-risingEnv;
                risingEnv = fallingEnv = 0;

                // --- //
            
                if(metro % 32 == 0) {
                    if(chordProgression.Count < 1) {
                        switch(progIndex) {
                        case 0:
                            chordProgression.Clear ();
                            BuildProgA();
                            progIndex = 1;
                            break;
                        case 1:
                            chordProgression.Clear ();
                            BuildProgA();
                            BuildProgB();
                            progIndex = 2;
                            break;
                        case 2:
                            chordProgression.Clear ();
                            BuildProgB ();
                            BuildProgB ();
                            progIndex = 0;
                            break;
                        default:
                            chordProgression.Clear ();
                            BuildProgA();
                            progIndex = 0;
                            break;
                        }
                        crash.Play ();
                    }
                    chord = chordProgression[0];
                    chordProgression.RemoveAt(0);
                }

                // --- //

                switch(progIndex) {
                case 0:
                    InstrumentationB ();
                    break;
                case 1:
                    InstrumentationC ();
                    break;
                case 2:
                    InstrumentationA ();
                    break;
                }

                if (metro % (Math.Pow (2,6)*8) == 0)
                    build.Stop ();

                metronome.Reset();
                metro++;
            }

            output += kick.Process();
            output += hats.Process()/2;
            output += snare.Process()/2;
            output += clap.Process();
            output += sBass.Process()/2;
            output += thirdsPad.Process()/2;
            output += fifthsPad.Process()/2;
            output += chimes.Process()/10;
            output += tb.Process()/4;
            output += lead.Process()/3;
            output += blip.Process()/4;
            output += superSaw.Process()*1.5;
            output += crash.Process()*AudioHelpers.dB2Linear(-3);

            output += build.Process();

            output += (AudioHelpers.dB2Linear(-8)*leadVerb.Process(output)) + output;

            foreach(KeyValuePair<string, PolyphonicSample> entry in soundEffects)
            {
                output += (entry.Value.Process() + entry.Value.Process()) / 2;
            }

            data[i] = (float)(output * gain);
            // if we have stereo, we copy the mono data to each channel
            if (channels == 2) data[i + 1] = data[i];
        }
    }
} 
