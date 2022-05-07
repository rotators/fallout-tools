unit Fillers1;
interface
uses Stream;
procedure FillTables;

type TIntFunc = function : integer;

function ZeroFill: integer;
function Return0: integer;
function LinearFill: integer;
function Kinda_1Signum_3bits: integer;
function Kinda_1Signum_2bits:integer;
function Through_Table_5bits: integer;
function Kinda_2Signum_4bits :integer;
function Kinda_2Signum_3bits :integer;
function Through_Table2_7bits :integer;
function Kinda_3Signum_5bits:integer;
function Kinda_3Signum_4bits :integer;
function Kinda_4Signum_5bits :integer;
function Kinda_4Signum_4bits :integer;
function Through_Table3_7bits: integer;

// Извраты идут тут
function LevcorpFill: integer;
function RUSFill: integer;

var strm: pStreamObject; //EAX
    ind: integer; // EBX
    pass: integer; // EDX

implementation
uses Expander, SysUtils;
var Table3: array [0..127] of byte;
    Table1: array [0..31] of byte;
    Table2: array [0..127] of word;
    TableFilled: integer = 0;

procedure FillTables;
var i,j,k: integer;
begin
   if TableFilled=1 then exit;

   for i:=0 to 2 do
      for j:=0 to 2 do
         for k:=0 to 2 do
            Table1 [i + 3*j + k*9] := i + 4*j + 16*k;
   for i:=0 to 4 do
      for j:=0 to 4 do
         for k:=0 to 4 do
             Table2 [i + 5*j + 25*k] := 
                i + 8*j + 64*k;
   for i:=0 to 10 do
      for j:=0 to 10 do
         Table3 [i + 11*j] := i + 16*j;
   TableFilled := 1;
end;

function Return0: integer;
begin
   result := 0;
end;


function ZeroFill: integer;
var i: integer;
    ptr: pointer;
begin
// 3c = 24 * 38 = SomeSize * 
   with strm^ do
   begin
      ptr := pointer(integer(SomeBuff)+Pass*4);
      for i:=0 to PackAttrs2-1 do
      begin
         integer(ptr^) := 0;
         inc (integer(ptr), SomeSize*4);
      end;
   end;
   result := 1;
end;

// 21.08.2003: Levcorp replaced 31st filler with its own. Their filler takes plain values from ACM.
function LevcorpFill: integer;
var i: integer;
    ptr: pointer;
begin
   with strm^ do
   begin
      ptr := pointer(integer(SomeBuff)+Pass*4);
      for i:=0 to SomeSize2-1 do
      begin
         ReadBits (strm, 16);
           dec(Memb_1C, 16);
           integer(ptr^) := (Sig and $FFFF) shl PackAttrs;
           Sig := Sig shr 16;
         inc (integer(ptr), 4);
      end;
   end;
   result := 0;
end;

// 15.09.2003: RUS localization uses own filler. Plain 8bit values.
function RUSFill: integer;
var i: integer;
    ptr: pointer;
begin
   with strm^ do
   begin
      ptr := SomeBuff;
      for i:=0 to SomeSize2-1 do
      begin
         ReadBits (strm, 8);
           dec(Memb_1C, 8);
           integer(ptr^) := (Sig and $FF) shl (PackAttrs+8);
           Sig := Sig shr 8;
         inc (integer(ptr), 4);
      end;
   end;
   result := 0;
end;

function LinearFill;
var ampPtr, ptr, tmpPtr: pointer;
    mask, i: integer;
begin
   with strm^ do
   begin
      ampPtr := pointer(integer(Buffer_Middle) + 2*( (-1) shl (Ind-1) ));
      ptr := pointer(integer(SomeBuff)+Pass*4);
      mask := (1 shl Ind) - 1;

      for i:=0 to PackAttrs2-1 do
      begin
         ReadBits (strm, Ind);
           dec(Memb_1C, Ind);
           tmpPtr := pointer(integer(ampPtr) + (Sig and Mask)*2 );
           integer(ptr^) := smallInt (tmpPtr^);
           Sig := Sig shr Ind;
         inc (integer(ptr), 4*SomeSize);
      end;
   end;
   result := 1;
end;

function Kinda_1Signum_3bits: integer;
var ampPtr, ptr: pointer;
    res:smallInt;
    i: integer;
