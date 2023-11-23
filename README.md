# Multitools
A tool I've been tinkering with for a considerable span of time for the intentions of providing utilities for multiple things.

## General Overview
This is a tool comprised of multiple functions and utilities, written in C# with a WPF UI.<br>
Some functions were made for a more overall general use, and others were made for more specific use cases.

## Implemented Functions
* Embedded File Extraction. Currently supported files include:
  * PNG files.
  * JPG / JPEG files.
  * PVR files.
  * WAV files.
  * SWF (both compressed and uncompressed) files.

* PVR Version Conversion.
  * PVR2 files -> PVR3 files

* Common File Compression / Decompression
  * ZLIB / Deflate
 
* Other File Compression / Decompression
  * XOR Key Encryption

* File Package Compression / Decompression
  * SERF (Decompression only, Microsoft Tinker Data File)
  * DPAK (Disney Package File)

* Other Misc. Functions
  * .mmf/.mmt/.mma File Data Reading (Alpine Model File / Model Texture / Model Animation)
       * Exporting .mmf to Standard Model Files (W.I.P).
         * Supported Export Formats: Collada DAE Model File (.dae)
       * Exporting .mma to Animation Files (W.I.P).
         * Supported Export Formats: 3ds Max Animation File (.xaf)
       * Exporting .mmt to contained image file. 
