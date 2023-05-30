using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class FIR : Algorithm
    {
        public Signal InputTimeDomainSignal { get; set; }
        public FILTER_TYPES InputFilterType { get; set; }
        public float inputFrequencySample { get; set; }
        public float? InputCutOffFrequency { get; set; }
        public float? InputF1 { get; set; }
        public float? InputF2 { get; set; }
        public float InputStopBandAttenuation { get; set; }
        public float InputTransitionBand { get; set; }
        public Signal OutputHn { get; set; }
        public Signal OutputYn { get; set; }

        public override void Run()
        {
            int N = 0;
            String windowName = "";
            float value = 0.0f;
            OutputHn = new Signal(new List<float>(), new List<int>(), false);
            if (InputStopBandAttenuation <= 21)
            {
                windowName = "rectangle";
                value = 0.9f;
            }
            else if (InputStopBandAttenuation > 21 && InputStopBandAttenuation <= 44)
            {
                windowName = "hanning";
                value = 3.1f;
            }
            else if (InputStopBandAttenuation > 44 && InputStopBandAttenuation <= 53)
            {
                windowName = "hamming";
                value = 3.3f;
            }
            else if (InputStopBandAttenuation > 53 && InputStopBandAttenuation <= 74)
            {
                windowName = "blackman";
                value = 5.5f;
            }
            N = (int)Math.Floor((value / (InputTransitionBand / inputFrequencySample)) + 1);
            // for low pass and high pass
            float cutOfFrequency1 = 0.0f;
            // for band pass and band stop  
            float cutOfFrequency2 = 0.0f;
            float cutOfFrequency3 = 0.0f;

            if (InputFilterType == FILTER_TYPES.LOW)
            {
                cutOfFrequency1 = (float)(InputCutOffFrequency + (InputTransitionBand / 2));
            }
            else if (InputFilterType == FILTER_TYPES.HIGH)
            {
                cutOfFrequency1 = (float)(InputCutOffFrequency - (InputTransitionBand / 2));
            }
            else if (InputFilterType == FILTER_TYPES.BAND_PASS)
            {
                cutOfFrequency2 = (float)(InputF1 - (InputTransitionBand / 2));
                cutOfFrequency3 = (float)(InputF2 + (InputTransitionBand / 2));
            }
            else if (InputFilterType == FILTER_TYPES.BAND_STOP)
            {
                cutOfFrequency2 = (float)(InputF1 + (InputTransitionBand / 2));
                cutOfFrequency3 = (float)(InputF2 - (InputTransitionBand / 2));
            }
            cutOfFrequency1 = cutOfFrequency1 / inputFrequencySample;
            cutOfFrequency2 = cutOfFrequency2 / inputFrequencySample;
            cutOfFrequency3 = cutOfFrequency3 / inputFrequencySample;

            for (int i = 0, n = (int)-N / 2; i < N; i++, n++)
            {
                OutputHn.SamplesIndices.Add(n);
            }

            if (InputFilterType == FILTER_TYPES.LOW)
            {
                for (int i = 0; i < N; i++)
                {
                    int index = Math.Abs(OutputHn.SamplesIndices[i]);
                    if (OutputHn.SamplesIndices[i] == 0)
                    {
                        float hOfn = 2 * cutOfFrequency1;
                        float windowN = window_function(windowName, index, N);
                        OutputHn.Samples.Add(hOfn * windowN);
                    }
                    else
                    {
                        float WC = (float)(2 * Math.PI * cutOfFrequency1 * index);
                        float hOfn = (float)(2 * cutOfFrequency1 * Math.Sin(WC) / WC);
                        float windowc = window_function(windowName, index, N);
                        OutputHn.Samples.Add(hOfn * windowc);
                    }
                }
            }
            else if (InputFilterType == FILTER_TYPES.HIGH)
            {
                for (int i = 0; i < N; i++)
                {
                    int index = Math.Abs(OutputHn.SamplesIndices[i]);
                    if (OutputHn.SamplesIndices[i] == 0)
                    {
                        float hOfn = (2 * cutOfFrequency1);
                        hOfn = 1 - hOfn;
                        float windowN = window_function(windowName, index, N);
                        OutputHn.Samples.Add(hOfn * windowN);
                    }
                    else
                    {
                        float windowC = (float)(2 * Math.PI * cutOfFrequency1 * index);
                        float hOfn = (float)(2 * cutOfFrequency1 * Math.Sin(windowC) / windowC);
                        hOfn = -hOfn;
                        float windowOfn = window_function(windowName, index, N);
                        OutputHn.Samples.Add(hOfn * windowOfn);
                    }
                }

            }
            else if (InputFilterType == FILTER_TYPES.BAND_PASS)
            {
                for (int i = 0; i < N; i++)
                {
                    int index = Math.Abs(OutputHn.SamplesIndices[i]);
                    if (OutputHn.SamplesIndices[i] == 0)
                    {
                        float hOfn = 2 * (cutOfFrequency3 - cutOfFrequency2);
                        float windowN = window_function(windowName, index, N);
                        OutputHn.Samples.Add(hOfn * windowN);
                    }
                    else
                    {
                        float window2 = (float)(2 * Math.PI * cutOfFrequency3 * index);
                        float window1 = (float)(2 * Math.PI * cutOfFrequency2 * index);
                        float hOfn = (float)((2 * cutOfFrequency3 * Math.Sin(window2) / window2)
                            - (2 * cutOfFrequency2 * Math.Sin(window1) / window1));

                        float windowN = (window_function(windowName, index, N));
                        OutputHn.Samples.Add(hOfn * windowN);
                    }
                }
            }
            else if (InputFilterType == FILTER_TYPES.BAND_STOP)
            {
                for (int i = 0; i < N; i++)
                {
                    int index = Math.Abs(OutputHn.SamplesIndices[i]);
                    if (OutputHn.SamplesIndices[i] == 0)
                    {
                        float hOfn = 1 - (2 * (cutOfFrequency3 - cutOfFrequency2));
                        float windowN = window_function(windowName, index, N);
                        OutputHn.Samples.Add(hOfn * windowN);
                    }
                    else
                    {
                        float window2 = (float)(2 * Math.PI * cutOfFrequency3 * index);
                        float window1 = (float)(2 * Math.PI * cutOfFrequency2 * index);
                        float hOfn = (float)((2 * cutOfFrequency2 * Math.Sin(window1) / window1)
                            - (2 * cutOfFrequency3 * Math.Sin(window2) / window2));

                        float windowN = (window_function(windowName, index, N));
                        OutputHn.Samples.Add(hOfn * windowN);
                    }
                }
            }
            DirectConvolution c = new DirectConvolution();
            c.InputSignal1 = InputTimeDomainSignal;
            c.InputSignal2 = OutputHn;
            c.Run();
            OutputYn = c.OutputConvolvedSignal;
        }
        public float window_function(String windowName, int n, int N)
        {
            float result = 0.0f;
            if (windowName == "rectangle")
            {
                result = 1;
            }
            else if (windowName == "hanning")
            {
                result = (float)0.5 + (float)(0.5 * Math.Cos((2 * Math.PI * n) / N));
            }
            else if (windowName == "hamming")
            {
                result = (float)0.54 + (float)(0.46 * Math.Cos((2 * Math.PI * n) / N));
            }
            else if (windowName == "blackman")
            {
                result = (float)(0.42 + (float)(0.5 * Math.Cos((2 * Math.PI * n) / (N - 1))) +
                      (float)(0.08 * Math.Cos((4 * Math.PI * n) / (N - 1))));
            }
            return result;
        }
    }
}