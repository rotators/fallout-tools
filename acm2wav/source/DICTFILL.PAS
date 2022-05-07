unit DictFill;
interface
uses Stream;
function ReadAndDecompress (strm: pStreamObject; buf: pointer; count: integer):integer; stdcall;

implementation
uses Fillers1, Expander;

var FillDictionary:array [0..31] of TIntFunc=
                (ZeroFill,
                 Return0,
                 Return0,
                 LinearFill,
                 LinearFill,
                 LinearFill,
                 LinearFill,
                 LinearFill,
                 LinearFill,
                 LinearFill,
                 LinearFill,
                 LinearFill,
                 LinearFill,
                 LinearFill,
                 LinearFill,
                 LinearFill,
                 LinearFill,
                 Kinda_1Signum_3bits,
                 Kinda_1Signum_2bits,
                 Through_Table_5bits,
                 Kinda_2Signum_4bits,
                 Kinda_2Signum_3bits,
                 Through_Table2_7bits,
                 Kinda_3Signum_5bits,
                 Kinda_3Signum_4bits,
                 Return0,
                 Kinda_4Signum_5bits,
                 Kinda_4Signum_4bits,
                 Return0,
                 Through_Table3_7bits,
                 Return0,
                 Return0);


function CreateAmplitudeDictionary : integer;
var pwr, cnt, i: integer;
    ptr: pointer;
    val, v:word;
begin
   with strm^ do
   begin
      ReadBits (strm, 4);
         dec (Memb_1C, 4);
         pwr := Sig and $0F;
         Sig := Sig shr 4;
      ReadBits (strm, 16);
         dec (Memb_1C, 16);
         val := Sig and $FFFF;
         Sig := Sig shr 16;

      cnt := 1 shl pwr;

      ptr := Buffer_Middle;
      v := 0;
      for i:=0 to cnt-1 do
      begin
         word (ptr^) := v;
         inc (v, val);
         inc (integer(ptr), 2);
      end;

      ptr := Buffer_Middle;
      dec (integer(ptr), 2);
      v := -val;
      for i:=0 to cnt-1 do
      begin
         word (ptr^) := v;
         dec (integer(ptr), 2);
         dec (v, val);
      end;

      FillTables;
      for i:=0 to SomeSize-1 do
      begin
         ReadBits (strm, 5);
         dec (Memb_1C, 5);
         v := Sig and $1F;
         Sig := Sig shr 5;

         Ind := v;
         Pass := i;

         // 21.08.2003: Warn if ACM looks like Levcorp's (first filler is #31)
         // 15.09.2003: The same for RUS
         if first_filler then begin
         	first_filler := false;
         	if levcorp_acm then
         		FillDictionary[$1F] := @LevcorpFill;
		if rus_acm then
			FillDictionary[$1F] := @RUSFill;
         	if (v = $1F) and not (levcorp_acm or rus_acm) then begin
         		writeln ('Warning: this ACM is probably from some localization. Try -l or -r options.');
         	end;
         end;

         if FillDictionary[v]=0 then
         begin
            result := 0;
            exit;
         end;
      end;
   end;
   result := 1;
end;

procedure sub_4D3FCC (Par_EAX: pointer; Par_EBX, Par_ECX: integer; Par_EDX: pointer);
var FirstPg: pointer;
    var_18: integer;
    PgCnt: integer;
    var_10: integer; //PageSize
    var_C: integer;
    Dic, DecompBuff: pointer;
    eax,ebx,ecx,edx,esi,edi,ebp: integer;