begin
   with strm^ do
   begin
      ampPtr := Buffer_Middle;
      ptr := pointer(integer(SomeBuff)+Pass*4);

      i:=0;
      while i<PackAttrs2 do
      begin
         ReadBits (strm, 3);
         res := Sig;
         if res and 1=0 then
         begin
            dec(Memb_1C, 1);
            Sig := Sig shr 1;
            integer (ptr^) := 0;
             inc (integer(ptr), 4*SomeSize);
            inc (i);
            if i=PackAttrs2 then break;
            integer (ptr^) := 0;
             inc (integer(ptr), 4*SomeSize);
            inc (i);
         end else
         if res and 2=0 then
         begin
            dec(Memb_1C, 2);
            Sig := Sig shr 2;
            integer (ptr^) := 0;
            inc (integer(ptr), 4*SomeSize);
            inc (i);
         end else
         begin
            dec(Memb_1C, 3);
            Sig := Sig shr 3;
            if res and 4<>0 then
               res := smallInt (pointer(integer(ampPtr) + 2)^)
            else
               res := smallInt (pointer(integer(ampPtr) - 2)^);
            integer (ptr^) := res;
            inc (integer(ptr), 4*SomeSize);
            inc (i);
         end;
      end;
   end;
   result := 1;
end;

function Kinda_1Signum_2bits:integer;
var ampPtr, ptr: pointer;
    res: smallInt;
    i: integer;
begin
   with strm^ do
   begin
      ampPtr := Buffer_Middle;
      ptr := pointer(integer(SomeBuff)+Pass*4);

      for i:=0 to PackAttrs2-1 do
      begin
         ReadBits (strm, 2);
         res := Sig;
         if res and 1=0 then
         begin
            dec(Memb_1C, 1);
            Sig := Sig shr 1;
            integer (ptr^) := 0;
            inc (integer(ptr), 4*SomeSize);
         end else
         begin
            dec(Memb_1C, 2);
            Sig := Sig shr 2;
            if res and 2<>0 then
               res := smallInt (pointer(integer(ampPtr) + 2)^)
            else
               res := smallInt (pointer(integer(ampPtr) - 2)^);
            integer (ptr^) := res;
            inc (integer(ptr), 4*SomeSize);
         end;
      end;
   end;
   result := 1;
end;

function Kinda_2Signum_4bits :integer;
var ampPtr, ptr: pointer;
    res: smallInt;
    i: integer;
begin
   with strm^ do
   begin
      ampPtr := Buffer_Middle;
      ptr := pointer(integer(SomeBuff)+Pass*4);

      i:=0;
      while i<PackAttrs2 do
      begin
         ReadBits (strm, 4);
         res := Sig;
         if res and 1=0 then
         begin
            dec(Memb_1C, 1);
            Sig := Sig shr 1;
            integer (ptr^) := 0;
             inc (integer(ptr), 4*SomeSize);
            inc (i);
            if i=PackAttrs2 then break;
            integer (ptr^) := 0;
             inc (integer(ptr), 4*SomeSize);
            inc (i);
         end else
         if res and 2=0 then
         begin
            dec(Memb_1C, 2);
            Sig := Sig shr 2;
            integer (ptr^) := 0;
            inc (integer(ptr), 4*SomeSize);
            inc (i);
         end else
         begin
            dec(Memb_1C, 4);
            Sig := Sig shr 4;
            if res and 8<>0 then
               if res and 4<>0 then
                  res := smallInt (pointer(integer(ampPtr) + 4)^)
               else
                  res := smallInt (pointer(integer(ampPtr) + 2)^)
            else
               if res and 4<>0 then
                  res := smallInt (pointer(integer(ampPtr) - 2)^)
               else
                  res := smallInt (pointer(integer(ampPtr) - 4)^);
            integer (ptr^) := res;
            inc (integer(ptr), 4*SomeSize);
            inc (i);
         end;
      end;
   end;
   result := 1;
end;

function Kinda_2Signum_3bits :integer;
var ampPtr, ptr: pointer;
    res: smallInt;
    i: integer;
begin
   with strm^ do
   begin
      ampPtr := Buffer_Middle;
      ptr := pointer(integer(SomeBuff)+Pass*4);

      for i:=0 to PackAttrs2-1 do
      begin
         ReadBits (strm, 3);
         res := Sig;
         if res and 1=0 then
         begin
            dec(Memb_1C, 1);
            Sig := Sig shr 1;
            integer (ptr^) := 0;
             inc (integer(ptr), 4*SomeSize);
         end else
         begin
            dec(Memb_1C, 3);
            Sig := Sig shr 3;
            if res and 4<>0 then
               if res and 2<>0 then
                  res := smallInt (pointer(integer(ampPtr) + 4)^)
               else
                  res := smallInt (pointer(integer(ampPtr) + 2)^)
            else
               if res and 2<>0 then
                  res := smallInt (pointer(integer(ampPtr) - 2)^)
               else
                  res := smallInt (pointer(integer(ampPtr) - 4)^);
            integer (ptr^) := res;
            inc (integer(ptr), 4*SomeSize);
         end;
      end;
   end;
   result := 1;
