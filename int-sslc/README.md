# sslc

Script compiler/parser for Fallout 1/2.

The `sslc` directory is based on
[sfall-team/sslc](https://github.com/sfall-team/sslc) at commit
`d55be0b1668ff0891b2e41373d343dcfeae48acd` (23 May 2026).

Local integration changes retain the six-argument `parse_main` interface used
by Sfall Script Editor, preserve the existing header-name casing, make repeated
Visual Studio builds non-interactive, and normalize Windows test paths. The
compiler and parser behavior otherwise follows that upstream revision.

# int2ssl

Script decompiler for Fallout 1/2.

Originally implemented by **Anchorite** (2005-2009).

Expanded for full *sfall* support by *Nirran* and *phobos2077* (2014-2015).

Rewritten for multiple platform support by *alexeevdv* (2015).