begin
ebx := Par_EBX;
ecx := Par_ECX;
                DecompBuff := Par_EAX;
                Dic := Par_EDX;

                esi := Par_EBX*4;
                eax := integer(Dic)+Par_EBX*4;
                FirstPg := pointer(eax);

                if ecx = 2 then
                begin
                   esi := ebx;
                   while esi<>0 do
                   begin
                      edx := integer (Dic^);
                      ebx := smallInt(DecompBuff^) +
                             2* smallint(pointer(integer(DecompBuff)+2)^)+
                             integer (Dic^);
                      integer(Dic^) := ebx;

                      ebx := integer(pointer(eax)^);
                      ecx := edx*2 -
                             smallInt(pointer(integer(DecompBuff)+2)^);
                      dec (esi);
                      ecx := ecx - ebx;
                      integer(pointer(eax)^) := ecx;
                      inc (eax, 4);
                      inc (integer (Dic), 4);
                      integer (DecompBuff^) := (edx and $FFFF) + ((ebx and $FFFF) shl 16);
                      inc (integer(DecompBuff), 4);
                   end;
                end else if ecx = 4 then
                begin
                   var_10 := ebx;

                   esi := integer(FirstPg);
                   ebp := integer(Dic) + 12*ebx;
                   edi := integer(Dic) + 8*ebx;

                   while Var_10<>0 do
                   begin
                      eax := integer (Dic^);
                      edx := 2* smallint(pointer(integer(DecompBuff)+2)^)+
                             smallInt (DecompBuff^)+
                             integer (Dic^);
                      integer(Dic^) := edx;
                      ecx := integer(pointer(esi)^);
                      ebx := 2*eax -
                             smallInt(pointer(integer(DecompBuff)+2)^)-
                             integer(pointer(esi)^);
                      inc (ebp, 4);
                      integer (pointer(esi)^) := ebx;
                      edx := integer(pointer(edi)^);
                      eax := eax + ecx * 2 + integer(pointer(edi)^);
                      inc (edi, 4);
                      inc (esi, 4);
                      integer (pointer(edi-4)^) := eax;
                      ebx := integer (pointer(ebp-4)^);
                      ecx := edx*2 - ecx - ebx;
                      integer (pointer(ebp-4)^) := ecx;
                      integer (DecompBuff^) := (edx and $FFFF) + ((ebx and $FFFF) shl 16);
                      inc (integer(DecompBuff), 4);

                      dec (Var_10);
                      inc (integer(Dic), 4);
                   end;
                end else begin
                   ecx := ecx div 2;
                   Var_C := ebx;
                   PgCnt := ecx;
                   edi := esi;
                   Var_18 := esi;
                   while Var_C <> 0 do
                   begin
                      ebp := PgCnt;
                      eax := integer (Dic);

                      if PgCnt and 1<>0 then
                      begin
                         edx := integer (Dic^);
                         ecx := smallInt(pointer(integer(DecompBuff)+2)^);
                         eax := 2*smallInt(pointer(integer(DecompBuff)+2)^)+
                                smallInt (DecompBuff^)+
                                integer (Dic^);
                         ebx := integer(Dic);
                         integer (Dic^) := eax;
                         eax := ebx + Var_18;
                         ebx := integer (pointer(eax)^);
                         ecx := edx*2 - ecx - ebx;
                         integer (pointer(eax)^) := ecx;
                         inc (eax, Var_18);
                      end else begin
                         ebx := smallInt(pointer(integer(DecompBuff)+2)^);
                         edx := smallInt (DecompBuff^);
                      end;
                      ebp := ebp div 2;

                      while ebp<>0 do
                      begin
                         dec (ebp);

                         ecx := integer (pointer(eax)^);
                         edx := edx + 2*ebx + ecx;
                         integer (pointer(eax)^) := edx;
                         inc (eax, edi);
                         esi := integer (pointer(eax)^);
                         ebx := 2*ecx - ebx - esi;
                         integer (pointer(eax)^) := ebx;
                         inc (eax, edi);
                         edx := integer(pointer(eax)^);
                         ecx := ecx + 2*esi + edx;
                         integer (pointer(eax)^) := ecx;
                         inc (eax, edi);
                         ebx := integer(pointer(eax)^);
                         esi := 2*edx - esi - ebx;
                         integer (pointer(eax)^) := esi;
                         inc (eax, edi);
                      end;
                      integer (DecompBuff^) := (edx and $FFFF) + ((ebx and $FFFF) shl 16);
                      dec (Var_C);
                      inc (integer(DecompBuff), 4);
                      inc (integer(Dic), 4);
                   end;
                end;
end;

procedure sub_4D420C (Par_Eax: pointer; Par_EBX, Par_ECX: integer; Par_EDX: pointer);
var Pages: integer;
    LocSomeSize: integer;
    var_C: integer;
    Dic: pointer;
    DecBuffPos: pointer;
    eax,ebx,ecx,edx,esi,edi,ebp: integer;
