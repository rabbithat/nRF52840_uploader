# nRF52840_uploader
utility for easily uploading Forth files

It seemed that all the upload utilitie I could find worked with only one particular terminal program or another.  So, instead of that, I wrote a generic uploader utility that will work with any terminal program.  

Thereofore, load this program into flash so that it will be there to assist with all future uploads.

To invoke the program, type "upload" on the nRF52840.  Then paste the file into your brother.  Finish the upload either by typing a tilde ("~") or including one at the end of theupload file.  Afterward, each line of the upload file will be evaluated and compiled onto the target.