end;

function Kinda_3Signum_5bits:integer;
var ampPtr, ptr: pointer;
    res: smallInt;
    i: integer;
begin
   with strm^ do
   begin
      ampPtr := Buffer_Middle;
      ptr := pointer(integer(SomeBuff)+Pass*4);

      i:=0;
      while i<PackAttrs2 do
      begin
         ReadBits (strm, 5);
         res := Sig;
         if res and 1=0 then
         begin
            dec(Memb_1C, 1);
            Sig := Sig shr 1;
            integer (ptr^) := 0;
             inc (integer(ptr), 4*SomeSize);
            inc (i);
            if i=PackAttrs2 then break;
            integer (ptr^) := 0;
             inc (integer(ptr), 4*SomeSize);
            inc (i);
         end else
         if res and 2=0 then
         begin
            dec(Memb_1C, 2);
            Sig := Sig shr 2;
            integer (ptr^) := 0;
            inc (integer(ptr), 4*SomeSize);
            inc (i);
         end else
         if res and 4=0 then
         begin
            dec(Memb_1C, 4);
            Sig := Sig shr 4;
            if res and 8<>0 then
               res := smallInt (pointer(integer(ampPtr) + 2)^)
            else
               res := smallInt (pointer(integer(ampPtr) - 2)^);
            integer (ptr^) := res;
            inc (integer(ptr), 4*SomeSize);
            inc (i);
         end else
         begin
            dec (Memb_1C, 5);
            Sig := Sig shr 5;
            res := (res shr 3) and 3;
            if res>=2 then res:=res+3;
            res:=res*2;
            res := smallInt (pointer(integer(ampPtr) -6 + res)^);

            integer (ptr^) := res;
            inc (integer(ptr), 4*SomeSize);
            inc (i);
         end;
      end;
   end;
   result := 1;
end;

function Kinda_3Signum_4bits :integer;
var ampPtr, ptr: pointer;
    res: smallInt;
    i: integer;
begin
   with strm^ do
   begin
      ampPtr := Buffer_Middle;
      ptr := pointer(integer(SomeBuff)+Pass*4);

      for i:=0 to PackAttrs2-1 do
      begin
         ReadBits (strm, 4);
         res := Sig;
         if res and 1=0 then
         begin
            dec(Memb_1C, 1);
            Sig := Sig shr 1;
            integer (ptr^) := 0;
             inc (integer(ptr), 4*SomeSize);
         end else
         if res and 2=0 then
         begin
            dec(Memb_1C, 3);
            Sig := Sig shr 3;
            if res and 4<>0 then
               res := smallInt (pointer(integer(ampPtr) + 2)^)
            else
               res := smallInt (pointer(integer(ampPtr) - 2)^);
            integer (ptr^) := res;
            inc (integer(ptr), 4*SomeSize);
         end else
         begin
            dec (Memb_1C, 4);
            Sig := Sig shr 4;
            res := (res shr 2) and 3;
            if res>=2 then res:=res+3;
            res:=res*2;
            res := smallInt (pointer(integer(ampPtr) -6 + res)^);

            integer (ptr^) := res;
            inc (integer(ptr), 4*SomeSize);
         end;
      end;
   end;
   result := 1;
end;

function Kinda_4Signum_5bits :integer;
var ampPtr, ptr, tmpPtr: pointer;
    res: smallInt;
    i: integer;
begin
   with strm^ do
   begin
      ampPtr := Buffer_Middle;
      ptr := pointer(integer(SomeBuff)+Pass*4);

      i:=0;
      while i<PackAttrs2 do
      begin
         ReadBits (strm, 5);
         res := Sig;
         if res and 1=0 then
         begin
            dec(Memb_1C, 1);
            Sig := Sig shr 1;
            integer (ptr^) := 0;
             inc (integer(ptr), 4*SomeSize);
            inc (i);
            if i=PackAttrs2 then break;
            integer (ptr^) := 0;
             inc (integer(ptr), 4*SomeSize);
            inc (i);
         end else
         if res and 2=0 then
         begin
            dec(Memb_1C, 2);
            Sig := Sig shr 2;
            integer (ptr^) := 0;
            inc (integer(ptr), 4*SomeSize);
            inc (i);
         end else
         begin
            dec (Memb_1C, 5);
            Sig := Sig shr 5;
            res := (res shr 2) and 7;
            if res>=4 then res:=res+1;
            res:=res*2;
            tmpPtr :=pointer(integer(ampPtr) -8 + res);
            res := smallInt (tmpPtr^);

            integer (ptr^) := res;
            inc (integer(ptr), 4*SomeSize);
            inc (i);
         end;
      end;
   end;
   result := 1;
end;

