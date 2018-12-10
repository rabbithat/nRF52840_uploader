# nRF52840_uploader
This is a utility for easily uploading Forth files to an nRF52840.

Background and motivation:  it seemed that all the upload utilities I could find worked with only one particular terminal program or another.  So, instead of that, I wrote this generic uploader utility that will work with practically *any* terminal program.  

Recommendation: use the "compiletoflash" command to compile this program into the nRF52840's flash memory so that it will be conveniently available to assist with all future uploads.

How to use:  to invoke the program, use a terminal to type "upload" at the nRF52840's REPL prompt.  Then paste into your terminal program the file you want to upload.  Finish the upload either by typing a tilde ("~") on the terminal connected to the nRF52840 or including a tilde at the end of the upload file.  Immediately after the nRF52840 receives the tilde, each line of the upload file will be automatically evaluated and compiled onto the nRF52840.

Done!  It's that easy.