begin
edx := integer(Par_EDX);
                DecBuffPos := Par_EAX;
                Dic := Par_EDX;
                ebp := 4*Par_EBX;

                if Par_ECX=4 then
                begin
                   LocSomeSize := Par_EBX;

                   ebp := 4*Par_EBX + edx;
                   edi := 12*Par_EBX + edx;
                   esi := 8*Par_EBX + edx;

                   while LocSomeSize <> 0 do
                   begin
                      edx := integer (Dic^);
                      ebx := 2*integer (pointer(integer(DecBuffPos)+4)^)+
                             integer (DecBuffPos^)+
                             integer (Dic^);
                      integer (Dic^) := ebx;
                      ecx := integer(pointer(ebp)^);
                      eax := edx*2 - integer (pointer(integer(DecBuffPos)+4)^)-
                             integer(pointer(ebp)^);
                      integer (pointer(ebp)^) := eax;
                      ebx := integer(pointer(esi)^);
                      edx := edx + 2*ecx + integer(pointer(esi)^);
                      inc (edi, 4);
                      integer (pointer(esi)^) := edx;
                      eax := integer (pointer(edi-4)^);
                      ecx := 2*ebx - ecx - integer (pointer(edi-4)^);
                      inc (esi, 4);
                      integer (pointer(edi-4)^) := ecx;
                      inc (ebp, 4);
                      integer (DecBuffPos^) := ebx;
                      integer (pointer(integer(DecBuffPos)+4)^) := eax;

                      inc (integer(Dic), 4);
                      dec (LocSomeSize);
                      inc (integer(DecBuffPos), 8);
                   end;
                end else begin
                   Var_C := Par_EBX;
                   Pages := Par_ECX div 4;

                   edi := ebp;
                   while Var_C <> 0 do
                   begin
                      ebp := Pages;
                      eax := integer (Dic);
                      edx := integer (DecBuffPos^);
                      ebx := integer(pointer(integer(DecBuffPos)+4)^);
                      while ebp<>0 do
                      begin
                         dec (ebp);
                         esi := integer (pointer(eax)^);
                         edx := edx + 2*ebx + integer (pointer(eax)^);
                         integer (pointer(eax)^) := edx;
                         inc (eax, edi);
                         ecx := integer (pointer(eax)^);
                         ebx := 2*esi - ebx - integer (pointer(eax)^);
                         integer (pointer(eax)^) := ebx;
                         inc (eax, edi);
                         edx := integer (pointer(eax)^);
                         esi := esi + 2* ecx + integer (pointer(eax)^);
                         integer (pointer(eax)^) := esi;
                         inc (eax, edi);
                         ebx := integer (pointer(eax)^);
                         ecx := 2*edx - ecx - integer (pointer(eax)^);
                         integer (pointer(eax)^) := ecx;
                         inc (eax, edi);
                      end;

                      integer (DecBuffPos^) := edx;
                      integer(pointer(integer (DecBuffPos) + 4)^) := ebx;
                      dec (Var_C);
                      inc (integer (DecBuffPos), 8);
                      inc (integer (Dic), 4);
                   end;
                end;
end;

procedure Sub_4D4370 ;
var Pages: integer;
    Dic, ebp: pointer;
    edi, esi: integer;
    i: integer;
    ptr: pointer;
begin
   with Strm^ do
   begin
      if PackAttrs=0 then exit;
      Pages := PackAttrs2;
      Dic := SomeBuff;

      while Pages >0 do
      begin
         edi := Blocks;
         esi := SomeSize div 2;
         ebp := DecompBuffer;

         if edi > Pages then
            edi := Pages;

         edi := 2*edi;

         Sub_4D3FCC (ebp, esi, edi, Dic);

         inc (integer(ebp), esi*4);
         ptr := Dic;
         for i:=0 to edi-1 do
         begin
            inc(integer(ptr^));
            inc(integer(ptr), esi*4);
         end;

         repeat
            esi := esi div 2;
            edi := 2*edi;
            if esi<>0 then
            begin
               Sub_4D420C (ebp, esi, edi, Dic);
               inc(integer(ebp),esi*8);
            end;
         until esi=0;

         dec(Pages, Blocks);
         inc(integer(Dic), TotalBlocksSize*4);
      end;
   end;
end;

function ReadAndDecompress (strm: pStreamObject; buf: pointer; count: integer):integer; stdcall;
var esi: pointer;
    ebx: integer;
begin
   result := 0;
   esi := strm^. Memb_4C;
   ebx := strm^. Memb_50;

   Fillers1. strm := strm;

   while result < Count do
   with strm^ do
   begin
      if ebx=0 then
      begin
         if DataSize = 0 then break;
// 21.08.2003: Levcorp's filler exits with zero, but we needn't exit.
// 15.09.2003: Same for RUS
         if CreateAmplitudeDictionary = 0 then begin
         	if not (levcorp_acm or rus_acm) then
         		break;
         end else
			Sub_4D4370;

         Memb_4C := SomeBuff;
         Memb_50 := SomeSize2;
         DataSize := DataSize - SomeSize2;

         if DataSize < 0 then
         begin
            Memb_50 := DataSize + Memb_50;
            DataSize := ebx;
         end;

         esi := Memb_4C;
         ebx := Memb_50;
      end;

      word(buf^) := (integer(esi^) shr PackAttrs);
      inc (integer(esi), 4);
      inc (integer(buf), 2);
      dec (ebx);
      inc (result, 2);
   end;

   strm^. Memb_4C := esi;
   strm^. Memb_50 := ebx;
end;

end.