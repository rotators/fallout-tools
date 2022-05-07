unit Stream;

interface
type pFile = ^File;
   pStreamObject = ^TStreamObject;
   TFileReader = record
      Flags: integer; //  1=Present, 2=Compressed
      FileVar: pFile;
      FileDecompressor: pStreamObject;
      FileLen: integer;
      FileSig2, FileSig1: integer;
      BytesInReadBuff:integer; // Change this name to FilePos, you have to.
   end;
   pFileReader = ^ TfileReader;

   TStreamObject = record
      readProc: pointer;
      FileVar: pfile;
      BufferPtr: pointer;
      BufferSize: integer;
      BufferPos:pointer;
      AvailBytes, Sig, Memb_1C: integer;
      PackAttrs, SomeSize, PackAttrs2, SomeSize2: integer;
      DecompBuffer, SomeBuff: pointer;
      Blocks, TotalBlocksSize: integer;
      SomeFileSig1, SomeFileSig2, DataSize: integer;
      Memb_4C: pointer;
      Memb_50: integer;
   end;


function CreateFileDecomp (f: pfile; var FileSig1, FileSig2, FileLen: integer): pStreamObject;
procedure ReadBits (obj: pStreamObject; bits: integer);
function ReadNextPortion (obj: pStreamObject): byte; register;

implementation
uses SysUtils, Expander, DictFill;

procedure SetStreamObj (obj: pStreamObject; f:pFile);
begin
   with obj^ do
   begin
      FileVar := f;
      getMem (BufferPtr, $200);
      BufferSize := $200;
      AvailBytes := 0;
   end;
end;

function ReadNextPortion (obj: pStreamObject): byte; register;
var ptr:pointer;
begin
   with obj^ do
   begin
      blockread (FileVar^, BufferPtr^, BufferSize, AvailBytes);
      if AvailBytes = 0 then
      begin
         fillChar (BufferPtr^, BufferSize, #0);
         AvailBytes := BufferSize;
      end;
      BufferPos := BufferPtr;
      dec (AvailBytes);
      ptr := BufferPos;
      BufferPos := pointer (integer(ptr)+1);
      result := byte (ptr^);
   end;
end;

procedure ReadBits (obj: pStreamObject; bits: integer);
var one: integer;
    ptr: pointer;
begin
   with obj^ do
      while Memb_1C < bits do
      begin
         dec (AvailBytes);
         if AvailBytes>=0 then
         begin
            one := byte (BufferPos^);
            inc(integer(BufferPos));
         end
         else
            one := ReadNextPortion (obj);
         one := (one and $FF) shl Memb_1C;
         Sig := Sig or one;
         inc (Memb_1C, 8);
      end;
end;

function CreateFileDecomp (f: pfile; var FileSig1, FileSig2, FileLen: integer): pStreamObject;
var i: integer;
begin
   new (result);
   fillChar (result^, sizeOf(Result), #0);
   SetStreamObj (result, f);

   with result^ do
   begin
      Sig := 0;  Memb_1C := 0;
      ReadBits (result, 24);
        dec (Memb_1C, 24);
        if Sig and $FFFFFF <> $032897 then
           raise Exception.Create ('File does not appear to be a valid ACM file');
        Sig := Sig shr 24;
      ReadBits (result, 8);
        dec (Memb_1C, 8);
        if Sig and $FF <> 1 then
           raise Exception.Create ('File does not appear to be a valid ACM file');
        Sig := Sig shr 8;
      ReadBits (result, 16);
        DataSize := Sig and $FFFF;
        dec (Memb_1C, 16);
        Sig := Sig shr 16;
      ReadBits (result, 16);
        dec (Memb_1C, 16);
        DataSize := DataSize + ((Sig and $FFFF) shl 16);
        Sig := Sig shr 16;
      ReadBits (result, 16);
        dec (Memb_1C, 16);
        SomeFileSig1 := Sig and $FFFF;
        Sig := Sig shr 16;
      ReadBits (result, 16);
        dec (Memb_1C, 16);
        SomeFileSig2 := Sig and $FFFF;
        Sig := Sig shr 16;

      ReadBits (result, 4);
        dec (Memb_1C, 4);
        PackAttrs := Sig and $0F;
        Sig := Sig shr 4;
      ReadBits (result, 12);
        dec (Memb_1C, 12);
        PackAttrs2 := Sig and $0FFF;
        Sig := Sig shr 12;

      SomeSize := 1 shl PackAttrs;
      SomeSize2 := SomeSize * PackAttrs2;

      if PackAttrs=0 then
         i := 0
      else
         i := (SomeSize*3 div 2) - 2;

      Blocks := ($800 div SomeSize) - 2;
      if Blocks < 1 then
         Blocks := 1;
      TotalBlocksSize := Blocks * SomeSize;

      if i<>0 then
      begin
         i := i shl 2;
         getmem (DecompBuffer, i);
         fillchar (DecompBuffer^, i, #0);
      end;
      getmem (SomeBuff, SomeSize2 shl 2);
      Memb_50 := 0;
      getmem (LargeBuffer, $20000);
      Buffer_Middle := pointer (integer(LargeBuffer)+$10000);

      FileSig1 := SomeFileSig1;
      FileSig2 := SomeFileSig2;
      FileLen := DataSize;
   end;
end;



end.