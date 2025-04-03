var_str_auto_0 = $13
STR $13 $B ; hello world
STR $14 $68 ; h
STR $15 $65 ; e
STR $16 $6C ; l
STR $17 $6C ; l
STR $18 $6F ; o
STR $19 $20 ;  
STR $1A $77 ; w
STR $1B $6F ; o
STR $1C $72 ; r
STR $1D $6C ; l
STR $1E $64 ; d


LDI var_str_auto_0
PUSH
LDR
PUSH
LDI $01
SYS

LDI $0A
PUSH
LDI $00
SYS