To create a new Eduspeak grammar package do the following

Install the Eduspeak SDK
This grammar package was built with SDK 2.60

Make sure the Nuance var has been set

If it's properly set you won't need to explicity set
the path to eduspeak-compile.exe as I've had to do below.
You can just use eduspeak-compile

this is the one line that's in the bat file

It takes the grammar and the words to add to the dictionary and
creates a new master package.

C:\Users\cjochumson\Documents\openme\DEV\2_60_SDK\bin\win32\eduspeak-compile.exe GE/GE english-p16-na-050930-v2 -override_dictionary GE/GE.dictionary >> GE\GEBUILDOUTPUT.txt 2>&1

The GE\GE folder is the actual master package which you'll need along 
with the ge.grammar file.