//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Rejc2.Audio
//{
//    public class Audio
//    {
//        private readonly float m_SampleRate;

//        public float SampleRate
//        {
//            [System.Diagnostics.DebuggerStepThrough]
//            get { return m_SampleRate; }
//        }

//        public event EventHandler<FillBufferEventArgs> FillBuffer;

//        internal protected virtual void OnFillBuffer(FillBufferEventArgs e)
//        {
//            EventHandler<FillBufferEventArgs> handler = FillBuffer;
//            if (handler != null)
//                handler(this, e);
//        }

//        public Audio(float sampleRate)
//        {
//            m_SampleRate = sampleRate;
//        }
//    }
//}
