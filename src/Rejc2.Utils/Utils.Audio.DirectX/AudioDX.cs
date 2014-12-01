//using System;
//using System.Collections.Generic;
//using System.Text;
//using DxVBLib;
//using System.Threading;

//namespace Decompose.Audio
//{
//    class AudioDX : AudioDevice
//    {
//        static readonly DirectX7Class s_DirectX = new DirectX7Class();

//        public IntPtr mainwin;

//        DirectSound sound;

//        DirectSoundBuffer mixbuf;
//        int NUM_BUFFERS;
//        int mixlen, silence;
//        int playing;
//        byte* locked_buf;
//        //IntPtr audio_event;

//        public AudioDX(AudioSpec spec)
//            : base(spec)
//        { }

//        int createAudioEvent();

//        public override bool openAudio(AudioSpec spec)
//        {
//            WAVEFORMATEX waveformat = new WAVEFORMATEX();
//            waveformat.nFormatTag = CONST_WAVEFORMATFLAGS.WAVE_FORMAT_1S16;
//            /* Set basic WAVE format parameters */
//            //memset(&waveformat, 0, sizeof(waveformat));
//            //waveformat.wFormatTag = WAVE_FORMAT_PCM;

//            /* Determine the audio parameters from the AudioSpec */
//            //switch ( spec.format & 0xFF ) {
//            //    case 8:
//            //        /* Unsigned 8 bit audio data */
//            //        spec.format = AUDIO_U8;
//            //        silence = 0x80;
//            //        waveformat.wBitsPerSample = 8;
//            //        break;
//            //    case 16:
//            //        /* Signed 16 bit audio data */
//            //        spec.format = AUDIO_S16;
//            //        silence = 0x00;
//            //        waveformat.wBitsPerSample = 16;
//            //        break;
//            //    default:
//            //        cerr << "Unsupported audio format" << endl ;
//            //        return false ;
//            //}
//            waveformat.nBitsPerSample = 16;
//            waveformat.nChannels = spec.channels;
//            waveformat.lSamplesPerSec = (int)spec.freq;
//            waveformat.nBlockAlign = waveformat.nChannels * (waveformat.nBitsPerSample / 8);
//            waveformat.lAvgBytesPerSec = waveformat.lSamplesPerSec * waveformat.nBlockAlign;

//            /* Update the fragment size as size in bytes */
//            //spec.calculate() ;

//            /* Open the audio device */
//            sound = s_DirectX.DirectSoundCreate(null);

//            /* Create the audio buffer to which we write */
//            NUM_BUFFERS = -1;
//            if (mainwin != IntPtr.Zero)
//            {
//                NUM_BUFFERS = createPrimary(sound, mainwin, mixbuf,
//                                waveformat, spec.size);
//            }
//            if (NUM_BUFFERS < 0)
//            {
//                NUM_BUFFERS = createSecondary(sound, mainwin, mixbuf,
//                                waveformat, spec.size);
//                if (NUM_BUFFERS < 0)
//                {
//                    return false;
//                }
//            }

//            /* The buffer will auto-start playing in DX5_WaitAudio() */
//            playing = 0;
//            mixlen = spec.size;
//        }

//        public override void threadInit()
//        {
//            Thread.CurrentThread.Priority = ThreadPriority.Highest;
//        }

//        public override void waitAudio()
//        {
//    //DWORD status;
//    DSCURSORS cursors, junk;
//    HRESULT result;

//    /* Semi-busy wait, since we have no way of getting play notification
//       on a primary mixing buffer located in hardware (DirectX 5.0)
//    */
//    //result = IDirectSoundBuffer_GetCurrentPosition(mixbuf, &cursor, &junk);
//            mixbuf.GetCurrentPosition(out cursors);
//    //if ( result != DS_OK ) {
//    //    if ( result == DSERR_BUFFERLOST ) {
//    //        mixbuf.restore();
//    //    }
//    //    return;
//    //}
//    int cursor = cursors.lPlay / mixlen;

//    while ( cursor == playing ) {
//        /* FIXME: find out how much time is left and sleep that long */
//        Thread.Sleep(10) ;

//        /* Try to restore a lost sound buffer */
//        IDirectSoundBuffer_GetStatus(mixbuf, &status);
//        CONST_DSBSTATUSFLAGS status = mixbuf.GetStatus();
//        if ( (status& CONST_DSBSTATUSFLAGS.DSBSTATUS_BUFFERLOST) ) {
//            //IDirectSoundBuffer_Restore(mixbuf);
//            mixbuf.restore();
//            //IDirectSoundBuffer_GetStatus(mixbuf, &status);
//            status = mixbuf.GetStatus();
//            if ( (status&DSBSTATUS_BUFFERLOST) ) {
//                break;
//            }
//        }
//        if ( ! (status&DSBSTATUS_PLAYING) ) {
////            result = IDirectSoundBuffer_Play(mixbuf, 0, 0, DSBPLAY_LOOPING);
////            if ( result == DS_OK ) {
////                continue;
////            }
////#ifdef DEBUG_SOUND
////            setDSerror("DirectSound Play", result);
////#endif

//            mixbuf.Play(CONST_DSBPLAYFLAGS.DSBPLAY_LOOPING);
//            continue;
//        }

//        /* Find out where we are playing */
//        //result = IDirectSoundBuffer_GetCurrentPosition(mixbuf,
//        //                        &cursor, &junk);
//        //if ( result != DS_OK ) {
//        //    setDSerror("DirectSound GetCurrentPosition", result);
//        //    return;
//        //}
//        //cursor /= mixlen;
//        mixbuf.GetCurrentPosition(out cursors);
//        cursor = cursors.lPlay / mixlen;
//    }
//        }

//        public override void playAudio()
//        {
//            /* Unlock the buffer, allowing it to play */
//            if (locked_buf)
//            {
//                //IDirectSoundBuffer_Unlock(mixbuf, locked_buf, mixlen, NULL, 0);
//                //mixbuf.Unlock
//                mixbuf.
//            }
//        }

//        public override byte* getAudioBuf()
//        {
//            throw new Exception("The method or operation is not implemented.");
//        }

//        public override void waitDone()
//        {
//            throw new Exception("The method or operation is not implemented.");
//        }

//        public override void closeAudio()
//        {
//            throw new Exception("The method or operation is not implemented.");
//        }
//    }
//}
