# nRF52840_uploader
This is a utility for easily uploading Forth files to an nRF52840.

Background and motivation:  it seemed that all the upload utilities I could find worked with only one particular terminal program or another.  So, instead of that, I wrote this generic uploader utility that will work with practically *any* terminal program.  

Recommendation: use the "compiletoflash" command to compile this program into the nRF52840's flash memory so that it will be conveniently available to assist with all future uploads.

How to use:  To invoke the program, use a terminal to type "upload" at the nRF52840's REPL prompt.  Then paste into your terminal program the file you want to upload.  Finish the upload either by typing a tilde ("~") on the terminal or including a tilde at the end of the upload file.  Immediately after the nRF52840 receives the tilde, each line of the upload file will be automatically evaluated and compiled onto the nRF52840.

Done!  It's that easy.

------------------------------------------------------------------------------------------------

IMPORTANT!  Before loading this file, load the current version of: https://github.com/rabbithat/nRF52_essential_definitions

------------------------------------------------------------------------------------------------

Revision History:

Version 6:  Version 5 no longer allowed files to be uploaded to flash.  Version 6 fixes that.  Just invoke compiletoflash, as 
per usual, before invoking upload, and the Forth definitions will be put into flash instead of RAM.

Version 5: no longer requires a large static buffer that persists after the upload is finished.  Instead, a buffer which is temporarily located at the midpoint of free RAM is used.

Version 4: Variable and list manipulation code has become a separate library file which this code now leverages:  https://github.com/rabbithat/nRF52_essential_definitions

Version 3: re-wrote the program by generalizing the code for manipulating variables and lists

Version 2: fixed a comment typo reported by Terry Porter
