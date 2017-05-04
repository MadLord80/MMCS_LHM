# MMCS_LHM - MMCS Loading Header Mod
Mod header in LOADING.KWI for MMCS with WinCE

Download - https://github.com/MadLord80/MMCS_LHM/tree/master/bin/Debug

This program allows to change the description of the blocks in the header of the LOADING.KWI file 
based on the Win CE OS (for example, from MMCS 2005-2010 navigation).

The LOADING.KWI file header contains 2 sections: a section describing the names of the blocks and 
a section describing the location of the blocks.

The program uses 2 methods to change the description of the names of blocks:
- replace - simply replace the description of the specified block in the current from donor
- add - the description of the name of the new block from donor is added before specified in current. 
Location addresses are also added, but simply duplicated description of the selected block of current.
This takes into account the size of the entire header, and if it increases, if necessary, the address 
of the location of the blocks is recalculated.

Features:
1. replace the block description ("<" button)
2. add the block description ("<<" button)
3. change the version of the block (double click on the block version in the current window)
4. change the name of the block (double click on the block name in the current window)

The program uses 2 modes:  
Simple mode - only MLD blocks are displayed in the current and can be replaced with blocks from the standard list.  
Advanced mode - displays a list of all the blocks and can be replaced by any block from the donor

Example (Simple mode) to make Russian loading (R-03) for Japanese (J-01):
1. Open current - open the file LOADING.KWI of R-03
2. In the list of blocks R-03, select the block NR261RM.MLD.ORG (if it is one, it is selected by default)
3. In the list of standard blocks, choose J-01
4. Press the "<<" button
5. See the changes in the R-03 window (the changed blocks are marked in red)
6. Click the "Save" button, the "writing" line shows the status of creating a new file
7. After the file is successfully created, the Info window will display a corresponding message
