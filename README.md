# PlayEncryptedMovies
If you want to play an encrypted movie, you have some ways such as decrypting it by creating a new file, but this way is vulnerable because at that time someone can use your decrypted file without any toil.

To solve this problem you should decrypt the file Anonymously and hiddenly. For that, you can decrypt the file into the computer's memory(RAM).
This project tries to play encrypted videos using memory without creating any decrypted files.

at first, this project used Windows media player dll. The encryption algorithm used in it is DES. If you want to read more with example go to https://github.com/Sooheil/EncryptionApplication, I explained it.

**Windows media player** can play the file using URL and I implement a virtual web app and **play** get API inside this project (on http://localhost:9000/) for sending requests from Windows media player to that API. In that API I decrypt all bytes of video using the memory stream and finally make **HttpResponseMessage** for the Windows media player URL.
