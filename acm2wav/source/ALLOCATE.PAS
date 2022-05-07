unit Allocate;
// AllocateSound
// AllocateExpander

interface
uses Expander;
function AllocateSound(ParECX, ParEBX: integer):pSoundExpander;

implementation

function AllocateExpander (ParEAX, ParEDX: integer): pSoundExpander;
var tmpPtr: pSoundExpander;
    p: pSoundExpander;
begin
   new (tmpPtr);
   fillchar (tmpPtr^, sizeof(tmpPtr^), #0);
   new (tmpPtr^.WaveFormatEx);
   fillchar (tmpPtr^.WaveFormatEx^, $12, #0);

   with tmpPtr^ do
   begin
      with WaveFormatEx^ do
      begin
         wFormatTag := WAVE_FORMAT_PCM;
         nChannels := 1;

         if (8 and ParEDX)<>0 then
            wBitsPerSample := 16
         else
            wBitsPerSample := 8;

         nSamplesPerSec := MixRate;
         nBlockAlign := (nChannels * wBitsPerSample) shr 3;
         cbSize := 0;
         nAvgBytesPerSec := nSamplesPerSec * nBlockAlign;
      end;

      ParEDX := ParEDX or 2;

      Flags1 := ParEDX;
      Flags2 := ParEAX;

      Somevar_6680E8 := SomeConst_6680E8;
      Somevar_6680EC := SomeConst_6680EC;
      ReadLimit := SomeConst_6680E8 * SomeConst_6680EC;
      Memb_64 :=0;
      Memb_24 :=0;
      Memb_40 :=0;
      Memb_28 := $14;
      Flags0 := $10000;

      if 2 and ParEDX<>0 then
         Flags0 := Flags0 or $80;
      if 4 and ParEDX<>0 then
         Flags0 := Flags0 or $40;
      if $40 and ParEDX<>0 then
         Flags0 := Flags0 or $20;

      if $10 and ParEAX<>0 then
      begin
         Flags1 := FLags1 or $20;
         loopPoint := -1;
      end else
         loopPoint := 0;

      Memb_58 := -1;
      Memb_5C := 1;
      Memb_4C := $7FFF;
      Memb_54 := 0;

      Prev := nil;
      Next := nil;
   end;

   result := tmpPtr;
end;

function AllocateSound(ParECX, ParEBX: integer):pSoundExpander;
var eax, edx:integer;
begin
   edx := $0A;

   case ParEBX of
     $0D: eax := 1;
     $0E: eax := 2;
     else eax:= 0;
   end;

   case ParECX of
     $0F: eax := eax or 4;
     $10: edx := edx or $20;
   end;

   result := AllocateExpander (eax, edx);
end;

end.