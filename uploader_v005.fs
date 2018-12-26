\ IMPORTANT!  Before loading this file, load the current version of:
\ https://github.com/rabbithat/nRF52_essential_definitions

\ \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
\ Directions:
\
\ Type "upload" and then upload the Forth file (e.g. by pasting it into your terminal program).
\ End the file with a tilde, or type a tilde at the terminal.  Each line of the file will 
\ then be evaluated.  If an error is encountered, the evaluation will stop just after the point at 
\ which the error occurs.
\
\ WARNING: this code takes advantage of the nRF52840's ability to read and write from addresses which 
\ are not 32-bit word aligned.  Running this code on a CPU without that capability will trigger a
\ hardware fault.
\
\ \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
\ Definition of Constants
\

#10 constant ASCII_lf
#13 constant ASCII_cr
#126 constant ASCII_tilde
ASCII_tilde constant _EOF_DELIMITER \ end of file delimiter

$FF constant maskByte0 \ bitmask for byte zero.
$FFFFFF00 constant maskByte321 \ bitmask for byte3 byte2 and byte1

\
\ \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


\ \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
\ RAM Status functions

\ address of the highest addressable 32-bit word of memory
(  --  maxRamAddress )
: maxRamAddress flashvar-here 4 - ;

\ amount of free ram remaining
( -- freeRam )
: freeRam maxRamAddress compiletoram here - ;

\ print the amount of free RAM remaining
( freeRam --  )
: showFreeRam freeRam . ." bytes " CR ;
\ Definition of Global Variables
\
\ \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
\
0 variable addrDlBuf \ base address of the upload buffer.  To be initialized below.

0 variable dlBuf_numChars \ number of characters presently in the download buffer
0 variable dlBuf_i \ index (placeholder) dedicated to the download buffer

#256 buffer: evalString  \ Counted string.  evalString[0] contains number of chars in the string
0 variable evalString_i \ index dedicated to evalString


\ \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
\ Domain Specific word definitions

\ The size of the "pseudo-allocated" dlBuf.
( --  numBytes )
: dlBufSize freeRam 2 / ;

: showDlBufSize CR CR ." Upload buffer size is " dlBufSize . ." bytes." CR ;

: dlBuf addrDlBuf @ ;  \ emulates dlBuf as though it were an actual allocated buffer.

\ T avoid having a large, unused allocated buffer after it is no longer needed,
\ dlBuf will *not* be officially allocated.  Instead, it will be set to the midpoint address
\ of the available free ram.  After it is finished being used, its contents can be
\ overwritten.
\ 
( --  )
: initializeDlBuf maxRamAddress dlBufSize - addrDlBuf ! ;

\ Print the given ASCII character.  For every ASCII_cr, also emit an ASCII_lf immediately afterward
\ ( char -- )
: showKey dup emit ASCII_cr = if ASCII_lf emit then ;

\ PRE-CONDITION: evalString_i equals the next location after the ASCII_cr which ends the string
\ Result:  Change evalString[0] to equal the number of characters *before* the ASCII_cr 
\ that ends the string
\ ( -- )
: updateImpliedCharCount__evalString evalString_i @ 2 - 0 evalString ivList!b ;

\ Starting from the current indexed location of the download buffer, copy the next line into evalString
\ ( -- )
: copyFrom_dlBuf_into_evalString evalString_i var1! begin dlBuf_i dlBuf i++List@b dup evalString_i evalString i++List!b ASCII_cr = until updateImpliedCharCount__evalString ; 

\ Starting at the beginning of the download buffer, copy each line into evalString and 
\ then print and evaluate it. 
\ ( -- )
: evaluateEachLine dlBuf_i var0! begin copyFrom_dlBuf_into_evalString CR evalString count type evalString count evaluate dlBuf_i @ dlBuf_numChars @  >= until ;

\ From over the terminal, download the entire file of new code.  Finished when a tilde is detected.
\ Note: In the download buffer, the tilde is replaced with an ASCII_cr
\ ( -- )
: download dlBuf_i var0! begin key dup dlBuf_i dlBuf i++List! ASCII_tilde = until dlBuf_i var--! ASCII_cr dlBuf_i dlBuf i++List! ;

\ Prompt the user the upload the code file.  Download the file but do not evaluate it.
: ul  initializeDlBuf showDlBufSize ." Begin upload: " download CR CR dlBuf_i @ dup dlBuf_numChars ! . ." characters uploaded." CR CR ;

\ Note: Diagnostic only.
\ show all the code that was received into the download buffer 
\
: sul cr dlBuf_i var0! begin dlBuf_i dlBuf i++List@b showKey dlBuf_i @ dlBuf_numChars @ = until CR ;

: upload   ul evaluateEachLine CR CR CR ." Done!" CR CR ;