{$AppType Console}
// Interplay Fallout ACM to WAV converter
program Acm2Wav;
uses LoadSnd, SysUtils, Expander;

{$R ver.res}

var n:string;
	need_to_write_mono_header: boolean = false;

begin
   FileMode := 0;
   writeln ('Interplay ACM to WAV converter v1.32, (c) 1999-2003 ABel [TeamX]');
   try
      if paramCount=0 then
      begin
         writeln;
         writeln (' Usage: ACM2WAV acm_file_name [-m|-l]');
         writeln ('    -m    produce mono WAV (effects and speech are mono files)');
         writeln ('    -l    convert sound effects from Levcorp localization');
         writeln ('    -r    convert sound effects from RUS localization')
      end else begin
         inFile := paramstr (1);
         noPath := ExtractFileName (inFile);
         resFile := ChangeFileExt (noPath,'.wav');
         write (' Converting ',inFile,' to ',resFile);
         if UpperCase(ParamStr(2))='-M' then
         begin
            writeln (' (mono)');
            need_to_write_mono_header := true;
         end else if UpperCase(ParamStr(2))='-L' then begin
            writeln (' (Levcorp, mono)');
            need_to_write_mono_header := true;
            levcorp_acm := true;
         end else if UpperCase(ParamStr(2))='-R' then begin
            writeln (' (RUS, mono)');
            need_to_write_mono_header := true;
            rus_acm := true;
         end else
            writeln;

         if need_to_write_mono_header then begin
            with RiffHeader do begin
               nChannels := 1;
               nBlockAlign := 2;
               nAvgBytesPerSec := 22050*2;
            end;         	
         end;

         n:=inFile+#0;
         LoadSound (@n[1], $e, $10, $c);
         writeln (resLen,' bytes written');
      end;
   except
      on e:Exception do
      begin
         writeln;
         writeln ('Error: ', e.Message);
      end;
   end;
end.