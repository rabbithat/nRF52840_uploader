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
\ \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
\ Definition of Global Variables
\

#10000 buffer: dlBuf \ base address of the download buffer
4 variable dlBuf_numChars \ number of characters presently in the download buffer
4 variable dlBuf_i \ index (placeholder) dedicated to the download buffer

#256 buffer: evalString  \ Counted string.  evalString[0] contains number of chars in the string
4 variable evalString_i \ index dedicated to evalString



\ \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
\
\ General Purpose simple variable manipulation
\

\ Return byte0 of the given variable
\ ( varAddr -- byte )
: var@b @ maskByte0 and ;

\ Write the given byte into byte0 of the given variable
\ ( byte varAddr -- )
: var!b dup @ maskByte321 and rot or swap ! ;

\ sets the variable to zero
\ (variableAddress -- )
: var0! 0 swap ! ;

\ sets the variable to one
\ (variableAddress -- )
: var1! 1 swap ! ;

\ increment the variable 
\ (variableAddress -- )
: var++! dup @ 1+ swap ! ;

\ decrement the variable 
\ (variableAddress -- )
: var--! dup @ 1- swap ! ;

\ decrement a variable by 2
\ (variableAddress -- )
: var-2! dup @ 2- swap ! ;

\
\ \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
\ General Purpose indexed list (one-dimensional array) manipulation
\

\ get the value of an indexed variable
\ ( indexValue baseAddressOfList -- valueAtIndexedLocation )
: ivList@ + @ ;

\ get the value of the low order byte at the indexed location
\ ( indexValue baseAddressOfList -- byte0AtIndexedLocation )
: ivList@b + var@b ;

\ write the given value at the indexed list location
\ ( value indexValue baseAddressOfList -- )
: ivList! + ! ;

\ write the given byte value into byte0 at indexed list location
\ ( byte indexValue baseAddressOfList -- )
: ivList!b + var!b ;

\ clear the value at the indexed list location
\ (indexValue baseAddressOfList -- )
: ivList0! + 0 swap ! ;

\ set the value at the indexed list location to 1
\ (indexValue baseAddressOfList -- )
: ivList1! + 1 swap ! ;

\ increment the value at the indexed list location
\ ( indexValue baseAddressOfList -- )
: ivList++! + var++! ;

\ decrement the value at the indexed list location
\ ( indexValue baseAddressOfList -- )
: ivList--! + var--! ;

\ get the value at the indexed location and then increment the index
\ (indexAddress baseAddressOfList -- value )
: i++List@ over @ + @ swap var++! ;

\ get the value at the indexed location and then increment the index
\ (indexAddress baseAddressOfList -- byte )
: i++List@b i++List@ maskByte0 and ;

\ get the value at the indexed location and then decrement the index
\ (indexAddress baseAddressOfList -- value )
: i--List@ over @ + @ swap var--! ;

\ write the value at the indexed location and then decrement the index
\ ( value indexAddress baseAddress -- )
: i--List! over @ + rot swap ! var--! ;

\ write the value at the indexed location and then increment the index
\ ( value indexAddress baseAddress -- )
: i++List! over @ + rot swap ! var++! ; 

\ write the byte to byte0 of the indexed location and then increment the given index
\ ( byte indexAddress baseAddress -- )
: i++List!b over over swap @ swap ivList@ maskByte321 and >R rot R> or rot rot i++List! ;


\ \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
\ Domain Specific word definitions

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
: ul CR ." Begin upload: " download CR CR dlBuf_i @ dup dlBuf_numChars ! . ." characters uploaded." CR CR ;

\ Note: Diagnostic only.
\ show all the code that was received into the download buffer 
\
: sul cr dlBuf_i var0! begin dlBuf_i dlBuf i++List@ maskByte0 and showKey dlBuf_i @ dlBuf_numChars @ = until CR ;


: upload ul evaluateEachLine CR ." Done!" CR CR ;