# VIX-2690 Element Tags

## Overview

Over time an element or group of elements can become unused, reserved for a special purpose, or useful to identify as
part of a workflow. For example, a user may want to mark elements as Deprecated, Whole House, Hidden, or with a custom
label that helps them sort and filter the display tree in setup or sequencing views.

Element Tags as lightweight labels that can be assigned to zero or more `IElementNode` instances where introduced in VIX-3933.
This improvement will build on that core functionality to expose some of the capability into user facing workflows.

## Problem

Vixen currently has no user-friendly way to label elements with simple organizational or workflow metadata. A user who
wants to stop using an element must remember that state manually, rename the element, move it in the tree, or rely on
external notes. This makes it easy to accidentally sequence against deprecated elements or difficult to filter large
displays by meaningful user-defined groups.

## JIRA Issue

VIX-2690 has some basic requirements. Reference it for a baseline for this spec document. Once these specs are refined, update
the JIRA issue with complete requirements, acceptance criteria, and test plans.

## In Scope

- Ability to tag element nodes in the display setup and the sequencer with the built-in tags. 
- Ability to untag element nodes in the display setup and the sequencer with the built-in tags. 
- Ability to colorize the element node labels in the Sequencer with the tag color when they are tagged with the built-in Deprecated tag. 
- Ability to hide the element nodes and their children when they are tagged with built-in Hidden tag.
- Ability to prevent new effects from being added to element nodes that are tagged with built-in Deprecated tag.
- Ability to set the color on built-in tags.
- Ability to notify the user upon sequence load that there are effects on element nodes tagged with the built-in Deprecated tag.

## Deferred Scope

- Element node deletion wizard that can check sequences. This should be created as a seperate related issue.
- Migration wizard to place effects set for deletion to some area. When an element node is unknown to a sequence, the user 
  is presented with a migration wizard when the sequence is loaded to allow them to map those effects to other existing element nodes.
  This should be noted as the preferred migration path for now. 

## References

docs\vix-3933-element-tags.md
docs\plans\vix-3933-element-tags-api.md
