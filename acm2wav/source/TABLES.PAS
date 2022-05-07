unit Tables;
interface
var TablesFilled: boolean = false;
    Table: array [0..26] of byte;
    Table2: array [0..124] of word;
    Table3: array [0..120] of byte;

procedure FillTables;

implementation

procedure FillTables;
var i,j,k: integer;
begin
   if TablesFilled then exit;

   for i:=0 to 2 do
      for j:=0 to 2 do
         for k:=0 to 2 do
            Table [i + 3*j + k*9] := i + 4*j + 16*k;
   for i:=0 to 4 do
      for j:=0 to 4 do
         for k:=0 to 4 do
             Table2 [i + 5*j + 25*k] := 
                i + 8*j + 64*k;
   for i:=0 to 10 do
      for j:=0 to 10 do
         Table3 [i + 11*j] := i + 16*j;
   TablesFilled := true;
end;

end.