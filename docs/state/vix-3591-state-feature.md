# State Feature for Marking IElementNodes

## Overview

There are certain Prop elements in a display that can be animated or have states that can be stepped through over a series of frames. 
This is similar in nature to the LipSync effect, but with broader usage and no phonemes. For example, a Santa may have
a waving arm funxction In one state the arm is up, and in another state the arm is down. The user needs to be able to define 
these states by marking the elements that belong to a state. Each state can have a name and multiple state items that have 
a name, color and IElementNode assignment. 

This feature will be the baseline for a future State Effect. The State effect will be similar to the LipSync effect, 
but enables you to have similar functionality for props that are not standard ‘Faces’ - such as Reindeer Coro faces, 
a seven segment FM ‘Tune to sign’ with a colon and a dot, or a Waving Santa. The 
xLights project has a similar state effect and in fact their xModels can be imported and translated to our elements and
we will want to incorporate the StateInfo lioke we do their faceinfo today.

## Requirements

- The overall state definition has a name that defines it's overall function.
- The overall state definition should have a description field so the user can give it a descriptive text over just the name.
- Each state definition can have multiple states that are defined by a name, color, and the IElementNodes associated with them.
- The user should be able to select an IElementNode in the Display Setup and add a state definition for it.
- In the setup screen, the user will be able to define the overall name and description.
- In the setup screen, the user should be able to add one to many states and define the name, color, and pick which IElementNodes
  that are children of the selected IElementNode that are associated with the state. 
- The edit form should be presented in a tabular grid for the state definitions.
- The user can click to edit the name inline in the grid cell.
- The color cell can be clicked and should use the standard Vixen Color Chooser to allow the user to set the color. 
- The color cell should have a background that is the same as the choosen color and should show the hex value as text.
- The IElementNodes should be selectable from a tree view of the selected element and its children.
- The Tree view should have checkboxes in each IElementNode to select or deselect the node.
- If a group IElementNode is selected, then all nodes below it are grayed out and assumed associated by the group selection.
- The Count cell should show the count of the IElementNodes associated with the state row.
- Each row when selected shows the tree view for its state.

## Technical

A Vixen Property under VixenModules.Property may be an option for implementing this, but do not consider that the sole option.
If there are better ways, then provide them as an option to review and choose.
The Face Property is similar, but used for LipSync specifically.

If a Property is used, there is already mechanisms for it to show up in the Display Setup and invoke it's setup form. Those
same mechanisms should be used if the property route is the best way. 

## Guidelines

- Use the project skills dotnet-best-practices, csharp-async, csharp-docs, and dotnet-design-pattern-review as part of the design process.
- Use plans.md for creating the plan to design and build the functionality.
- The first implementation milestone must include Updating JIRA issue VIX-3591 in the VIX project covering the work in 
  the plan including requirements, high level design, acceptance criteria and testing steps. Provide information to paste into 
  into the JIRA ticket in markdown in the plan.
- Call out any risks or concerns in the plan when creating the design.
- Ask questions if you need further clarification.


 