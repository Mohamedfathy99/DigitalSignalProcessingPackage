﻿using DSPAlgorithms.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace DSPAlgorithms.Algorithms
{
    public class Task2 : Algorithm
    {
        public String SignalPath { get; set; }
        public float Fs { get; set; }
        public float miniF { get; set; }
        public float maxF { get; set; }
        public Signal OutputFreqDomainSignal { get; set; }


        private FIR applyFilter(Signal InputSignal)
        {
            FIR fir = new FIR();
            fir.InputFilterType = FILTER_TYPES.BAND_PASS;
            fir.InputTimeDomainSignal = InputSignal;

            fir.InputStopBandAttenuation = 50;
            fir.InputTransitionBand = 500;

            fir.inputFrequencySample = Fs;
            fir.InputF1 = miniF;
            fir.InputF2 = maxF;

            fir.Run();
            return fir;
        }
        private DCT apply_DCT(Signal InputSignal)
        {
            DCT dct = new DCT();
            dct.InputSignal = InputSignal;
            dct.Run();
            return dct;
        }
        private DC_Component applyDC(Signal InputSignal)
        {
            DC_Component dc = new DC_Component();
            dc.InputSignal = InputSignal;
            dc.Run();
            return dc;
        }

        private Normalizer applyNormalization(Signal InputSignal)
        {
            Normalizer normalizer = new Normalizer();
            normalizer.InputMaxRange = 1;
            normalizer.InputMinRange = -1;
            normalizer.InputSignal = InputSignal;
            normalizer.Run();
            return normalizer;
        }
        private DiscreteFourierTransform applyDFT(Signal InputSignal)
        {
            DiscreteFourierTransform dft = new DiscreteFourierTransform();
            dft.InputTimeDomainSignal = InputSignal;
            dft.InputSamplingFrequency = Fs;
            dft.Run();
            return dft;
        }

        //-----------------------------------------------------
        //------------------------------------------------------
        public override void Run()
        {
            //---------------------------------------------------------------------------------------------------------
            Signal InputSignal = LoadSignal(SignalPath);
            FIR Out = applyFilter(InputSignal);
            DCT in_fir = apply_DCT(Out.OutputYn);
            DC_Component in_dct = applyDC(in_fir.OutputSignal);
            Normalizer in_dc_comp = applyNormalization(in_dct.OutputSignal);
            DiscreteFourierTransform in_norm = applyDFT(in_dc_comp.OutputNormalizedSignal);
            OutputFreqDomainSignal = in_norm.OutputFreqDomainSignal;

        }

        public Signal LoadSignal(string filePath)
        {
            Stream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var sr = new StreamReader(stream);

            var sigType = byte.Parse(sr.ReadLine());
            var isPeriodic = byte.Parse(sr.ReadLine());
            long N1 = long.Parse(sr.ReadLine());

            List<float> SigSamples = new List<float>(unchecked((int)N1));
            List<int> SigIndices = new List<int>(unchecked((int)N1));
            List<float> SigFreq = new List<float>(unchecked((int)N1));
            List<float> SigFreqAmp = new List<float>(unchecked((int)N1));
            List<float> SigPhaseShift = new List<float>(unchecked((int)N1));

            if (sigType == 1)
            {
                SigSamples = null;
                SigIndices = null;
            }

            for (int i = 0; i < N1; i++)
            {
                if (sigType == 0 || sigType == 2)
                {
                    var timeIndex_SampleAmplitude = sr.ReadLine().Split();
                    SigIndices.Add(int.Parse(timeIndex_SampleAmplitude[0]));
                    SigSamples.Add(float.Parse(timeIndex_SampleAmplitude[1]));
                }
                else
                {
                    var Freq_Amp_PhaseShift = sr.ReadLine().Split();
                    SigFreq.Add(float.Parse(Freq_Amp_PhaseShift[0]));
                    SigFreqAmp.Add(float.Parse(Freq_Amp_PhaseShift[1]));
                    SigPhaseShift.Add(float.Parse(Freq_Amp_PhaseShift[2]));
                }
            }

            if (!sr.EndOfStream)
            {
                long N2 = long.Parse(sr.ReadLine());

                for (int i = 0; i < N2; i++)
                {
                    var Freq_Amp_PhaseShift = sr.ReadLine().Split();
                    SigFreq.Add(float.Parse(Freq_Amp_PhaseShift[0]));
                    SigFreqAmp.Add(float.Parse(Freq_Amp_PhaseShift[1]));
                    SigPhaseShift.Add(float.Parse(Freq_Amp_PhaseShift[2]));
                }
            }

            stream.Close();
            return new Signal(SigSamples, SigIndices, isPeriodic == 1, SigFreq, SigFreqAmp, SigPhaseShift);
        }
    }
}