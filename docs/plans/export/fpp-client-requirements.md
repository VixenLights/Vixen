# FPP API Client

## Overview

The purpose of this feature is to create a client in Vixen that can make api calls to an FPP instance to get and send relevant data.

## API Reference

The FPP REST API reference is located at docs\references\fpp-rest-api.md. This docuement should be consulted to understand the 
various endpoints available. 

## Function Client Operations

The client should be able to upload a sequence (*.fseq) file.
The client should be able to upload any type of file that FPP supports.
The client should have more specific methods to upload a video or music file that can wrap the general file api since those are most common.
The client should be able to get a list of sequences. 
The client should be able to get the the system info.
The client should be able to get a list of playlists.
The client should be able to get the list of sequences in a playlist.
The client project should land in the Vixen.Modules\App space as an independant project.
The Client project should have a full test suite in Vixen.Tests
Only implement the API endpoints needed to solve for these use cases.
The Client may implement other endpoints in the future, so do not constrain the design in any way that prevents future expansion.

## Guidelines

The client code should follow best practices for RESTful API clients.
Use the project skills dotnet-best-practices, csharp-async, csharp-docs, and dotnet-design-pattern-review.
Use plans.md for creating the plan to design and build the client. Include steps to create a JIRA issue covering the work in the plan.
Call out any risks or concerns in the plan when creating the design.


