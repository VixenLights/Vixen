# State Assignment Enhancement

## Overview

Currently in the State Property setup, the user can select elements in a tree structure to assign them to a State Item.
This works ok for a few elements, but when a user wants to select many elements it would be easier if they could select a 
range of elements to toggle on or off.

## Requirements 

- Enhance the element tree in the State Property Setup to allow the user to select a range of elements that can be toggled on or off.
- If the selected range is toggled, then any elements that are unchecked will be checked. Any that are checked, will be unchecked.
- The user can have mixed checked or unchecked in the range and each selection will be toggled.
- The range should be selectable via conventional means. Ctrl + click adds or removes from the select. Shift + click selects everything 
  between the first selection and clicked item. These should mimic well known behaviors for selection.
- Current group selection rules should be maintained. If a selection spans a group item, then everything in the checked/unchecked group
  behaves as it does today.

