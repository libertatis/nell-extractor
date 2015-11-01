# nell-extractor
A rough extractor of data from CMU's NELL project.

How to use:
Download the data from NELL: http://rtw.ml.cmu.edu/rtw/resources (top link)
Rename to 'nell.csv'
Run from the command line:

nell-extractor.exe \<category\> \<generalised\>

Where category is the type of concept (e.g. food, beverage, televisionstation) and generalised is either 'general' or any other string.

If the argument is passed as 'general', it will also check the indirect columns (e.g. something listed as a generalization of meat will not be caught under 'food' unless general is used as an argument).
