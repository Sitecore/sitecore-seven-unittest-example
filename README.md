Sitecore 7 - Unit Test Example
===============================

A simple example project to show different aspects of unit testing Sitecore 7


Setup
--
Copy `Sitecore.ContentSearch.dll` and `Sitecore.ContentSearch.Linq.dll` from your Sitecore 7.0 installation to the `sc.lib` directory.

When you build NuGet should download the missing packages.

This project uses 3 open-source frameworks:
 - FakeItEasy
 - Fluent Assertions
 - NUnit

Notes
--
This example should demonstrate that with the new architecture of the search components that you dont need `Sitecore.Kernel` (and all the associated config and HttpContext) to run unit tests against the search.
This example currently shows a full faking of the search results, other partial faking examples will follow.
