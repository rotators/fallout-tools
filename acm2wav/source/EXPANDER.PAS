unit Expander;

interface
uses procs, Stream;

const WAVE_FORMAT_PCM = 1;
type PWaveFormatEx = ^TWaveFormatEx;
     TWaveFormatEx = record
       wFormatTag: Word;
       nChannels: Word;
       nSamplesPerSec: integer;
       nAvgBytesPerSec: integer;
       nBlockAlign: Word;
       wBitsPerSample: Word;
       cbSize: Word;
     end;

     pSoundExpander = ^ TSoundExpander;
     TSoundExpander = record
      OpenProc  : TOpenProc;
      CloseProc : Procedure;
      ReadProc  : TReadProc;
      WriteProc : Procedure;
      SeekProc  : Procedure;
      TellProc  : TFileLenProc;
      FileLengthProc: TFileLenProc;

      Memb_20: integer;
      FileReader: pFileReader;
      Memb_24, Memb_28: integer;
      Flags0: integer;
      Memb_30, Memb_34: integer;
      WaveFormatEx: pWaveFormatEx;
      Flags1: integer;
      Memb_40: integer;
      Flags2: integer;
      Memb_48: integer;
      Memb_4C: integer;
      loopPoint: integer;
      Memb_54,  Memb_58,  Memb_5C: integer;
      FileLength: integer;
      Memb_64: integer;
      ReadLimit: integer;
      Memb_68: integer;
      Memb_70, Memb_74: integer;
      SomeVar_6680EC,   SomeVar_6680E8: integer;
      Memb_80: integer;
      UnknownFlag_84: integer;
      SoundCallback: Procedure;
      Memb_8C, Memb_90: integer;
      Prev, Next: pSoundExpander;
   end;

var DecompObj: pSoundExpander = nil;
    MixRate: integer = 22050;
    Someconst_6680E8: integer = $10000;//$8000;
    Someconst_6680EC: integer = $10;//$18;
    LargeBuffer: pointer = nil;
    Buffer_Middle: pointer = nil;

    // 21.08.2003: Levcorp ACM autodetection
    levcorp_acm: boolean = false;
    first_filler: boolean = true;

    // 15.09.2003: RUS localization
    rus_acm: boolean = false;

implementation

end.
