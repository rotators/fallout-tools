unit SetChans;
// SetChannels

interface
uses Expander;
procedure SetChannels (exp: pSoundExpander; flag: integer);

implementation
procedure SetChannels (exp: pSoundExpander; flag: integer);
begin
   if flag=3 then
   begin
      with exp^. WaveFormatEx^ do
      begin
         nBlockAlign := wBitsPerSample shr 2;
         nChannels := 2; // типа, стерео
         nAvgBytesPerSec := MixRate * nBlockAlign;
      end;
   end;
end;

end.