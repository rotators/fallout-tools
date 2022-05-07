unit LoadSnd;
// Corresponds to LoadBackSound_Probably func of FallDisasm

interface

procedure LoadSound (Name: PChar; ParEBX, ParECX, ParEDX:integer);
// purpose of Pars is not completely understood, kinda magic

type TRiffHeader = record
        RiffSig: array [1..4] of char;
        TotalLenM8: integer;
        WaveSig: array [1..8] of char;
        FormatexLen: integer;
        wFormatTag, nChannels: word;
        nSamplesPerSec, nAvgBytesPerSec: integer;
        nBlockAlign, wBitsPerSample: word;
        DataSig: array [1..4] of char;
        RawDataLen: integer;
     end;
var RiffHeader: TRiffHeader=(
      RiffSig: 'RIFF';
      WaveSig: 'WAVEfmt ';
      FormatExLen: 16;
      wFormatTag: 1;
      nChannels: 2;
      nSamplesPerSec: 22050;
      nAvgBytesPerSec: 22050*4;
      nBlockAlign: 4;
      wBitsPerSample: 16;
      DataSig: 'data');
    inFile, resFile, noPath: string;
    resLen: integer = 0;
    noPathLen: integer;

implementation
uses Stream, Expander, Allocate, Procs, SetChans, SysUtils;

type TListInfo = packed record
        ListSig: array [1..4] of char;
        LenM8: integer;
        InfoSig: array [1..4] of char;
        ISFT: array [1..4] of char;
        SFTLen: integer;
        SoftName: array [1..48] of char;
//        ICOP: array [1..4] of char;
//        COPLen: integer;
//        Copyright: array [1..50] of char;
        INAM: array [1..4] of char;
        NAMLen: integer;
     end;

var ListInfo: TListInfo = (
      ListSig: 'LIST';
      InfoSig: 'INFO';
      ISFT: 'ISFT';
      SFTLen: 48;
      SoftName: 'Interplay ACM to WAV Converter by ABel [TeamX]'#0#0;
//      ICOP: 'ICOP';
//      COPLen: 50;
//      Copyright: 'No Copyright. Feel free to distribute this file.'#0#0;
      INAM: 'INAM';
    );



procedure AfterOpenDecomp (dec: pSoundExpander);
var btr, res: integer;
    buff: pointer;
    f:file;
begin
   with dec^ do
   begin
      FileLength := TFileLenProc(FileLengthProc) (FileReader);
      btr := FileLength;
      if (2 and Flags2<>0) then
      begin
         if $20 and Flags2=0 then
            Flags2 := Flags2 or $120;
         if Somevar_6680EC*Somevar_6680E8 < btr then
            btr := Somevar_6680EC*Somevar_6680E8
         else
            if FileLength mod Somevar_6680E8 <> 0 then
               btr := ((FileLength div SomeVar_6680E8) + 1) * Somevar_6680E8;
      end else
         Flags2 := (Flags2 and $FC) or 1;

      getmem (buff, btr);
      assignFile (f,resFile); rewrite (f,1);

      RiffHeader. RawDataLen := FileLength;
//      noPathLen := ((length(noPath)-1) div 2 + 1) * 2;
//        // Making nameLen an even number.
// 10.01.2001 Must be on the DWord boundary
      noPathLen := ((length(noPath)-1) div 4 + 1) * 4;
      ListInfo. NAMLen := noPathLen;
      RiffHeader. TotalLenM8 := FileLength + sizeOf (RiffHeader)-8 +
         SizeOf (ListInfo) + ListInfo.NAMLen;
      ListInfo. LenM8 := SizeOf (ListInfo) + ListInfo.NAMLen - 8;
      blockwrite (f, RiffHeader, sizeof(RiffHeader));

      res:=1;
      while res<>0 do
      begin
         res := TReadProc(ReadProc) (FileReader, buff, btr);
{         if res<>btr then
         begin
            if (flags1 and $20<>0) and (flags1 and $100=0) then
            begin
               integer(buff)+res
               if res<btr then
                  // xere;
            end else
               fillchar (pointer(integer(buff)+res)^, btr-res, #0);
         end else begin

         end;}

         if res<>0 then
         begin
            blockwrite (f, buff^, res);
            write ('#');
            inc (resLen, res);
         end;
      end;
      writeln;

      noPath := noPath + #0#0#0;
      blockwrite (f, ListInfo, SizeOf (ListInfo));
      blockwrite (f, noPath[1], noPathLen);

      closeFile (f);
   end;

// !!! This function is incomplete. If we finish it then we will do all the dirty work
end;

procedure SoundLoad (Dec: pSoundExpander; name: PChar);
begin
   Dec^. FileReader := TOpenPRoc(Dec^.OpenProc) (Name, $200);
   AfterOpenDecomp (Dec);
end;

procedure LoadSound (Name: PChar; ParEBX, ParECX, ParEDX:integer);
begin
   DecompObj := AllocateSound ( ParECX, ParEBX );
   with DecompObj^ do
   begin
      OpenProc := @OpenDecompressor;
      CloseProc:= nil;//@CloseDecompressor;
      ReadProc := @ReadDecompressor;
      WriteProc:= nil;//@WriteDecompressor;
      SeekProc := nil;//@SeekDecompressor;
      TellProc := nil;//@RetrunM1;
      FileLengthProc := @FileLenDecompressor;
   end;

   SetChannels (DecompObj, 3);

// not implemented, because нафиг надо
//   SetBacksndLooping;
//   SoundCallbackSet;
//   SetReadLimit;

   SoundLoad (DecompObj, Name);

//   SetReadLimit;
//   StartToPlay;
end;

end.