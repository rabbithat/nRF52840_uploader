\ Type "upload" and then upload the Forth file (e.g. by pasting it into your terminal program).
\ End the file with a tilde ("~"), or type a tilde at the terminal.  Each line of the file will 
\ then be evaluated.  If an error is encountered, the evaluation will stop just after the point at 
\ which the error occurs.

#13 constant ASCII_cr
#10 constant ASCII_lf
#126 constant ASCII_tilde
ASCII_tilde constant _EOF_DELIMITER \ end of file delimiter
0 constant _FALSE
$FFFFFFFF constant _TRUE
$FF constant maskByte0 \ bitmask for byte zero.
$FFFFFF00 constant maskByte321 \ bitmask for byte3 byte2 and byte1

#100000 buffer: downloadBuffer \ create a 100,000 byte buffer
4 variable downloadBuffer_numChars
4 variable downloadBuffer_index

\ ( -- )
: clear__downloadBuffer_index 0 downloadBuffer_index ! ;

\ ( -- value )
: get__downloadBuffer_index downloadBuffer_index @ ;

\ ( value -- )
: write__downloadBuffer_index downloadBuffer_index ! ;

: increment__downloadBuffer_index downloadBuffer_index @ 1+ downloadBuffer_index ! ;

: decrement__downloadBuffer_index downloadBuffer_index @ 1- downloadBuffer_index ! ;

\ ( -- valueAtIndexedLocation ) \ 
: get_indexed_downloadBuffer downloadBuffer_index @ downloadBuffer + @ maskByte0 and ;

: get_indexed++_downloadBuffer get_indexed_downloadBuffer increment__downloadBuffer_index ;

\ (value  -- )
: write_indexed_downloadBuffer downloadBuffer_index @ downloadBuffer + ! ;

\  (value -- )
: write_indexed++_downloadBuffer write_indexed_downloadBuffer increment__downloadBuffer_index ;

( numberOfChars -- )
: write__downloadBuffer_numChars downloadBuffer_numChars ! ;

\ ( -- numberOfChars )
: get__downloadBuffer_numChars downloadBuffer_numChars @ ;

\( key -- ) \ for every CR received, also emit an LF immediately afterward
: showKey dup emit ASCII_cr = if ASCII_lf emit then ;



#256 buffer: evalString
4 variable evalString_index

: clear__evalString_index 0 evalString_index ! ;

: get__evalString_index evalString_index @ ;

\ ( value --  )
: write__evalString_index evalString_index ! ;

: increment__evalString_index evalString_index dup @ 1+ swap ! ;

: decrement__evalString_index evalString_index dup @ 1- swap ! ;

\ ( value -- )
: write_indexed_evalString evalString_index @ evalString + ! ;
\ : clear_charCount_evalString 0 evalString ! ;

: write_indexed++_evalString write_indexed_evalString increment__evalString_index ;

4 variable evalString_charCounter
: evalString_charCounter++ evalString_charCounter dup @ dup 1+ swap ! ;

: clear_evalString_charCounter 0 evalString_charCounter ! ;
: evalString_charCounter++ evalString_charCounter dup @ 1+ swap ! ; 

: update__evalString_charCount evalString @ maskByte321 and decrement__evalString_index decrement__evalstring_index get__evalString_index or evalString ! ;

: copyFromBufferIntoEvalString 1 evalString_index ! begin get_indexed++_downloadBuffer dup write_indexed++_evalString ASCII_cr = until update__evalString_charCount ; 

: copyToEvalString  clear__downloadBuffer_index  begin copyFromBufferIntoEvalString CR evalString count type evalString count evaluate get__downloadBuffer_index get__downloadBuffer_numChars >= until ;

: print__evalString evalString count type CR ;

: show__evalString evalString @ maskByte0 and . ;

: download clear__downloadBuffer_index begin key dup write_indexed++_downloadBuffer ASCII_tilde = until decrement__downloadBuffer_index ASCII_cr write_indexed++_downloadBuffer ;
  
: ul CR ." Begin upload: " download CR CR ASCII_cr write_indexed_downloadBuffer  get__downloadBuffer_index dup  write__downloadBuffer_numChars  . ." characters uploaded." CR CR ;

\ show characters received into downloadBuffer
\
: sul cr clear__downloadBuffer_index begin get_indexed++_downloadBuffer maskByte0 and showKey get__downloadBuffer_index get__downloadBuffer_numChars = until CR ;


: upload ul copyToEvalString ;