function Kinda_4Signum_4bits :integer;
var ampPtr, ptr: pointer;
    res: smallInt;
    i: integer;
begin
   with strm^ do
   begin
      ampPtr := Buffer_Middle;
      ptr := pointer(integer(SomeBuff)+Pass*4);

      for i:=0 to PackAttrs2-1 do
      begin
         ReadBits (strm, 4);
         res := Sig;
         if res and 1=0 then
         begin
            dec(Memb_1C, 1);
            Sig := Sig shr 1;
            integer (ptr^) := 0;
             inc (integer(ptr), 4*SomeSize);
         end else
         begin
            dec (Memb_1C, 4);
            Sig := Sig shr 4;
            res := (res shr 1) and 7;
            if res>=4 then res:=res+1;
            res:=res*2;
            res := smallInt (pointer(integer(ampPtr) -8 + res)^);

            integer (ptr^) := res;
            inc (integer(ptr), 4*SomeSize);
         end;
      end;
   end;
   result := 1;
end;

function Through_Table_5bits: integer;
var ampPtr, ptr: pointer;
    mask, i: integer;
    res: smallInt;
    a: integer;
begin
   with strm^ do
   begin
      ampPtr := Buffer_Middle;
      ptr := pointer(integer(SomeBuff)+Pass*4);

      i:=0;
      while i<PackAttrs2 do
      begin
         ReadBits (strm, 5);
           dec(Memb_1C, 5);
           res := Sig and $1F;
           Sig := Sig shr 5;
         a := Table1 [res];

         res := 2*(a and 3);
         res := smallInt (pointer(integer(ampPtr) -2 + res)^);
         integer (ptr^) := res;
         inc (integer(ptr), 4*SomeSize);
         inc (i); if i=PackAttrs2 then break;

         a := a shr 2;
         res := 2*(a and 3);
         res := smallInt (pointer(integer(ampPtr) -2 + res)^);
         integer (ptr^) := res;
         inc (integer(ptr), 4*SomeSize);
         inc (i); if i=PackAttrs2 then break;

         a := a shr 2;
         res := 2*(a and 3);
         res := smallInt (pointer(integer(ampPtr) -2 + res)^);
         integer (ptr^) := res;
         inc (integer(ptr), 4*SomeSize);
         inc (i);
      end;
   end;
   result := 1;
end;

function Through_Table2_7bits :integer;
var ampPtr, ptr, tmpPtr: pointer;
    mask, i: integer;
    res: smallInt;
    a: integer;
begin
   with strm^ do
   begin
      ampPtr := Buffer_Middle;
      ptr := pointer(integer(SomeBuff)+Pass*4);

      i:=0;
      while i<PackAttrs2 do
      begin
         ReadBits (strm, 7);
           dec(Memb_1C, 7);
           res := Sig and $7F;
           Sig := Sig shr 7;
         a := Table2 [res];

         res := 2*(a and 7);
         tmpPtr :=pointer(integer(ampPtr) -4 + res);
         res := smallInt (tmpPtr^);
         integer (ptr^) := res;
         inc (integer(ptr), 4*SomeSize);
         inc (i); if i=PackAttrs2 then break;

         a := a shr 3;
         res := 2*(a and 7);
         res := smallInt (pointer(integer(ampPtr) -4 + res)^);
         integer (ptr^) := res;
         inc (integer(ptr), 4*SomeSize);
         inc (i); if i=PackAttrs2 then break;

         a := a shr 3;
         res := 2*(a and 7);
         res := smallInt (pointer(integer(ampPtr) -4 + res)^);
         integer (ptr^) := res;
         inc (integer(ptr), 4*SomeSize);
         inc (i);
      end;
   end;
   result := 1;
end;

function Through_Table3_7bits :integer;
var ampPtr, ptr: pointer;
    mask, i: integer;
    res: integer;
    a: integer;
begin
   with strm^ do
   begin
      ampPtr := Buffer_Middle;
      ptr := pointer(integer(SomeBuff)+Pass*4);

      i:=0;
      while i<PackAttrs2 do
      begin
         ReadBits (strm, 7);
           dec(Memb_1C, 7);
           res := Sig and $7F;
           Sig := Sig shr 7;
         a := Table3 [res];

         res := 2*(a and $0F);
         res := smallInt (pointer(integer(ampPtr) -10 + res)^);
         integer (ptr^) := res;
         inc (integer(ptr), 4*SomeSize);
         inc (i); if i=PackAttrs2 then break;

         a := a shr 4;
         res := 2*(a and $0F);
         res := smallInt (pointer(integer(ampPtr) -10 + res)^);
         integer (ptr^) := res;
         inc (integer(ptr), 4*SomeSize);
         inc (i);
      end;
   end;
   result := 1;
end;

end.
