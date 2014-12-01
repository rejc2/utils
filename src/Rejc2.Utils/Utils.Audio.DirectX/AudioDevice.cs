//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading;

//namespace Decompose.Audio
//{
//    public unsafe delegate void AudioCallback(byte *buffer, int length);

//    class AudioSpec
//    {
//        public float freq;
//        public float channels;
//        public int samples;

//        public event AudioCallback callback;
//    }

//    abstract unsafe class AudioDevice : IDisposable
//    {
//        public abstract bool openAudio(AudioSpec spec);
//        public abstract void threadInit(); // Called by audio thread at start
//        public abstract void waitAudio();
//        public abstract void playAudio();
//        public abstract byte* getAudioBuf();
//        public abstract void waitDone();
//        public abstract void closeAudio();

//        /* The current audio specification (shared with audio thread) */
//        public AudioSpec m_Spec;

//        /* An audio conversion block for audio format emulation */
//        //AudioCVT convert ;

//        /* Current state flags */
//        public bool enabled;
//        public bool paused;

//        /* Fake audio buffer for when the audio hardware is busy */
//        public byte* fake_stream;

//        /* A semaphore for locking the mixing buffers */
//        public object mixer_lock = new object();

//        /* A thread to feed the audio device */
//        public Thread thread;

//        public void runAudio()
//        {
//            throw new NotImplementedException();
//        }

//        protected AudioDevice(AudioSpec spec)
//        {
//            m_Spec = spec;
//        }

//        #region IDisposable Members

//        public void Dispose()
//        {
//            throw new Exception("The method or operation is not implemented.");
//        }

//        #endregion
//    }
//}
