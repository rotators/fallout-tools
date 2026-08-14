# sslc

Script compiler/parser for Fallout 2/sfall scripts.

## Usage

    compile {switches} filename [-o outputname] [filename [..]]

### Switches
    -q    don't wait for input on error
    -n    no warnings
    -b    use backward compatibility mode
    -l    no logo
    -p    preprocess source
    -P    preprocess only (don't generate .int)
    -F    write full file paths in #line directives
    -O<level>  optimize code
               0 - none
               1 - only remove unreferenced variables/procedures (default)
               2 - full (same as -O)
               3 - full+experimental, don't use!
    -d    show debug info
    -s    enable short-circuit evaluation for boolean operators (AND, OR)
    -D    dump abstract syntax tree after optimizations
    -m<macro>[=<val>]  define a macro named "macro" for conditional compilation
    -I<path>  specify an additional directory to search for include files
