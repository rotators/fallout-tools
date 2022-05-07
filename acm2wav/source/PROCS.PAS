unit Procs;
// Expander Procs
//  OpenDecompressor
//  CloseDecompressor
//  ReadDecompressor
//  WriteDecompressor
//  SeekDecompressor
//  TellDecompressor

interface
uses Stream;
type TOpenProc = function (name:PChar; OpenMode: integer): pFileReader;
     TReadProc = function (fr: pFileReader; readBuf: pointer; count: integer): integer;
     TFileLenProc = function (fr: pFileReader):integer;

function OpenDecompressor (name:PChar; OpenMode: integer): pFileReader;
function ReadDecompressor (fr: pFileReader; readBuf: pointer; count: integer): integer;
function FileLenDecompressor (fr: pFileReader):integer;
function TellDecompressor (fr: pFileReader):integer;
function CloseDecompressor (fr: pFileReader):integer;


implementation
uses DictFill;

function ReverseDD (i: longint): longint;
var b:byte;
    a: array [1..4] of byte absolute i;
begin
   b:=a[1];a[1]:=a[4];a[4]:=b;
   b:=a[2];a[2]:=a[3];a[3]:=b;
   result := i;
end;

function OpenDecompressor (name:PChar; OpenMode: integer): pFileReader;
var Compressed:boolean;
    tmpPtr: pFileReader;
	file_sig, skip_len: longint;
begin
   Compressed := true; // Raw data is only in internal version

   new (tmpPtr);
   fillChar (tmpPtr^, sizeOf (tmpPtr^), #0);

   with tmpPtr^ do
   begin
      Flags := 1;
      new (FileVar);
      assignFile (FileVar^, name); reset (FileVar^,1);

// 13.03.2003: The Starfleet Academy's ACMs have additional header. We are goung to locate and skip it.
		blockread (FileVar^, file_sig, 4);
		if file_sig = $44414653 then begin // SFA ACMs, 'SFAD' signature
			blockread (FileVar^, skip_len, 4);
			skip_len := ReverseDD (skip_len);
			seek (FileVar^, skip_len+8); // skipping SFA header
		end else
			seek (FileVar^, 0); // standard processing

      if Compressed then
      begin
         Flags := Flags or 2;
         FileDecompressor := CreateFileDecomp (FileVar, FileSig1, FileSig2, FileLen);
         FileLen := FileLen * 2;
      end
      else
         FileLen := FileSize (FileVar^);
      BytesInReadBuff := 0;
   end;

   result := tmpPtr;
end;

function ReadDecompressor (fr: pFileReader; readBuf: pointer; count: integer): integer;
begin
   with fr^ do
   begin
      if 2 and Flags <> 0 then
         result := ReadAndDecompress (FileDecompressor, readBuf, count)
      else
         blockread (FileVar^, readBuf^, count, result);
      inc (BytesInReadBuff, result);
   end;
end;

function FileLenDecompressor (fr: pFileReader):integer;
begin
   result := fr^. FileLen;
end;

function TellDecompressor (fr: pFileReader):integer;
begin
   result := fr^. BytesInReadBuff;
end;

function CloseDecompressor (fr: pFileReader):integer;
begin
   closeFile(fr^. FileVar^);
end;

end.